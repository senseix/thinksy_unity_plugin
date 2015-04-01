using UnityEngine;
using System.Collections;

public class PresentTextEncouragements : MonoBehaviour {


	void OnEnable()
	{
		ThinksyEvents.onEncouragementReceived += PresentText;
	}
	
	void OnDisable()
	{
		ThinksyEvents.onEncouragementReceived -= PresentText;
	}

	private void PresentText(ProblemPart[] possibleTexts)
	{
		Debug.Log ("I got " + possibleTexts.Length + " possible texts");
		foreach (ProblemPart possibleText in possibleTexts)
		{
			gameObject.GetComponent<UnityEngine.UI.Text> ().text += possibleText.GetString ();
		}
	}
}
