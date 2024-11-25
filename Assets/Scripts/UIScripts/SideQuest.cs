using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideQuest : MonoBehaviour
{
	public Image background;
	public Util.QuestName quest_name;
	private void Start()
	{
		EventBus.Subscribe<OtherStoryClickedEvent>(OnOtherStoryClicked);
	}
	void OnOtherStoryClicked(OtherStoryClickedEvent e)
	{
		background.color = StoryCanvas.light_gray_color;
	}
	public void OnClick()
    {
        EventBus.Publish(new OtherStoryClickedEvent());
		background.color = StoryCanvas.green_color;
		StoryCanvas.Inst.CurrentQuestName = quest_name;
	}
}
