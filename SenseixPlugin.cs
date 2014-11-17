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
	public GameObject emergencyWindow;

	private static SenseixPlugin singletonInstance;
	private static Problem mostRecentProblem;

	private const int reconnectRetryInterval = 6000;

	static public void ShowEmergencyWindow()
	{
		singletonInstance.ShowThisEmergencyWindow ();
	}

	private void ShowThisEmergencyWindow()
	{
		emergencyWindow.SetActive (true);
	}

	void Awake()
	{	
		Debug.Log ("HENRY. INITIALIZING.");
		if (singletonInstance != null)
		{
			throw new Exception("Something is creating a SenseixPlugin, but there is already an " +
				"instance in existance.  There should only be one SenseixPlugin component at any " +
				"time.  You can access its features through the class's static methods.");
		}
		singletonInstance = this;
		senseix.ProblemKeeper.CopyFailsafeOver ();
		senseix.SenseixController.InitializeSenseix (developerAccessToken);
	}

	void Update()
	{
		if (!senseix.SenseixController.GetSessionState() && Time.frameCount%reconnectRetryInterval == 0)
		{
			senseix.SenseixController.InitializeSenseix(developerAccessToken);
		}

		senseix.message.Request.CheckResults ();
	}

	/// <summary>
	/// Registers the device with the SenseiX server, allows a temporary account to be created
	/// and the player to begin playing without logging in. Once an account is registered
	/// or created the temporary account is transitioned into a permanent one.  
	/// </summary>
	public void ReregisterDevice()
	{
		senseix.SenseixController.RegisterDevice ();
	}

	public static void UpdateCurrentPlayerScore (UInt32 score)
	{
		senseix.SenseixController.UpdateCurrentPlayerScore (score);
	}
	
	/// <summary>
	/// Returns the next problem for the player as an instance of the Problem class.  If there aren't 
	/// enough problems left in the queue, an asynchronous task will retrieve more from the SenseiX
	/// server.
	/// </summary>
	public static Problem NextProblem()
	{
		Debug.Log ("NEXT PROBLEM");
		senseix.message.problem.ProblemData.Builder protobufsProblemBuilder = senseix.SenseixController.PullProblem ();
		mostRecentProblem = new Problem (protobufsProblemBuilder);
		senseix.QuestionDisplay.singletonInstance.UpdateDisplay ();
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

	public static ProblemPart[] GetMostRecentProblemDistractors(int howManyDistractors)
	{
		return GetMostRecentProblem ().GetDistractors (howManyDistractors);
	}

	public static void AddGivenAnswerPartToMostRecentProblem(ProblemPart givenAnswerPart)
	{
		GetMostRecentProblem ().AddGivenAnswerPart (givenAnswerPart);
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
	/// Whichever player is the currently active player in the SenseiX menus, this
	/// will set that player's high score to the UInt32 argument.
	/// </summary>
	public static void SetCurrentPlayerHighScore (UInt32 score)
	{
		senseix.SenseixController.UpdateCurrentPlayerScore(score);
	}
	
	public static ArrayList GetMostRecentAnswerParts()
	{
		return GetMostRecentProblem ().GetCorrectAnswer ().GetAnswerParts ();
	}

	public static ProblemPart GetNextCorrectAnswerPart()
	{
		return GetMostRecentProblem ().GetNextCorrectAnswerPart ();
	}

	public static bool CheckMostRecentProblemAnswer()
	{
		return GetMostRecentProblem ().CheckAnswer ();
	}

	public static bool AllAnswerPartsGiven()
	{
		return GetMostRecentProblem().AnswersGivenSoFar() == GetCurrentCorrectAnswer().AnswerPartsCount();
	}

	public static Answer GetCurrentCorrectAnswer()
	{
		return GetMostRecentProblem ().GetCorrectAnswer ();
	}

	public static string GetMostRecentProblemHTML()
	{
		return GetMostRecentProblem ().GetQuestion ().GetHTML ();
	}
}

public class Problem 
{
	private senseix.message.problem.ProblemData.Builder protobufsProblemBuilder;
	private Answer givenAnswer = new Answer();
	
	public Problem(senseix.message.problem.ProblemData.Builder newProtobufsProblemBuilder)
	{
		protobufsProblemBuilder = newProtobufsProblemBuilder;
	}

	/// <summary>
	/// Returns the correct answer to this problem
	/// </summary>
	public Answer GetCorrectAnswer()
	{
		return new Answer(protobufsProblemBuilder.Answer);
	}

	public ProblemPart GetNextCorrectAnswerPart()
	{
		int answersGivenSoFar = givenAnswer.AnswerPartsCount ();
		if (answersGivenSoFar >= GetCorrectAnswer().AnswerPartsCount())
		{
			throw new Exception("There is no next correct answer part- all answer parts have already been given!");
		}
		ProblemPart nextCorrectAnswer = GetCorrectAnswer ().GetAnswerPart (answersGivenSoFar);
		return nextCorrectAnswer;
	}

	public ProblemPart[] GetDistractors(int howManyDistractors)
	{
		if (protobufsProblemBuilder.Distractor.AtomCount < howManyDistractors)
		{
			throw new Exception("There aren't enough distractors!  There are only "
			                    + protobufsProblemBuilder.Distractor.AtomCount + " distractors.");
		}
		ProblemPart[] distractors = new ProblemPart[howManyDistractors];
		for (int i = 0; i < howManyDistractors; i++)
		{
			senseix.message.problem.Atom distractorAtom = protobufsProblemBuilder.Distractor.AtomList[i];
			ProblemPart distractor = new ProblemPart(distractorAtom);
			distractors[i] = distractor;
		}
		return distractors;

	}

	/// <summary>
	/// Returns the answer set by SetGivenAnswer
	/// </summary>
	public Answer GetGivenAnswer()
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
	public Question GetQuestion()
	{
		return new Question(protobufsProblemBuilder.Question);
	}

	/// <summary>
	/// Sets the given answer.  This can then be checked for correctness through CheckAnswer.
	/// </summary>
	public void AddGivenAnswerPart(ProblemPart newGivenAnswerPart)
	{
		givenAnswer.AddAnswerPart(newGivenAnswerPart);
	}

	/// <summary>
	/// Checks the problem's given answer against its correct answer.  Also reports the player's answer
	/// (correct or incorrect) to the SenseiX server.
	/// </summary>
	public bool CheckAnswer()
	{
		return senseix.SenseixController.CheckAnswer (protobufsProblemBuilder, GetGivenAnswer());
	}

	public ArrayList GetGivenAnswerIDs()
	{
		return GetGivenAnswer ().GetAnswerIDs ();
	}

	public int AnswersGivenSoFar()
	{
		return GetGivenAnswer ().GetAnswerParts ().Count ;
	}

	public string GetQuestionHTML()
	{
		return GetQuestion ().GetHTML ();
	}
}

public class Answer
{
	ArrayList answerParts = new ArrayList();
	
	public Answer(senseix.message.problem.Answer protoAnswer)
	{
		foreach (senseix.message.problem.Atom atom in protoAnswer.AtomList)
		{
			answerParts.Add(new ProblemPart(atom));
		}
	}

	public Answer()
	{

	}

	public void AddAnswerPart(ProblemPart part)
	{
		answerParts.Add(part);
	}

	public ArrayList GetAnswerIDs()
	{
		ArrayList answerIDs = new ArrayList ();
		foreach(ProblemPart part in answerParts)
		{
			answerIDs.Add(part.GetUniqueID());
		}
		return answerIDs;
	}

	public ArrayList GetAnswerParts()
	{
		return answerParts;
	}

	public ProblemPart GetAnswerPart(int index)
	{
		return (ProblemPart)answerParts [index];
	}

	public int AnswerPartsCount()
	{
		return answerParts.Count;
	}
}

public class Question
{
	private senseix.message.problem.Question question;
	private IList<senseix.message.problem.Atom> atomList;

	public Question(senseix.message.problem.Question newQuestion)
	{
		question = newQuestion;
		atomList = newQuestion.AtomList;
	}

	public string GetHTML()
	{
		string html64 = question.Format.Html;
		byte[] htmlutf = System.Convert.FromBase64String (html64);
		string html = ASCIIEncoding.ASCII.GetString (htmlutf);
		return html;
	}

	public System.Collections.IEnumerator GetEnumerator()
	{
		foreach(senseix.message.problem.Atom atom in atomList)
		{
			yield return new ProblemPart(atom);
		}
	}

	public ProblemPart GetQuestionPart(int index)
	{
		try 
		{
			return new ProblemPart(atomList[index]);
		}
		catch (Exception e)
		{
			throw new Exception("Question part index out of range");
		}
	}
}

public class ProblemPart
{
	senseix.message.problem.Atom atom;

	public ProblemPart (senseix.message.problem.Atom newAtom)
	{
		atom = newAtom;
	}

	public string GetUniqueID()
	{
		return atom.Uuid;
	}

	public bool IsString()
	{
		return atom.Type == senseix.message.problem.Atom.Types.Type.TEXT;
	}

	public bool IsImage()
	{
		return atom.Type == senseix.message.problem.Atom.Types.Type.IMAGE;
	}

	public string GetString()
	{
		if (!IsString())
			throw new Exception ("This QuestionPart is not a string.  Be sure to check IsString before GetString.");
		string base64string = atom.Data.ToStringUtf8 ();
		byte[] decodedBytes = System.Convert.FromBase64String (base64string);
		return Encoding.ASCII.GetString (decodedBytes);
	}

	public Texture2D GetImage()
	{
		if (!IsImage())
			throw new Exception ("This QuestionPart is not an image.  Be sure to check IsImage before GetImage.");
		Texture2D returnImage = new Texture2D(0, 0);
		string base64string = atom.Data.ToStringUtf8 ();
		byte[] imageBytes = System.Convert.FromBase64String (base64string);
		returnImage.LoadImage (imageBytes);
		return returnImage;
	}
}
