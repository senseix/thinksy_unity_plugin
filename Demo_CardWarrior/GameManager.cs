using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;
using System;
using System.Threading;

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
public class GameManager : MonoBehaviour
{
    // Effect Prefab
    public GameObject goodEffect, badEffect, soulEffect, happyEffect;
    // Actor Animator Component
	public Animator friendAnimator, enemyAnimator, mmfriendAnimator,mmenemyAnimator;
    // Actor HP Manager Component
    public HpManager friendHpMan, enemyHpMan;
	public const string errorLogin = "Login Failed";
	public const string errorSignup = "Signup Failed";
	public const string errorCreateProfile = "Create profile failed";
	public const string errorNetFail = "Network Failed";
	private string errorTitle = "";
	public List<UILabel> profileLables;
	public int[] hpDamage;
	public int currentLevel = 0;
	public UILabel loadingLable;
	//public int idid;
	public UIPanel buttonPanel;
	public UIPanel signupPanel;
	public UIPanel loginPanel;
	public UIPanel profileSelectPanel;
	public UIPanel createProfilePanel;
	public UIPanel winPanel;
	public UIPanel errorPanel;
	public UIPanel enemyTurnPanel;
	public SkinnedMeshRenderer skeleton;
	public UIInput signupEmail;
	public UIInput signupPassword;
	public UIInput signupName;
	public UILabel roundLable;
	public UILabel errorContent;
	public UILabel enemyChose;
	public UIInput loginEmail;
	public UIInput loginPassword;
	public UIInput createProfileName;
	private bool inError = false;
	public static ArrayList players = null;
	private bool enemyTurn = false;
	private bool switchPlay = false;
	private int wait = 0;
    // Save Start Position
	Vector3 friendPos, enemyPos, friendHpPos, enemyHpPos, shieldPos,mmfriendPos,mmenemyPos,mainmenuPanelPos,loginPanelPos,signupPanelPos,profileSelectPanelPos,roundLablePos,createProfilePos,errorPos,winPanelPos,enemyTurnPanelPos;
    Transform friendHpGroup, enemyHpGroup, shieldGroup;

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

	private int[] cardOffseth;
	private int[] cardOffsetv;
    // Quiz List Array
    List<QuizData> quizList;
    int quizTotal;
    int quizIndex = 0;

    [HideInInspector]
    public int quizLength = 0;

