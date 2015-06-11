using UnityEngine;
using System.Collections;

public class StagingStrike : MonoBehaviour {

	static public void Boom()
	{
		FindObjectOfType<StagingStrike> ().InstanceBoom ();
	}

	public void InstanceBoom()
	{
		StartCoroutine (BoomCoroutine ());
	}

	private IEnumerator BoomCoroutine()
	{
		for (int i = 0; i < 300; i++)
		{
			if (i % 30 == 0) gameObject.GetComponent<UnityEngine.UI.Image> ().color = new Color(255, 255, 255, 255);
			if (i % 30 == 15) gameObject.GetComponent<UnityEngine.UI.Image> ().color = new Color(255, 255, 255, 0);
			yield return null;
		}
	}
}
