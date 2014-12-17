using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Problem 
{
	private Senseix.Message.Problem.ProblemData.Builder protobufsProblemBuilder;
	private Answer givenAnswer = new Answer();
	private bool submitted = false;
	private static uint problemsAnsweredCorrectly = 0;

	/// <summary>
	/// Counts the problems answered correctly so far.
	/// </summary>
	/// <returns>The problems answered correctly so far.</returns>
	public static uint CountProblemsAnsweredCorrectlySoFar()
	{
		return problemsAnsweredCorrectly;
	}

	/// <summary>
	/// Don't use this unless you know what you're doing- instead get a problem from
	/// SenseixPlugin.NextProblem().
	/// </summary>
	public Problem(Senseix.Message.Problem.ProblemData.Builder newProtobufsProblemBuilder)
	{
		protobufsProblemBuilder = newProtobufsProblemBuilder;
	}
	
	~Problem()
	{
		if (!submitted)
		{
			Debug.Log("A problem which had never been submitted died.  That is sad. :( " +
			          "If you are seeing this message a lot, it might mean that you are " +
			          "using more than one problem at a time,\n and not submitting every " +
			          "problem you use.  For best results, remember to use Problem.SubmitAnswer()");
		}
	}
	
	/// <summary>
	/// Sets the given answer.  This will then be reflected in CheckAnswer().
	/// </summary>
	/// <param name="newGivenAnswer">New given answer.</param>
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
	
	/// <summary>
	/// Gets the next correct answer part based on how many answer parts have been
	/// given so far.
	/// </summary>
	/// <returns>The next correct answer part.</returns>
	public ProblemPart GetCurrentCorrectAnswerPart()
	{
		int answersGivenSoFar = givenAnswer.AnswerPartsCount ();
		if (answersGivenSoFar >= GetCorrectAnswer().AnswerPartsCount())
		{
			throw new Exception("There is no next correct answer part- all answer parts have already been given!");
		}
		ProblemPart nextCorrectAnswer = GetCorrectAnswer ().GetAnswerPart (answersGivenSoFar);
		return nextCorrectAnswer;
	}
	
	/// <summary>
	/// Gets distractors.
	/// These are wrong answers which can be presented as options to the player.
	/// </summary>
	/// <returns>The distractors.</returns>
	/// <param name="howManyDistractors">How many random distractors to return.</param>
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
			Senseix.Message.Atom.Atom distractorAtom = protobufsProblemBuilder.Distractor.AtomList[i];
			ProblemPart distractor = new ProblemPart(distractorAtom);
			allDistractors.Add(distractor);
		} //find all the distractors
		
		ProblemPart[] resultDistractors = new ProblemPart[howManyDistractors];
		for (int i = 0; i < howManyDistractors; i++)
		{
			int randomDistractorIndex = UnityEngine.Random.Range (0, allDistractors.Count);
			resultDistractors[i] = (ProblemPart)allDistractors[randomDistractorIndex];
			allDistractors.RemoveAt(randomDistractorIndex);
		} //take random ones
		
		return resultDistractors;
	}

	/// <summary>
	/// Gets one distractor.
	/// Distractors are wrong answers which can be presented as options to the player.
	/// </summary>
	/// <returns>The distractor.</returns>
	public ProblemPart GetDistractor()
	{
		return GetDistractors (1) [0];
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
	/// Checks the Problem's given answer against its correct answer.  DOES NOT report 
	/// to the senseix server.  Consider SubmitAnswer() if this is a final answer.
	/// </summary>
	public bool CheckAnswer()
	{
		return CheckAnswer (GetGivenAnswer ());
	}
	
	/// <summary>
	/// Sets the given answer to the argument, then checks it.  DOES NOT submit.
	/// Consider SubmitAnswer() if this is a final answer.
	/// </summary>
	public bool CheckAnswer(Answer answer)
	{
		return Senseix.SenseixSession.CheckAnswer (protobufsProblemBuilder, answer);
	}
	
	/// <summary>
	/// Submits the given answer.  This will update communicate with the senseix server
	/// asynchronously and update your player's progress accordingly.
	/// </summary>
	/// <returns>Whether or not the answer is correct</returns>
	public bool SubmitAnswer()
	{
		bool correct = Senseix.SenseixSession.CheckAnswer (protobufsProblemBuilder, GetGivenAnswer());
		Senseix.SenseixSession.SubmitAnswer (protobufsProblemBuilder, GetGivenAnswer(), correct);
		submitted = true;
		if (correct)
			problemsAnsweredCorrectly++;
		return correct;
	}
	
	/// <summary>
	/// Sets the given answer, then submits.  This will update communicate with the senseix server
	/// asynchronously and update your player's progress accordingly.
	/// </summary>
	/// <returns>Whether or not the answer is correct</returns>
	public bool SubmitAnswer(Answer answer)
	{
		SetGivenAnswer (answer);
		return SubmitAnswer ();
	}
	
	/// <summary>
	/// Gets the IDs of the given answers.  Mostly for internal use.
	/// Only use this if you want a bunch of uuids.
	/// </summary>
	public string[] GetGivenAnswerIDs()
	{
		return GetGivenAnswer ().GetAnswerIDs ();
	}
	
	/// <summary>
	/// Returns the number of answers which have been given so far.
	/// </summary>
	public int AnswersGivenSoFar()
	{
		return GetGivenAnswer ().GetAnswerParts ().Length;
	}
	
	/// <summary>
	/// Gets an HTML representation of this problem's question.
	/// </summary>
	public string GetQuestionHTML()
	{
		return GetQuestion ().GetHTML ();
	}

	/// <summary>
	/// Gets the question image.
	/// The same as GetQuestion ().GetImage ()
	/// </summary>
	/// <returns>The question image.</returns>
	public Texture2D GetQuestionImage()
	{
		return GetQuestion ().GetImage ();
	}

	public bool HasBeenSubmitted()
	{
		return submitted;
	}
}