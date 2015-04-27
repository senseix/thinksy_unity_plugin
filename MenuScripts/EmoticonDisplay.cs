using UnityEngine;
using System.Collections;

public class EmoticonDisplay : MonoBehaviour
{
	public float maximumScale; // the maximum size of the emoticon when it is being displayed
	public float increment; // the speed at which the size will change during display
	public bool displayImmediately = true; // uncheck this to manually controll when encouragements are displayed

	private bool showingOff = false;
	private Queue spritesToShowOff = new Queue ();

	void OnEnable()
	{
		ThinksyEvents.onEncouragementReceived += EnqueueEmoticons;
	}

	void OnDisable()
	{
		ThinksyEvents.onEncouragementReceived -= EnqueueEmoticons;
	}

	private void EnqueueEmoticons(ProblemPart[] possibleEmoticons)
	{
		Debug.Log ("I received " + possibleEmoticons.Length + " possible emoticons.");
		foreach(ProblemPart possibleEmoticon in possibleEmoticons)
		{
			if (possibleEmoticon.HasSprite())
			{
				spritesToShowOff.Enqueue (possibleEmoticon.GetSprite());
			}
		}
		Debug.Log ("Emoticon Queue length now " + CountEmoticonsWaiting ().ToString ());
	}

	void Update()
	{
		if (displayImmediately)
		{
			DisplayNextEmoticon();
		}
	}

	/// <summary>
	/// Displays the next emoticon.  If an emoticon is already being displayed,
	/// does nothing.
	/// </summary>
	public void DisplayNextEmoticon()
	{
		if (spritesToShowOff.Count == 0 || showingOff)
		{
			return;
		}
		Sprite nextSprite = spritesToShowOff.Dequeue () as Sprite;
		GetComponent<UnityEngine.UI.Image> ().sprite = nextSprite;
		StartCoroutine (ShowOff ());
	}

	/// <summary>
	/// Starts a coroutine that displays all emoticons, one after the next.
	/// </summary>
	public void DisplayAllEmoticons()
	{
		StartCoroutine (DisplayAllEmoticonsCoroutine());
	}

	/// <summary>
	/// Counts the emoticons waiting.
	/// </summary>
	public int CountEmoticonsWaiting()
	{
		return spritesToShowOff.Count;
	}

	private IEnumerator DisplayAllEmoticonsCoroutine()
	{
		while (spritesToShowOff.Count > 0)
		{
			DisplayNextEmoticon();
			yield return null;
		}
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
		rectTransform.localScale = new Vector3 (scale, scale, scale);
	}
}