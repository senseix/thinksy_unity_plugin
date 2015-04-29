using UnityEngine;
using System.Collections;

public class MessageDisplay : MonoBehaviour
{
	public float scrollSpeed = 10f;
	
	public bool displayImmediately = true; // uncheck this to manually controll when encouragements are displayed
	
	private float startOffset;
	private bool showingOff = false;
	private Queue messagesToShowOff = new Queue ();
	
	void OnEnable()
	{
		ThinksyEvents.onEncouragementReceived += EnqueueMessages;
		gameObject.GetComponent<RectTransform> ().offsetMin = new Vector2(Screen.width, 0f);
		startOffset = gameObject.GetComponent<RectTransform> ().offsetMin.x;
	}
	
	void Update()
	{
		if (displayImmediately)
		{
			DisplayNextEmoticon();
		}
	}
	
	void OnDisable()
	{
		ThinksyEvents.onEncouragementReceived -= EnqueueMessages;
	}
	
	private void EnqueueMessages(ProblemPart[] possibleMessages)
	{
		Debug.Log ("I received " + possibleMessages.Length + " possible messages.");
		foreach(ProblemPart possibleMessage in possibleMessages)
		{
			if (possibleMessage.HasString())
			{
				messagesToShowOff.Enqueue (possibleMessage.GetString());
			}
		}
		Debug.Log ("Queue length now " + CountMessagesWaiting ().ToString ());
	}
	
	private IEnumerator DoScroll()
	{
		showingOff = true;
		RectTransform rectTransform = gameObject.GetComponent<RectTransform> ();
		float width = Screen.width;
		rectTransform.offsetMin = new Vector2 (startOffset, 0);
		rectTransform.offsetMax = new Vector2 (startOffset + width, 0);
		while (rectTransform.offsetMax.x > -startOffset)
		{
			float oldOffset = rectTransform.offsetMin.x;
			float newOffset = oldOffset - scrollSpeed;
			rectTransform.offsetMin = new Vector2 (newOffset, 0);
			rectTransform.offsetMax = new Vector2 (newOffset + width, 0);
			yield return null;
		}
		showingOff = false;
	}
	
	public void DisplayNextEmoticon()
	{
		if (messagesToShowOff.Count == 0 || showingOff)
		{
			return;
		}
		string nextMessage = messagesToShowOff.Dequeue () as string;
		GetComponent<UnityEngine.UI.Text> ().text = nextMessage;
		StartCoroutine (DoScroll ());
	}
	
	/// <summary>
	/// Starts a coroutine that displays all messages, one after the next.
	/// </summary>
	public void DisplayAllEmoticons()
	{
		StartCoroutine (DisplayAllMessagesCoroutine());
	}
	
	/// <summary>
	/// Counts the messages waiting.
	/// </summary>
	public int CountMessagesWaiting()
	{
		return messagesToShowOff.Count;
	}
	
	private IEnumerator DisplayAllMessagesCoroutine()
	{
		while (messagesToShowOff.Count > 0)
		{
			DisplayNextEmoticon();
			yield return null;
		}
	}
}