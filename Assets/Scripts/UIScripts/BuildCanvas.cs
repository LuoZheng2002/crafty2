using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NumberClickedEvent { }

public class BuildCanvas : MonoBehaviour
{
    public static BuildCanvas Inst
    {
        get { Debug.Assert(inst != null, "Build Canvas not set");return inst; }
    }
    static BuildCanvas inst;
    Transform itemBar;
    Transform rightButton;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(inst == null, "Build Canvas Already Set");
        inst = this;
        Util.Delay(this, () =>
        {
            DragImage.DetachAll();
            gameObject.SetActive(false);
        });
        itemBar = transform.Find("ItemBar");
        rightButton = itemBar.Find("Right");
        Debug.Assert(rightButton != null);
    }
    public void Show()
    {
        EventBus.Publish(new GameStateChangedEvent(false));
        gameObject.SetActive(true);
        if (GameState.Inst.IntroducePreset)
        {
            GameState.Inst.IntroducePreset = false;
			// TODO for Gabriel: introduce preset
            StartCoroutine(PresetIntroduction());
		}
        if (GameState.Inst.introduce_space)
        {
            GameState.Inst.introduce_space = false;
            StartCoroutine(GameState.Inst.IntroduceSpace());
        }
        AudioPlayer.Inst.TransitionToBuild();
        if (GameState.Inst.tour_mode2)
        {
            GameState.Inst.tour_mode2 = false;
            GameState.Inst.StartCoroutine(HandleTour());
        }
	}
    IEnumerator HandleTour()
    {
        yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Pack all the tourists with us!",null, Util.VoiceLine.pack);
	}
	public IEnumerator PresetIntroduction()
	{
        ConfirmButton.Inst.EnableConfirm = false;
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "It seems for the incoming challenges, we need different vehicle designs.", null, Util.VoiceLine.it_seems_for_the);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "It would be beneficial to save your current design for future retrieval.", null, Util.VoiceLine.it_would_be);
        button_scale_2.ScaleStart();
		yield return LineCanvas.Top.DisplayLineAndWaitForEvent("Shirley", "Click on the number buttons on the top to switch between designs.", (NumberClickedEvent e)=>true, Util.VoiceLine.click_on_the_number);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Perfect! By building in the second slot, you can preserve your previous build.", null, Util.VoiceLine.perfect_by);
		yield return LineCanvas.Top.DisplayLineAndWaitForClick("Shirley", "Let's move on!", null, Util.VoiceLine.lets_move_on);
		LineCanvas.Top.Hide();
        ConfirmButton.Inst.EnableConfirm = true;
	}

    public ButtonScale button_scale_2;
	public void Hide()
    {
        gameObject.SetActive(false);
    }
    int items_offset = 0;
    List<DragImage> nonzero_images;
    void UpdateImages()
    {
		DragImage.DetachAll();
        rightButton.SetParent(null);
        Queue<DragImage> emptyImages = new Queue<DragImage>(DragImage.EmptyImages.Values);
        for (int i = 0; i < 5; i++)
        {
            if (i + items_offset < nonzero_images.Count)
            {
                nonzero_images[i + items_offset].transform.SetParent(itemBar);
				nonzero_images[i + items_offset].transform.localScale = Vector3.one;

			}
            else
            {
                Debug.Assert(emptyImages.Count > 0);
                var img = emptyImages.Dequeue();
                img.transform.SetParent(itemBar);
                img.transform.localScale = Vector3.one;
            }
        }
        rightButton.SetParent(itemBar);
	}
    bool CanClickRight()
    {
        return nonzero_images.Count >= items_offset + 1 + 5;
	}
    bool CanClickLeft()
    {
        return items_offset > 0;
	}
    public void OnRightClicked()
    {
        if (CanClickRight())
        {
            items_offset++;
            UpdateImages();
        }
        button_scale.ScaleStop();
    }
    public void OnLeftClicked()
    {
        if (CanClickLeft())
        {
            items_offset--;
            UpdateImages();
        }
    }
	public void InitializeItems()
    {
        DragImage.ClearCountAll();
        var items = GameSave.Inventory;
        foreach (var item in items)
        {
            DragImage.DragImages[item.Key].SetInitialCount(item.Value);
        }
        if (GameSave.IsMainStory)
        {
            DragImage.DragImages[Util.Component.Partner].SetInitialCount(1);
        }
        nonzero_images = new();
		List<Util.Component> components = new() { Util.Component.Pig, Util.Component.Partner, Util.Component.Tourist, Util.Component.WoodenCrate, Util.Component.Wheel, 
            Util.Component.TurnWheel, Util.Component.MotorWheel, 
            Util.Component.Umbrella, Util.Component.Rocket};


		foreach (var component in components)
        {
            var dragImage = DragImage.DragImages[component];
			if (dragImage.Count > 0)
            {
                nonzero_images.Add(dragImage);
            }
        }
        items_offset = 0;
        UpdateImages();
        Debug.Log("Tested");
        if (CanClickRight())
        {
            Debug.Log("Succeed");
            Util.Delay(this, 5, () =>
            {
                button_scale.ScaleStart();
            });
        }
    }
    public ButtonScale button_scale;
    public Sprite one_selected;
	public Sprite two_selected;
	public Sprite three_selected;
    public Sprite one_idle;
    public Sprite two_idle;
	public Sprite three_idle;
    public Image one_image;
    public Image two_image;
	public Image three_image;
	public void OnOneClicked()
    {
        one_image.sprite = one_selected;
		two_image.sprite = two_idle;
        three_image.sprite = three_idle;
        OnNumberClicked(0);
	}
    public void OnTwoClicked()
    {
        one_image.sprite = one_idle;
		two_image.sprite = two_selected;
		three_image.sprite = three_idle;
		OnNumberClicked(1);
        button_scale_2.ScaleStop();
	}
    public void OnThreeClicked()
    {
        one_image.sprite = one_idle;
		two_image.sprite = two_idle;
		three_image.sprite = three_selected;
		OnNumberClicked(2);
	}
    void OnNumberClicked(int number)
    {
        EventBus.Publish(new OtherItemSelectedEvent());
        EventBus.Publish(new NumberClickedEvent());
        Util.Delay(this, () =>
        {
            GameSave.CurrentMemory.Memorize();
            if (GameSave.GridMemories[number].Empty())
            {
                GameSave.GridMemories[number].MemCrates = (Util.Component[,,])GameSave.CurrentMemory.MemCrates.Clone();
				GameSave.GridMemories[number].MemAccessories = (Util.Component[,,])GameSave.CurrentMemory.MemAccessories.Clone();
				GameSave.GridMemories[number].MemLoads = (Util.Component[,,])GameSave.CurrentMemory.MemLoads.Clone();
                GameSave.GridMemories[number].AccessoryDirections = (int[,,])GameSave.CurrentMemory.AccessoryDirections.Clone();
			}
            GridMatrix.Inst.DesignNumber = number;
        });        
    }
    public GameObject design_numbers;
    public void ShowDesignNumbers()
    {
        design_numbers.SetActive(true);
	}
    public void HideDesignNumbers()
    {
        design_numbers.SetActive(false);
    }
}
