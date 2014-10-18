using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;

namespace Senseix { 
public class SenseixMenus : MonoBehaviour {

	static private string[] SignInKeys = new string[4]{"SenseixLogin", "Email", "UserName", "Password"};
	static private Dictionary<string, GameObject> SignIn = new Dictionary<string, GameObject>();
	private Message.Request request = new Message.Request();

	public void Start()
	{

		foreach (string key in SignInKeys)
		{
			SignIn[key] = GameObject.Find(key);
			print(SignIn[key]);
		}
		SignIn["SenseixLogin"].SetActive (false);	
	}


	public void SignInUser()
	{
		string email = SignIn["Email"].GetComponent<InputField> ().value;
		string password = SignIn["Password"].GetComponent<InputField> ().value;
		request.SignInParent (email, password);
	}

	public void SignOutUser()
	{
		request.SignOutParent ();
	}
	public void RegisterUser()
	{
		string email = SignIn["Email"].GetComponent<InputField> ().value;
		string name = SignIn["UserName"].GetComponent<InputField> ().value;
		string password = SignIn["Password"].GetComponent<InputField> ().value;
		request.RegisterParent (email, name, password);
	}

	public void DrawLoginWindow()
	{
		SignIn["SenseixLogin"].SetActive (true);
	}
	public void CloseLoginWindow()
	{
		SignIn["SenseixLogin"].SetActive (false);
	}
		
}
}
