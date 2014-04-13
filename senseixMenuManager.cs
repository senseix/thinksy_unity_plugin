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

	public const int MENU_3_RUNNING = 4;

	public const int MENU_4_PROFILE = 5;
	public const int MENU_4_CREATE_PROFILE = 6;

	public const int MENU_WIN_NUM = 9876;
	//public const int MAINMENUE_2_

}
public class senseixMenuManager : MonoBehaviour {
	private static bool popSenseixMenu = false;
	public static int menuState = 0;
	private static Rect windowRect = new Rect(1,1,1,1);
	private static Rect blurLayer = new Rect(1,1,1,1);

	private string emailText = "Your E-mail";
	private string passwordText = "Your password";
	private string nameText = "Your name";

	public static Queue players = null;

	public static problem currentProblem = null;
	private static bool answerProvided = true;
	public static void SenseixMenu(string access_token = null)
	{
		if (access_token != null && menuState == senseixMenuConst.MENU_0_MAIN) 
		{
			senseix.initSenseix (access_token);
			if (senseix.inSession)
			{
				senseixMenuManager.menuState = senseixMenuConst.MENU_3_RUNNING;
				return;
			}
			players = senseix.getCachedPlayer ();
			popSenseixMenu = true;
		}
		else if(access_token != null && menuState != senseixMenuConst.MENU_0_MAIN)
		{
			//This is error that developer should not reinitialize this program
			//once it has been initialized
		}
		else if(access_token == null && menuState != senseixMenuConst.MENU_0_MAIN)
		{
			//This case is that game has been initialized, and player called 
			//menu out. we just need to set popSenseixMenu to true
			popSenseixMenu = true;
		}
	}
	public static void debug_menu_state()
	{
		print (senseixMenuManager.menuState);
	}
	public static void SenseixStopMenu()
	{
		popSenseixMenu = false;
	}
	void Start()
	{
		//print ("this is menue Manager1");
	}
	void Update()
	{
		//print ("this is menue Manager2");
	}
	void OnGUI()
	{
		if(popSenseixMenu)
		{
			switch(menuState)
			{
				case senseixMenuConst.MENU_3_RUNNING:
					//This should be the menu that poped during game.
					//So developers can have their own buttons in game, which can trigger this menu
					windowRect = new Rect(Screen.width/2-90, Screen.height/2-80, 180, 100);
					windowRect = GUILayout.Window(senseixMenuConst.MENU_WIN_NUM, windowRect, drawRunning, "SenseiX");
					blurBackground();
					GUI.BringWindowToFront (senseixMenuConst.MENU_WIN_NUM);
					break;
				case senseixMenuConst.MENU_0_MAIN:
					windowRect = new Rect(Screen.width/2-90, Screen.height/2-80, 180, 100);
					windowRect = GUILayout.Window(senseixMenuConst.MENU_WIN_NUM, windowRect, drawMain, "SenseiX");
					blurBackground();
					GUI.BringWindowToFront (senseixMenuConst.MENU_WIN_NUM);
				break;
				case senseixMenuConst.MENU_1_LOGIN:
					windowRect = new Rect(Screen.width/2-60, Screen.height/2-100, 120, 100);
					windowRect = GUILayout.Window(senseixMenuConst.MENU_WIN_NUM, windowRect, drawLogin, "Login");
					blurBackground();
					GUI.BringWindowToFront (senseixMenuConst.MENU_WIN_NUM);
				break;
				case senseixMenuConst.MENU_1_SIGNUP:
					windowRect = new Rect(Screen.width/2-60, Screen.height/2-100, 120, 100);
					windowRect = GUILayout.Window(senseixMenuConst.MENU_WIN_NUM, windowRect, drawSignup, "Signup");
					blurBackground();
					GUI.BringWindowToFront (senseixMenuConst.MENU_WIN_NUM);
				break;
				case senseixMenuConst.MENU_2_FINISH:
					//popSenseixMenu = false;
					if(senseix.inSession)
						menuState = senseixMenuConst.MENU_4_PROFILE;
				break;
				case senseixMenuConst.MENU_4_PROFILE:
					//print ("This is profile");
					windowRect = new Rect(Screen.width/2-60, Screen.height/2-100-players.Count*10, 120, 100);
					windowRect = GUILayout.Window(senseixMenuConst.MENU_WIN_NUM, windowRect, drawProfile, "Profiles");
					blurBackground();
					GUI.BringWindowToFront (senseixMenuConst.MENU_WIN_NUM);
				break;
				case senseixMenuConst.MENU_4_CREATE_PROFILE:
					windowRect = new Rect(Screen.width/2-60, Screen.height/2-100, 120, 100);
					windowRect = GUILayout.Window(senseixMenuConst.MENU_WIN_NUM, windowRect, drawSignup, "Create Profile");
					blurBackground();
					GUI.BringWindowToFront (senseixMenuConst.MENU_WIN_NUM);
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
			emailText="80640000@qq.com";
			passwordText="password.com";
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
				/*
				emailText="Your E-mail";
				passwordText="Your password";
				*/
				emailText="80640000@qq.com";
				passwordText="password.com";
				print ("Login sucessful");
				players = senseix.getPlayer();
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
	void drawProfile(int windowID)
	{
		heavyUser player = null;
		StringBuilder result = new StringBuilder();
		if (GUILayout.Button("create player"))
		{	

		}
		if(players != null)
		for(int i=0;i<players.Count;i++)
		{

			player = (heavyUser)players.Dequeue();
			if(player != null)
			{
				result = new StringBuilder();
				if(player.name.Length > 20)
					result.Append("Length Error");
				else
					result.Append(player.name);
				players.Enqueue(player);
				if (GUILayout.Button(result.ToString()))
				{
					senseix.name=player.name;
					senseix.id=Convert.ToInt32(player.id);
					senseix.saveProfileID();
					print (senseix.name + " " + senseix.id.ToString());
					menuState = senseixMenuConst.MENU_3_RUNNING;
					popSenseixMenu = false;
				}
			}	
		}
	}
	void drawRunning(int windowID)
	{
		if (GUILayout.Button("Resume"))
		{	
			popSenseixMenu = false;
		}
		if (GUILayout.Button("Sign out"))
		{	
			senseix.coachLogout();
			menuState = senseixMenuConst.MENU_0_MAIN;
			popSenseixMenu = false;
		}
	}
	private static void blurBackground()
	{
		blurLayer = new Rect(0,0,Screen.width,Screen.height);
		blurLayer = GUILayout.Window(0, blurLayer, null,"");

	}
	/*
	public static string getQuestion()
	{
		if(answerProvided)
		{
			Queue problems = senseix.getCachedProblemQ();
			if (problems.Count == 0) 
			{
				problems = senseix.pullProblemQ (4, "Mathematics", 1);
			} 
			else 
			{
				currentProblem=(problem)problems.Dequeue();
			}
			answerProvided = false;
		}
		return currentProblem.content;
	}
	*/
	public static string getProblem()
	{
		if(answerProvided)
		{
			print ("Problem debug: current Problem setup");
			currentProblem = senseixGameManager.getProblem();
			answerProvided = false;
		}
		return currentProblem.content;
	}
	public static bool gotAnswer(string answer)
	{
		//FIXME:
		bool correct = true; //senseix.checkAnswer(tmp,demoProblem);
		senseix.pushProblemA(currentProblem.problemID,0,correct,1,1,answer);
		answerProvided = true;
		return true;
	}

}