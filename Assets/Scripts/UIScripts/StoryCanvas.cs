using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherStoryClickedEvent
{

}

public class StoryCanvas : MonoBehaviour
{
	static Util.QuestName current_quest_name = Util.QuestName.None;
	public Util.MainStoryName MainStoryName { get; private set; } = Util.MainStoryName.GoToTown;
	public void SetMainStory(Util.MainStoryName main_story_name)
	{
		MainStoryName = main_story_name;
		switch(main_story_name)
		{
			case Util.MainStoryName.GoToTown:
				main_story_text.text = "Go To the Town";
				break;
			case Util.MainStoryName.Volcano:
				main_story_text.text = "March Towards the Volcano";
				break;
			case Util.MainStoryName.BossFight:
				main_story_text.text = "Defeat Groundhog the Juggernaut";
				break;
			case Util.MainStoryName.AllCompleted:
				main_story_text.text = "Main Story Completed";
				break;
		}
	}
	public Util.QuestName CurrentQuestName
	{
		get { return current_quest_name; }
		set
		{
			current_quest_name = value;
			switch (current_quest_name)
			{
				case Util.QuestName.MainStory:
					if (MainStoryName == Util.MainStoryName.GoToTown)
						Inst.SetGoToTown();
					else if (MainStoryName == Util.MainStoryName.Volcano)
						Inst.SetVolcano();
					else if (MainStoryName == Util.MainStoryName.BossFight)
						Inst.SetBossFight();
					else if (MainStoryName == Util.MainStoryName.AllCompleted)
						Inst.SetAllCompleted();
					break;
				case Util.QuestName.PiggylandCenter:
					SetPiggylandCenter();
					break;
				case Util.QuestName.Loop:
					SetLoop();
					break;
				case Util.QuestName.TourDePiggyland:
					SetTour();
					break;
				case Util.QuestName.GroundhogFestival:
					SetFestival();
					break;
			}
			// to do
		}
	}

