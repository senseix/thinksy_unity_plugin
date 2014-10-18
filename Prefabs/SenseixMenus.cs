using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;

namespace Senseix { 
public class SenseixMenus : MonoBehaviour {

	public string SenseixAccessToken = "";
	static private string[] SignInKeys = new string[4]{"SenseixLogin", "Email", "UserName", "Password"};
	static private Dictionary<string, GameObject> SignIn = new Dictionary<string, GameObject>();
	static private GameObject ResponseMessage;

	public void Start()
	{
		if (SenseixAccessToken == ""){
			throw new Exception("The SenseiX Token you have provided is empty, please register at developer.senseix.com to create a valid key");
		}
		SenseixController.initSenseix (SenseixAccessToken);
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
		Message.Request.SignInParent (email, password);
	}

	public void SignOutUser()
	{
		Message.Request.SignOutParent ();
	}
	public void RegisterUser()
	{
		string email = SignIn["Email"].GetComponent<InputField> ().value;
		string name = SignIn["UserName"].GetComponent<InputField> ().value;
		string password = SignIn["Password"].GetComponent<InputField> ().value;
		Message.Request.RegisterParent (email, name, password);
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
