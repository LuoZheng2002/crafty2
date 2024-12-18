using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static Util;

public class GameStateChangedEvent
{
	public bool is_play = false;
	public GameStateChangedEvent(bool is_play)
	{
		this.is_play = is_play;
	}
}

public class TrackStartEvent { }
public class InvisibleStateUpdateEvent
{
}
public class WASDPressedEvent { }
public class GameState : MonoBehaviour
{
	public static int unlocked_levels = 1;
	public static int start_level = 1;


	public static Dictionary<string, bool> button_clicked = new()
	{
		{"layers", false},
		{"trashcan", false },
		{"drag_images", false },
		{"placed_a_component", false },
		{"third_person", false },
		{"eraser", false },
		{"confirm", false },
		{"menu", false },
		{"view", false },
		{"help", false },
		{"retry", false }
	};
	public static bool shown_layers = false;
	public static bool shown_trashcan = false;
	public static bool shown_drag_images = false;
	public static bool shown_third_person = false;
	public static bool shown_eraser = false;
	public static bool shown_confirm = false;
	public static bool shown_menu = false;
	public static bool shown_view = false;
	public static bool shown_help = false;
	public static bool shown_retry = false;
	public static bool drag_screen_shown = false;
	public static List<bool> shown_tutorials = new() { false, false, false, false, false };

	

	public GameObject cameraAnimationPrefab;
	bool camera_follow_pig = false;
	public int current_level_num = 1;
	public float move_to_pig_time = 0.5f;
	public float camera_rotation_time = 0.5f;
	public float retry_move_time = 0.5f;
	public float rise_height = 10.0f;
	

	Util.WaypointName retry_waypoint = Util.WaypointName.None;
	Util.GoalName retry_goal = Util.GoalName.None;
	public bool IsFirstPerson
	{
		get { return first_person; }
		set 
		{ 
			first_person = value;
			EventBus.Publish(new InvisibleStateUpdateEvent());
			// PiggyCameraPivot.Inst.OnFirstPersonChanged(value);
		}
	}
	private bool first_person = true;
	public bool PiggyPermitInvisible { get; set; } = false;
	public List<VehicleComponent> Components { get; set; } = new();

	public PiggyPreview Piggy { get; set; }

	static GameState inst;
	public static GameState Inst
	{
		get { Debug.Assert(inst != null, "Game State not set"); return inst; }
	}
	

	public Level CurrentLevel { get
		{
			Level level = GameObject.Find($"Level{current_level_num}").GetComponent<Level>();
			Debug.Assert(level != null);
			
			return level;
		} }

