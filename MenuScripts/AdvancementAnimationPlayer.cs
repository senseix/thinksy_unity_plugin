using UnityEngine;
using System.Collections;

public class AdvancementAnimationPlayer : MonoBehaviour {

	void OnEnable()
	{
		ThinksyEvents.onAdvanceCategory += PlayAnimation;
	}

	void OnDisable()
	{
		ThinksyEvents.onAdvanceCategory -= PlayAnimation;
	}

	public void PlayAnimation()
	{
		gameObject.GetComponent<ParticleSystem>().Play ();
	}
}
