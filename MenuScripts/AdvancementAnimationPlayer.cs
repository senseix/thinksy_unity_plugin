using UnityEngine;
using System.Collections;

public class AdvancementAnimationPlayer : MonoBehaviour {

	public ParticleSystem particleSystemToPlay;
	//public Animator animatorToPlay;

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
		particleSystemToPlay.Play ();
		//animatorToPlay.Play (0);
	}
}