	//public void GoBackToBuild()
	//{
	//	StartCoroutine(MoveCameraToGrid(false));
	//}
	bool official_start = true;
	private bool is_trailer = false;
	void Yikai()
	{
		Util.unbreakable = true;
		
		if (official_start)
		{
			
			if (is_trailer)
			{
				StartCoroutine(TrailerBuild());
			}
			else
			{
				BlackoutCanvas.Inst.BlackoutAsync(3.0f, 1.0f, 0.0f);
				PlayCanvas.Inst.HideStory();
				TransitionToStory(Util.StoryName.Crash);
				StoryCanvas.Inst.HideSideQuests();
			}
		}
		else
		{
			Util.unbreakable = false;
			GameSave.IncrementGridSize(1, 1, 0);
			GameSave.CurrentMemory.MemCrates[1, 0, 0] = Util.Component.WoodenCrate;
			GameSave.CurrentMemory.MemAccessories[1, 0, 1] = Util.Component.Rocket;
			GameSave.CurrentMemory.MemCrates[1, 0, 2] = Util.Component.WoodenCrate;
			GameSave.CurrentMemory.MemAccessories[1, 1, 0] = Util.Component.Rocket;
			GameSave.CurrentMemory.MemCrates[1, 1, 1] = Util.Component.WoodenCrate;
			GameSave.CurrentMemory.MemAccessories[1, 1, 2] = Util.Component.Rocket;
			GameSave.CurrentMemory.MemCrates[1, 2, 0] = Util.Component.WoodenCrate;
			GameSave.CurrentMemory.MemAccessories[1, 2, 1] = Util.Component.Rocket;
			GameSave.CurrentMemory.MemCrates[1, 2, 2] = Util.Component.WoodenCrate;
			GameSave.CurrentMemory.MemLoads[1, 2, 2] = Util.Component.Pig;
			GameSave.CurrentMemory.MemLoads[1, 0, 2] = Util.Component.Partner;
			GameSave.CurrentMemory.MemAccessories[0, 0, 0] = Util.Component.MotorWheel;
			GameSave.CurrentMemory.MemAccessories[0, 0, 2] = Util.Component.TurnWheel;
			GameSave.CurrentMemory.MemAccessories[0, 2, 0] = Util.Component.MotorWheel;
			GameSave.CurrentMemory.MemAccessories[0, 2, 2] = Util.Component.TurnWheel;
			GameSave.Inventory[Util.Component.MotorWheel] += 100;
			GameSave.Inventory[Util.Component.TurnWheel] += 100;
			GameSave.Inventory[Util.Component.WoodenCrate] = 100;
			GameSave.Inventory[Util.Component.Rocket] = 100;
			GameSave.Inventory[Util.Component.Umbrella] = 100;
			// GameSave.Inventory[Util.Component.Tourist] += 100;
			BuildCanvas.Inst.ShowDesignNumbers();

			MapCanvas.Inst.PermWaypoints.Add(Util.WaypointName.TownWaypoint);
			PlayCanvas.Inst.ShowStory();
			PlayCanvas.Inst.SetStoryText("Test");
			// Walls.Inst.WarpImmediately();
			// CarCore.Inst.VerticalOffset = 2.0f;


			// GoToCheckpointAsync(Util.WaypointName.Hammer, true);
			StartCoroutine(BeginSideQuest());
		}
		// TransitionToFirstBuild();
		
		
		//GoToCheckpointAsync(Util.WaypointName.VolcRoom4, true);

		// TransitionToBuild(Util.WaypointName.PreStory2, Util.GoalName.PreStory2);
		//  TransitionToBuild(Util.WaypointName.None, Util.GoalName.PreStory2);
		// TransitionToStory(Util.StoryName.TownWaypoint);
		// town_waypoint_met = true;
		// TransitionToBuild(Util.WaypointName.C1S1, Util.GoalName.None);
		// TransitionToStory(Util.StoryName.C1S1);
		// TransitionToBuild(Util.WaypointName.Volcano, Util.GoalName.VolcBottom);
		// TransitionToBuild(Util.WaypointName.VolcBottom, Util.GoalName.VolcTop);
		// FirstPerson.Inst.Show();
		// Retry.Inst.Show();
		// GoalCanvas.Inst.GoalToFollow = Util.GoalName.PreStory1;
		// Goal.Activate(Util.GoalName.PreStory1);
		PlayCanvas.Inst.HideUmbrella();
		PlayCanvas.Inst.HideRocket();
	}
	IEnumerator DelayPlayingAnimation()
	{
		yield return new WaitForSeconds(2.0f);
		yield return MainCamera.Inst.WarpTo(TRef.Get(TRefName.CameraPrestory2_2), 1.0f);
		story_animation.SetActive(true);
	}
	public GameObject post_process_volume;
	IEnumerator OnTownStory()
	{
		// yield return BlackoutCanvas.Inst.Blackout(1.0f, 0.0f, 1.0f);
		CarCore.Inst.DeactivateContainer();
		CarCore.Inst.Fix();


		//yield return BlackoutCanvas.Inst.DisplaySub("You heard from the local that your rocket engines are likely to be stolen by the evil beast: Groundhog the Juggernaut.", 1.0f, 0.0f, 1.0f);
		//yield return WaitForClick();
		//yield return BlackoutCanvas.Inst.DisplaySub(null, 1.0f, 1.0f, 0.0f);
		//yield return BlackoutCanvas.Inst.DisplaySub("And the only way to defeat the beast is to obtain the undestructable Adamantium material lying in the heart of the volcano.", 1.0f, 0.0f, 1.0f);
		//yield return WaitForClick();
		//yield return BlackoutCanvas.Inst.DisplaySub(null, 1.0f, 1.0f, 0.0f);
		//yield return BlackoutCanvas.Inst.DisplaySub("So you decide to march towards the volcano.", 1.0f, 0.0f, 1.0f);
		//yield return WaitForClick();
		//yield return BlackoutCanvas.Inst.DisplaySub(null, 1.0f, 1.0f, 0.0f);
		PlayCanvas.Inst.Hide();
		MainCamera.Inst.Stop();
		Character.Piggy.WarpTo(TRef.Get(TRefName.NPC1C1S2));
		Character.Partner.WarpTo(TRef.Get(TRefName.NPC2C1S2));
		AudioPlayer.Inst.TransitionToStory();
		yield return MainCamera.Inst.WarpTo(TRef.Get(TRefName.CameraPrestory2_1), 1.0f);
		yield return new WaitForSeconds(1.0f);
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "Excuse me. We lost our rocket engines. Do you have any clues?", Character.GetCharacter(CharacterName.Partner), VoiceLine.excuse_me);
		GoalCanvas.Inst.CheckpointToFollow = WaypointName.None;
		StartCoroutine(DelayPlayingAnimation());
		yield return LineCanvas.Bottom.DisplayLine("Villager", "Oh, I saw a big guy carrying two rocket engines towards the volcano.", Character.GetCharacter(CharacterName.NPC1), VoiceLine.oh_i_saw);
		yield return new WaitForSeconds(7.0f);
		post_process_volume.SetActive(false);
		yield return MainCamera.Inst.WarpTo(TRef.Get(TRefName.CameraPrestory2_1), 1.0f);
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "Thank you! We will go to the volcano to retrieve our engines.", Character.GetCharacter(CharacterName.Partner), VoiceLine.thank_you);
		LineCanvas.Bottom.Hide();
		MainCamera.Inst.MoveAndStickTo(CarCore.Inst.CameraEnd);
		Character.Piggy.WarpTo(TRef.Get(TRefName.Origin));
		Character.Partner.WarpTo(TRef.Get(TRefName.Origin));
		CarCore.Inst.Unfix();
		CarCore.Inst.ActivateContainer();
		yield return new WaitForSeconds(1.0f);
		AudioPlayer.Inst.TransitionToPlay();
		yield return DisplayChapter("Main Story Quest: Act 1", "Treasures in the Flaming Mountain", "Started", () => { LineCanvas.Bottom.DisplayLineAsync("Shirley", "Let's march towards the volcano!", 2.0f, VoiceLine.lets_march); });
		GoalCanvas.Inst.CheckpointToFollow = WaypointName.VolcanoGate;
		PlayCanvas.Inst.SetStoryText("March to the volcano");
		PlayCanvas.Inst.Show();
		PlayCanvas.Inst.ShowStory();
		StoryCanvas.Inst.SetMainStory(Util.MainStoryName.Volcano);
		// yield return BlackoutCanvas.Inst.Blackout(1.0f, 1.0f, 0.0f);
	}

	IEnumerator AtTownWaypoint()
	{
		CarCore.Inst.DampStart();
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "It seems we've reached a permanet waypoint.", null, Util.VoiceLine.it_seems);
		MapImage.Inst.Show();
		MapImage.Inst.StartScale();
		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "Let's click on the map to see the whole region.", (MapImageClickedEvent e) => true, Util.VoiceLine.lets_click);
		LineCanvas.Top.Hide();
		CarCore.Inst.DampStop();
	}
	IEnumerator DisplayChapter(string type_str, string text_str, string state_str, Action callback)
	{
		yield return BlackoutCanvas.Inst.Blackout(1.0f, 0.0f, 1.0f);
		ChapterCanvas.Inst.DisplayTextAsync(type_str, text_str, state_str, 0.5f, callback);
		yield return BlackoutCanvas.Inst.Blackout(0.5f, 1.0f, 0.0f);
	}
	public bool IntroducePreset { get; set; } = false;

	public void UpdateRetryAndGoal(Util.WaypointName retry_waypoint, Util.WaypointName goal_waypoint)
	{
		GameSave.Progresses[Util.QuestName.MainStory] = (retry_waypoint, goal_waypoint);
		EventBus.Publish(new DeactivateAllCheckpointEvent());
		GameSave.CurrentCheckpoint = retry_waypoint;
		if (goal_waypoint != WaypointName.None)
		{
			Checkpoint.Get(goal_waypoint).Activate();
		}
		GoalCanvas.Inst.CheckpointToFollow = goal_waypoint;
	}
	public bool introduce_space = false;
	public void OnCheckpointReached(CheckpointReachedEvent e)
	{
		if (e.waypoint_name != WaypointName.TownStory && e.waypoint_name != WaypointName.CaveTrap && e.waypoint_name != WaypointName.BossStart
			&& e.waypoint_name != WaypointName.CenterEntrance &&
			e.waypoint_name != WaypointName.LoopEntrance && e.waypoint_name != WaypointName.TourStart 
			&& e.waypoint_name != WaypointName.TrackEntrance)
		{
			AudioPlayer.Inst.PlayCheckpoint();
		}
		Util.Delay(this, 1, () =>
		{
			switch (e.waypoint_name)
			{
				case WaypointName.MotorWheel:
					UpdateRetryAndGoal(WaypointName.MotorWheel, WaypointName.TurnWheel);
					GameSave.Inventory[Util.Component.MotorWheel] = 2;
					GameSave.Inventory[Util.Component.Wheel] -= 2;
					GameSave.CurrentMemory.MemAccessories[0, 0, 0] = Util.Component.None;
					GameSave.CurrentMemory.MemAccessories[0, 1, 0] = Util.Component.None;
					ObtainCanvas.Inst.Show(Util.Component.MotorWheel);
					Retry.Inst.Show();
					RebuildButton.Inst.Show();
					RebuildButton.Inst.StartScale();
					// BackButton.Inst.Show();
					break;
				case WaypointName.TurnWheel:
					Util.unbreakable = false;
					UpdateRetryAndGoal(WaypointName.TurnWheel, WaypointName.Turn1);
					GameSave.Inventory[Util.Component.TurnWheel] = 2;
					GameSave.Inventory[Util.Component.Wheel] -= 2;
					GameSave.Inventory[Util.Component.WoodenCrate] += 3;
					ObtainCanvas.Inst.Show(Util.Component.TurnWheel);
					RebuildButton.Inst.StartScale();
					GameSave.IncrementGridSize(0, 1, 0);
					break;
				case WaypointName.Turn1:
					UpdateRetryAndGoal(WaypointName.Turn1, WaypointName.Lake);
					break;
				case WaypointName.Lake:
					UpdateRetryAndGoal(WaypointName.Lake, WaypointName.TownEntrance);
					LineCanvas.Bottom.DisplayLineAsync("Shirley", "The town is in sight! Rush towards it!", 2.0f, Util.VoiceLine.the_town_is);
					break;
				case WaypointName.TownEntrance:
					UpdateRetryAndGoal(WaypointName.TownEntrance, WaypointName.TownWaypoint);
					LineCanvas.Bottom.DisplayLineAsync("Shirley", "Woohoo. We've arrived!", 2.0f, Util.VoiceLine.woohoo_weve);
					break;
				case WaypointName.TownWaypoint:
					UpdateRetryAndGoal(WaypointName.TownWaypoint, WaypointName.TownStory);
					StartCoroutine(AtTownWaypoint());
					break;
				case WaypointName.TownStory:
					UpdateRetryAndGoal(WaypointName.TownStory, WaypointName.VolcanoGate);
					StartCoroutine(OnTownStory());
					break;
				//case WaypointName.Rocket:
				//	UpdateRetryAndGoal(WaypointName.Rocket, WaypointName.Umbrella);
				//	GameSave.Inventory[Util.Component.Rocket] = 6;
				//	GameSave.IncrementGridSize(1, 0, 0);
				//	ObtainCanvas.Inst.Show(Util.Component.Rocket);
				//	Retry.Inst.Show();
				//	RebuildButton.Inst.StartScale();
				//	introduce_space = true;
				//	break;
				//case WaypointName.Umbrella:
				//	UpdateRetryAndGoal(WaypointName.Umbrella, WaypointName.VolcanoGate);
				//	GameSave.Inventory[Util.Component.Umbrella] = 6;
				//	ObtainCanvas.Inst.Show(Util.Component.Umbrella);
				//	Retry.Inst.Show();
				//	RebuildButton.Inst.StartScale();
				//	break;
				case WaypointName.VolcanoGate:
					UpdateRetryAndGoal(WaypointName.VolcanoGate, WaypointName.BeforeDesign1);
					break;
				case WaypointName.BeforeDesign1:
					BuildCanvas.Inst.ShowDesignNumbers();
					UpdateRetryAndGoal(WaypointName.BeforeDesign1, WaypointName.BeforeDesign2);
					IntroducePreset = true;
					break;
				case WaypointName.BeforeDesign2:
					UpdateRetryAndGoal(WaypointName.BeforeDesign2, WaypointName.Umbrella);

					break;
				//case WaypointName.BeforeDesign3:
				//	UpdateRetryAndGoal(WaypointName.BeforeDesign3, WaypointName.BeforeLeap);
				//	StartCoroutine(HintDesign3());
				// 	break;
				case WaypointName.Umbrella:
					UpdateRetryAndGoal(WaypointName.Umbrella, WaypointName.Leap1);
					GameSave.Inventory[Util.Component.Umbrella] = 6;
					ObtainCanvas.Inst.Show(Util.Component.Umbrella);
					RebuildButton.Inst.StartScale();
					GameSave.IncrementGridSize(1, 0, 0);
					introduce_space = true;
					break;
				case WaypointName.Leap1:
					GameSave.Inventory[Util.Component.Rocket] = 6;
					ObtainCanvas.Inst.Show(Util.Component.Rocket);
					RebuildButton.Inst.StartScale();
					UpdateRetryAndGoal(WaypointName.Leap1, WaypointName.Leap2);
					break;
				case WaypointName.Leap2:
					UpdateRetryAndGoal(WaypointName.Leap2, WaypointName.Hammer);
					break;
				case WaypointName.Hammer:
					UpdateRetryAndGoal(WaypointName.Hammer, WaypointName.Obsidian);
					break;
				case WaypointName.Obsidian:
					UpdateRetryAndGoal(WaypointName.TownWaypoint, WaypointName.None);
					MapCanvas.Inst.ScaleTownWaypoint = true;
					StartCoroutine(CollectedObsidian());
					break;
				case WaypointName.CenterEntrance:
					UpdateRetryAndGoal(WaypointName.CenterEntrance, WaypointName.Center1);
					StartCoroutine(OnCenterEntrance());
					break;
				case WaypointName.Center1:
					UpdateRetryAndGoal(WaypointName.Center1, WaypointName.Center2);
					LineCanvas.Top.DisplayLineAsync("Shirley", "20 turns to go.", 2.0f, VoiceLine.twenty);
					break;
				case WaypointName.Center2:
					UpdateRetryAndGoal(WaypointName.Center2, WaypointName.Center3);
					LineCanvas.Top.DisplayLineAsync("Shirley", "15 turns to go.", 2.0f, Util.VoiceLine.fifteen);
					break;
				case WaypointName.Center3:
					UpdateRetryAndGoal(WaypointName.Center3, WaypointName.Center4);
					LineCanvas.Top.DisplayLineAsync("Shirley", "10 turns to go.", 2.0f, Util.VoiceLine.ten);
					break;
				case WaypointName.Center4:
					UpdateRetryAndGoal(WaypointName.Center4, WaypointName.CenterTop);
					LineCanvas.Top.DisplayLineAsync("Shirley", "5 turns to go.", 2.0f, VoiceLine.five);
					break;
				case WaypointName.CenterTop:
					UpdateRetryAndGoal(WaypointName.TownWaypoint, WaypointName.None);
					StartCoroutine(OnCenterTop());
					break;
				case WaypointName.LoopEntrance:
					UpdateRetryAndGoal(WaypointName.LoopEntrance, WaypointName.LoopStart);
					StartCoroutine(OnLoopStart());
					break;
				case WaypointName.LoopStart:
					UpdateRetryAndGoal(WaypointName.LoopStart, WaypointName.LoopEnd);
					LineCanvas.Top.DisplayLineAsync("Shirley", "Let's go!", 1.0f, VoiceLine.letsgo);
					break;
				case WaypointName.LoopEnd:
					UpdateRetryAndGoal(WaypointName.TownWaypoint, WaypointName.None);
					StartCoroutine(OnLoopEnd());
					break;
				case WaypointName.CaveRoom1:
					ObtainCanvas.Inst.Show(Util.Component.Wheel);
					GameSave.Inventory[Util.Component.Wheel] += 4;
					RebuildButton.Inst.StartScale();
					UpdateRetryAndGoal(WaypointName.CaveRoom1, WaypointName.CaveRoom2);
					break;
				case WaypointName.CaveRoom2:
					UpdateRetryAndGoal(WaypointName.CaveRoom2, WaypointName.CaveTrap);
					break;
				case WaypointName.CaveTrap:
					Walls.Inst.TryingWall = true;
					UpdateRetryAndGoal(WaypointName.CaveRoom2, WaypointName.BossStart);
					StartCoroutine(DoTrap());
					break;
				case WaypointName.BossStart:
					Walls.Inst.TryingWall = false;
					UpdateRetryAndGoal(WaypointName.BossStart, WaypointName.None);
					StartCoroutine(HandleBossStart());
					break;
				case WaypointName.TrackEntrance:
					UpdateRetryAndGoal(WaypointName.TrackEntrance, WaypointName.TrackStart);
					StartCoroutine(HandleTrack());
					break;
				case WaypointName.TrackStart:
					UpdateRetryAndGoal(WaypointName.TrackEntrance, WaypointName.TrackDest);
					EventBus.Publish(new TrackStartEvent());
					break;
				case WaypointName.TrackDest:
					UpdateRetryAndGoal(WaypointName.TownWaypoint, WaypointName.None);
					StartCoroutine(HandleTrackEnd());
					break;
				case WaypointName.TourStart:
					GameSave.Inventory[Util.Component.Tourist] = 20;
					UpdateRetryAndGoal(WaypointName.TourStart, WaypointName.Tour1);
					// StartCoroutine(HandleTourStart());
					GoToCheckpointAsync(WaypointName.TourStart, true);
					tour_mode = true;
					tour_mode2 = true;
					break;
				case WaypointName.Tour1:
					UpdateRetryAndGoal(WaypointName.Tour1, WaypointName.Tour2);
					break;
				case WaypointName.Tour2:
					UpdateRetryAndGoal(WaypointName.Tour2, WaypointName.Tour3);
					break;
				case WaypointName.Tour3:
					UpdateRetryAndGoal(WaypointName.Tour3, WaypointName.Tour4);
					break;
				case WaypointName.Tour4:
					UpdateRetryAndGoal(WaypointName.Tour4, WaypointName.Tour5);
					break;
				case WaypointName.Tour5:
					UpdateRetryAndGoal(WaypointName.Tour5, WaypointName.TourDest);
					break;
				case WaypointName.TourDest:
					UpdateRetryAndGoal(WaypointName.TownWaypoint, WaypointName.None);
					StartCoroutine(HandleTourEnd());
					break;
			}
		});
	}
	public Transform platform;
	public Vector3 platform_pos;

	bool tour_mode = false;
	public bool tour_mode2 = false;

	IEnumerator HandleTrack()
	{
		VehicleComponent.Damp = 100;
		CarCore.Inst.DampStart();
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "The boss will chase us once we enter the track.", null, VoiceLine.chase_enter);
		LineCanvas.Top.Hide();
		yield return DisplayChapter("Side Quest III", "Groundhog Festival", "Started", null);
		CarCore.Inst.DampStop();
		VehicleComponent.Damp = 2;
	}

	IEnumerator HandleTrackEnd()
	{
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "That was a tough one.", null, VoiceLine.tough_one);
		LineCanvas.Top.Hide();
		yield return DisplayChapter("Side Quest III", "Groundhog Festival", "Completed", null);
	}
	IEnumerator HandleTourStart()
	{
		CarCore.Inst.DampStart();
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Buckle up, everyone.", null, VoiceLine.buckle_up);
		LineCanvas.Top.Hide();
		yield return DisplayChapter("Side Quest II", "Tour de Piggyland", "Started", null);
		CarCore.Inst.DampStop();
	}
	IEnumerator HandleTourEnd()
	{
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "The tourists enjoyed this trip, I guess.", null, VoiceLine.tourist_guess);
		LineCanvas.Top.Hide();
		yield return DisplayChapter("Side Quest II", "Tour de Piggyland", "Completed", null);
	}

	IEnumerator HandleBossStart()
	{
		PlayCanvas.Inst.Hide();
		CarCore.Inst.DampStart();
		MainCamera.Inst.Stop();
		yield return MainCamera.Inst.WarpTo(TRef.Get(TRefName.CaveCamera2), 2.0f);
		yield return Door.Inst.Close();
		MainCamera.Inst.Shake(1.0f, 1.0f);
		yield return LineCanvas.Bottom.DisplayLine("Shirley", "Oh no! We are trapped!", null, VoiceLine.oh_trapped);
		yield return new WaitForSeconds(1.5f);
		yield return MainCamera.Inst.WarpTo(TRef.Get(TRefName.CaveCamera3), 1.0f);
		AudioPlayer.Inst.PlaySie();
		yield return LineCanvas.Bottom.DisplayLine("Groundhog the Juggernaut", "I have been waiting for you, piggies!", null, VoiceLine.None);
		Boss.Inst.Roar();
		yield return new WaitForSeconds(2.0f);
		Boss.Inst.Idle();
		MainCamera.Inst.MoveAndStickTo(CarCore.Inst.CameraEnd);
		PlayCanvas.Inst.Show();
		PlayCanvas.Inst.ShowAndResetDeathCount();
		HealthBar.Inst.Show();
		CarCore.Inst.DampStop();
		Boss.Inst.BossStartCoroutine = StartCoroutine(Boss.Inst.BossStart());
	}
	public IEnumerator HandleEnding()
	{
		PlayCanvas.Inst.SetStoryText("Main Story Completed");
		PlayCanvas.Inst.ShowStory();
		StoryCanvas.Inst.SetMainStory(Util.MainStoryName.AllCompleted);
		GameSave.Progresses[Util.QuestName.MainStory] = (Util.WaypointName.TownWaypoint, WaypointName.None);
		yield return new WaitForSeconds(2.0f);
		yield return BlackoutCanvas.Inst.Blackout(1.0f, 0.0f, 1.0f);
		AudioPlayer.Inst.TransitionToIntro();
		yield return new WaitForSeconds(1.0f);
		PlayCanvas.Inst.Hide();
		BuildCanvas.Inst.Hide();
		new_spacecraft.gameObject.SetActive(true);
		MainCamera.Inst.Stop();
		MainCamera.Inst.WarpTo(TRef.Get(TRefName.FinalStoryCamera1));
		yield return BlackoutCanvas.Inst.Blackout(1.0f, 1.0f, 0.0f);
		yield return new WaitForSeconds(0.5f);
		yield return MainCamera.Inst.WarpTo(TRef.Get(TRefName.FinalStoryCamera2), 1.0f);
		yield return MainCamera.Inst.WarpTo(TRef.Get(TRefName.FinalStoryCamera3), 2.0f);
		StartCoroutine(FlyRocket());
		yield return new WaitForSeconds(0.7f);
		yield return MainCamera.Inst.WarpTo(TRef.Get(TRefName.FinalStoryCamera4), 2.0f);
		yield return new WaitForSeconds(4.0f);
		yield return BlackoutCanvas.Inst.Blackout(2.0f, 0.0f, 1.0f);
		yield return BlackoutCanvas.Inst.DisplaySubAndFade("The End.", 1.0f, 1.0f);
		yield return BlackoutCanvas.Inst.DisplaySubAndFade("Crafty Piggies 3D\n\nPresented by EECS494 Studio 1", 2.0f, 1.0f);
		yield return BlackoutCanvas.Inst.DisplaySubAndFade("Zheng Luo", 1.0f, 1.0f);
		yield return BlackoutCanvas.Inst.DisplaySubAndFade("Yikai Li", 1.0f, 1.0f);
		yield return BlackoutCanvas.Inst.DisplaySubAndFade("Gabriel Froehner", 1.0f, 1.0f);
		yield return BlackoutCanvas.Inst.DisplaySubAndFade("Javier Guerrero", 1.0f, 1.0f);
		yield return BlackoutCanvas.Inst.DisplaySubAndFade("Special thanks to:\n\n Instructor Prof. Austin Yarger\n\n Music: Erika Chen", 2.0f, 2.0f);
		yield return BlackoutCanvas.Inst.DisplaySubAndFade("Thanks for Playing", 2.0f, 2.0f);
		yield return BeginSideQuest();		
	}

	public bool is_trailer2 = false;
	IEnumerator BeginSideQuest()
	{
		PlayCanvas.Inst.HideDeathCount();		
		side_quests_begin = true;
		if (is_trailer2)
		{
			GameSave.SetGridSize(7, 7, 7);
			GameSave.Inventory[Util.Component.WoodenCrate] = 1000;
			GameSave.Inventory[Util.Component.Wheel] = 1000;
			GameSave.Inventory[Util.Component.MotorWheel] = 1000;
			GameSave.Inventory[Util.Component.TurnWheel] = 1000;
			GameSave.Inventory[Util.Component.Rocket] = 1000;
			GameSave.Inventory[Util.Component.Umbrella] = 1000;
			GameSave.Inventory[Util.Component.Tourist] = 1000;
		}
		else
		{
			GameSave.SetGridSize(3, 4, 4);
			GameSave.Inventory[Util.Component.WoodenCrate] = 100;
			GameSave.Inventory[Util.Component.Wheel] = 100;
			GameSave.Inventory[Util.Component.MotorWheel] = 100;
			GameSave.Inventory[Util.Component.TurnWheel] = 100;
			GameSave.Inventory[Util.Component.Rocket] = 100;
			GameSave.Inventory[Util.Component.Umbrella] = 100;
		}
		
		yield return GoToCheckpoint(WaypointName.TownWaypoint, true);
		ChapterCanvas.Inst.DisplayTextAsync("Main Story: Act II", "Defeat Groundhog the Juggernaut", "Completed", 0.5f, null);
		yield return BlackoutCanvas.Inst.Blackout(1.0f, 1.0f, 0.0f);
	}
	bool side_quests_begin = false;

	//IEnumerator PromptSideQuest()
	//{
	//	yield return null;
	//	CarCore.Inst.DampStart();
	//	yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Woohoo! We returned safely with the Adamantium.", null, VoiceLine.woohoo);
	//	yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "I can't wait to start a new adventure.", null, VoiceLine.i_cant_wait);
	//	LineCanvas.Top.Hide();
	//	StoryCanvas.Inst.SetMainStory(MainStoryName.BossFight);
	//	GameSave.Progresses[Util.QuestName.MainStory] = (Util.WaypointName.TownWaypoint, WaypointName.CaveEntrance);
	//	yield return DisplayChapter("Main Story Quest: Act 1", "Treasures in the Flaming Mountain", "Completed");
	//	yield return WaitForClick();
	//	yield return new WaitForSeconds(1.5f);
	//	yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Now, would you like to have some fun in the town, or continue our journey for home?", null, VoiceLine.now_would);
	//	LineCanvas.Top.Hide();
	//	PlayCanvas.Inst.ShowStory();
	//	StoryCanvas.Inst.ShowSideQuests();
	//	PlayCanvas.Inst.SetStoryText("Select a Quest");
	//	PlayCanvas.Inst.ScaleStory();
	//	CarCore.Inst.DampStop();
	//}

	float fly_acceleration = 5.0f;
	public IEnumerator FlyRocket()
	{
		float start_time = Time.time;
		Vector3 initial_position = new_spacecraft.position;
		while (true)
		{
			float new_y = initial_position.y + (Time.time - start_time) * (Time.time - start_time) * fly_acceleration;
			new_spacecraft.position = new Vector3(initial_position.x, new_y, initial_position.z);
			yield return null;
		}
	}
	public Transform new_spacecraft;
	IEnumerator SinkPlatform()
	{
		float start_time = Time.time;
		float duration = 2.0f;
		Vector3 start_pos = platform.transform.position;
		Vector3 end_pos = start_pos - new Vector3(0, 10, 0);
		while (Time.time - start_time < duration)
		{
			platform.transform.position = Vector3.Lerp(start_pos, end_pos, (Time.time - start_time) / duration);
			yield return null;
		}
	}
	IEnumerator DoTrap()
	{
		CarCore.Inst.DampStart();
		MainCamera.Inst.Stop();
		yield return MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CaveCamera1), 1.5f);
		MainCamera.Inst.Shake(1.5f, 1.0f);
		StartCoroutine(SinkPlatform());
		yield return LineCanvas.Bottom.DisplayLine("Shirley", "Oh no! We are trapped!",null, VoiceLine.oh_trapped);
		yield return new WaitForSeconds(1.0f);
		MainCamera.Inst.MoveAndStickTo(CarCore.Inst.CameraEnd);
		CarCore.Inst.DampStop();
		Walls.Inst.StartWarp();
		yield return new WaitForSeconds(1.0f);
		yield return LineCanvas.Bottom.DisplayLine("Groundhog the Juggernaut", "I know you will come, poor little piggies!",null, VoiceLine.i_know_you);
		yield return new WaitForSeconds(2.0f);
		yield return LineCanvas.Bottom.DisplayLine("Groundhog the Juggernaut", "You'll never get what you want. Take this!",null, VoiceLine.you_never_get);
		yield return new WaitForSeconds(3.0f);
		yield return LineCanvas.Bottom.DisplayLine("Shirley", "Watch out the walls!",null, VoiceLine.watch_out_wall);
		yield return new WaitForSeconds(2.0f);
		LineCanvas.Bottom.Hide();
	}
	IEnumerator HintDesign3()
	{
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "This is a tricky one.", null, VoiceLine.this_is);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "But I've already come up with some evil thoughts.", null, VoiceLine.but);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Hint: the goal is pretty close to the wall.", null, VoiceLine.hint);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Haha, I saw you blushed.", null, VoiceLine.haha);
		LineCanvas.Top.Hide();
	}
	public IEnumerator IntroduceSpace()
	{
		ConfirmButton.Inst.EnableConfirm = false;
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Now there's an **important** feature that comes in handy.", null, VoiceLine.now_there);
		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "**Press \"Space\" to toggle build layers.**", (SwitchLayerEvent e) => true, VoiceLine.press_space);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Perfect! Since your build space is incrementing, it would be hard to locate a cell without specifying layers.", null, VoiceLine.perfect_since);
		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "Now, press \"Space\" a few more times to go back to the full layer mode.", (FullLayerEvent e) => true, VoiceLine.now_press);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Great! Let's move on!", null, VoiceLine.great);
		LineCanvas.Top.Hide();
		ConfirmButton.Inst.EnableConfirm = true;
	}

	IEnumerator OnCenterEntrance()
	{
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "A journey of a thousand miles begins with a single step.", null, VoiceLine.A_journey);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Let's climb!", null, VoiceLine.lets_climb);
		LineCanvas.Top.Hide();
		yield return DisplayChapter("Side Quest IV", "A Bird's Eye View", "Started", null);
	}
	IEnumerator OnCenterTop()
	{
		PlayCanvas.Inst.SetStoryText("Select a Quest");
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Woohoo! We made it!", null, VoiceLine.woohoo_we_made);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Although there is barely anything underneath to appreciate.", null, VoiceLine.Although);
		LineCanvas.Top.Hide();
		yield return DisplayChapter("Side Quest IV", "A Bird's Eye View", "Completed", null);
	}
	IEnumerator OnLoopStart()
	{
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Don't be a coward, my darling.", null, VoiceLine.dont);
		CarCore.Inst.DampStart();
		yield return DisplayChapter("Side Quest I", "Loop", "Started", null);
		CarCore.Inst.DampStop();
		LineCanvas.Top.Hide();
	}
	IEnumerator OnLoopEnd()
	{
		PlayCanvas.Inst.SetStoryText("Select a Quest");
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Awwwww! That's more scary than I anticipated.", null, VoiceLine.awwww);
		LineCanvas.Top.Hide();
		yield return DisplayChapter("Side Quest I", "Loop", "Completed", null);
	}
	bool prompt_choose_quest1 = false;
	bool prompt_choose_quest2 = false;

	public GameObject story_animation;
	IEnumerator CollectedObsidian()
	{
		// PlayCanvas.Inst.HideStory();
		VehicleComponent.Damp = 100.0f;
		CarCore.Inst.DampStart();
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "We found the treasure of the evil creature! Hooray!", null, VoiceLine.treasure_evil);
		LineCanvas.Top.Hide();
		StoryCanvas.Inst.SetMainStory(MainStoryName.BossFight);
		GameSave.Progresses[Util.QuestName.MainStory] = (Util.WaypointName.TownWaypoint, WaypointName.CaveEntrance);
		yield return DisplayChapter("Main Story Quest: Act 1", "Treasures in the Flaming Mountain", "Completed", null);
		yield return WaitForClick();
		// yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Now we can warp back to the town and prepare for the fight!", null, VoiceLine.now_we);
		PlayCanvas.Inst.Hide();
		MainCamera.Inst.Stop();
		yield return MainCamera.Inst.WarpTo(TRef.Get(TRefName.VolcanoCamera), 1.0f);
		yield return new WaitForSeconds(0.5f);
		LavaSink.Inst.Sink();
		MainCamera.Inst.Shake(2.0f, 1.0f);
		yield return new WaitForSeconds(2.0f);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "This must be the hive of the boss. Let's jump into it!", null, VoiceLine.boss_hive);
		LineCanvas.Top.Hide();
		MainCamera.Inst.MoveAndStickTo(CarCore.Inst.CameraEnd);
		CarCore.Inst.DampStop();
		yield return DisplayChapter("Main Story Quest: Act II", "Defeat Groundhog the Juggernaut", "Started", null);
		yield return WaitForClick();
		yield return new WaitForSeconds(1.0f);
		PlayCanvas.Inst.Show();
		VehicleComponent.Damp = 2.0f;
		prompt_choose_quest1 = true;
		UpdateRetryAndGoal(WaypointName.Obsidian, WaypointName.CaveRoom1);
	}
	private void Start()
	{
		platform_pos = platform.position;
		// temporary shut down
		Debug.Assert(inst == null, "Game State already instantiated");
		inst = this;
		Util.Delay(this, () =>
		{
			BuildCanvas.Inst.HideDesignNumbers();
			Yikai();
		});
		EventBus.Subscribe<GoalReachedEvent>(OnGoalReached);
		EventBus.Subscribe<TouchLavaEvent>(OnTouchLava);
		EventBus.Subscribe<ScanSuccessEvent>(OnScanSuccess);
		EventBus.Subscribe<ScanFailEvent>(OnScanFail);
		EventBus.Subscribe<CheckpointReachedEvent>(OnCheckpointReached);
		EventBus.Subscribe<WallRetryEvent>(OnWallRetry);
		Util.Delay(this, 1, ()=> { MapCanvas.Inst.Deactivate(); });
	}

	void OnWallRetry(WallRetryEvent e)
	{
		platform.position = platform_pos;
	}
	IEnumerator ScanSuccessHelper()
	{
		PlayCanvas.Inst.Hide();
		Debug.LogWarning($"joints: {CarCore.Inst.joint != null}, {CarCore.Inst.fix_joint != null}");
		CarCore.Inst.ActivateContainer();
		CarCore.Inst.DestroyComponents();		
		CarCore.Inst.Unfix();
		yield return null;
		yield return CarCore.Inst.AlignToGridMatrix();
		CarCore.Inst.Fix();
		BuildCanvas.Inst.Show();
		BuildCanvas.Inst.InitializeItems();
		yield return GridMatrix.Inst.Activate();
	}
	void OnScanSuccess(ScanSuccessEvent e)
	{
		StartCoroutine(ScanSuccessHelper());
	}
	public void GoBack()
	{
		PlayCanvas.Inst.Show();
		BuildCanvas.Inst.Hide();
		GridMatrix.Inst.Deactivate();
		CarCore.Inst.Unfix();
		CarCore.Inst.ActivateContainer();
	}
	void OnScanFail(ScanFailEvent e)
	{
		CarCore.Inst.ActivateContainer();
		CarCore.Inst.Unfix();
	}
	public IEnumerator GoToCheckpoint(Util.WaypointName waypoint_name, bool camera_follow)
	{
		if (camera_follow)
		{
			MainCamera.Inst.Stop();
		}
		AudioPlayer.Inst.StopSoundEffect();
		PlayCanvas.Inst.Hide();
		GridMatrix.Inst.MoveToCheckpoint(waypoint_name);
		yield return null;
		Debug.Assert(!GridMatrix.Inst.Active);
		CarCore.Inst.DestroyComponents();
		yield return null;
		yield return CarCore.Inst.AlignToGridMatrix();
		Debug.Log("Fix!");
		CarCore.Inst.Fix();
		BuildCanvas.Inst.Show();
		BuildCanvas.Inst.InitializeItems();
		yield return GridMatrix.Inst.Activate();
		GridMatrix.Inst.MoveProbeToOrigin();
		CarCore.Inst.ResetPivot();
		if (camera_follow)
		{
			MainCamera.Inst.MoveAndStickTo(CarCore.Inst.CameraEnd);
		}
		if (prompt_choose_quest1 && waypoint_name == WaypointName.TownWaypoint)
		{
			prompt_choose_quest1 = false;
			prompt_choose_quest2 = true;
		}
	}
	public void GoToCheckpointAsync(Util.WaypointName waypoint_name, bool camera_follow = true)
	{
		StartCoroutine(GoToCheckpoint(waypoint_name, camera_follow));
	}
	private void OnDestroy()
	{
		inst = null;
	}
	//void OnNext(NextEvent e)
	//{
	//	if (current_level_num >= Util.WaypointItems.Count) // count starting from 1
	//	{
	//		ToastManager.Toast("More levels coming soon!\nThanks for playing!");
	//		return;
	//	}
	//	current_level_num++;
	//	TransitionToIntro();
	//}
	public void OnRetry()
	{
		//// PiggyCameraPivot.Inst.EndFollow();
		//camera_follow_pig = false;
		//PiggyPermitInvisible = false;
		//Util.BuildInfo build_info = last_choice_name == Util.ChoiceName.NeedHelp ? Util.BuildInfo.NeedHelp : Util.BuildInfo.DontNeedHelpButRetry;
		//TransitionToBuild(retry_waypoint, retry_goal, build_info);
		if (GameSave.CurrentCheckpoint == WaypointName.TrackEntrance)
		{
			Checkpoint.Checkpoints[WaypointName.TrackStart].Activate();
		}
		GoToCheckpointAsync(GameSave.CurrentCheckpoint);
		if (Walls.Inst.TryingWall)
		{
			EventBus.Publish(new WallRetryEvent());
		}
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A)
			|| Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
		{
			EventBus.Publish(new WASDPressedEvent());
		}
		CheatCode();
	}

	bool play_canvas_hided = false;
	public TrailerMenu trailer_menu;
	void CheatCode()
	{
		//if (Input.GetKeyDown(KeyCode.Y))
		//{
		//	TryScan();
		//}
		//if (Input.GetKeyDown(KeyCode.E))
		//{
		//	CarCore.Inst.Move();
		//}
		//if (Input.GetKeyDown(KeyCode.R))
		//{
		//	GoToCheckpointAsync(Util.WaypointName.PreStory1);
		//}
		//if (Input.GetKeyDown(KeyCode.T))
		//{
		//	GoToMap();
		//}
		if (Input.GetKeyDown(KeyCode.R))
		{
			OnRetry();
		}
		
		//if (Input.GetKeyDown(KeyCode.C))
		//{
		//	trailer_menu.ShowTitle();
		//}
		//if (Input.GetKeyDown(KeyCode.V))
		//{
		//	trailer_menu.ShowText1();
		//}
		//if (Input.GetKeyDown(KeyCode.B))
		//{
		//	trailer_menu.ShowText2();
		//}
		if (Input.GetKeyDown(KeyCode.U))
		{
			trailer_menu.HideAll();
		}
		if (Input.GetKeyDown(KeyCode.H))
		{
			play_canvas_hided = !play_canvas_hided;
			if (play_canvas_hided)
			{
				PlayCanvas.Inst.Hide();
			}
			else
			{
				PlayCanvas.Inst.Show();
			}
		}
		if (Input.GetKeyDown(KeyCode.Return))
		{
			CarCore.Inst.Unfix();
			CarCore.Inst.DestroyComponents();
			CarCore.Inst.UnbindPig();
			GridMatrix.Inst.Cleanup();
		}
		//for(int i = 0;i < 7;i++)
		//{
		//	List<Action> funcs = new()
		//	{
		//		Boss.Inst.Rush, Boss.Inst.Pant, Boss.Inst.Smash, Boss.Inst.Spell, Boss.Inst.Knockout, Boss.Inst.Hit, Boss.Inst.Roar
		//	};
		//	if (Input.GetKeyDown((KeyCode)((int)(KeyCode.Alpha1) + i)))
		//	{
		//		funcs[i]();
		//	}
		//}
	}
	public void GoToMap()
	{
		MainCamera.Inst.Stop();
		MainCamera.Inst.MoveAndStickTo(BigMapCamera.Inst.transform);
		MapCanvas.Inst.Activate();
		BigMapCamera.Inst.Activate();
		PlayCanvas.Inst.Hide();
	}
	public void DampStart()
	{
		foreach (var component in Components)
		{
			component.DampStart();
		}
	}
	public void DampStop()
	{
		foreach (var component in Components)
		{
			component.DampStop();
		}
	}

	void DestroyComponentsInScene()
	{
		foreach(var component in Components)
		{
			Destroy(component.gameObject);
		}
		Components.Clear();
		Piggy = null;
	}
	public void TransitionToStory(Util.StoryName story_name)
	{
		if (story_name == Util.StoryName.Crash)
		{
			AudioPlayer.Inst.TransitionToStory();
		}
		Goal.Deselect();
		PlayCanvas.Inst.Hide();
		// PiggyCameraPivot.Inst.EndFollow();
		PiggyPermitInvisible = false;
		MainCamera.Inst.Stop();
		EventBus.Publish(new InvisibleStateUpdateEvent());
		if (story_name != Util.StoryName.Intro && story_name!= Util.StoryName.FallOffCliff
			&& story_name != Util.StoryName.InTown && story_name != Util.StoryName.C1S2)
		{
			DestroyComponentsInScene();
		}
		switch (story_name)
		{
			case Util.StoryName.Crash:
				StartCoroutine(TransitionToStoryCrash());
				break;
			case Util.StoryName.Intro:
				StartCoroutine(TransitionToStoryIntro());
				break;
			case Util.StoryName.FallOffCliff:
				StartCoroutine(TransitionToStoryCliff());
				break;
			case Util.StoryName.InTown:
				StartCoroutine(TransitionToStoryInTown());
				break;
			case Util.StoryName.TownWaypoint:
				StartCoroutine(TransitionToStoryTownWaypoint());
				break;
			case Util.StoryName.C1S1:
				StartCoroutine(TransitionToStoryC1S1());
				break;
			case Util.StoryName.C1S2:
				// StartCoroutine(TransitionToStoryC1S2());
				break;
		}
	}
	IEnumerator TransitionToStoryC1S1()
	{
		yield return MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraC1S1), 1.0f);
		// yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "It seems you've recovered. Let's travel around and see if anyone knows about your girlfriend.", Character.Partner);
		ChoiceCanvas.Inst.DisplayChoices(new() { ("Sure. Let's go!", Util.ChoiceName.DontCare), ("Definitely!", Util.ChoiceName.DontCare), ("Absolutely!", Util.ChoiceName.DontCare) });
		Util.ChoiceObj choice_obj = new();
		yield return WaitForChoice(choice_obj);
		LineCanvas.Bottom.Hide();
		Character.Partner.WarpTo(TRef.Get(Util.TRefName.Origin));
		Character.GetCharacter(Util.CharacterName.NPC1).WarpTo(TRef.Get(Util.TRefName.NPC1C1S2));
		Character.GetCharacter(Util.CharacterName.NPC2).WarpTo(TRef.Get(Util.TRefName.NPC2C1S2));
		Character.GetCharacter(Util.CharacterName.NPC3).WarpTo(TRef.Get(Util.TRefName.NPC3C1S2));
		// TransitionToBuild(Util.WaypointName.C1S1, Util.GoalName.C1S2);
	}
	IEnumerator WaitForClick()
	{
		while (!Input.GetMouseButtonDown(0)) {
			yield return null;
		}
	}
	IEnumerator TransitionToStoryTownWaypoint()
	{
		// yield return BlackoutCanvas.Inst.Blackout(1.0f, true);
		// yield return null;
		// yield return LineCanvas.Bottom.DisplayLine("Shirley","Arrived!");
		Character.Partner.WarpTo(TRef.Get(Util.TRefName.PartnerTownW));
		yield return MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraTownW1), 1.0f);
		//yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "Congratulations! You found the Waypoint of the town.", Character.Partner);
		//yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "Waypoints are scattered across the world that enables you to rebuild your vehicle.", Character.Partner);
		//yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "The waypoint in the town opens for free to you, but you will have to complete challenging challenges to unlock some of them in the wild.", Character.Partner);
		//yield return MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraTownW2), 1.0f);
		//yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Waypoint de New Sorpigal", "As long as you do not lose faith, the world will open to you.", null);
		//// Waypoint.Waypoints[Util.WaypointName.Town].ChangeToGreen();
		//yield return WaitForClick();
		//yield return MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraTownW1), 1.0f);
		//yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "Let's try it out!", Character.Partner);
		//LineCanvas.Bottom.Hide();
		Character.Partner.WarpTo(TRef.Get(Util.TRefName.Origin));
		// TransitionToBuild(Util.WaypointName.Town, Util.GoalName.None);
	}

	public void Test(Action func)
	{
		try
		{
			func();
		}
		catch (Exception e)
		{
			ToastManager.Toast($"{e.Message}\n{e.StackTrace}");
		}
	}
	IEnumerator PlayCrash()
	{
		yield return new WaitForSeconds(2.0f);
		AudioPlayer.Inst.PlayCrash();
	}
	IEnumerator TransitionToStoryCrash()
	{
		
		//yield return BlackoutCanvas.Inst.Blackout(1.0f, 1.0f, 1.0f);
		//	yield return BlackoutCanvas.Inst.DisplaySub("You and your girlfriend's spaceship crashed to this planet because of an attack.", 1.5f, 0.0f, 1.0f);
		//	yield return WaitForClick();
		//	yield return BlackoutCanvas.Inst.DisplaySub("You and your girlfriend's spaceship crashed to this planet because of an attack.", 0.5f, 1.0f, 0.0f);
		Test(() =>
		{
			MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraPrestory1_1));
		});
		StartCoroutine(PlayCrash());
		yield return LineCanvas.Bottom.WaitForEvent((PigfallAnimationEndEvent e) => true);
		Character.Piggy.WarpTo(TRef.Get(TRefName.PigPrestory1));
		Character.Partner.WarpTo(TRef.Get(TRefName.PartnerPrestory1));
		yield return new WaitForSeconds(1.0f);
		yield return AtTheSameTime(
			Character.Piggy.WarpTo(TRef.Get(TRefName.PigPrestory2), 0.5f),
			Character.Partner.WarpTo(TRef.Get(TRefName.PartnerPrestory2), 0.5f)
			);
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "Honey, I didn't see our rocket engines. They may have been stolen.", Character.Partner, VoiceLine.honey);
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("You", "Without them, we can't leave this planet.", Character.Piggy, VoiceLine.without);
		yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "Let's travel around and find some clues.", Character.Partner, VoiceLine.lets_travel);
		LineCanvas.Bottom.Hide();
		Character.Piggy.WarpTo(TRef.Get(TRefName.Origin));
		Character.Partner.WarpTo(TRef.Get(TRefName.Origin));
		yield return TransitionToFirstBuild();
	}
	public float rise_time = 5.0f;

	public IEnumerator AtTheSameTime(IEnumerator task1, IEnumerator task2)
	{
		var coroutine = StartCoroutine(task1);
		yield return task2;
		yield return coroutine;
	}
	IEnumerator TransitionToStoryIntro()
	{
		Goal.Select(Util.GoalName.FallOffCliff);
		MainCamera.Inst.Stop();
		PiggyPermitInvisible = false;
		EventBus.Publish(new InvisibleStateUpdateEvent());
		float start_time = Time.time;
		Vector3 initial_position = MainCamera.Inst.transform.position;
		Quaternion initial_rotation = MainCamera.Inst.transform.rotation;
		Transform introCameraTransform = IntroCamera.Inst.transform;
		AudioPlayer.Inst.TransitionToTitle();
		IntroCanvas.Inst.Play(2.5f);
		while (Time.time - start_time < rise_time)
		{
			MainCamera.Inst.transform.position = Vector3.Lerp(initial_position, introCameraTransform.position, (Time.time - start_time) / rise_time);
			Quaternion target_rotation = Quaternion.Slerp(initial_rotation, introCameraTransform.rotation, (Time.time - start_time) / rise_time);
			// Vector3 look_dir = Piggy.transform.position - MainCamera.Inst.transform.position;
			// Quaternion lookat_rotation = Quaternion.LookRotation(look_dir);
			MainCamera.Inst.transform.rotation = Quaternion.Slerp(initial_rotation, target_rotation, (Time.time - start_time) / rise_time);
			yield return null;
		}
		MainCamera.Inst.transform.position = introCameraTransform.position;
		MainCamera.Inst.transform.rotation = introCameraTransform.rotation;
	}
	public float shake_duration = 2.0f;
	public float shake_intensity = 1.0f;
	Util.ChoiceName last_choice_name = Util.ChoiceName.None;
	IEnumerator TransitionToStoryCliff()
	{
		MainCamera.Inst.Stop();
		Debug.Log("Falling off cliff!");
		AudioPlayer.Inst.Wilhelm();
		yield return new WaitForSeconds(1.5f);
		Vector3 original_position = MainCamera.Inst.transform.position;
		float elapsedTime = 0f;
		AudioPlayer.Inst.Scream(null);
		while (elapsedTime < shake_duration)
		{
			// Calculate vibration offset using Perlin noise
			float x = (Mathf.PerlinNoise(Time.time * 10, 0) - 0.5f) * 2 * shake_intensity;
			float y = (Mathf.PerlinNoise(0, Time.time * 10) - 0.5f) * 2 * shake_intensity;

			// Apply vibration offset to the original position
			MainCamera.Inst.transform.position = original_position + new Vector3(x, y, 0);

			elapsedTime += Time.deltaTime;
			yield return null; // Wait for the next frame
		}

		Retry.Inst.Hide();
		// Reset to the original position
		MainCamera.Inst.transform.position = original_position;

		yield return GoToCheckpoint(Util.WaypointName.Cliff, false);
		// yield return new WaitForSeconds(5.0f);
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		ConfirmButton.Inst.ForceConfirmClicked();
		// DestroyComponentsInScene();
		yield return null;
		MainCamera.Inst.MoveAndStickTo(CarCore.Inst.CameraEnd);

		// GameSave.Inventory[Util.Component.MotorWheel] = 2;
		// ObtainCanvas.Inst.Show(Util.Component.MotorWheel);
		GridMatrix.Inst.ForceDesign = false;

		// Goal.Activate(Util.GoalName.TurnWheel);
		Checkpoint.Get(Util.WaypointName.MotorWheel).Activate();
		GoalCanvas.Inst.CheckpointToFollow = Util.WaypointName.MotorWheel;
		// GameSave.CurrentCheckpoint = Util.WaypointName.PreStory2;
		// GoToCheckpointAsync(Util.WaypointName.PreStory2);
		
		// BackButton.Inst.Hide();
		yield return new WaitForSeconds(0.5f);
		PlayCanvas.Inst.ShowStory();
		PlayCanvas.Inst.SetStoryText("Go to the town");
		StoryCanvas.Inst.SetMainStory(MainStoryName.GoToTown);

		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Oh, no! We turned over!", null, VoiceLine.ohno);
		RebuildButton.Inst.Show();
		RebuildButton.Inst.StartScale();
		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "Press the wrench icon to rebuild.", (RebuildButtonClickedEvent e) => true, VoiceLine.press_the);
		LineCanvas.Top.Hide();
	}
	IEnumerator WaitForChoice(Util.ChoiceObj choice_obj)
	{
		Action<ChoiceSelectedEvent> handler = (ChoiceSelectedEvent e) => { choice_obj.choice_name = e.choice_name; };
		var subscription = EventBus.Subscribe<ChoiceSelectedEvent>(handler);
		while(choice_obj.choice_name == Util.ChoiceName.None)
		{
			yield return null;
		}
		EventBus.Unsubscribe(subscription);
	}
	IEnumerator TransitionToStoryInTown()
	{
		yield break;
		//DampStart();
		//yield return MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraInTown1), 1.5f);
		//yield return AtTheSameTime(
		//	MainCamera.Inst.WarpTo(TRef.Get(Util.TRefName.CameraInTown2), 1.5f),
		//	LineCanvas.Bottom.DisplayLine("Shirley", "We've arrived! Let's explore the town."));
		//yield return new WaitForSeconds(1.0f);
		//LineCanvas.Bottom.Hide();
		//DampStop();
		//// MainCamera.Inst.FollowStory();
		//yield return new WaitForSeconds(1.0f);
		//TransitionToPlay(false);
		//Goal.Activate(Util.GoalName.Town);

		//BlackoutCanvas.Inst.Blackout(3.0f, 3.0f, () =>
		//{
		//	TransitionToBuild(Util.WaypointName.PreStory1, Util.GoalName.PreStory1);
		//});
	}
	//public void TransitionToIntro()
	//{
	//	DestroyComponentsInScene();
	//	PiggyCameraPivot.Inst.EndFollow();
	//	ToastManager.Toast($"Level {current_level_num}");

	//	StartCoroutine(PlayAnimation());
	//}
	static HashSet<Util.WaypointName> can_retry_waypoints = new()
	{
		Util.WaypointName.PreStory1,
		// Util.WaypointName.PreStory2
	};

	IEnumerator TransitionToFirstBuild()
	{
		Test(() =>
		{
			Debug.Assert(GameSave.GridMemories[0].MemCrates.GetLength(0) == 2);
			Debug.Assert(GameSave.GridMemories[0].MemCrates.GetLength(1) == 2);
			Debug.Assert(GameSave.GridMemories[0].MemCrates.GetLength(2) == 3);
			GameSave.GridMemories[0].Clear();
			GameSave.GridMemories[0].MemCrates[0, 0, 0] = Util.Component.WoodenCrate;
			GameSave.GridMemories[0].MemCrates[1, 0, 0] = Util.Component.WoodenCrate;
			GameSave.GridMemories[0].MemCrates[1, 1, 0] = Util.Component.WoodenCrate;
			GameSave.GridMemories[0].MemCrates[1, 1, 1] = Util.Component.WoodenCrate;
			GameSave.GridMemories[0].MemAccessories[1, 0, 1] = Util.Component.Wheel;
			GameSave.GridMemories[0].MemAccessories[1, 1, 2] = Util.Component.Wheel;
			GameSave.GridMemories[0].AccessoryDirections[1, 0, 1] = 1;
			GameSave.GridMemories[0].AccessoryDirections[1, 1, 2] = 2;
		});
		
		yield return null;

		yield return GoToCheckpoint(Util.WaypointName.PreStory1, true);
		Test(() =>
		{
			DragImage.Current = null;
			// GridMatrix.DeselectGridMatrix();
			// GridMatrix.SelectGridMatrix(waypoint_name, build_info != Util.BuildInfo.NeedHelp);
			Goal.Activate(Util.GoalName.PreStory1);
			// MainCamera.Inst.MoveAndStickToGridMatrix(0.5f, 0.5f, 0.5f);
			PiggyPermitInvisible = false;
			StartCoroutine(Prestory1Build());
		});
	}

	//void TransitionToBuild(Util.WaypointName waypoint_name, Util.GoalName goal_name, Util.BuildInfo build_info = Util.BuildInfo.NeedHelp)
	//{
	//	AudioPlayer.Inst.TransitionToStory();
	//	current_waypoint = waypoint_name;
	//	retry_waypoint = waypoint_name;
	//	retry_goal = goal_name;
	//	//if (can_retry_waypoints.Contains(waypoint_name))
	//	//{

	//	//}
	//	//else
	//	//{
	//	//	// waypoint_name = Util.WaypointName.None;
	//	//	retry_waypoint = Util.WaypointName.None;
	//	//	retry_goal = Util.GoalName.None;
	//	//}
	//	BuildCanvas.Inst.Show();
	//	BuildCanvas.Inst.InitializeItems();
	//	GridMatrix.Inst.ActivateAsync();
	//	GridMatrix.Inst.MoveToCheckpoint(Util.WaypointName.PreStory1);
	//	PlayCanvas.Inst.Hide();
	//	// AudioPlayer.Inst.TransitionToBuild();
	//	DestroyComponentsInScene();		
	//	DragImage.Current = null;
	//	GridMatrix.DeselectGridMatrix();
	//	GridMatrix.SelectGridMatrix(waypoint_name, build_info != Util.BuildInfo.NeedHelp);
	//	if (goal_name != Util.GoalName.None)
	//	{
	//		Goal.Select(goal_name);
	//	}
	//	// MainCamera.Inst.MoveAndStickToGridMatrix(0.5f, 0.5f, 0.5f);
	//	PiggyPermitInvisible = false;
	//	// PiggyCameraPivot.Inst.EndFollow();

	//	switch(waypoint_name)
	//	{
	//		case Util.WaypointName.PreStory1:
	//			StartCoroutine(Prestory1Build());
	//			break;
	//		//case Util.WaypointName.PreStory2:
	//		//	StartCoroutine(Prestory2Build(build_info));
	//		//	break;
	//		//case Util.WaypointName.Town:
	//		//	if (!town_waypoint_met)
	//		//	{
	//		//		town_waypoint_met = true;
	//		//		StartCoroutine(TownWaypointBuild());
	//		//	}
	//		//	break;
	//		//case Util.WaypointName.Volcano:
	//		//	StartCoroutine(VolcanoBuild());
	//		//	break;
	//		//case Util.WaypointName.VolcBottom:
	//		//	StartCoroutine(VolcBottomBuild());
	//		//	break;
	//		//case Util.WaypointName.VolcTop:
	//		//	StartCoroutine(VolcTopBuild());
	//		//	break;
	//	}
	//}
	IEnumerator VolcanoBuild()
	{
		yield return new WaitForSeconds(1.5f);
		// yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "The volcano is too tall, so we have to take a detour.", null);
		// yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "The destination is deep, so be prepared.", null);
		LineCanvas.Top.Hide();
	}
	IEnumerator VolcBottomBuild()
	{
		yield return new WaitForSeconds(1.5f);
		// yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "The incline is too steep. We can't make it with only wheels.", null);
		LineCanvas.Top.Hide();
	}
	IEnumerator VolcTopBuild()
	{
		yield return new WaitForSeconds(1.5f);
		// yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Let's take a leap of faith.", null);
		LineCanvas.Top.Hide();
	}
	bool town_waypoint_met = false;
	IEnumerator TrailerBuild()
	{
		PlayCanvas.Inst.SetStoryText("Go to Town");
		yield return GoToCheckpoint(Util.WaypointName.PreStory1, true);
		MapImage.Inst.Hide();
		RebuildButton.Inst.Hide();
		Retry.Inst.Hide();
	}
	IEnumerator Prestory1Build()
	{
		yield return new WaitForSeconds(1.5f);

		MapImage.Inst.Hide();
		RebuildButton.Inst.Hide();
		Retry.Inst.Hide();

		Trash.Inst.gameObject.SetActive(false);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Hope you still remember how to use the grid building system.", null, VoiceLine.hopeyou);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Oh! There is a mess! Let's clean it up using the eraser!", null, VoiceLine.oh_there);
		EraserImage.Inst.StartScale();
		//// to do: display line and wait for event

		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "Start by clicking the eraser.", (ToolClickedEvent e) =>
		{
			return e.cursor_mode == Util.CursorMode.Erase;
		}, VoiceLine.start_by);
		EraserImage.Inst.EndScale();
		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "Good! Hover your mouse on the grid and click to erase a component.",
			(ItemErasedEvent e) => true, VoiceLine.good);
		Trash.Inst.gameObject.SetActive(true);
		Trash.Inst.StartScale();
		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "Perfect! Now let's use the trashcan to remove all the components at once!",
			(ResetCountEvent e) => true, VoiceLine.perfect_now_lets);
		Trash.Inst.EndScale();
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Perfect! Now we have a clear space to build our vehicle!", null, VoiceLine.perfect_now_we);
		// GridMatrix.Current.ShowDesign();

		// force design
		GridMatrix.Inst.ShowDesign(Util.DesignPrestory1());
		GridMatrix.Inst.ForceDesign = true;
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Let's first use the standard vehicle design.", null, VoiceLine.lets_first);
		DragImage.StartScaleAll();
		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "Start by clicking on a component icon.",
			(DragImageClickedEvent e) => true, VoiceLine.start_by_clicking);
		DragImage.EndScaleAll();
		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "Perfect! Hover your mouse on the grid and click to place a component.",
			(ComponentAddedEvent e) => true, VoiceLine.prefect_hover);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Perfect! Let's place the rest of the components.", null, VoiceLine.perfect_lets);
		yield return LineCanvas.Top.WaitForEvent((ReadyToGoEvent e) => true);
		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "You are learning fast! Now click the confirm button to start our journey!",
			(ConfirmSuccessEvent e) => true, VoiceLine.you_are_learning);
		LineCanvas.Top.Hide();
	}

	public IEnumerator Prestory2Build(Util.BuildInfo build_info)
	{
		StoryAnimation.Inst.CanSpeedup = false;
		yield return new WaitForSeconds(1.5f);
		switch (build_info)
		{
			case Util.BuildInfo.NeedHelp:
				{
					ConfirmButton.Inst.EnableConfirm = false;
					// yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Follow the design to build the vehicle.", null);
					LineCanvas.Top.Hide();
					ConfirmButton.Inst.EnableConfirm = true;
					yield return LineCanvas.Top.WaitForEvent((ReadyToGoEvent e) => true);
					// yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "Let's roll!",
					// 	(ConfirmSuccessEvent e) => true);
					LineCanvas.Top.Hide();
				}
				break;
			case Util.BuildInfo.DontNeedHelp:
				{
					ConfirmButton.Inst.EnableConfirm = false;
					// yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Try to build the vehicle yourself!", null);
					LineCanvas.Top.Hide();
					ConfirmButton.Inst.EnableConfirm = true;
					// yield return LineCanvas.Top.WaitForEvent((ConfirmSuccessEvent e) => true);
				}
				break;
			case Util.BuildInfo.DontNeedHelpButRetry:
				{
					ConfirmButton.Inst.EnableConfirm = false;
					// yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "It seems you had a rough time. Would you like to get some hint?", null);
					ChoiceCanvas.Inst.DisplayChoices(new() { ("Ok, I need some help.", Util.ChoiceName.NeedHelp), ("No way. Let me try it myself!", Util.ChoiceName.DontNeedHelp) });
					Util.ChoiceObj choice_obj = new();
					last_choice_name = choice_obj.choice_name;
					yield return WaitForChoice(choice_obj);
					if (choice_obj.choice_name == Util.ChoiceName.DontNeedHelp)
					{
						// yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "All right. Good luck!", null);
						LineCanvas.Top.Hide();
						ConfirmButton.Inst.EnableConfirm = true;
					}
					else
					{
						// yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Good Choice! Let's figure it out together!", null);
						LineCanvas.Top.Hide();
						ConfirmButton.Inst.EnableConfirm = true;
						// TransitionToBuild(Util.WaypointName.PreStory2, Util.GoalName.PreStory2, Util.BuildInfo.NeedHelp);
						yield break;
					}
				}
				break;
		}
	}
	public IEnumerator TownWaypointBuild()
	{
		ConfirmButton.Inst.EnableConfirm = false;
		yield return new WaitForSeconds(1.5f);

		//yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "**Drag the screen to view the grid**", (CanvasDragEvent e) => true);
		//yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Perfect! Now there's an **important** feature that you want to learn", null);
		//yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "**Press \"Space\" to toggle build layers.**", (SwitchLayerEvent e) => true);
		//yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Perfect! Without previous design constraints, it would be hard to locate a cell without specifying layers.", null);
		//yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "Now, press \"Space\" a few more times to go back to the full layer mode.", (FullLayerEvent e) => true);
		//yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Awesome! Feel free to explore the world!", null);
		//LineCanvas.Top.Hide();
		Goal.Activate(Util.GoalName.C1S1);
		Character.Partner.WarpTo(TRef.Get(Util.TRefName.PartnerC1S1));
		ConfirmButton.Inst.EnableConfirm = true;
		yield break;
	}
	void OnGoalReached(GoalReachedEvent e)
	{
		
		// Debug.LogError("Deprecated");
		switch (e.goal_name)
		{
			case Util.GoalName.PreStory1:
				TransitionToStory(Util.StoryName.Intro);
				break;
			case Util.GoalName.FallOffCliff:
				TransitionToStory(Util.StoryName.FallOffCliff);
				break;
			//case Util.GoalName.TurnWheel:
			//	GameSave.CurrentCheckpoint = Util.WaypointName.TurnWheel;
			//	GameSave.Inventory[Util.Component.TurnWheel] = 2;
			//	GameSave.IncrementGridSize(0, 1, 0);
			//	ObtainCanvas.Inst.Show(Util.Component.TurnWheel);
			//	Goal.Activate(GoalName.Umbrella);
			//	GoalCanvas.Inst.GoalToFollow = Util.GoalName.Umbrella;
			//	break;
			//case Util.GoalName.Umbrella:
			//	GameSave.CurrentCheckpoint = WaypointName.Umbrella;
			//	GameSave.Inventory[Util.Component.Umbrella] = 4;
			//	GameSave.IncrementGridSize(1, 0, 0);
			//	ObtainCanvas.Inst.Show(Util.Component.Umbrella);
			//	Goal.Activate(GoalName.Rocket);
			//	GoalCanvas.Inst.GoalToFollow = Util.GoalName.Rocket;
			//	break;
			//case Util.GoalName.PreStory2:
			//	TransitionToStory(Util.StoryName.InTown);
			//	break;
			//case Util.GoalName.Town:
			//	TransitionToStory(Util.StoryName.TownWaypoint);
			//	break;
			//case Util.GoalName.C1S1:
			//	TransitionToStory(Util.StoryName.C1S1);
			//	break;
			//case Util.GoalName.C1S2:
			//	TransitionToStory(Util.StoryName.C1S2);
			//	break;
			//case Util.GoalName.Volcano:
			//	TransitionToBuild(Util.WaypointName.Volcano, Util.GoalName.VolcBottom);
			//	break;
			//case Util.GoalName.VolcBottom:
			//	TransitionToBuild(Util.WaypointName.VolcBottom, Util.GoalName.VolcTop);
			//	break;
			//case Util.GoalName.VolcTop:
			//	TransitionToBuild(Util.WaypointName.VolcTop, Util.GoalName.VolcAfter);
			//	break;
			//case Util.GoalName.VolcAfter:
			//	StartCoroutine(HandleVolcAfter());
			//	break;
			default:
				Debug.LogError("Goal reached not handled");
				break;
		}
	}
	void OnTouchLava(TouchLavaEvent e)
	{
		StartCoroutine(TouchLavaHelper());
	}
	IEnumerator TouchLavaHelper()
	{
		yield return BlackoutCanvas.Inst.Blackout(0.5f, 0.0f, 1.0f);
		yield return BlackoutCanvas.Inst.DisplaySub("You tried to swim in lava.", 0.5f, 0.0f, 1.0f);
		OnRetry();
		yield return BlackoutCanvas.Inst.DisplaySub("You tried to swim in lava.", 0.5f, 1.0f, 0.0f);
		yield return BlackoutCanvas.Inst.Blackout(0.5f, 1.0f, 0.0f);
	}
	IEnumerator HandleVolcAfter()
	{
		yield return BlackoutCanvas.Inst.Blackout(0.5f, 0.0f, 1.0f);
		yield return BlackoutCanvas.Inst.DisplaySub("Thanks for playing", 1.5f, 0.0f, 1.0f);
		yield return BlackoutCanvas.Inst.DisplaySub("More Contents Coming Soon", 1.5f, 1.0f, 0.0f);
		yield return BlackoutCanvas.Inst.Blackout(0.5f, 1.0f, 0.0f);
	}
	Util.WaypointName current_waypoint;


	public void TryScan()
	{
		CarCore.Inst.DeactivateContainer();
		GridMatrix.Inst.Scan();
	}
	IEnumerator PromptChooseQuest()
	{
		yield return null;
		CarCore.Inst.DampStart();
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "What an adventure! Now, shall we go around and have some fun?", null, VoiceLine.what_adventure);
		LineCanvas.Top.Hide();
		PlayCanvas.Inst.ShowStory();
		StoryCanvas.Inst.ShowSideQuests();
		PlayCanvas.Inst.SetStoryText("Select a Quest");
		PlayCanvas.Inst.ScaleStory();
		CarCore.Inst.DampStop();
	}
	public void TransitionToPlay(bool build)
	{
		AudioPlayer.Inst.TransitionToPlay();
		BuildCanvas.Inst.Hide();
		PlayCanvas.Inst.Show();
		
		// AudioPlayer.Inst.TransitionToPlay();
		if (build)
		{
			StartCoroutine(GridMatrix.Inst.BuildAndDeactivate());
		}
		if (prompt_choose_quest2)
		{
			prompt_choose_quest2 = false;
			// StartCoroutine(PromptChooseQuest());
		}
		if (side_quests_begin)
		{
			side_quests_begin = false;
			StartCoroutine(PromptChooseQuest());
		}
		if (tour_mode)
		{
			tour_mode = false;
			StartCoroutine(HandleTourStart());
		}
		// PiggyCameraPivot.Inst.StartFollow(Piggy);
		// coroutine that moves camera to position
		// MainCamera.Inst.MoveAndStickToPig(move_to_pig_time, camera_rotation_time);
		//if (build)
		//{
		//	switch (current_waypoint)
		//	{
		//		case Util.WaypointName.PreStory1:
		//			FirstPerson.Inst.Hide();
		//			Retry.Inst.Hide();
		//			break;
		//		case Util.WaypointName.PreStory2:
		//			StartCoroutine(PlayPreStory2());
		//			break;
		//	}
		//}
	}
	public IEnumerator PlayPreStory2()
	{
		yield return new WaitForSeconds(1.0f);
		Retry.Inst.Show();
		FirstPerson.Inst.Show();
		// yield return LineCanvas.Bottom.DisplayLineAndWaitForClick("Shirley", "Drive straight into the winding valley. That's the shortest path.", null);
		LineCanvas.Bottom.Hide();
	}
}
