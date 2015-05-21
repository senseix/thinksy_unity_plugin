using UnityEngine;
using System.Collections;

public class CombinedEncouragementDisplay : MonoBehaviour 
{
	public UnityEngine.UI.Text displayText;
	public UnityEngine.UI.Image displayImage;
	public GameObject displayObject;
	public Transform offscreenPosition;
	public Transform onscreenPosition;

	private Queue spriteEncouragementsToDisplay = new Queue();
	private Queue stringEncouragementsToDisplay = new Queue();
	private bool showing = false;
	private bool firstEnableHappened = false;

	void OnEnable()
	{
		ThinksyEvents.onEncouragementReceived += EncouragementsReceived;
		if (!firstEnableHappened) firstEnableHappened = true;
		else StartCoroutine (ShowAllEncouragementsCoroutine ());
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
		yield return StartCoroutine (GoToTransform (onscreenPosition, 90));
		yield return new WaitForSeconds(seconds);
		yield return StartCoroutine (GoToTransform (offscreenPosition, 90));
	}

	private IEnumerator GoToTransform(Transform goToMe, int frames)
	{
		float displayObjectX = displayObject.transform.position.x;
		float displayObjectY = displayObject.transform.position.y;
		float xDifference = goToMe.position.x - displayObjectX;
		float yDifference = goToMe.position.y - displayObjectY;
		float xPerFrame = xDifference / frames;
		float yPerFrame = yDifference / frames;
		for (int i = 0; i < frames; i++)
		{
			displayObject.transform.position = 
				displayObject.transform.position +
					new Vector3(xPerFrame * ((((float)frames * 2) - ((float)i*2)) / frames),
					            yPerFrame * ((((float)frames * 2) - ((float)i*2)) / frames),
					            0);
			yield return null;
		}
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
			Debug.Log("show encouragements");
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
		yield return StartCoroutine (GoToTransform (offscreenPosition, 90));
		showing = false;
	}
}