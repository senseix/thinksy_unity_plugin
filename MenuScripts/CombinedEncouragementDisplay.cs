using UnityEngine;
using System.Collections;

public class CombinedEncouragementDisplay : MonoBehaviour 
{
	public UnityEngine.UI.Text displayText;
	public UnityEngine.UI.Image displayImage;
	public GameObject displayObject;

	private Queue spriteEncouragementsToDisplay = new Queue();
	private Queue stringEncouragementsToDisplay = new Queue();
	private bool showing = false;

	void OnEnable()
	{
		ThinksyEvents.onEncouragementReceived += EncouragementsReceived;
	}
	
	void OnDisable()
	{
		ThinksyEvents.onEncouragementReceived -= EncouragementsReceived;
	}

	private void EncouragementsReceived(ProblemPart[] encouragements)
	{
		foreach(ProblemPart encouragement in encouragements)
		{
			if (encouragement.HasSprite())
			{
				spriteEncouragementsToDisplay.Enqueue(encouragement.GetSprite());
			}
			if (encouragement.HasString())
			{
				stringEncouragementsToDisplay.Enqueue(encouragement.GetString());
			}
		}
		ShowAllEncouragements ();
	}

	private IEnumerator ShowDisplayForSeconds (float seconds)
	{
		displayObject.SetActive (true);
		yield return new WaitForSeconds(seconds);
		displayObject.SetActive(false);
	}

	private void ShowAllEncouragements()
	{
		if (!showing)
		{
			showing = true;
			StartCoroutine (ShowAllEncouragementsCoroutine ());
		}
	}

	private IEnumerator ShowAllEncouragementsCoroutine()
	{
		while (stringEncouragementsToDisplay.Count != 0 || 
		       spriteEncouragementsToDisplay.Count != 0 )
		{
			if (stringEncouragementsToDisplay.Count > 0)
			{
				displayText.text = (string)stringEncouragementsToDisplay.Dequeue();
			}
			if (spriteEncouragementsToDisplay.Count > 0)
			{
				displayImage.sprite = (Sprite)spriteEncouragementsToDisplay.Dequeue();
			}
			yield return StartCoroutine(ShowDisplayForSeconds(12.5f));
			yield return new WaitForSeconds(3f);
		}
		showing = false;
	}
}