using UnityEngine;
using System.Collections;

public class ItemsDisplay : MonoBehaviour 
{

	public UnityEngine.UI.Image currentItemImage;
	public UnityEngine.UI.Text currentPageText;

	private int currentPageNumber;
	private Sprite[] itemSpritesToDisplay = new Sprite[0];

	private static ItemsDisplay singletonInstance;

	public static ItemsDisplay GetSingletonInstance()
	{
		if (singletonInstance == null)
			singletonInstance = FindObjectOfType<ItemsDisplay>();
		return singletonInstance;
	}

	public void NextItem()
	{
		currentPageNumber++;
		if (currentPageNumber >= itemSpritesToDisplay.Length)
		{
			currentPageNumber = 0;
		}
		UpdateDisplay ();
	}

	public void PreviousItem()
	{
		currentPageNumber--;
		if (currentPageNumber < 0)
		{
			currentPageNumber = itemSpritesToDisplay.Length-1;
		}
		UpdateDisplay ();
	}

	private void UpdateDisplay()
	{
		if (itemSpritesToDisplay.Length != 0) 
		{
			currentItemImage.sprite = itemSpritesToDisplay [currentPageNumber];
			currentPageText.text = (currentPageNumber + 1).ToString();
		}
		else
		{
			currentPageText.text = "None";
		}
	}

	void OnEnable()
	{
		Senseix.SenseixSession.ListCurrentPlayerItems();
	}

	public static void SetItemsToDisplay(ProblemPart[] items)
	{
		GetSingletonInstance ().InstanceSetItemsToDisplay (items);
	}

	public void InstanceSetItemsToDisplay(ProblemPart[] items)
	{
		itemSpritesToDisplay = new Sprite[items.Length];
		for (int i = 0; i < items.Length; i++)
		{
			itemSpritesToDisplay[i] = items[i].GetSprite();
		}
		UpdateDisplay ();
	}
}