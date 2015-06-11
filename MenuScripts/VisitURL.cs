using UnityEngine;
using System.Collections;

public class VisitURL : MonoBehaviour 
{
	public Senseix.GameVerificationCode codeSource;

	private string targetURL;

	// Use this for initialization
	void Start () 
	{
		//ResetURL();
	}
	
	private void ResetURL()
	{
		string appendMe = "?ephemeral_token=" + codeSource.GetCode ();

		targetURL = Senseix.Message.Request.GetEnrollGameURL() + appendMe;
	}

	public void GoToTargetURL()
	{
		ResetURL();
		Debug.Log ("visiting " + targetURL);
		Application.OpenURL (targetURL);
	}
}