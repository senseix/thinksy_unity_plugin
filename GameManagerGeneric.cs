using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;
using System;
using System.Threading;
using System.IO;
//When app opened, first send out a UID ack
//*if respond say it is tmp user, then automaticly use that tmp profile
//then use main menu: sign up, login, start
//*if respond say it is regular user, then show the list of profile and allow user to select one profile
//then use in game menu: logout, check profile, start game


/// <summary>
/// Game Manager Main Class.
/// </summary>
/// Cheng: quiz list is the place that store those questions
/// 		where need us to modify.
public class GameManagerGeneric : MonoBehaviour
{
    // Effect Prefab
    //public GameObject goodEffect, badEffect, soulEffect, happyEffect;
    // Actor Animator Component
	//public Animator friendAnimator, enemyAnimator, mmfriendAnimator,mmenemyAnimator;
    // Actor HP Manager Component
    //public HpManager friendHpMan, enemyHpMan;
    private static bool showMenu = true;
    private int SHOW_OFFSET = Screen.height;
	public const string errorLogin = "Login Failed";
	public const string errorSignup = "Signup Failed";
	public const string errorCreateProfile = "Create profile failed";
	public const string errorNetFail = "Network Failed";
	private string errorTitle = "";
	public List<UILabel> profileLables;
	public List<UILabel> ldNameLable;
	public List<UILabel> ldScoreLable;
	//public int[] hpDamage;
	public int currentLevel = 0;
	public UILabel loadingLable;
	//public int idid;
	public UILabel onlineDebug;
	public UILabel debugText;
	public UIPanel buttonPanel;
	public UIPanel signupPanel;
	public UIPanel loginPanel;
	public UIPanel leaderboardPanel;
	public UIPanel profileSelectPanel;
	public UIPanel createProfilePanel;
	public UIPanel winPanel;
	public UIPanel errorPanel;
	//public SkinnedMeshRenderer skeleton;
	public UIInput signupEmail;
	public UIInput signupPassword;
	public UIInput signupName;
	//public UILabel roundLable;
	public UILabel errorContent;
	//public UILabel enemyChose;
	public UIInput loginEmail;
	public UIInput loginPassword;
	public UIInput createProfileName;
	private bool inError = false;
	public static ArrayList players = null;
	private bool switchPlay = false;
	private int wait = 0;
	//public UniWebView webb = null;
    // Save Start Position
	//Vector3 friendPos, enemyPos, friendHpPos, enemyHpPos, shieldPos,mmfriendPos,mmenemyPos,mainmenuPanelPos,loginPanelPos,signupPanelPos,profileSelectPanelPos,roundLablePos,createProfilePos,errorPos,winPanelPos,enemyTurnPanelPos,leaderboardPanelPos;
	Vector3 mainmenuPanelPos,loginPanelPos,signupPanelPos,profileSelectPanelPos,createProfilePos,errorPos,winPanelPos,leaderboardPanelPos;
    //Transform friendHpGroup, enemyHpGroup, shieldGroup;

    // Save Question & Answer Display Position
    Transform questionTf;
    Transform[] answerTfs;
    UILabel questionLabel;
    UILabel[] answerLabels;

	TweenParms[] origParms;

	private int[] numberOnCard;
	private int currentAnwser;
	private string currentQuestion;
	private bool inInit = true;

	//private int[] cardOffseth;
	//private int[] cardOffsetv;
    // Quiz List Array
    //List<QuizData> quizList;
    int quizTotal;
    int quizIndex = 0;

    //[HideInInspector]
    //public int quizLength = 0;

