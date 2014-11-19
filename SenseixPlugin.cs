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

	public string developerAccessToken; //this is your developer access token obtained from 
										//the Senseix website.
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
		Senseix.ProblemKeeper.CopyFailsafeOver ();
		Senseix.SenseixController.InitializeSenseix (developerAccessToken);
	}

	void Update()
	{
		if (!Senseix.SenseixController.GetSessionState() && Time.frameCount%reconnectRetryInterval == 0)
		{
			Senseix.SenseixController.InitializeSenseix(developerAccessToken);
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
		Senseix.SenseixController.RegisterDevice ();
	}

	public static void UpdateCurrentPlayerScore (UInt32 score)
	{
		Senseix.SenseixController.UpdateCurrentPlayerScore (score);
	}
	
	/// <summary>
	/// Returns the next Problem for the Player as an instance of the Problem class.  If there aren't 
	/// enough Problems left in the queue, an asynchronous task will retrieve more from the Senseix
	/// server.
	/// </summary>
	public static Problem NextProblem()
	{
		Debug.Log ("NEXT Problem");
		Senseix.Message.Problem.ProblemData.Builder protobufsProblemBuilder = Senseix.SenseixController.PullProblem ();
		mostRecentProblem = new Problem (protobufsProblemBuilder);
		Senseix.QuestionDisplay.Update ();
		return mostRecentProblem;
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

	public static Answer GetMostRecentGivenAnswer()
	{
		return GetMostRecentProblem ().GetGivenAnswer ();
	}

	public static Question GetMostRecentProblemQuestion()
	{
		return GetMostRecentProblem ().GetQuestion();
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
	/// Checks the Problem's given answer against its correct answer.  Also reports the Player's answer
	/// (correct or incorrect) to the Senseix server.  Given and correct answer can be found in the Problem class.
	/// </summary>
	public static bool CheckAnswer(Problem Problem)
	{
		return Problem.CheckAnswer ();
	}

	/// <summary>
	/// Whichever Player is the currently active Player in the Senseix menus, this
	/// will set that Player's high score to the UInt32 argument.
	/// </summary>
	public static void SetCurrentPlayerHighScore (UInt32 score)
	{
		Senseix.SenseixController.UpdateCurrentPlayerScore(score);
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

	public static Texture2D GetMostRecentProblemImage()
	{
//		Texture2D returnImage = new Texture2D(0, 0);
//		string exampleText = System.IO.File.ReadAllText (System.IO.Path.Combine (Application.dataPath, "example.proto"));
//		string base64string = exampleText;
//		byte[] imageBytes = System.Convert.FromBase64String (base64string);
//		returnImage.LoadImage (imageBytes);
//		return returnImage;
		return GetMostRecentProblem ().GetQuestion ().GetImage ();
	}
}

public class Problem 
{
	private Senseix.Message.Problem.ProblemData.Builder protobufsProblemBuilder;
	private Answer givenAnswer = new Answer();
	
	public Problem(Senseix.Message.Problem.ProblemData.Builder newProtobufsProblemBuilder)
	{
		protobufsProblemBuilder = newProtobufsProblemBuilder;
	}

	public void SetGivenAnswer(Answer newGivenAnswer)
	{
		givenAnswer = newGivenAnswer;
	}

	/// <summary>
	/// Returns the correct answer to this Problem
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
		int availableDistractors = protobufsProblemBuilder.Distractor.AtomCount;
		if (availableDistractors < howManyDistractors)
		{
			throw new Exception("There aren't enough distractors!  There are only "
			                    + availableDistractors + " distractors.");
		}
		ArrayList allDistractors = new ArrayList();
		for (int i = 0; i < availableDistractors; i++)
		{
			Senseix.Message.Problem.Atom distractorAtom = protobufsProblemBuilder.Distractor.AtomList[i];
			ProblemPart distractor = new ProblemPart(distractorAtom);
			allDistractors.Add(distractor);
		} //find all the distractors

		ProblemPart[] resultDistractors = new ProblemPart[howManyDistractors];
		System.Random random = new System.Random ();
		for (int i = 0; i < howManyDistractors; i++)
		{
			int randomDistractorIndex = random.Next (allDistractors.Count);
			resultDistractors[i] = (ProblemPart)allDistractors[randomDistractorIndex];
			allDistractors.RemoveAt(randomDistractorIndex);
		} //take random ones

		return resultDistractors;
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
		Debug.Log ("Problem ID" + protobufsProblemBuilder.Uuid);
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
	/// Checks the Problem's given answer against its correct answer.  Also reports the Player's answer
	/// (correct or incorrect) to the Senseix server.
	/// </summary>
	public bool CheckAnswer()
	{
		return Senseix.SenseixController.CheckAnswer (protobufsProblemBuilder, GetGivenAnswer());
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

	public Texture2D GetQuestionImage()
	{
		return GetQuestion ().GetImage ();
	}
}

public class Answer
{
	ArrayList answerParts = new ArrayList();
	
	public Answer(Senseix.Message.Problem.Answer protoAnswer)
	{
		foreach (Senseix.Message.Problem.Atom atom in protoAnswer.AtomList)
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
		Senseix.QuestionDisplay.Update ();
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
	private Senseix.Message.Problem.Question question;
	private IList<Senseix.Message.Problem.Atom> atomList;

	public Question(Senseix.Message.Problem.Question newQuestion)
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

	public Texture2D GetImage()
	{
		Texture2D returnImage = new Texture2D(0, 0);
		Debug.Log ("LENGTH OF THE IMAGE BYTES FIELD " + question.Image.Length);
		//byte[] imageBytes = Senseix.SenseixController.DecodeServerBytes (question.Image);
		string base64 = question.Image.ToStringUtf8 ();
		byte[] imageBytes = System.Convert.FromBase64String (base64);
		//Debug.Log ("BYTES AS HEX: " + System.BitConverter.ToString(imageBytes));
		returnImage.LoadImage (imageBytes);
		Debug.Log (returnImage.GetPixels ().Length);
		return returnImage;
	}

	public System.Collections.IEnumerator GetEnumerator()
	{
		foreach(Senseix.Message.Problem.Atom atom in atomList)
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
	Senseix.Message.Problem.Atom atom;

	public ProblemPart (Senseix.Message.Problem.Atom newAtom)
	{
		atom = newAtom;
	}

	public string GetUniqueID()
	{
		return atom.Uuid;
	}

	public bool IsString()
	{
		return atom.Type == Senseix.Message.Problem.Atom.Types.Type.TEXT;
	}

	public bool IsImage()
	{
		return atom.Type == Senseix.Message.Problem.Atom.Types.Type.IMAGE;
	}

	public string GetString()
	{
		if (!IsString())
			throw new Exception ("This QuestionPart is not a string.  Be sure to check IsString before GetString.");
		byte[] decodedBytes = Senseix.SenseixController.DecodeServerBytes (atom.Data);
		return Encoding.ASCII.GetString (decodedBytes);
	}

	public Texture2D GetImage()
	{
		if (!IsImage())
			throw new Exception ("This QuestionPart is not an image.  Be sure to check IsImage before GetImage.");
		Texture2D returnImage = new Texture2D(0, 0);
		byte[] imageBytes = Senseix.SenseixController.DecodeServerBytes (atom.Data);
		returnImage.LoadImage (imageBytes);
		return returnImage;
	}
}
