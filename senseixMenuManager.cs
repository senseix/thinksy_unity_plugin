using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System;
using System.Threading;
public class senseixMenuConst{
	public const int MENU_0_MAIN = 0;
	public const int MENU_1_LOGIN = 1;
	public const int MENU_1_SIGNUP = 2;

	public const int MENU_2_FINISH = 3;

	public const int MENU_3_RUNNING = 4;

	public const int MENU_4_PROFILE = 5;
	public const int MENU_4_CREATE_PROFILE = 6;
	public const int MENU_5_LEADERBOARD_SHOW = 7;

	public const int MENU_WIN_NUM = 9876;
	//public const int MAINMENUE_2_

}
public class senseixMenuManager : MonoBehaviour {
	public static bool popSenseixMenu = false;
	public static bool specifyNumber = true;
	public static bool gameStarted = false;
	public static int skipCount = 1;
	public static int menuState = 0;
	private static Rect windowRect = new Rect(1,1,1,1);
	private static Rect blurLayer = new Rect(1,1,1,1);

	private string emailText = "Your E-mail";
	private string passwordText = "Your password";
	private string nameText = "Your name";
	private string profileNameText = "Profile Name";
	//public static Queue players = null;
	public static ArrayList players = new ArrayList();

	public static SenseixStyle mainStyle = new SenseixStyle();
	public static GUIStyle buttonStyle = null;
	public static problem currentProblem = null;
	private static bool answerProvided = true;

	private static messageLine line = new messageLine();
	//current answer result beg
	public static string currentAnswer = null;
	public static bool currentCorrectness = false;

	public static int currentLeaderboardPage = 0;

