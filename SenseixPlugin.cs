using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Threading;
using System.ComponentModel;

class SenseixPlugin : MonoBehaviour
{

	public string developerAccessToken; //this is your developer access token obtained from the SenseiX website.

	private static SenseixPlugin singletonInstance;
	private static Problem mostRecentProblem;


	void Start()
	{	
		Debug.Log ("HENRY. INITIALIZING.");
		if (singletonInstance != null)
		{
			throw new Exception("Something is creating a SenseixPlugin, but there is already an " +
				"instance in existance.  There should only be one SenseixPlugin component at any " +
				"time.  You can access its features through the class's static methods.");
		}
		singletonInstance = this;
		Senseix.SenseixController.InitializeSenseix (developerAccessToken);
	}

	~SenseixPlugin()
	{
		singletonInstance = null;
		Senseix.SenseixController.EndLife ();
	}

	/// <summary>
	/// Registers the device with the SenseiX server, allows a temporary account to be created
	/// and the player to begin playing without logging in. Once an account is registered
	/// or created the temporary account is transitioned into a permanent one.  
	/// </summary>
	public void ReregisterDevice()
	{
		Senseix.SenseixController.RegisterDevice ();
	}

	/// <summary>
	/// Returns the next problem for the player as an instance of the Problem class.  If there aren't 
	/// enough problems left in the queue, and asynchronous task will retrieve more from the SenseiX
	/// server.
	/// </summary>
	public static Problem NextProblem()
	{
		Senseix.Message.Problem.ProblemData.Builder protobufsProblemBuilder = Senseix.SenseixController.PullProblem ();
		mostRecentProblem = new Problem (protobufsProblemBuilder);
		return mostRecentProblem;
	}

	/// <summary>
	/// Returns the most recent problem returned by the NextProblem() function.
	/// </summary>
	public static Problem GetMostRecentProblem()
	{
		if (mostRecentProblem == null)
		{
			throw new Exception("There are not yet any problems.  Please use SenseixPlugin.NextProblem()");
		}
		return mostRecentProblem;
	}

	/// <summary>
	/// Checks the problem's given answer against its correct answer.  Also reports the player's answer
	/// (correct or incorrect) to the SenseiX server.  Given and correct answer can be found in the Problem class.
	/// </summary>
	public static bool CheckAnswer(Problem problem)
	{
		return problem.CheckAnswer ();
	}

	/// <summary>
	/// Sets the problem's given answer to the string answer, and checks the problem's given answer against its correct answer.  
	/// Also reports the player's answer (correct or incorrect) to the SenseiX server.  Given and correct answer 
	/// can be found in the Problem class.
	/// </summary>
	public static bool CheckAnswer(Problem problem, string answer)
	{
		return problem.CheckAnswer (answer);
	}

	/// <summary>
	/// Whichever player is the currently active player in the SenseiX menus, this
	/// will set that player's high score to the UInt32 argument.
	/// </summary>
	public static void SetCurrentPlayerHighScore (UInt32 score)
	{
		Senseix.SenseixController.UpdateCurrentPlayerScore(score);
	}

}

public class Problem {

	private Senseix.Message.Problem.ProblemData.Builder protobufsProblemBuilder;
	private string givenAnswer;

	public Problem(Senseix.Message.Problem.ProblemData.Builder newProtobufsProblemBuilder)
	{
		protobufsProblemBuilder = newProtobufsProblemBuilder;
	}

	/// <summary>
	/// Returns the correct answer to this problem
	/// </summary>
	public string GetCorrectAnswer()
	{
		return protobufsProblemBuilder.Answer;
	}

	/// <summary>
	/// Returns the answer set by SetGivenAnswer
	/// </summary>
	public string GetGivenAnswer()
	{
		if (givenAnswer == null)
		{
			throw new Exception("No answers have yet been given.  Use Problem.SetGivenAnswer(someString)");
		}
		return givenAnswer;
	}

	/// <summary>
	/// Returns the question to be answered.
	/// </summary>
	public string GetQuestion()
	{
		return protobufsProblemBuilder.Question;
	}

	/// <summary>
	/// Sets the given answer.  This can then be checked for correctness through CheckAnswer.
	/// </summary>
	public void SetGivenAnswer(string newGivenAnswer)
	{
		givenAnswer = newGivenAnswer;
	}

	/// <summary>
	/// Checks the problem's given answer against its correct answer.  Also reports the player's answer
	/// (correct or incorrect) to the SenseiX server.
	/// </summary>
	public bool CheckAnswer()
	{
		return Senseix.SenseixController.CheckAnswer (protobufsProblemBuilder, GetGivenAnswer());
	}

	/// <summary>
	/// Sets the problem's given answer to the string answer, and checks the problem's given answer against its correct answer.  
	/// Also reports the player's answer (correct or incorrect) to the SenseiX server.
	/// </summary>
	public bool CheckAnswer(string answer)
	{
		SetGivenAnswer (answer);
		return CheckAnswer();
	}

}
