using UnityEngine;
using System.Collections;

public class VisitURL : MonoBehaviour 
{
	public bool visitEnrollDeviceURL;
	public bool visitRegistrationURL;
	public Senseix.GameVerificationCode codeSource;
	public string otherURL;

	private string targetURL;



	// Use this for initialization
	void Start () 
	{
		//ResetURL();
	}
	
	private void ResetURL()
	{
		string appendMe = "?ephemeral_token=" + codeSource.GetCode ();

		if (visitEnrollDeviceURL)
		{
			targetURL = Senseix.Message.Request.GetEnrollGameURL() + appendMe;
		}
		else if (visitRegistrationURL)
			targetURL = Senseix.Message.Request.WEBSITE_URL + appendMe;
		else
			targetURL = otherURL;
	}

	public void GoToTargetURL()
	{
		ResetURL();
		Debug.Log ("visiting " + targetURL);
		Application.OpenURL (targetURL);
	}

	// Update is called once per frame
	void Update () 
	{
	
	}
}