	static StoryCanvas inst;
	public static StoryCanvas Inst
	{
		get
		{
			Debug.Assert(inst != null);
			return inst;
		}
	}
	public GameObject container;
	public void ShowSideQuests()
	{
		container.SetActive(true);
	}
	public void HideSideQuests() {
		container.SetActive(false);
	}
	private void Start()
	{
		Debug.Assert(inst == null);
		inst = this;
		EventBus.Subscribe<OtherStoryClickedEvent>(OnOtherStoryClicked);
		gameObject.SetActive(false);
		description_image.sprite = null;
		description.text = "";
	}
	void OnOtherStoryClicked(OtherStoryClickedEvent e)
	{
		main_story_background.color = light_gray_color;
	}
	public void Show()
	{
		gameObject.SetActive(true);
	}
	private void OnDestroy()
	{
		inst = null;
	}
	public void OnConfirmClicked()
	{
		gameObject.SetActive(false);
		Debug.Assert(GameSave.Progresses.ContainsKey(CurrentQuestName));
		(Util.WaypointName retry_waypoint, Util.WaypointName goal_waypoint) = GameSave.Progresses[CurrentQuestName];
		Debug.Assert(retry_waypoint != Util.WaypointName.None);
		GameState.Inst.UpdateRetryAndGoal(retry_waypoint, goal_waypoint);

		GameSave.Inventory[Util.Component.Tourist] = 0;

		if (CurrentQuestName == Util.QuestName.GroundhogFestival)
		{
			track_sidequest.SetActive(true);
		}
		else
		{
			track_sidequest.SetActive(false);
		}

		if (CurrentQuestName == Util.QuestName.PiggylandCenter)
		{
			CarCore.Inst.AutoRotation = true;
		}
		else
		{
			CarCore.Inst.AutoRotation = false;
		}
		switch (CurrentQuestName)
		{
			case Util.QuestName.MainStory:
				PlayCanvas.Inst.SetStoryText(main_story_text.text);
				break;
			case Util.QuestName.PiggylandCenter:
				PlayCanvas.Inst.SetStoryText("A Bird's Eye View");
				GameState.Inst.StartCoroutine(PiggylandCenterHint());
				break;
			case Util.QuestName.Loop:
				PlayCanvas.Inst.SetStoryText("Loop");
				GameState.Inst.StartCoroutine(LoopHint());
				break;
			case Util.QuestName.TourDePiggyland:
				PlayCanvas.Inst.SetStoryText("Tour de Piggyland");
				GameState.Inst.StartCoroutine(TourDePiggylandHint());
				break;
			case Util.QuestName.GroundhogFestival:
				PlayCanvas.Inst.SetStoryText("Groundhog Festival");
				GameState.Inst.StartCoroutine(GroundhogFestivalHint());
				break;
		}		
	}
	public GameObject track_sidequest;
	IEnumerator TourDePiggylandHint()
	{
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "There are some tourists that want a ride. Let's help them!", null, Util.VoiceLine.tourist_help);
		LineCanvas.Top.Hide();
	}
	IEnumerator PiggylandCenterHint()
	{
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Let's explore the Piggyland Center in the town!", null, Util.VoiceLine.lets_explore);
		LineCanvas.Top.Hide();
	}
	IEnumerator LoopHint()
	{
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "I saw a loop at the lake. Let's try it out!", null, Util.VoiceLine.loop);
		LineCanvas.Top.Hide();
	}
	IEnumerator GroundhogFestivalHint()
	{
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "The villagers are celebrating the extermination of the Groundhog. Let's join them!", null, Util.VoiceLine.celebrating);
		LineCanvas.Top.DisplayLineAsync("Shirley", "Teleporting in 3, 2, 1...", 2.0f, Util.VoiceLine.teleporting);
		yield return new WaitForSeconds(2.0f);
		GameState.Inst.GoToCheckpointAsync(Util.WaypointName.TrackEntrance);
	}


	public void OnBackClicked()
	{
		gameObject.SetActive(false);
	}
	public Image main_story_background;
	public static Color green_color = new Color(0.2f, 1.0f, 0.2f, 1);
	public static Color light_gray_color = new Color(0.8f, 0.8f, 0.8f, 0.8f);
	public void OnMainStoryClicked()
	{
		EventBus.Publish(new OtherStoryClickedEvent());
		main_story_background.color = green_color;
		CurrentQuestName = Util.QuestName.MainStory;
	}
	public Sprite go_to_town_image;
	public Sprite volcano_image;
	public Sprite boss_fight_image;
	public Image description_image;
	public Text description;
	public Text main_story_text;
	public void SetGoToTown()
	{
		
		description_image.sprite = go_to_town_image;
		description.text = "Prestory: Go to to the nearby town.\n" +
			"You and your girlfriend embarked on an unexpected journey to find the missing rocket engines.\n" +
			"Your first destination is the nearby town, where you can seek for help.";
	}
	public void SetVolcano()
	{
		description_image.sprite = volcano_image;
		description.text = "Main Story Act 1: March Towards the Volcano.\n" +
			"You are informed that the rocket engines may be stolen by an evil creature,\n" +
			"and the way to defeat it is to obtain the undestructable Adamantium lying in the heart of the volcano.\n" +
			"It's a sinister place. So be prepared!";
	}
	public void SetBossFight()
	{
		description_image.sprite = boss_fight_image;
		description.text = "Main Story Act 2: Defeat the lord of evil Groundhog the Juggernaut!\n" +
			"It's the final step towards freedom!\n" +
			"The evil creature is taking away what are rightfully yours.\n" +
			"Defeat him using the OVERPOWERED Admantium and claim your treasure!";
	}
	public Sprite all_completed_image;
	public void SetAllCompleted()
	{
		description_image.sprite = all_completed_image;
		description.text = "You have completed the main story!\n" +
			"Go and have fun with interesting side quests!";
	}
	public Sprite center_image;
	public void SetPiggylandCenter()
	{
		description_image.sprite = center_image;
		description.text = "A Bird's Eye View\n" +
			"You and your girlfriend witnissed a conspicuous building in the town.\n" +
			"According to the locals, it is called the Piggyland Center.\n"+
			"\"It must be fascinating to see from the highest spot in the town,\" said your girlfriend\n"+
			"\"Just with a little bit of pain getting that high,\" you added.";	
	}
	public Sprite loop_image;
	public void SetLoop()
	{
		description_image.sprite = loop_image;
		description.text = "Loop\n" +
			"A loop is the most thrilling part of a rollercoaster, and there is one not far from the town.\n" +
			"\"Great. Now we have to start the engine ourselves just to frighten ourselves,\" your girlfriend bantered.\n";
	}
	public Sprite tour_image;
	public void SetTour()
	{
		description_image.sprite = tour_image;
		description.text = "Tour de Piggyland\n" +
			"You are sort of an environmentalist, and you find your vehicle is emitting too much CO2.\n" +
			"What about taking more passengers? This will amortize the CO2 emission per capita.\n";
	}
	public Sprite festival_image;
	public void SetFestival()
	{
		description_image.sprite = festival_image;
		description.text = "Groundhog Festival\n" +
			"To celebrate the extermination of the evil Groundhog, villagers are preparing an interesting festival activity.\n" +
			"It aims at reproducing the heroic moments when two outlanders dodge the attack of the Groundhog.\n" +
			"\"It looks much more sinister than the actual boss fight,\" said your girlfriend.\n";
	}
}
