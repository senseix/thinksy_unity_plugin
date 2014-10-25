using UnityEngine;
using System.Collections;

public class VisitURL : MonoBehaviour {

	public bool visitEnrollDeviceURL;
	public bool visitRegistrationURL;
	public string otherURL;

	private string targetURL;



	// Use this for initialization
	void Start () {
		if (visitEnrollDeviceURL)
			targetURL = Senseix.Message.Request.ENROLL_GAME_URL;
		else if (visitRegistrationURL)
			targetURL = Senseix.Message.Request.WEBSITE_URL;
		else
			targetURL = otherURL;
	}

	public void GoToTargetURL()
	{
		Debug.Log ("visiting " + targetURL);
		Application.OpenURL (targetURL);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
