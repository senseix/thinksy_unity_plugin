using UnityEngine;
using System.Collections;

public class DisableIfNotTemporaryAccount : MonoBehaviour {

	void OnEnable()
	{
		if (Senseix.SenseixSession.IsSignedIn ())
		{
			GetComponent<UnityEngine.UI.Button>().interactable = false;
			UnityEngine.UI.Text text = GetComponentInChildren<UnityEngine.UI.Text>();
			if (text.text[text.text.Length-1] != '✓')
				text.text += "✓";
		}
		else
		{
			GetComponent<UnityEngine.UI.Button>().interactable = true;
			UnityEngine.UI.Text text = GetComponentInChildren<UnityEngine.UI.Text>();
			text.text = text.text.Trim('✓');
		}
	}

}
