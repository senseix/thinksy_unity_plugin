using UnityEngine;
using System.Collections;

namespace Senseix
{
	public class DisableLeaderboard : MonoBehaviour {

		void OnEnable()
		{
			if (!ThinksyPlugin.UsesLeaderboard ())
				gameObject.SetActive (false);
		}
	}
}