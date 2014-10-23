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

	public string developerAccessToken;

	private static Problem mostRecentProblem;

	void Start()
	{
		Senseix.SenseixController.InitializeSenseix (developerAccessToken);
	}

	~SenseixPlugin(){
		Senseix.SenseixController.EndLife ();
	}

	
	public void ReregisterDevice()
	{
		Senseix.SenseixController.RegisterDevice ();
	}
	
	public static Problem NextProblem()
	{
		Senseix.Message.Problem.ProblemData.Builder protobufsProblemBuilder = Senseix.SenseixController.PullProblem ();
		mostRecentProblem = new Problem (protobufsProblemBuilder);
		return mostRecentProblem;
	}

	public static Problem GetMostRecentProblem()
	{
		if (mostRecentProblem == null)
		{
			throw new Exception("There are not yet any problems.  Please user SenseixPlugin.NextProblem()");
		}
		return mostRecentProblem;
	}

	public static bool CheckAnswer(Problem problem)
	{
		return problem.CheckAnswer ();
	}

	public static bool CheckAnswer(Problem problem, string answer)
	{
		return problem.CheckAnswer (answer);
	}

}

public class Problem {

	private Senseix.Message.Problem.ProblemData.Builder protobufsProblemBuilder;
	private string givenAnswer;

	public Problem(Senseix.Message.Problem.ProblemData.Builder newProtobufsProblemBuilder)
	{
		protobufsProblemBuilder = newProtobufsProblemBuilder;
	}

	public string GetCorrectAnswer()
	{
		return protobufsProblemBuilder.Answer;
	}

	public string GetGivenAnswer()
	{
		if (givenAnswer == null)
		{
			throw new Exception("No answers have yet been given.  Use Problem.SetGivenAnswer(someString)");
		}
		return givenAnswer;
	}

	public string GetQuestion()
	{
		return protobufsProblemBuilder.Question;
	}

	public void SetGivenAnswer(string newGivenAnswer)
	{
		givenAnswer = newGivenAnswer;
	}

	public bool CheckAnswer()
	{
		return Senseix.SenseixController.CheckAnswer (protobufsProblemBuilder, GetGivenAnswer());
	}

	public bool CheckAnswer(string answer)
	{
		return Senseix.SenseixController.CheckAnswer (protobufsProblemBuilder, answer);
	}

}