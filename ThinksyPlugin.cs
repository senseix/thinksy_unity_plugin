using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ThinksyPlugin : MonoBehaviour
{	
	public string gameAccessToken; //this is your developer access token obtained from 
	//the Senseix website.
	public bool offlineMode;	//check this box from the unity GUI to enable offline mode, 
								//useful for testing or offline development
	public bool useLeaderboard; //check this box if you plan to use Thinksy leaderboard functionality
								//it will present a leaderboard button in the menu
	public GameObject emergencyWindow;	//this game object will be activated in the hopefully unlikely
										//scenario of problems in the thinksy plugin
	
	private static ThinksyPlugin singletonInstance;
	private static Problem mostRecentProblem;
	
	private const int reconnectRetryInterval = 3000;
	private const int encouragementGetInterval = 801;

	/// <summary>
	/// Shows a window indicating that something horrible has happened.
	/// Use this if something horrible happens.
	/// </summary>
	static public void ShowEmergencyWindow(string additionalMessage)
	{
		Senseix.SenseixSession.SubmitBugReport ("Emergency window being displayed: " + additionalMessage);
		singletonInstance.ShowThisEmergencyWindow (additionalMessage);
	}

	static public bool IsInOfflineMode()
	{
		return singletonInstance.offlineMode;
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

		Senseix.ProblemKeeper.CopyFailsafeOver ();
		if (!offlineMode) Senseix.SenseixSession.InitializeSenseix (gameAccessToken);
		NextProblem ();
	}

	void Update()
	{
		if (!offlineMode && !Senseix.SenseixSession.GetSessionState() && Time.frameCount%reconnectRetryInterval == 0)
		{
			Debug.Log ("Attempting to reconnect...");
			Senseix.SenseixSession.InitializeSenseix(gameAccessToken);
		}
		if (Senseix.SenseixSession.GetSessionState() && Time.frameCount%encouragementGetInterval == 0 &&  Time.frameCount != 0)
		{
			Debug.Log("Getting encouragements...");
			Senseix.SenseixSession.GetEncouragements();
		}
		Senseix.Message.Request.CheckResults ();
	}
	
	/// <summary>
	/// Registers the device with the Senseix server, allows a temporary account to be created
	/// and the Player to begin playing without logging in. Once an account is registered
	/// or created the temporary account is transitioned into a permanent one.  
	/// </summary>
	public void ReregisterDevice()
	{
		Senseix.SenseixSession.RegisterDevice ();
	}
	
	/// <summary>
	/// Updates the high score of the player on the SenseiX server.  This will then
	/// be reflected in the leaderboard in the SenseiX menus.  If this is not a high
	/// score, it will not override previous, higher scores.
	/// </summary>
	public static void UpdateCurrentPlayerScore (UInt32 score)
	{
		if (IsInOfflineMode ())
			Debug.LogWarning ("We are currently in offline mode.");
		Senseix.SenseixSession.UpdateCurrentPlayerScore (score);
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
			SubmitMostRecentProblemAnswer();
		}
		Senseix.Message.Problem.ProblemData.Builder protobufsProblemBuilder = Senseix.SenseixSession.PullProblem ();
		//Debug.Log ("Next problem!  Problem ID: " + protobufsProblemBuilder.Uuid);
		mostRecentProblem = new Problem (protobufsProblemBuilder);
		Senseix.QuestionDisplay.Update ();
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
			throw new Exception("There are not yet any Problems.  Please use SenseixPlugin.NextProblem()");
		}
		return mostRecentProblem;
	}
	
	/// <summary>
	/// Returns given answer of the most recent Problem returned by the
	/// NextProblem() function.  Set given answers with Problem.AddGivenAnswerPart(),
	/// Problem.SetGivenAnswer(), or AddGivenAnswerPartToMostRecentProblem().
	/// </summary>
	public static Answer GetMostRecentGivenAnswer()
	{
		return GetMostRecentProblem ().GetGivenAnswer ();
	}

	/// <summary>
	/// Clears all the answers given so far, allowing the player to try again.
	/// </summary>
	public static void ClearMostRecentGivenAnswerParts()
	{
		GetMostRecentGivenAnswer().ClearAnswerParts();
	}
	
	/// <summary>
	/// Gets the question portion of the of the most recent Problem returned by the
	/// NextProblem() function.  The question will also be displayed the the SenseiX
	/// Question Panel (found under the SenseiX Display Canvas).
	/// </summary>
	public static Question GetMostRecentProblemQuestion()
	{
		return GetMostRecentProblem ().GetQuestion();
	}
	
	/// <summary>
	/// Gets the distractors for the problem most recently returned by NextProblem().
	/// These are wrong answers which can be presented as options to the player.
	/// </summary>
	/// <returns>The most recent problem distractors.</returns>
	/// <param name="howManyDistractors">How many distractors.</param>
	public static ProblemPart[] GetMostRecentProblemDistractors(int howManyDistractors)
	{
		return GetMostRecentProblem ().GetDistractors (howManyDistractors);
	}

	/// <summary>
	/// The same an GetMostRecentProblemDistractors, but gets only one distractor.
	/// Distractors are wrong answers which can be presented as options to the player.
	/// </summary>
	/// <returns>The most recent problem distractor.</returns>
	public static ProblemPart GetMostRecentProblemDistractor()
	{
		return GetMostRecentProblem ().GetDistractor ();
	}
	
	/// <summary>
	/// Adds the given answer part to most recent problem.  This will be considered 
	/// part of the player's answer for submissions and checking unless it is removed.
	/// </summary>
	/// <param name="givenAnswerPart">Given answer part.</param>
	public static void AddGivenAnswerPartToMostRecentProblem(ProblemPart givenAnswerPart)
	{
		GetMostRecentProblem ().AddGivenAnswerPart (givenAnswerPart);
	}
	
	/// <summary>
	/// Checks the Problem's given answer against its correct answer.
	/// This WILL NOT report anything to the SenseiX server, and this therefore DOES NOT
	/// influence your player's progress.  When you have a final answer, submit it with
	/// Problem.SubmitAnswer() or SenseixPlugin.SubmitMostRecentProblemAnswer()
	/// Given and correct answer can be found in the Problem class.
	/// </summary>
	public static bool CheckAnswer(Problem problem)
	{
		return problem.CheckAnswer ();
	}


	/// <summary>
	/// Gets the correct answer parts for the most recent problem generated by
	/// NextProblem().
	/// </summary>
	/// <returns>The most recent answer parts.</returns>
	public static ProblemPart[] GetMostRecentCorrectAnswerParts()
	{
		return GetMostRecentProblem ().GetCorrectAnswer ().GetAnswerParts ();
	}
	
	/// <summary>
	/// Based on how many answers have been given so far, gets the next correct answer part.
	/// The same as GetMostRecentProblem ().GetNextCorrectAnswerPart ();
	/// </summary>
	/// <returns>The next correct answer part.</returns>
	public static ProblemPart GetCurrentCorrectAnswerPart()
	{
		return GetMostRecentProblem ().GetCurrentCorrectAnswerPart ();
	}
	
	/// <summary>
	/// Checks the most recent problem's given answer.
	/// The same as GetMostRecentProblem ().CheckAnswer ().
	/// </summary>
	/// <returns>Whether or not the problem's given answer is correct</returns>
	public static bool CheckMostRecentProblemAnswer()
	{
		return GetMostRecentProblem ().CheckAnswer ();
	}
	
	/// <summary>
	/// Submits the most recent problem's given answer to the SenseiX server.
	/// This will update your player's progress and metrics.
	/// This is important!  If you don't submit answers, your players will
	/// receive the same problems over and over again, and become bored.
	/// </summary>
	/// <returns>Whether or not the problem's given answer is correct</returns>
	public static bool SubmitMostRecentProblemAnswer()
	{
		return GetMostRecentProblem ().SubmitAnswer ();
	}
	
	/// <summary>
	/// Returns whether or not the number of answer parts given to the most recent problem
	/// is equal to the correct number of answer parts.
	/// </summary>
	public static bool AllAnswerPartsGiven()
	{
		if (mostRecentProblem == null) return false;
		return GetMostRecentProblem().AnswersGivenSoFar() == GetCurrentCorrectAnswer().AnswerPartsCount();
	}
	
	/// <summary>
	/// Gets the correct answer to the most recent problem.
	/// The same as GetMostRecentProblem ().GetCorrectAnswer ().
	/// </summary>
	/// <returns>The current correct answer.</returns>
	public static Answer GetCurrentCorrectAnswer()
	{
		return GetMostRecentProblem ().GetCorrectAnswer ();
	}
	
	/// <summary>
	/// Gets the most recent problem HTML.
	/// The same as GetMostRecentProblem ().GetQuestion ().GetHTML ().
	/// </summary>
	/// <returns>The most recent problem HTML.</returns>
	public static string GetMostRecentProblemHTML()
	{
		return GetMostRecentProblem ().GetQuestion ().GetHTML ();
	}
	
	/// <summary>
	/// Gets the most recent problem's question image.
	/// The same as GetMostRecentProblem ().GetQuestion ().GetImage ().
	/// </summary>
	/// <returns>The most recent problem image.</returns>
	public static Texture2D GetMostRecentProblemImage()
	{
		return GetMostRecentProblem ().GetQuestion ().GetImage ();
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
		return singletonInstance.useLeaderboard;
	}
}
