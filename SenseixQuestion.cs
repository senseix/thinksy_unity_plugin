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
		atomList = newQuestion.AtomList;
	}
	
	/// <summary>
	/// This question represented as HTML.
	/// </summary>
	public string GetHTML()
	{
		string html64 = question.Format.Html;
		byte[] htmlutf = System.Convert.FromBase64String (html64);
		string html = ASCIIEncoding.ASCII.GetString (htmlutf);
		return html;
	}
	
	/// <summary>
	/// An image which communicated this question.  This is what the senseix display displays.
	/// </summary>
	public Texture2D GetImage()
	{
		Texture2D returnImage = new Texture2D(0, 0);
		//Debug.Log ("LENGTH OF THE IMAGE BYTES FIELD " + question.Image.Length);
		byte[] imageBytes = question.Image.ToByteArray();//Senseix.SenseixController.DecodeServerBytes (question.Image);
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
			yield return new ProblemPart(atom);
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
			return new ProblemPart(atomList[index]);
		}
		catch
		{
			throw new Exception("Question part index out of range");
		}
	}
}