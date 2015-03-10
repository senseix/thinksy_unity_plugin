using UnityEngine;
using System.Collections;

public class DisableIfOnePlayerAccount : MonoBehaviour 
{
	public UnityEngine.UI.Image disableMeImage;
	public UnityEngine.UI.Text disableMeText;

	void OnEnable()
	{
		//Debug.Log (Senseix.SenseixSession.GetCurrentPlayerList ().Count);
		if (Senseix.SenseixSession.GetCurrentPlayerList ().Count <= 1)
			SetThem (false);
		else
			SetThem (true);
	}


	private void SetThem(bool toSet)
	{
		if (disableMeText != null) disableMeText.enabled = toSet;
		if (disableMeText != null) disableMeImage.enabled = toSet;
	}
}
