using UnityEngine;
using System.Collections;

public class AdvancementAnimationPlayer : MonoBehaviour {

	void OnEnable()
	{
		ThinksyEvents.OnAdvanceCategory += PlayAnimation;
	}

	void OnDisable()
	{
		ThinksyEvents.OnAdvanceCategory -= PlayAnimation;
	}

	public void PlayAnimation()
	{
		gameObject.GetComponent<ParticleSystem>().Play ();
	}
}
