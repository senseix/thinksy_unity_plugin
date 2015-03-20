using UnityEngine;
using System.Collections;

public class AdvancementAnimationPlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PlayAnimation()
	{
		gameObject.GetComponent<ParticleSystem>().Play ();
	}
}
