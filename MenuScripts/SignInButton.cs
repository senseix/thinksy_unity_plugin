using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace Senseix
{
	public class SignInButton : MonoBehaviour 
	{
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
			if (Senseix.SenseixController.IsSignedIn())
			{
				SignOut();
			}
			else
			{
				MainMenu.SetActive(false);
				SignInMenu.SetActive(true);
			}
		}

		private void SignOut()
		{
			SenseixController.SignOutParent ();
		}
	}
}