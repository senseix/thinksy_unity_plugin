using UnityEngine;
using System.Collections;

public class DisableIfNotTemporaryAccount : MonoBehaviour {

	private string originalText = "";

	void OnEnable()
	{
		if (Senseix.SenseixSession.IsSignedIn ())
		{
			GetComponent<UnityEngine.UI.Button>().interactable = false;
			UnityEngine.UI.Text buttonPrompt = GetComponentInChildren<UnityEngine.UI.Text>();
			if (buttonPrompt.text[buttonPrompt.text.Length-1] != '>')
			{
				originalText = buttonPrompt.text;
				buttonPrompt.text += "<color=green>✓</color>";
			}
		}
		else
		{
			GetComponent<UnityEngine.UI.Button>().interactable = true;
			UnityEngine.UI.Text buttonPrompt = GetComponentInChildren<UnityEngine.UI.Text>();
			if (originalText != "") buttonPrompt.text = originalText;
		}
	}

}