    // Quiz Condition
    //bool quizOn = true;
	bool playing = false;
	//bool InGameMenu = false;
	//ssx
	int checkConnectivity()
	{
		return 0;
	}	
	void initSsxPanelPos()
	{
		mainmenuPanelPos = buttonPanel.transform.localPosition;
		signupPanelPos = signupPanel.transform.localPosition;
		loginPanelPos = loginPanel.transform.localPosition;
		profileSelectPanelPos = profileSelectPanel.transform.localPosition;
		createProfilePos = createProfilePanel.transform.localPosition;
		errorPos = errorPanel.transform.localPosition;
		winPanelPos = winPanel.transform.localPosition;
		leaderboardPanelPos = leaderboardPanel.transform.localPosition;
	}
	void initSsxProfileLables()
	{
		for(int i=0;i<9;i++)
		{
			profileLables[i].text="Empty";
		}
	}
	void initSsxLDLables()
	{
		for(int i=0;i<15;i++)
		{
			ldScoreLable[i].text="Score";
			ldNameLable[i].text="EMPTY";
		}
	}
	void Load()
	{
		loadingLable.text = "Loading......";
		loadingLable.text = "";
		print ("loading");
	}
	/*
	 * This is code that show or hide single menu
	 */
#if false	 
	public void showEnemyTurnPanel()
	{
		Vector3 pos = new Vector3(enemyTurnPanelPos.x,enemyTurnPanelPos.y+720,enemyTurnPanelPos.z);
		//buttonPanel.transform.localPosition = new Vector3(pos.x, pos.y+3f, pos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(enemyTurnPanel.transform, 1f, parms);
	}
	public void hideEnemyTurnPanel()
	{
		Vector3 pos = new Vector3(enemyTurnPanelPos.x,enemyTurnPanelPos.y,enemyTurnPanelPos.z);
		//buttonPanel.transform.localPosition = new Vector3(pos.x, pos.y+3f, pos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(enemyTurnPanel.transform, 1f, parms);
	}
#endif
	public void showLeaderboard()
	{
		senseix.pullLeaderboard(2);
		//leaderboard.debugPrint();
		drawLeaderboardList();
		Vector3 pos = new Vector3(leaderboardPanelPos.x,leaderboardPanelPos.y+SHOW_OFFSET,leaderboardPanelPos.z);
		//buttonPanel.transform.localPosition = new Vector3(pos.x, pos.y+3f, pos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(leaderboardPanel.transform, 1f, parms);
	}
	public void hideLeaderboard()
	{
		Vector3 pos = new Vector3(leaderboardPanelPos.x,leaderboardPanelPos.y,leaderboardPanelPos.z);
		//buttonPanel.transform.localPosition = new Vector3(pos.x, pos.y+3f, pos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(leaderboardPanel.transform, 1f, parms);
	}
	public void showWinPanel()
	{
		
		Vector3 pos = new Vector3(winPanelPos.x,winPanelPos.y+SHOW_OFFSET,winPanelPos.z);
		//buttonPanel.transform.localPosition = new Vector3(pos.x, pos.y+3f, pos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(winPanel.transform, 1f, parms);
	}
	public void hideWinPanel()
	{
		Vector3 pos = new Vector3(winPanelPos.x,winPanelPos.y,winPanelPos.z);
		//buttonPanel.transform.localPosition = new Vector3(pos.x, pos.y+3f, pos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(winPanel.transform, 1f, parms);
		ssxStartGame();
	}
	public void showSignupPanel()
	{
		Vector3 pos = new Vector3(signupPanelPos.x,signupPanelPos.y+SHOW_OFFSET,signupPanelPos.z);
		//buttonPanel.transform.localPosition = new Vector3(pos.x, pos.y+3f, pos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(signupPanel.transform, 1f, parms);
	}
	public void hideSignupPanel()
	{
		Vector3 pos = new Vector3(signupPanelPos.x,signupPanelPos.y,signupPanelPos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(signupPanel.transform, 1f, parms);
	}
	public void showCreateProfile()
	{
		//hideProfileList ();
		//hideMainmenuPanel ();
		Vector3 pos = new Vector3(createProfilePos.x,createProfilePos.y+SHOW_OFFSET,createProfilePos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(createProfilePanel.transform, 1f, parms);
	}
	public void hideCreateProfile()
	{
		Vector3 pos = new Vector3(signupPanelPos.x,signupPanelPos.y,signupPanelPos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(createProfilePanel.transform, 1f, parms);
		drawProfileList();
		//showProfileList ();
		players=senseixManager.getPlayers();
	}
#if false
	public void showRound()
	{
		roundLable.text = "Round " + currentLevel.ToString();
		Vector3 pos = new Vector3(roundLablePos.x,roundLablePos.y+720,roundLablePos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(roundLable.transform, 1f, parms);
		/*
		pos = new Vector3(roundLablePos.x,roundLablePos.y+Screen.height+5,roundLablePos.z);
		parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(roundLable.transform, 0.0000001f, parms);
		*/
		hideRound ();
	}
	public void hideRound()
	{
		Vector3 pos = new Vector3(roundLablePos.x,roundLablePos.y,roundLablePos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(roundLable.transform, 1f, parms);
	}
#endif
	public void debugs(string text)
	{
		debugText.text = text;
	}
	public void showLoginPanel()
	{
		Vector3 pos = new Vector3(loginPanelPos.x,loginPanelPos.y+SHOW_OFFSET,loginPanelPos.z);
		//buttonPanel.transform.localPosition = new Vector3(pos.x, pos.y+3f, pos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(loginPanel.transform, 1f, parms);
	}
	public void hideLoginPanel()
	{
		Vector3 pos = new Vector3(loginPanelPos.x,loginPanelPos.y,loginPanelPos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(loginPanel.transform, 1f, parms);
//		showMainmenuPanel ();
		//print ("signup");
	}
	public void showProfileList()
	{
		//hideLoginPanel ();
		players = senseixManager.getCachedPlayers ();
		if(players != null)
			drawProfileList ();
		Vector3 pos = new Vector3(profileSelectPanelPos.x,profileSelectPanelPos.y+SHOW_OFFSET,profileSelectPanelPos.z);
		//buttonPanel.transform.localPosition = new Vector3(pos.x, pos.y+3f, pos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(profileSelectPanel.transform, 1f, parms);
	}
	public void hideProfileList()
	{
		Vector3 pos = new Vector3(profileSelectPanelPos.x,profileSelectPanelPos.y,profileSelectPanelPos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(profileSelectPanel.transform, 1f, parms);
		//showMainmenuPanel ();
	}
	public void shakePanel(UIPanel toShake)
	{
		for(int i=0;i<20;i++)
		{
		Vector3 Pos = new Vector3(toShake.transform.localPosition.x,toShake.transform.localPosition.y+20,toShake.transform.localPosition.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition",Pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(toShake.transform, 1f, parms);
		Thread.Sleep(2000);
		Pos = new Vector3(toShake.transform.localPosition.x,toShake.transform.localPosition.y-20,toShake.transform.localPosition.z);
		parms = new TweenParms ().Prop ("localPosition",Pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(toShake.transform, 1f, parms);
		}
	}
	public void drawLeaderboardList()
	{
		//so far we only support 9 in list
		if(leaderboard.entries.Count > 0)
		{
			for(int i=0;i<leaderboard.entries.Count && i < 15;i++)
			{
				print ("Drawing "+ ((lbEntry)leaderboard.entries[i]).name + " " + (leaderboard.entries.Count-1 - i));
				if(((lbEntry)leaderboard.entries[leaderboard.entries.Count-1 - i]).name.Length>15)
					ldNameLable[i].text = ((lbEntry)leaderboard.entries[leaderboard.entries.Count-1 - i]).name.Substring(0,15);
				else
					ldNameLable[i].text = ((lbEntry)leaderboard.entries[leaderboard.entries.Count-1 - i]).name;
				ldScoreLable[i].text = ((lbEntry)leaderboard.entries[leaderboard.entries.Count-1 - i]).score.ToString();
			}
		}
	}
	public void drawProfileList()
	{
		string playerName = null;
		for(int i=0;i<players.Count;i++)
		{
			if(((heavyUser)players[i]).name.Length>15)
				playerName = profileLables[i].text=((heavyUser)players[i]).name.Substring(0,15);
			else
				playerName = profileLables[i].text=((heavyUser)players[i]).name;
		}
	}
	public void showMainmenuPanel()
	{
		Vector3 pos = new Vector3(mainmenuPanelPos.x,mainmenuPanelPos.y,mainmenuPanelPos.z);
		//buttonPanel.transform.localPosition = new Vector3(pos.x, pos.y+3f, pos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(buttonPanel.transform, 1f, parms);
	}
	void hideMainmenuPanel()
	{
		Vector3 pos = new Vector3(mainmenuPanelPos.x,mainmenuPanelPos.y+SHOW_OFFSET,mainmenuPanelPos.z);
		//buttonPanel.transform.localPosition = new Vector3(pos.x, pos.y+3f, pos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(buttonPanel.transform, 1f, parms);
	}
	public void showError()
	{
		inError = true;
		Vector3 pos = new Vector3(errorPos.x,errorPos.y+SHOW_OFFSET,errorPos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(errorPanel.transform, 1f, parms);
	}
	public void hideError()
	{
		inError = false;
		Vector3 pos = new Vector3(errorPos.x,errorPos.y,errorPos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(errorPanel.transform, 1f, parms);
		if(!senseixManager.network()) // if network is not ready, or we don't have cached problems
		{
			if ((senseixManager.initSenseixManager ("0af8c34ad030e7088145bcb4667b5b76d3bbc1019fa243330da56e9473f60e77") != 0 || !senseix.inSession)) 
			{
				onlineDebug.text = senseix.inSession?"online":"offline";
				errorContent.text = errorNetFail;
				showError();
			}
		}
	}
	/*
	 * This part is code that combine different kind of show and hide, hide first, show second
	 */
	public void hideMain_showLogin()
	{
		hideMainmenuPanel ();
		showLoginPanel ();
	}
	public void hideMain_showSignup()
	{
		hideMainmenuPanel ();
		showSignupPanel ();
	}
	public void hideMain_showInstruction()
	{
		hideMainmenuPanel ();

	}
	public void hideLogin_showMain()
	{
		hideLoginPanel ();
		showMainmenuPanel ();
	}
	public void hideLogin_showProfile()
	{
		hideLoginPanel ();
		showProfileList ();
	}
	public void hideSignup_showMain()
	{
		hideSignupPanel ();
		showMainmenuPanel ();
	}
	public void hideSignup_showProfile()
	{
		hideSignupPanel ();
		showProfileList ();
	}
	public void hideSignup_show()
	{
		
	}
	public void hideProfile_showCreatProfile()
	{
		hideProfileList ();
		showCreateProfile ();
	}
	public void hideProfile_showMain()
	{
		hideProfileList ();
		showMainmenuPanel ();
	}
	public void hideCreateProfile_showProfile()
	{
		hideCreateProfile ();
		showProfileList ();
	}
	public void hideMain_showProfile()
	{
		hideMainmenuPanel ();
		showProfileList ();
	}
	/*
	 * This part is code that gonna send out request using lower level of senseix pluggin
	 */
	public void sendSignup()
	{

		string emailText = signupEmail.value;
		string name = signupName.value;
		string passwordText = signupPassword.value;
		if (senseix.parentSignUp (emailText, name, passwordText) == 0) 
		{
			print("senseix sign up successful");

			hideSignup_showMain();
		}
		else
		{
			print("senseix sign up failed");
			errorContent.text = errorSignup;
			showError();
		}
	}
	public void sendLogin()
	{
		senseix.cleanData ();
		string emailText = loginEmail.value;
		string passwordText = loginPassword.value;
		if(senseix.parentLogin(emailText,passwordText) == 0)
		{
			print("senseix sign in successful");
			hideLoginPanel();
			showProfileList();
			//showMainmenuPanel();
			players=senseixManager.getPlayers();
			drawProfileList();
		}
		else
		{
			errorContent.text = errorLogin;
			showError();
			print("senseix  sign in failed");
		}
	}
	public void sendCreateProfile()
	{
		if(senseix.createPlayer(createProfileName.value) == 0)
		{
			print("Success");
			//SUCCESS
		}
		else
		{
			errorContent.text = errorCreateProfile;
			showError();
			//ERROR
		}
	}
	private void profileSelect(int index)
	{
		if (senseixManager.selectProfile (index) == 0)
		{	
			senseixMenuManager.storeProblems(0);
			senseixGameManager.prepareProblem (5, "Mathematics", 1);
			currentQuestion = senseixManager.getProblem ();
			currentAnwser = Convert.ToInt32(senseixManager.getAnwser ());
			//senseix.pullLeaderboard(1);
			//leaderboard.debugPrint();
			//drawLeaderboardList();
			hideProfile_showMain();
		}
	}
	public void profileSelect0()
	{
		profileSelect (0);
	}
	public void profileSelect1()
	{
		profileSelect (1);
	}
	public void profileSelect2()
	{
		profileSelect (2);
	}
	public void profileSelect3()
	{
		profileSelect (3);
	}
	public void profileSelect4()
	{
		profileSelect (4);
	}
	public void profileSelect5()
	{
		profileSelect (5);
	}
	public void profileSelect6()
	{
		profileSelect (6);
	}
	public void profileSelect7()
	{
		profileSelect (7);
	}
	public void profileSelect8()
	{
		profileSelect (8);
	}
	public void ssxStartGame()
	{
		//when it is offline and offline problems are loaded, we should try to connect again before begin game
		if(!senseix.inSession && senseixMenuManager.offlineProblemLoadedInSenseix)
			senseixManager.initSenseixManager ("ee1eee434f66ea73785a15ba9aa50a894ba469a0e342eb0910557960f68d7c2b");
		onlineDebug.text = senseix.inSession?"online":"offline";
		if(senseixManager.id != 0 || (!senseix.inSession && senseixMenuManager.offlineProblemLoadedInSenseix))
		{
			print (senseixManager.id+ " " + senseix.inSession.ToString() + " " + senseixMenuManager.offlineProblemLoadedInSenseix.ToString());
			hideMainmenuPanel ();
			StartGame ();
		}
		else
		{
			hideMain_showProfile();
		}
		showMenu = false;
		//print ("StartGame");
	}
	//ssx
    void Awake()
    {
        // Set mobile display res.
        //Screen.SetResolution(480, 800, false); 
    }
    void Start()
    {
		//Load ();//Block the process and only display "Loading"
		//When it finishes, load buttons.
		InitGame("ee1eee434f66ea73785a15ba9aa50a894ba469a0e342eb0910557960f68d7c2b");
        //HideGame();
        //StartGame();
    }

    // Hide game interface for next quiz
    void HideGame()
    {
        //ClearQuiz();
    }
	// Start game & draw next quiz
    void StartGame()
    {
		playing = true;
		//startgamefunction
    }
    // Init Quiz Game
    public void InitGame(string gameToken)
    {
		inInit = true;
		initSsxPanelPos ();
		initSsxProfileLables ();
		print ("============================");
		if (senseixManager.initSenseixManager (gameToken) != 0 || !senseix.inSession) 
		{
			onlineDebug.text = senseix.inSession?"online":"offline";
			errorContent.text = errorNetFail;
			showError();
		}
		debugText.text = PlayerPrefs.GetString("problem00").Length.ToString() + " " +senseixMenuManager.cachedProblemLevelCeiling.ToString();
    }
	void winGame()
	{
		playing = false;
	}
	void lostGame()
	{
		playing = false;
	}
    // time delay action
    public static bool showingMenu()
    {
    	return showMenu;
    }
}
