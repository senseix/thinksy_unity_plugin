using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SignInButton : MonoBehaviour {

	public Text buttonText;
	public GameObject SignInMenu;
	public GameObject MainMenu;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable() {
		if (Senseix.SenseixController.IsSignedIn())
		{
			buttonText.text = "Sign out";
		}
		else
		{
			buttonText.text = "Sign in / Add game";
		}
	}

	public void SignInClicked ()
	{
		MainMenu.SetActive(false);
		SignInMenu.SetActive(true);
	}
}
