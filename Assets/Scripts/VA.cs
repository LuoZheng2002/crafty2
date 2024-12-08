using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VA : MonoBehaviour
{
    static VA inst;
	public AudioSource audio_source;
	public AudioClip   A_journey;
	public AudioClip Although;
	public AudioClip   but;
	public AudioClip dont;
	public AudioClip   fifteen;
	public AudioClip good;
	public AudioClip   great;
	public AudioClip haha;
	public AudioClip   hint;
	public AudioClip honey;
	public AudioClip   hopeyou;
	public AudioClip i_cant_wait;
	public AudioClip   lets_climb;
	public AudioClip lets_first;
	public AudioClip   lets_travel;
	public AudioClip letsgo;
	public AudioClip   now_press;
	public AudioClip now_there;
	public AudioClip   now_we;
	public AudioClip now_would;
	public AudioClip   oh_there;
	public AudioClip ohno;
	public AudioClip   prefect_hover;
	public AudioClip perfect_lets;
	public AudioClip   perfect_now_we;
	public AudioClip perfect_now_lets;
	public AudioClip   perfect_since;
	public AudioClip press_space;
	public AudioClip   press_the;
	public AudioClip start_by;
	public AudioClip   start_by_clicking;
	public AudioClip the_town_is;
	public AudioClip   this_is;
	public AudioClip twenty;
	public AudioClip   without;
	public AudioClip woohoo;
	public AudioClip   woohoo_we_made;
	public AudioClip woohoo_weve;
	public AudioClip you_are_learning;
	public AudioClip it_seems;
	public AudioClip lets_click;
	public AudioClip ten;
	public AudioClip five;
	public AudioClip awwww;
	public AudioClip this_must;
	public AudioClip it_seems_for_the;
	public AudioClip it_would_be;
	public AudioClip click_on_the_number;
	public AudioClip perfect_by;
	public AudioClip lets_move_on;
	public AudioClip oh_no_our_car;
	public AudioClip however;
	public AudioClip what_adventure;
	public AudioClip boss_hive;
	public AudioClip oh_trapped;
	public AudioClip boss_tired;
	public AudioClip watch_out;
	public AudioClip the_boss_is;
	public AudioClip tourist_guess;
	public AudioClip buckle_up;
	public AudioClip tough_one;
	public AudioClip chase_enter;
	public AudioClip lets_march;
	public AudioClip thank_you;
	public AudioClip oh_i_saw;
	public AudioClip excuse_me;
	public AudioClip treasure_evil;
	public AudioClip watch_out_wall;
	public AudioClip you_never_get;
	public AudioClip i_know_you;
	public AudioClip tourist_help;
	public AudioClip lets_explore;
	public AudioClip loop;
	public AudioClip celebrating;
	public AudioClip teleporting;
	public AudioClip ultimate;
	public AudioClip pack;
	public static VA Inst
	{
		get
		{
			Debug.Assert(inst != null);
			return inst;
		}
	}
	private void Start()
	{
		Debug.Assert(inst == null);
		inst = this;
	}
	private void OnDestroy()
	{
		inst = null;
	}
	public void PlayAudio(Util.VoiceLine voice_line)
	{
		if (audio_source.isPlaying)
			audio_source.Stop();
		if (voice_line == Util.VoiceLine.None)
			return;
		switch (voice_line)
		{
			case Util.VoiceLine.None:
				break;
			case Util.VoiceLine.A_journey:
				audio_source.clip = A_journey;
				break;
			case Util.VoiceLine.Although:
				audio_source.clip = Although;
				break;
			case Util.VoiceLine.but:
				audio_source.clip = but;
				break;
			case Util.VoiceLine.dont:
				audio_source.clip = dont;
				break;
			case Util.VoiceLine.fifteen:
				audio_source.clip = fifteen;
				break;
			case Util.VoiceLine.good:
				audio_source.clip = good;
				break;
			case Util.VoiceLine.great:
				audio_source.clip = great;
				break;
			case Util.VoiceLine.haha:
				audio_source.clip = haha;
				break;
			case Util.VoiceLine.hint:
				audio_source.clip = hint;
				break;
			case Util.VoiceLine.honey:
				audio_source.clip = honey;
				break;
			case Util.VoiceLine.hopeyou:
				audio_source.clip = hopeyou;
				break;
			case Util.VoiceLine.i_cant_wait:
				audio_source.clip = i_cant_wait;
				break;
			case Util.VoiceLine.lets_climb:
				audio_source.clip = lets_climb;
				break;
			case Util.VoiceLine.lets_first:
				audio_source.clip = lets_first;
				break;
			case Util.VoiceLine.lets_travel:
				audio_source.clip = lets_travel;
				break;
			case Util.VoiceLine.letsgo:
				audio_source.clip = letsgo;
				break;
			case Util.VoiceLine.now_press:
				audio_source.clip = now_press;
				break;
			case Util.VoiceLine.now_there:
				audio_source.clip = now_there;
				break;
			case Util.VoiceLine.now_we:
				audio_source.clip = now_we;
				break;
			case Util.VoiceLine.now_would:
				audio_source.clip = now_would;
				break;
			case Util.VoiceLine.oh_there:
				audio_source.clip = oh_there;
				break;
			case Util.VoiceLine.ohno:
				audio_source.clip = ohno;
				break;
			case Util.VoiceLine.prefect_hover:
				audio_source.clip = prefect_hover;
				break;
			case Util.VoiceLine.perfect_lets:
				audio_source.clip = perfect_lets;
				break;
			case Util.VoiceLine.perfect_now_we:
				audio_source.clip = perfect_now_we;
				break;
			case Util.VoiceLine.perfect_now_lets:
				audio_source.clip = perfect_now_lets;
				break;
			case Util.VoiceLine.perfect_since:
				audio_source.clip = perfect_since;
				break;
			case Util.VoiceLine.press_space:
				audio_source.clip = press_space;
				break;
			case Util.VoiceLine.press_the:
				audio_source.clip = press_the;
				break;
			case Util.VoiceLine.start_by:
				audio_source.clip = start_by;
				break;
			case Util.VoiceLine.start_by_clicking:
				audio_source.clip = start_by_clicking;
				break;
			case Util.VoiceLine.the_town_is:
				audio_source.clip = the_town_is;
				break;
			case Util.VoiceLine.this_is:
				audio_source.clip = this_is;
				break;
			case Util.VoiceLine.twenty:
				audio_source.clip = twenty;
				break;
			case Util.VoiceLine.without:
				audio_source.clip = without;
				break;
			case Util.VoiceLine.woohoo:
				audio_source.clip = woohoo;
				break;
			case Util.VoiceLine.woohoo_we_made:
				audio_source.clip = woohoo_we_made;
				break;
			case Util.VoiceLine.woohoo_weve:
				audio_source.clip = woohoo_weve;
				break;
			case Util.VoiceLine.you_are_learning:
				audio_source.clip = you_are_learning;
				break;
			case Util.VoiceLine.it_seems:
				audio_source.clip = it_seems;
				break;
			case Util.VoiceLine.lets_click:
				audio_source.clip = lets_click;
				break;
			case Util.VoiceLine.ten:
				audio_source.clip = ten;
				break;
			case Util.VoiceLine.five:
				audio_source.clip = five;
				break;
			case Util.VoiceLine.awwww:
				audio_source.clip = awwww;
				break;
			case Util.VoiceLine.this_must:
				audio_source.clip = this_must;
				break;
			case Util.VoiceLine.it_seems_for_the:
				audio_source.clip = it_seems_for_the;
				break;
			case Util.VoiceLine.it_would_be:
				audio_source.clip = it_would_be;
				break;
			case Util.VoiceLine.click_on_the_number:
				audio_source.clip = click_on_the_number;
				break;
			case Util.VoiceLine.perfect_by:
				audio_source.clip = perfect_by;
				break;
			case Util.VoiceLine.lets_move_on:
				audio_source.clip = lets_move_on;
				break;
			case Util.VoiceLine.oh_no_our_car:
				audio_source.clip = oh_no_our_car;
				break;
			case Util.VoiceLine.however:
				audio_source.clip = however;
				break;
			case Util.VoiceLine.what_adventure:
				audio_source.clip = what_adventure;
				break;
			case Util.VoiceLine.boss_hive:
				audio_source.clip = boss_hive;
				break;
			case Util.VoiceLine.oh_trapped:
				audio_source.clip = oh_trapped;
				break;
			case Util.VoiceLine.boss_tired:
				audio_source.clip = boss_tired;
				break;
			case Util.VoiceLine.watch_out:
				audio_source.clip = watch_out;
				break;
			case Util.VoiceLine.the_boss_is:
				audio_source.clip = the_boss_is;
				break;
			case Util.VoiceLine.tourist_guess:
				audio_source.clip = tourist_guess;
				break;
			case Util.VoiceLine.buckle_up:
				audio_source.clip = buckle_up;
				break;
			case Util.VoiceLine.tough_one:
				audio_source.clip = tough_one;
				break;
			case Util.VoiceLine.chase_enter:
				audio_source.clip = chase_enter;
				break;
			case Util.VoiceLine.lets_march:
				audio_source.clip = lets_march;
				break;
			case Util.VoiceLine.thank_you:
				audio_source.clip = thank_you;
				break;
			case Util.VoiceLine.oh_i_saw:
				audio_source.clip = oh_i_saw;
				break;
			case Util.VoiceLine.excuse_me:
				audio_source.clip = excuse_me;
				break;
			case Util.VoiceLine.treasure_evil:
				audio_source.clip = treasure_evil;
				break;
			case Util.VoiceLine.watch_out_wall:
				audio_source.clip = watch_out_wall;
				break;
			case Util.VoiceLine.you_never_get:
				audio_source.clip = you_never_get;
				break;
			case Util.VoiceLine.i_know_you:
				audio_source.clip = i_know_you;
				break;
			case Util.VoiceLine.tourist_help:
				audio_source.clip = tourist_help;
				break;
			case Util.VoiceLine.lets_explore:
				audio_source.clip = lets_explore;
				break;
			case Util.VoiceLine.loop:
				audio_source.clip = loop;
				break;
			case Util.VoiceLine.celebrating:
				audio_source.clip = celebrating;
				break;
			case Util.VoiceLine.teleporting:
				audio_source.clip = teleporting;
				break;
			case Util.VoiceLine.ultimate:
				audio_source.clip = ultimate;
				break;
			case Util.VoiceLine.pack:
				audio_source.clip = pack;
				break;
		}
		audio_source.loop = false;
		audio_source.Play();
	}
}
