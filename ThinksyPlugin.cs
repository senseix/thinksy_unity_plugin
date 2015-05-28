using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ThinksyPlugin : MonoBehaviour
{
	public string gameAccessToken = null; 	
								//this is your developer access token obtained from 
								//the Senseix website.
	public bool testingMode = false;	
								//check this box from the unity GUI to enable offline mode, 
								//useful for testing or offline development
	public bool useLeaderboard = false; 
								//check this box if you plan to use Thinksy leaderboard functionality
								//it will present a leaderboard button in the menu
	public GameObject emergencyWindow = null;	
								//this game object will be activated in the hopefully unlikely
								//scenario of problems in the thinksy plugin
	
	private static ThinksyPlugin singletonInstance;
	private static Problem mostRecentProblem;
	
	private const int reconnectRetryInterval = 3000;
	private static uint heartbeatInterval = 1401;

	static private ThinksyPlugin GetSingletonInstance()
	{
		if (singletonInstance == null)
		{
			throw new Exception("Please drag the Thinksy prefab located in " +
				"thinsy_unity_plugin/prefabs into your object heierarchy");
		}
		return singletonInstance;
	}

	void OnApplicationFocus(bool isFocused)
	{
		if (isFocused) StaticReinitialize ();
	}

	/// <summary>
	/// Shows a window indicating that something horrible has happened.
	/// Use this if something horrible happens.
	/// </summary>
	static public void ShowEmergencyWindow(string additionalMessage)
	{
		GetSingletonInstance().StartCoroutine(Senseix.SenseixSession.SubmitBugReport ("Emergency window being displayed: " + additionalMessage));
		GetSingletonInstance().ShowThisEmergencyWindow (additionalMessage);
	}

	static public bool IsInTestingMode()
	{
		return GetSingletonInstance().testingMode;
	}

	private void ShowThisEmergencyWindow(string additionalMessage)
	{
		emergencyWindow.SetActive (true);
		UnityEngine.UI.Text emergencyText = emergencyWindow.GetComponentInChildren<UnityEngine.UI.Text> ();
		emergencyText.text += " " + additionalMessage;
	}
	
	void Awake()
	{	
		if (singletonInstance != null)
		{
			Debug.LogWarning ("Something is creating a SenseixPlugin, but there is already an " +
			                  "instance in existance.  There should only be one SenseixPlugin component at any " +
			                  "time.  You can access its features \nthrough the class's static methods.   The object this message is coming" +
			                  " from is redundant.  I'm going to delete myself.");
			Destroy(gameObject);
		}

		singletonInstance = this;

		if (testingMode)
		{
			Senseix.ProblemKeeper.DeleteAllSeeds();
		}
		Senseix.ProblemKeeper.CopyFailsafeOver ();

		if (gameAccessToken == null || gameAccessToken == "")
			throw new Exception ("Please enter a game access token.");

		if (!testingMode)
		{
			StartCoroutine(Senseix.SenseixSession.InitializeSenseix (gameAccessToken));
		}
	}

	void Update()
	{
		if (!testingMode && !Senseix.SenseixSession.GetSessionState() && Time.frameCount%reconnectRetryInterval == 0)
		{
			Debug.Log ("Attempting to reconnect...");
			StartCoroutine(Senseix.SenseixSession.InitializeSenseix(gameAccessToken));
		}
		if (Senseix.SenseixSession.GetSessionState() && Time.frameCount%heartbeatInterval == 0 &&  Time.frameCount != 0)
		{
			Senseix.Logger.BasicLog("Getting encouragements...");
			Senseix.SenseixSession.Heartbeat();
		}
	}

	public static void NewHeartbeatTiming(uint newTiming)
	{
		if (newTiming < 100)
			return;
		heartbeatInterval = newTiming;
	}

	public static void StaticReinitialize()
	{
		GetSingletonInstance().Reinitialize ();
	}

	/// <summary>
	/// Resends all the server communication involved in initializing the game.
	/// Primarily a debugging tool.
	/// </summary>
	public void Reinitialize()
	{
		//Debug.Log ("Reinitializing");
		if (!testingMode)
		{
			StartCoroutine(Senseix.SenseixSession.InitializeSenseix (gameAccessToken));
		}
	}
	
	/// <summary>
	/// Updates the high score of the player on the SenseiX server.  This will then
	/// be reflected in the leaderboard in the SenseiX menus.  If this is not a high
	/// score, it will not override previous, higher scores.
	/// </summary>
	public static void UpdateCurrentPlayerScore (UInt32 score)
	{
		if (IsInTestingMode ())
			Debug.LogWarning ("We are currently in offline mode.");
		GetSingletonInstance().StartCoroutine(Senseix.SenseixSession.UpdateCurrentPlayerScore (score));
	}
	
	/// <summary>
	/// Returns the next Problem for the Player as an instance of the Problem class.  If there aren't 
	/// enough Problems left in the queue, an asynchronous task will retrieve more from the Senseix
	/// server.
	/// </summary>
	public static Problem NextProblem()
	{
		if (AllAnswerPartsGiven() && !GetMostRecentProblem().HasBeenSubmitted())
		{
			ThinksyPlugin.GetMostRecentProblem ().SubmitAnswer ();
		}
		Senseix.Message.Problem.ProblemData protobufsProblem = Senseix.SenseixSession.PullProblem ();
		Senseix.Logger.BasicLog ("Next problem!  Problem ID: " + protobufsProblem.uuid + " Category: " + protobufsProblem.category_name);
		//Debug.Log ("Next problem!  Problem ID: " + protobufsProblem.uuid + " Category: " + protobufsProblem.category_name);
		mostRecentProblem = new Problem (protobufsProblem);
		ThinksyQuestionDisplay.DisplayCurrentQuestion ();
		return mostRecentProblem;
	}

	public void NextProblemFromInstance()
	{
		NextProblem ();
	}
	
	/// <summary>
	/// Returns the most recent Problem returned by the NextProblem() function.
	/// </summary>
	public static Problem GetMostRecentProblem()
	{
		if (mostRecentProblem == null)
		{
			//throw new Exception("There are not yet any Problems.  Please use SenseixPlugin.NextProblem()");
			NextProblem();
		}
		return mostRecentProblem;
	}
	
	/// <summary>
	/// Returns whether or not the number of answer parts given to the most recent problem
	/// is equal to the correct number of answer parts.
	/// </summary>
	public static bool AllAnswerPartsGiven()
	{
		if (mostRecentProblem == null) return false;
		return GetMostRecentProblem().AnswersGivenSoFar() == GetMostRecentProblem().GetCorrectAnswer().AnswerPartsCount();
	}

	/// <summary>
	/// Counts the problems answered correctly so far.
	/// </summary>
	/// <returns>The problems answered correctly so far.</returns>
	public static uint CountProblemsAnsweredCorrectlySoFar()
	{
		return Problem.CountProblemsAnsweredCorrectlySoFar();
	}

	public static bool UsesLeaderboard()
	{
		return GetSingletonInstance().useLeaderboard;
	}
	
	public static void SetAccessToken(string newAccessToken)
	{
		GetSingletonInstance().gameAccessToken = newAccessToken;
	}
}