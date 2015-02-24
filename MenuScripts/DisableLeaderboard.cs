using UnityEngine;
using System.Collections;

public class DisableLeaderboard : MonoBehaviour {

	void OnEnable()
	{
		if (!ThinksyPlugin.UsesLeaderboard ())
			gameObject.SetActive (false);
	}
}