	//current answer result beg
	public static void SenseixMenu(string access_token = null)
	{
		if (access_token != null && menuState == senseixMenuConst.MENU_0_MAIN) 
		{
			senseix.initSenseix (access_token);
			players = senseix.getCachedPlayerA();

			//channelThread.Start();
			buttonStyle = new GUIStyle();
			buttonStyle.fontSize = Screen.width/70;
			buttonStyle.normal.textColor= Color.white;
			buttonStyle.alignment = TextAnchor.MiddleCenter;
			buttonStyle.richText = true;
			buttonStyle.fontStyle = FontStyle.Bold;
			popSenseixMenu = true;
			if (senseix.inSession)
			{
				senseixMenuManager.menuState = senseixMenuConst.MENU_3_RUNNING;
				senseixGameManager.prepareProblem (3, "Mathematics", 1);
				getProblem();
				return;
			}
			//mainStyle.initButton();

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
	public static void pushQAnswer()
	{
		//print ("[DEBUG][Threading] pushing Answer");
		senseix.pushProblemA(currentProblem.problemID,0,currentCorrectness,1,1,currentAnswer);
	}
	public static void debug_menu_state()
	{
		//print (senseixMenuManager.menuState);
	}
	public static void SenseixStopMenu()
	{
		popSenseixMenu = false;
	}
	void Start()
	{
		//print ("this is menue Manager1");
	}
	public static void updateScanner()
	{
		//print ("this is menue Manager2");
		line.scanMessages ();
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
					windowRect = new Rect(Screen.width/2-Screen.width/8, Screen.height/2-80, Screen.width/5, 100);
					windowRect = GUILayout.Window(senseixMenuConst.MENU_WIN_NUM, windowRect, drawRunning, "SenseiX");
					//blurBackground();
					//GUI.BringWindowToFront (senseixMenuConst.MENU_WIN_NUM);
					break;
				case senseixMenuConst.MENU_0_MAIN:
				windowRect = new Rect(Screen.width/2-Screen.width/10, Screen.height/2-80, Screen.width/5, 100);
					windowRect = GUILayout.Window(senseixMenuConst.MENU_WIN_NUM, windowRect, drawMain, "SenseiX");
					//blurBackground();
					//GUI.BringWindowToFront (senseixMenuConst.MENU_WIN_NUM);
				break;
				case senseixMenuConst.MENU_1_LOGIN:
				windowRect = new Rect(Screen.width/2-Screen.width/10, Screen.height/2-100, Screen.width/5, 100);
					windowRect = GUILayout.Window(senseixMenuConst.MENU_WIN_NUM, windowRect, drawLogin, "Login");
					//blurBackground();
					//GUI.BringWindowToFront (senseixMenuConst.MENU_WIN_NUM);
				break;
				case senseixMenuConst.MENU_1_SIGNUP:
				windowRect = new Rect(Screen.width/2-Screen.width/10, Screen.height/2-100, Screen.width/5, 100);
					windowRect = GUILayout.Window(senseixMenuConst.MENU_WIN_NUM, windowRect, drawSignup, "Signup");
					//blurBackground();
					//GUI.BringWindowToFront (senseixMenuConst.MENU_WIN_NUM);
				break;
				case senseixMenuConst.MENU_2_FINISH:
					//popSenseixMenu = false;
					if(senseix.inSession)
						menuState = senseixMenuConst.MENU_4_PROFILE;
				break;
				case senseixMenuConst.MENU_4_PROFILE:
					//print ("This is profile");
					if(players != null)
					windowRect = new Rect(Screen.width/2-Screen.width/10, Screen.height/2-100-players.Count*10, Screen.width/5, 100);
					else
					windowRect = new Rect(Screen.width/2-Screen.width/10, Screen.height/2-100,Screen.width/5,20);
					windowRect = GUILayout.Window(senseixMenuConst.MENU_WIN_NUM, windowRect, drawProfile, "Profiles");
					//blurBackground();
					//GUI.BringWindowToFront (senseixMenuConst.MENU_WIN_NUM);
				break;
				case senseixMenuConst.MENU_4_CREATE_PROFILE:
				windowRect = new Rect(Screen.width/2-Screen.width/10, Screen.height/2-100, Screen.width/5, 100);
					windowRect = GUILayout.Window(senseixMenuConst.MENU_WIN_NUM, windowRect, drawCreateProfile, "Create Profile");
					//blurBackground();
					//GUI.BringWindowToFront (senseixMenuConst.MENU_WIN_NUM);
				break;
				case senseixMenuConst.MENU_5_LEADERBOARD_SHOW:
				windowRect = new Rect(Screen.width/2-Screen.width/10, Screen.height/2-80, Screen.width/5,200);
					windowRect = GUILayout.Window(senseixMenuConst.MENU_WIN_NUM, windowRect, drawLeaderboard, "SenseiX");
				break;
				default:
				break;
			}
		}
	}
	void drawMain(int windowID)
	{
		if (GUILayout.Button("Login",buttonStyle,GUILayout.Height(50)))
		{	
			emailText="80640000@qq.com";
			passwordText="password.com";
			menuState = senseixMenuConst.MENU_1_LOGIN;
		}
		if (GUILayout.Button("Signup",buttonStyle,GUILayout.Height(50)))
		{	
			menuState = senseixMenuConst.MENU_1_SIGNUP;
		}
		//if (GUILayout.Button("Skip"))
		//{	
		//	menuState = senseixMenuConst.MENU_2_FINISH;
		//}
	}
	void drawLogin(int windowID)
	{
		emailText = GUILayout.TextField(emailText);
		passwordText = GUILayout.PasswordField(passwordText,"*"[0]);
		if (GUILayout.Button("Login",buttonStyle,GUILayout.Height(50)))
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
				players = senseix.getPlayerA();
				menuState = senseixMenuConst.MENU_2_FINISH;
			}
			else
			{
				print ("Login failed");
				menuState = senseixMenuConst.MENU_1_LOGIN;
			}
		}
		if (GUILayout.Button("Cancel",buttonStyle,GUILayout.Height(50)))
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
		if (GUILayout.Button("Summit",buttonStyle,GUILayout.Height(Screen.height/8)))
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
				//print ("Sign up failed");
				menuState = senseixMenuConst.MENU_1_LOGIN;
			}
		}
		if (GUILayout.Button("Cancel",buttonStyle,GUILayout.Height(Screen.height/8)))
		{	
			emailText="Your E-mail";
			passwordText="Your password";
			name="Your name";
			menuState = senseixMenuConst.MENU_0_MAIN;
		}
	}
	void drawLeaderboard(int windowID)
	{
		if (leaderboard.ready ())
		{
			StringBuilder result = new StringBuilder();
			for(int i=currentLeaderboardPage*5;i<leaderboard.entries.Count && i<(1+currentLeaderboardPage)*5;i++)
			{
				result.Append(((lbEntry)leaderboard.entries[i]).rank+". Name: "+((lbEntry)leaderboard.entries[i]).name);
				GUILayout.Label(result.ToString());
				result.Remove(0,result.Length);
			}
		}
		if (GUILayout.Button("Next Page",buttonStyle,GUILayout.Height(Screen.height/16)))
		{	
			if(currentLeaderboardPage<(leaderboard.entries.Count-1)/5)
				currentLeaderboardPage++;
		}
		if (GUILayout.Button("Prev Page",buttonStyle,GUILayout.Height(Screen.height/16)))
		{	
			if(currentLeaderboardPage>0)
				currentLeaderboardPage--;
		}
		if (GUILayout.Button("Return",buttonStyle,GUILayout.Height(Screen.height/16)))
		{	
			menuState = senseixMenuConst.MENU_3_RUNNING;
		}
	}
	void drawProfile(int windowID)
	{
		heavyUser player = null;
		StringBuilder result = new StringBuilder();
		if (GUILayout.Button("create player",buttonStyle,GUILayout.Height(Screen.height/10)))
		{	
			menuState = senseixMenuConst.MENU_4_CREATE_PROFILE;
		}
		if(players != null)
		for(int i=0;i<players.Count;i++)
		{

			//queue to array
			/*
			player = (heavyUser)players.Dequeue();
			if(player != null)
			{
				result = new StringBuilder();
				if(player.name.Length > 20)
					result.Append("Length Error");
				else
					result.Append(player.name);
				players.Enqueue(player);
				if (GUILayout.Button(result.ToString(),buttonStyle,GUILayout.Height(Screen.height/10)))
				{
					senseix.name=player.name;
					senseix.id=Convert.ToInt32(player.id);
					senseix.saveProfileID();
					//print (senseix.name + " " + senseix.id.ToString());
					menuState = senseixMenuConst.MENU_3_RUNNING;
					senseixGameManager.prepareProblem (3, "Mathematics", 1);
					getProblem();
					popSenseixMenu = false;
				}
			}	
			*/
		}
	}
	void drawCreateProfile(int windowID)
	{
		profileNameText = GUILayout.TextField(profileNameText);
		if (GUILayout.Button("Summit",buttonStyle,GUILayout.Height(Screen.height/14)))
		{	
			if(senseix.createPlayer(profileNameText) == 0)
			{
				profileNameText = "Profile Name";
				players = senseix.getPlayerA();
				menuState = senseixMenuConst.MENU_4_PROFILE;
			}
		}
		if (GUILayout.Button("Cancel",buttonStyle,GUILayout.Height(Screen.height/14)))
		{	
			profileNameText = "Profile name";
			menuState = senseixMenuConst.MENU_4_PROFILE;
		}
	}
	void drawRunning(int windowID)
	{
		if (GUILayout.Button("Resume",buttonStyle,GUILayout.Height(Screen.height/10)))
		{	
			popSenseixMenu = false;
		}
		if (GUILayout.Button("Show Leaderboard",buttonStyle,GUILayout.Height(Screen.height/10)))
		{	
			senseix.pullLeaderboard(1);
			leaderboard.debugPrint();
			menuState = senseixMenuConst.MENU_5_LEADERBOARD_SHOW;
			currentLeaderboardPage = 0;
		}
		if (GUILayout.Button("Sign out",buttonStyle,GUILayout.Height(Screen.height/10)))
		{	
			senseix.coachLogout();
			menuState = senseixMenuConst.MENU_0_MAIN;
			//popSenseixMenu = true;
		}
		senseix.str1 = GUILayout.TextField (senseix.str1);
	}
	private static void blurBackground()
	{
		blurLayer = new Rect(0,0,Screen.width,Screen.height);
		blurLayer = GUILayout.Window(6677, blurLayer, null,"");
	}
	public static string getProblem()
	{
		if(!gameStarted)
			gameStarted = true;
		if(answerProvided)
		{
			//print ("getProblem set to false");
			answerProvided = false;
			//print ("Problem debug: current Problem setup");
			currentProblem = senseixGameManager.getProblem();
		}
		//else
			//print ("Problem debug: current Problem not setup");
		//print ("===Answer is " + currentProblem.answer);
		if (currentProblem != null)
		{
			//print ("===Got problem: " + currentProblem.content);
			return currentProblem.content;
		}
		else
			return "Please login SenseiX";
	}
	public static string getAnswer()
	{
		getProblem();
		//specifyNumber = false;
		return currentProblem.answer;
	}
	public static bool gotAnswer(string answer)
	{
		//FIXME:
		bool correct = (Convert.ToInt32(answer) == Convert.ToInt32(currentProblem.answer)); //senseix.checkAnswer(tmp,demoProblem);
		currentAnswer = answer;
		currentCorrectness = correct;
		answerProvided = true;
		specifyNumber = true;
		skipCount = UnityEngine.Random.Range (0,3);
		//print ("Got new skipCount "+skipCount);
		container message = senseix.pushProblemAMT(currentProblem.problemID,1,correct,1,1,answer);
		message.formBinary();
		//print (message.url);
		WWW recvResult =new WWW (message.url,message.binary);
		//print ("Answer:" + correct.ToString() );
		line.addMessage(new pagePack(messageType.MESSAGETYPE_PROBLEM_PUSH,recvResult));
		return correct;
	}
	//FIXME: how about case that pulling problems fail
	static public void storeProblems(int index)
	{
		string storeName = "problem0" + index.ToString ();
		string problemStr =senseix.pullProblemQStr (20,"Mathematics",senseixManager.levelDecider());
		print ("stored problems: " + problemStr);
		PlayerPrefs.SetString (storeName,problemStr);
		PlayerPrefs.Save();	
	}
}