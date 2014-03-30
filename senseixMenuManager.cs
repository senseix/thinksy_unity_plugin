using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System;
public class senseixMenuConst{
	public const int MENU_0_MAIN = 0;
	public const int MENU_1_LOGIN = 1;
	public const int MENU_1_SIGNUP = 2;

	public const int MENU_2_FINISH = 3;
	//public const int MAINMENUE_2_

}
public class senseixMenuManager : MonoBehaviour {
	private static bool popSenseixMenu = false;
	private static int menuState = 0;
	private static Rect windowRect = new Rect(1,1,1,1);

	private string emailText = "Your E-mail";
	private string passwordText = "Your password";
	private string nameText = "Your name";

	public static void SenseixMenu()
	{
		popSenseixMenu = true;
	}
	public static void SenseixStopMenu()
	{
		popSenseixMenu = false;
	}
	void Start()
	{
		print ("this is menue Manager1");
	}
	void Update()
	{
		print ("this is menue Manager2");
	}
	void OnGUI()
	{

		if(popSenseixMenu)
		{
			switch(menuState)
			{
				case senseixMenuConst.MENU_0_MAIN:
					windowRect = new Rect(Screen.width/2-90, Screen.height/2-80, 180, 100);
					windowRect = GUILayout.Window(0, windowRect, drawMain, "SenseiX");
				break;
				case senseixMenuConst.MENU_1_LOGIN:
					windowRect = new Rect(Screen.width/2-60, Screen.height/2-100, 120, 200);
					windowRect = GUILayout.Window(0, windowRect, drawLogin, "Login");
				break;
				case senseixMenuConst.MENU_1_SIGNUP:
					windowRect = new Rect(Screen.width/2-60, Screen.height/2-100, 120, 200);
					windowRect = GUILayout.Window(0, windowRect, drawSignup, "Signup");
					break;
				case senseixMenuConst.MENU_2_FINISH:
					popSenseixMenu = false;
				break;
				default:
				break;
			}
		}
	}
	void drawMain(int windowID)
	{
		if (GUILayout.Button("Login"))
		{	
			menuState = senseixMenuConst.MENU_1_LOGIN;
		}
		if (GUILayout.Button("Signup"))
		{	
			menuState = senseixMenuConst.MENU_1_SIGNUP;
		}
		if (GUILayout.Button("Skip"))
		{	
			menuState = senseixMenuConst.MENU_2_FINISH;
		}
	}
	void drawLogin(int windowID)
	{
		emailText = GUILayout.TextField(emailText);
		passwordText = GUILayout.PasswordField(passwordText,"*"[0]);
		if (GUILayout.Button("Login"))
		{	
			if(senseix.coachLogin(emailText,passwordText) == 0)
			{
				emailText="Your E-mail";
				passwordText="Your password";
				menuState = senseixMenuConst.MENU_2_FINISH;
			}
			else
			{
				print ("Login failed");
				menuState = senseixMenuConst.MENU_1_LOGIN;
			}
		}
		if (GUILayout.Button("Cancel"))
		{	
			emailText="Your E-mail";
			passwordText="Your password";
			name="Your name";
			menuState = senseixMenuConst.MENU_0_MAIN;
		}
	}
	void drawSignup(int windowID)
	{
		emailText = GUILayout.TextField(emailText);
		passwordText = GUILayout.PasswordField(passwordText,"*"[0]);
		nameText = GUILayout.TextField (nameText);
		if (GUILayout.Button("Summit"))
		{	
			if(senseix.coachSignUp(emailText,name,passwordText) == 0)
			{
				emailText="Your E-mail";
				passwordText="Your password";
				name="Your name";
				menuState = senseixMenuConst.MENU_2_FINISH;
			}
			else
			{
				print ("Sign up failed");
				menuState = senseixMenuConst.MENU_1_LOGIN;
			}
		}
		if (GUILayout.Button("Cancel"))
		{	
			emailText="Your E-mail";
			passwordText="Your password";
			name="Your name";
			menuState = senseixMenuConst.MENU_0_MAIN;
		}
	}
	
}