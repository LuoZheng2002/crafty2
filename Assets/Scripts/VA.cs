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
		}
		audio_source.loop = false;
		audio_source.Play();
	}
}
