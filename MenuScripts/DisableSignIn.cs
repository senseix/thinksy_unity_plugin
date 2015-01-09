using UnityEngine;
using System.Collections;

public class DisableSignIn : MonoBehaviour {
	void OnEnable()
	{
		if (Senseix.SenseixSession.IsSignedIn())
		{
			gameObject.SetActive(false);
		}
	}
}
