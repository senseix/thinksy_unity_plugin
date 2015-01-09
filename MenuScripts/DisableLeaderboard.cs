using UnityEngine;
using System.Collections;

public class DisableLeaderboard : MonoBehaviour {

	void OnEnable()
	{
		if (!SenseixPlugin.UsesLeaderboard ())
			gameObject.SetActive (false);
	}
}
