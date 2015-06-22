using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Question
{
	private Senseix.Message.Problem.Question question;
	private IList<Senseix.Message.Atom.Atom> atomList;
	
	/// <summary>
	/// Don't use this constructor unless you know what you're doing.  You can get the question from a
	/// server-generated problem instead.  Take a look at Problem.GetQuestion().
	/// </summary>
	public Question(Senseix.Message.Problem.Question newQuestion)
	{
		question = newQuestion;
		atomList = newQuestion.atoms;
	}
	
	/// <summary>
	/// This question represented as HTML.
	/// </summary>
	public string GetHTML()
	{
		string html64 = question.format.html;
		byte[] htmlutf = System.Convert.FromBase64String (html64);
		string html = ASCIIEncoding.ASCII.GetString (htmlutf);
		Debug.Log (html.Length);
		return html;
	}
	
	/// <summary>
	/// An image which communicated this question.  This is what the senseix display displays.
	/// </summary>
	public Texture2D GetImage()
	{
		Texture2D returnImage = new Texture2D(0, 0);
		//Debug.Log ("LENGTH OF THE IMAGE BYTES FIELD " + question.image.Length);
		byte[] imageBytes = question.image;//Senseix.SenseixController.DecodeServerBytes (question.Image);
		//string base64 = question.Image.ToStringUtf8 ();
		//byte[] imageBytes = System.Convert.FromBase64String (base64);
		//Debug.Log ("BYTES AS HEX: " + System.BitConverter.ToString(imageBytes));
		returnImage.LoadImage (imageBytes);
		//Debug.Log (returnImage.GetPixels ().Length);
		return returnImage;
	}
	
	public System.Collections.IEnumerator GetEnumerator()
	{
		foreach(Senseix.Message.Atom.Atom atom in atomList)
		{
			yield return ProblemPart.CreateProblemPart(atom);
		}
	}
	
	/// <summary>
	/// Questions can be represented as a series of ProblemParts
	/// which contain information to communicate the problem.
	/// This gets the question part at the given index.
	/// You can also iterate through a question using foreach
	/// </summary>
	public ProblemPart GetQuestionPart(int index)
	{
		try 
		{
			return ProblemPart.CreateProblemPart(atomList[index]);
		}
		catch
		{
			throw new Exception("Question part index out of range");
		}
	}

	/// <summary>
	/// Gets the question part count.  You may want to know this
	/// in order to calculate how much space to display each
	/// question part in.
	/// </summary>
	/// <returns>The question part count.</returns>
	public int GetQuestionPartCount()
	{
		return atomList.Count;
	}

	/// <summary>
	/// Gets the maximum number of times any one image is repeated in a
	/// problem part in this question.  Possibly useful for question
	/// display purposes.
	/// </summary>
	/// <returns>The maximum question part repeated.</returns>
	public int GetMaximumQuestionPartRepeated()
	{
		int maximumRepeated = 0;
		foreach(ProblemPart part in this)
		{
			if (part.TimesRepeated() > maximumRepeated)
				maximumRepeated = part.TimesRepeated();
		}
		return maximumRepeated;
	}

	/// <summary>
	/// Gets the number of problem parts which are multiple choice.
	/// You may want to know this in order to calculate how much space
	/// to give each problem part, single multiple choice letters hardly
	/// take up space.
	/// </summary>
	public int GetMultipleChoiceLetterCount()
	{
		int multipleChoiceCount = 0;
		foreach (ProblemPart questionPart in this)
		{
			if (questionPart.IsMultipleChoiceLetter())
				multipleChoiceCount++;
		}
		
		return multipleChoiceCount;
	}
}