    // Quiz Condition
    bool quizOn = true;
	bool playing = false;
	bool InGameMenu = false;
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
		roundLablePos = roundLable.transform.localPosition;
		createProfilePos = createProfilePanel.transform.localPosition;
		errorPos = errorPanel.transform.localPosition;
		winPanelPos = winPanel.transform.localPosition;
		enemyTurnPanelPos = enemyTurnPanel.transform.localPosition;
	}
	void initSsxProfileLables()
	{
		for(int i=0;i<9;i++)
		{
			profileLables[i].text="Empty";
		}
	}
	void Load()
	{
		loadingLable.text = "Loading......";
		loadingLable.text = "";
		print ("loading");
	}
	public void showSelectProfileMenu()
	{
	
	}
	public void hideSelectProfileMenu()
	{
		
	}
	/*
	 * This is code that show or hide single menu
	 */
	public void showEnemyTurnPanel()
	{
		Vector3 pos = new Vector3(enemyTurnPanelPos.x,enemyTurnPanelPos.y+Screen.height,enemyTurnPanelPos.z);
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
	public void showWinPanel()
	{
		Vector3 pos = new Vector3(winPanelPos.x,winPanelPos.y+Screen.height,winPanelPos.z);
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
		//hideMainmenuPanel ();
		Vector3 pos = new Vector3(signupPanelPos.x,signupPanelPos.y+Screen.height,signupPanelPos.z);
		//buttonPanel.transform.localPosition = new Vector3(pos.x, pos.y+3f, pos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(signupPanel.transform, 1f, parms);
	}
	public void hideSignupPanel()
	{
		Vector3 pos = new Vector3(signupPanelPos.x,signupPanelPos.y,signupPanelPos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(signupPanel.transform, 1f, parms);
		//showMainmenuPanel ();
	}
	public void showCreateProfile()
	{
		//hideProfileList ();
		//hideMainmenuPanel ();
		Vector3 pos = new Vector3(createProfilePos.x,createProfilePos.y+Screen.height,createProfilePos.z);
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
	public void showRound()
	{
		roundLable.text = "Round " + currentLevel.ToString();
		Vector3 pos = new Vector3(roundLablePos.x,roundLablePos.y+Screen.height,roundLablePos.z);
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
	public void showLoginPanel()
	{
		senseix.cleanData ();
		//hideMainmenuPanel ();
		Vector3 pos = new Vector3(loginPanelPos.x,loginPanelPos.y+Screen.height,loginPanelPos.z);
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
	public void showProfile()
	{
		
	}
	public void showProfileList()
	{
		//hideLoginPanel ();
		players = senseixManager.getCachedPlayers ();
		if(players != null)
			drawProfileList ();
		Vector3 pos = new Vector3(profileSelectPanelPos.x,profileSelectPanelPos.y+Screen.height,profileSelectPanelPos.z);
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
		Vector3 pos = new Vector3(mainmenuPanelPos.x,mainmenuPanelPos.y+Screen.height,mainmenuPanelPos.z);
		//buttonPanel.transform.localPosition = new Vector3(pos.x, pos.y+3f, pos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(buttonPanel.transform, 1f, parms);
	}
	public void showError()
	{
		inError = true;
		Vector3 pos = new Vector3(errorPos.x,errorPos.y+Screen.height,errorPos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(errorPanel.transform, 1f, parms);
	}
	public void hideError()
	{
		inError = false;
		Vector3 pos = new Vector3(errorPos.x,errorPos.y,errorPos.z);
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(errorPanel.transform, 1f, parms);
		if(!senseixManager.network())
		{
			if (senseixManager.initSenseixManager ("5dc215f2d2906b0dd81f82a0a959d80aa3aba0b665c292a5da7ff6431b9ee484") != 0 || !senseix.inSession) 
			{
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
		showProfile ();
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
		if (senseix.coachSignUp (emailText, name, passwordText) == 0) 
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
		if(senseix.coachLogin(emailText,passwordText) == 0)
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
			QuizInit();
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
	public void showInGameMenu()
	{
		
	}
	public void hideInGameMenu()
	{
		
	}

	public void ssxStartGame()
	{
		if(senseixManager.id != 0)
		{
			hideMainmenuPanel ();
			StartGame ();
		}
		else
		{
			hideMain_showProfile();
		}
		//print ("StartGame");
	}
	//ssx
    void Awake()
    {
        // Set mobile display res.
        Screen.SetResolution(480, 800, false); 
    }
    void Start()
    {
		mmfriendAnimator.CrossFade("Walk", 0.2f);
		mmenemyAnimator.CrossFade("Walk", 0.2f);
		Load ();//Block the process and only display "Loading"
		//When it finishes, load buttons.
		InitGame();
        HideGame();
        //StartGame();
    }

    // Hide game interface for next quiz
    void HideGame()
    {
        ClearQuiz();
        Vector3 pos = friendPos;
        friendAnimator.transform.localPosition = new Vector3(pos.x * 3f, pos.y, pos.z);
        pos = enemyPos;
        enemyAnimator.transform.localPosition = new Vector3(pos.x * 3f, pos.y, pos.z);
        pos = friendHpPos;
        friendHpGroup.localPosition = new Vector3(pos.x * 3f, pos.y, pos.z);
        pos = enemyHpPos;
        enemyHpGroup.localPosition = new Vector3(pos.x * 3f, pos.y, pos.z);
        shieldGroup.localScale = new Vector3(2f, 2f, 1f);
        pos = shieldPos;
        shieldGroup.localPosition = new Vector3(pos.x, 0f, pos.z);
    }
	void showAvatar()
	{
		Vector3 pos = mmfriendPos;
		mmfriendAnimator.transform.localPosition = new Vector3(pos.x,pos.y, pos.z);
		pos = mmenemyPos;
		mmenemyAnimator.transform.localPosition = new Vector3(pos.x, pos.y, pos.z);
	}
	// Start game & draw next quiz
    void StartGame()
    {
        IntroGame();
		showRound ();
		playing = true;
		enemyTurn = false;
        DrawQuiz();
    }

    // Draw Quiz
    void DrawQuiz()
    {
        HideQuiz();
        StartCoroutine(DelayActoin(1f, () =>
        {
            SetQuiz();
            ShowQuiz();
        }));
    }

    // Init Quiz List
    void QuizInit()
    {
        quizList = new List<QuizData>();
        List<string> champs = new List<string>();
        string[,] dic = LolSkillData.dic;
        for (int i = 0; i < dic.GetLength(0); i++)
        {
            string champ = dic[i,2];
            if (!champs.Contains(champ)) champs.Add(champ);
        }
        for (int i = 0; i < dic.GetLength(0); i++)
        {
            string idx = dic[i, 0];
            string skill = dic[i, 1];
            string champ = dic[i, 2];
            QuizData quiz = new QuizData();

            int t = champs.IndexOf(champ);
            List<int> ansIdList = new List<int>();
            Hashtable ansValList = new Hashtable();
            ansValList[0] = champs[t];
            ansValList[1] = champs[(t+1)%champs.Count];
            ansValList[2] = champs[(t+2)%champs.Count];
            ansValList[3] = champs[(t+3)%champs.Count];
            for (int j = 0; j < 4; j++) ansIdList.Add(j);
            ansIdList.Shuffle();
            for (int j = 0; j < 4; j++)
                if (ansIdList[j] == 0) quiz.correct = j;
            quiz.answer1 = "1. " + ansValList[ansIdList[0]] as string;
            quiz.answer2 = "2. " + ansValList[ansIdList[1]] as string;
            quiz.answer3 = "3. " + ansValList[ansIdList[2]] as string;
            quiz.answer4 = "4. " + ansValList[ansIdList[3]] as string;
            quiz.question = skill + "?";
            quiz.id = int.Parse(idx);
            quizList.Add(quiz);
        }
        quizTotal = quizList.Count;
		//print ("quizTotal " + quizTotal);
    }

    // Init Quiz Game
    void InitGame()
    {
		inInit = true;
		friendHpMan.InitHp();
        enemyHpMan.InitHp();
        questionTf = GameObject.Find("Question").transform;
        questionLabel = questionTf.GetComponentInChildren<UILabel>();
        answerLabels = new UILabel[4];
        answerTfs = new Transform[4];
		initSsxPanelPos ();
		initSsxProfileLables ();
        int i = 0;
		origParms = new TweenParms[4];
		cardOffseth = new int[4];
		cardOffsetv = new int[4];
		cardOffseth [0] = -120;
		cardOffseth [1] = 120;
		cardOffseth [2] = -120;
		cardOffseth [3] = 120;

		cardOffsetv [0] = 50;
		cardOffsetv [1] = 113;
		cardOffsetv [2] = -43;
		cardOffsetv [3] = 10;
		hpDamage = new int[100];
		for (int j=0; j<100; j++) 
		{
			hpDamage[j]=20+j*10;
			if(hpDamage[j]>=100)
				hpDamage[j]=100;
		}
		numberOnCard = new int[4];
		/*
		senseix.initSenseix ("5dc215f2d2906b0dd81f82a0a959d80aa3aba0b665c292a5da7ff6431b9ee484");
		players = senseix.getCachedPlayerA ();
		if (senseix.inSession) 
		{
			senseixGameManager.prepareProblem (3, "Mathematics", 1);
			//getProblem();
			InGameMenu = true;
		}
		else
			senseix.coachUidPush();
		*/
		if (senseixManager.initSenseixManager ("5dc215f2d2906b0dd81f82a0a959d80aa3aba0b665c292a5da7ff6431b9ee484") != 0 || !senseix.inSession) 
		{
			errorContent.text = errorNetFail;
			showError();
		}

		foreach (Transform tf in GameObject.Find("Answers").transform)
        {
            answerTfs[i] = tf;
			origParms[i] = new TweenParms().Prop("localPosition", new Vector3(tf.localPosition.x,tf.localPosition.y, tf.localPosition.z));
            answerLabels[i] = tf.GetComponentInChildren<UILabel>();
            i++;
        }
		//These code should be after we got id of player
		/*
		currentQuestion = senseixManager.getProblem ();
		currentAnwser = Convert.ToInt32(senseixManager.getAnwser ());
        QuizInit();
        */
		//above should be after we got id of player
        shieldGroup = GameObject.Find("ShieldGroup").transform;
        shieldPos = shieldGroup.localPosition;
        friendPos = friendAnimator.transform.localPosition;
        enemyPos = enemyAnimator.transform.localPosition;
        friendHpGroup = friendHpMan.hpBar.transform.parent;
        enemyHpGroup = enemyHpMan.hpBar.transform.parent;
        friendHpPos = friendHpGroup.localPosition;
        enemyHpPos = enemyHpGroup.localPosition;
		mmenemyPos = mmenemyAnimator.transform.localPosition;
		mmfriendPos = mmfriendAnimator.transform.localPosition;
    }

    // Draw quiz intro motion
    void IntroGame()
    {
        friendAnimator.CrossFade("Walk", 0.2f);
        enemyAnimator.CrossFade("Walk", 0.2f);

		friendHpMan.InitHp();
		enemyHpMan.InitHp();
		playing = true;
		Vector3 pos = new Vector3(mmfriendPos.x-400,mmfriendPos.y+45,mmfriendPos.z-700);
		mmfriendAnimator.transform.localPosition = new Vector3 ();
		TweenParms parms = new TweenParms ().Prop ("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(mmfriendAnimator.transform, 2f, parms);

		pos = new Vector3 (mmenemyPos.x+400,mmenemyPos.y+45,mmenemyPos.z-700);
		mmenemyAnimator.transform.localPosition = new Vector3 ();
		parms = new TweenParms().Prop("localPosition", pos);//.Ease(EaseType.Linear).OnComplete(OnFriendStop);
		HOTween.To(mmenemyAnimator.transform, 2f, parms);

        pos = friendPos;
        friendAnimator.transform.localPosition = new Vector3(pos.x * 3f, pos.y, pos.z);
        parms = new TweenParms().Prop("localPosition", friendPos).Ease(EaseType.Linear).OnComplete(OnFriendStop);
        HOTween.To(friendAnimator.transform, 2f, parms);
        
        pos = enemyPos;
        enemyAnimator.transform.localPosition = new Vector3(pos.x * 3f, pos.y, pos.z);
        parms = new TweenParms().Prop("localPosition", pos).Ease(EaseType.Linear).OnComplete(OnEnemyStop);
        HOTween.To(enemyAnimator.transform, 2f, parms);

        pos = shieldPos;
        shieldGroup.localPosition = new Vector3(pos.x, 0f, pos.z);
        parms = new TweenParms().Prop("localPosition", pos).Delay(1f);
        HOTween.To(shieldGroup, 1f, parms);

        shieldGroup.localScale = new Vector3(2f, 2f, 1f);
        parms = new TweenParms().Prop("localScale", new Vector3(0.8f, 0.8f, 1f));
        HOTween.To(shieldGroup, 1f, parms);

        pos = friendHpPos;
        friendHpGroup.localPosition = new Vector3(pos.x * 3f, pos.y, pos.z);
        parms = new TweenParms().Prop("localPosition", pos).Delay(0.5f);
        HOTween.To(friendHpGroup, 1f, parms);

        pos = enemyHpPos;
        enemyHpGroup.localPosition = new Vector3(pos.x * 3f, pos.y, pos.z);
        parms = new TweenParms().Prop("localPosition", pos).Delay(0.5f);
        HOTween.To(enemyHpGroup, 1f, parms);
		/*
		if (inInit) 
		{
			int i = 0;
			foreach (Transform tf in GameObject.Find("Answers").transform) 
			{
				origParms [i] = new TweenParms ().Prop ("localPosition", new Vector3 (tf.localPosition.x, tf.localPosition.y, tf.localPosition.z));
				i++;
			}
			inInit = false;
		}
		*/
	}
	
	// Stop Friend Actor Animation.
    void OnFriendStop()
    {
        friendAnimator.CrossFade("Idle", 0.2f);
    }

    // Stop Enemy Actor Animation.
    void OnEnemyStop()
    {
        enemyAnimator.CrossFade("Idle", 0.2f);
    }

    // Clear Quiz Display
    void ClearQuiz()
    {
        questionTf.localScale = new Vector3(0f, 1f, 1f);
        int i = -1;
        foreach (Transform tf in answerTfs)
        {
            tf.localPosition = new Vector3(600f * i, tf.localPosition.y, tf.localPosition.z);
            i *= -1;
        }
    }

    // Hide quiz motion 
    void HideQuiz()
    {
        TweenParms parms = new TweenParms().Prop("localScale", new Vector3(0f, 1f, 1f));
        HOTween.To(questionTf, 0.5f, parms);
		Transform tf;
/*
		int i = -1;
        foreach (Transform tf in answerTfs)
        {
            parms = new TweenParms().Prop("localPosition", new Vector3(600f * i, tf.localPosition.y, tf.localPosition.z));
            HOTween.To(tf, 0.5f, parms);
            i *= -1;
        }
*/
		for (int i=0; i<4; i++) 
		{
			tf = answerTfs[i];
			HOTween.To(tf, 0.5f, origParms[i]);
		}
    }

    // Display Quiz Question like typewriter
    void TypeQuiz()
    {
        questionLabel.text = currentQuestion.Substring(0, quizLength);
    }

    // Show Quiz Display Motion
    void ShowQuiz()
    {
		if(playing)
		{
			TweenParms parms = new TweenParms().Prop("localScale", new Vector3(1f, 1f, 1f));
	        HOTween.To(questionTf, 0.5f, parms);
	        int i = 1;
	        foreach (Transform tf in answerTfs)
	        {
				parms = new TweenParms().Prop("localPosition", new Vector3(cardOffseth[i-1],tf.localPosition.y+cardOffsetv[i-1], tf.localPosition.z)).Delay(0.3f * i++);
	            HOTween.To(tf, 1.5f, parms);
	        }
	        quizOn = true;
	        quizLength = 0;
	        parms = new TweenParms().Prop("quizLength", currentQuestion.Length).Ease(EaseType.Linear).OnUpdate(TypeQuiz);
	        HOTween.To(this, 1f, parms);
		}
    }

    // Make String Max Length
    string QuizMakeString(string str) 
    {
        return (str.Length > 41) ? str.Substring(0, 40) : str;
    }

    // set Quiz Answer & Question variables
    void SetQuiz()
    {
		bool hasAns = false;
		quizIndex = UnityEngine.Random.Range(0, quizTotal) % quizTotal;
        QuizData item = quizList[quizIndex];
		currentQuestion = senseixManager.getProblem ();
		currentAnwser = Convert.ToInt32(senseixManager.getAnwser ());
		//print ("SetQuiz current answer " + currentAnwser);
		for(int i=0;i<4;i++)
		{
			int got = UnityEngine.Random.Range(0,9)%10;
			//print ("SetQuiz go random " + got);
			if(got==currentAnwser)
			{
				hasAns=true;
			}
			answerLabels[i].text = (got).ToString();
			numberOnCard[i] = got;
		}
		if (!hasAns) 
		{
			int randIndex = UnityEngine.Random.Range (0, 3) % 4;
			numberOnCard[randIndex] = currentAnwser;
			answerLabels [randIndex].text = currentAnwser.ToString ();
		}
        questionLabel.text = currentQuestion;
    }
    
	void Update () {
        // Quit Application
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        if (Input.GetButton("Fire1"))
        {
            //DrawQuiz();
        }
		if(switchPlay)
		{
			if(wait < 150)
			{
				wait++;
			}
			else
			{
				switchPlay = false;
				hideEnemyTurnPanel();
			}
		}
		if(enemyTurn)
		{
			if(wait < 500)
			{
				wait++;
			}
			else
			{
				wait=0;
				int result = UnityEngine.Random.Range(0,3);
				ClickAnswer(result);
				enemyChose.text = numberOnCard[result].ToString();
			}
		}
	}

    void ClickAnswer(int no)
    {
        if (!quizOn) return;
        quizOn = false;
        QuizData item = quizList[quizIndex];
//		print (numberOnCard[no] + " " + currentAnwser);
        // Is answer collect?
        //send answer to server, only when it is realy player
		senseixManager.gotAnwser (numberOnCard[no].ToString(),!enemyTurn);
		if(!enemyTurn)
		{
	        if (numberOnCard[no] == currentAnwser) 
	        {
	            // Display good Effect
	            Instantiate(goodEffect);
	            // Display soul trail effect
	            GameObject go = Instantiate(soulEffect) as GameObject;
	            go.GetComponent<SoulEffect>().posX = -1f;
	            // Display Happy Effect
	            StartCoroutine(DelayActoin(0.6f, () =>
	            {
	                go = Instantiate(happyEffect, new Vector3(-0.7f, 1f, 0f), Quaternion.identity) as GameObject;
	                enemyHpMan.DoDamageHp(10);
	            }));
	            // Display Actor's motion
	            friendAnimator.CrossFade("Good", 0.2f);
	            enemyAnimator.CrossFade("Bad", 0.2f);
				if(enemyHpMan.getHP()<=0)
					winGame();
	        }
	        else
	        {
	            // Display Bad Effect
	            Instantiate(badEffect);
	            // Display soul trail effect
	            GameObject go = Instantiate(soulEffect) as GameObject;
	            go.GetComponent<SoulEffect>().posX = 1f;
	            // Display Happy Effect
	            StartCoroutine(DelayActoin(0.6f, () =>
	            {
	                go = Instantiate(happyEffect, new Vector3(0.7f, 1f, 0f), Quaternion.identity) as GameObject;
	                friendHpMan.DoDamageHp(hpDamage[currentLevel]);
	            }));
	            // Display Actor's motion
	            friendAnimator.CrossFade("Bad", 0.2f);
	            enemyAnimator.CrossFade("Good", 0.2f);
				if(friendHpMan.getHP()<=0)
					lostGame();
				else
				{
					enemyTurn = true;
					wait=0;
					enemyChose.text = "?";
					showEnemyTurnPanel();
				}
	        }
        }
        else
        {
			//when this is AI who is playing
			wait = 0;
			if (numberOnCard[no] != currentAnwser) 
			{
				// Display good Effect
				Instantiate(goodEffect);
				// Display soul trail effect
				GameObject go = Instantiate(soulEffect) as GameObject;
				go.GetComponent<SoulEffect>().posX = -1f;
				// Display Happy Effect
				StartCoroutine(DelayActoin(0.6f, () =>
				                           {
					go = Instantiate(happyEffect, new Vector3(-0.7f, 1f, 0f), Quaternion.identity) as GameObject;
					enemyHpMan.DoDamageHp(10);
				}));
				// Display Actor's motion
				friendAnimator.CrossFade("Good", 0.2f);
				enemyAnimator.CrossFade("Bad", 0.2f);
				enemyTurn = false;
				wait = 0;
				switchPlay = true;
				if(enemyHpMan.getHP()<=0)
					winGame();
			}
			else
			{
				// Display Bad Effect
				Instantiate(badEffect);
				// Display soul trail effect
				GameObject go = Instantiate(soulEffect) as GameObject;
				go.GetComponent<SoulEffect>().posX = 1f;
				// Display Happy Effect
				StartCoroutine(DelayActoin(0.6f, () =>
				                           {
					go = Instantiate(happyEffect, new Vector3(0.7f, 1f, 0f), Quaternion.identity) as GameObject;
					friendHpMan.DoDamageHp(hpDamage[currentLevel]);
				}));
				// Display Actor's motion
				friendAnimator.CrossFade("Bad", 0.2f);
				enemyAnimator.CrossFade("Good", 0.2f);
				if(friendHpMan.getHP()<=0)
					lostGame();
			}
		}


        StartCoroutine(DelayActoin(3f, () =>
        {
            DrawQuiz();
        }));
    }
	void winGame()
	{
		quizOn = false;
		playing = false;
		currentLevel++;
		if (currentLevel >= 100)
			currentLevel = 99;
		HideGame ();
		showWinPanel();
		//showMainmenuPanel ();
		//showAvatar ();
		//StartGame ();
		//ssxStartGame ();
	}
	void lostGame()
	{
		quizOn = false;
		playing = false;
		HideGame ();
		showMainmenuPanel ();
		showAvatar ();
	}
    public void OnClickAnswer1()
    {
        ClickAnswer(0);
    }
    public void OnClickAnswer2()
    {
        ClickAnswer(1);
    }
    public void OnClickAnswer3()
    {
        ClickAnswer(2);
    }
    public void OnClickAnswer4()
    {
        ClickAnswer(3);
    }
	
    // time delay action
    public IEnumerator DelayActoin(float dtime, System.Action callback)
    {
        yield return new WaitForSeconds(dtime);
        callback();
    }
}
