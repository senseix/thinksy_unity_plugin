using UnityEngine;
using System.Collections;

public class ShowOffEmoticons : MonoBehaviour 
{
	public float maximumScale;
	public float increment;

	private bool showingOff = false;
	private Queue spritesToShowOff = new Queue ();

	void OnEnable()
	{
		ThinksyEvents.onEncouragementReceived += DisplayEmoticons;
	}

	void OnDisable()
	{
		ThinksyEvents.onEncouragementReceived -= DisplayEmoticons;
	}

	private void DisplayEmoticons(ProblemPart[] possibleEmoticons)
	{
		Debug.Log ("I received " + possibleEmoticons.Length + " possible emoticons.");
		foreach(ProblemPart possibleEmoticon in possibleEmoticons)
		{
			if (possibleEmoticon.IsImage())
			{
				QueueSpriteToShowOff(possibleEmoticon.GetSprite("Emoticons"));
			}
		}
	}

	public void QueueSpriteToShowOff(Sprite sprite)
	{
		spritesToShowOff.Enqueue (sprite);
	}

	void Update()
	{
		if (!showingOff)
		{
			ShowOffNextSprite();
		}
	}

	private void ShowOffNextSprite()
	{
		if (spritesToShowOff.Count == 0)
		{
			return;
		}
		Sprite nextSprite = spritesToShowOff.Dequeue () as Sprite;
		GetComponent<UnityEngine.UI.Image> ().sprite = nextSprite;
		StartCoroutine (ShowOff ());
	}

	private IEnumerator ShowOff()
	{
		showingOff = true;
		yield return StartCoroutine (ExpandTo (maximumScale));
		yield return StartCoroutine (ShrinkTo ((float)(2*maximumScale)/3f));
		yield return StartCoroutine (ExpandTo (maximumScale));
		yield return StartCoroutine (ShrinkTo (0f));
		showingOff = false;
	}

	private IEnumerator ExpandTo(float scale)
	{
		RectTransform rectTransform = this.GetComponent<RectTransform> ();
		while (rectTransform.localScale.x < scale)
		{
			rectTransform.localScale += new Vector3(increment, increment, increment);
			yield return null;
		}
	}

	private IEnumerator ShrinkTo(float scale)
	{
		RectTransform rectTransform = this.GetComponent<RectTransform> ();
		while (rectTransform.localScale.x > scale)
		{
			rectTransform.localScale -= new Vector3(increment, increment, increment);
			yield return null;
		}
	}
}