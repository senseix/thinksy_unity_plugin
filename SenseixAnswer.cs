using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Answer
{
	ArrayList answerParts = new ArrayList();
	
	/// <summary>
	/// Don't use this unless you know what you're doing.  Instead, use the other constructors.
	/// </summary>
	public Answer(Senseix.Message.Problem.Answer protoAnswer)
	{
		foreach (Senseix.Message.Atom.Atom atom in protoAnswer.AtomList)
		{
			answerParts.Add(new ProblemPart(atom));
		}
	}
	
	/// <summary>
	/// Initializes a new empty instance of the <see cref="Answer"/> class.
	/// </summary>
	public Answer()
	{
		
	}
	
	/// <summary>
	/// Adds a part to this answer.
	/// </summary>
	/// <param name="part">Added Part.</param>
	public void AddAnswerPart(ProblemPart part)
	{
		answerParts.Add(part);
		Senseix.QuestionDisplay.Update ();
	}
	
	/// <summary>
	/// Clears the answer parts.  You might use this if you want to give your player a second
	/// chance at giving answers.
	/// </summary>
	public void ClearAnswerParts()
	{
		answerParts.Clear ();
	}
	
	/// <summary>
	/// Removes the most recent answer part.  You might use this if you want to give your player
	/// a chance to retry an incorrect answer part, without retrying the whole problem.
	/// </summary>
	public void RemoveMostRecentAnswerPart()
	{
		answerParts.RemoveAt (answerParts.Count - 1);
		Senseix.QuestionDisplay.Update ();
	}
	
	/// <summary>
	/// Unless you know how to use them, this will return a bunch of nonsensical strings.
	/// Use this if you want a bunch of uuids.
	/// </summary>
	public string[] GetAnswerIDs()
	{
		string[] answerIDs = new string[answerParts.Count];
		for (int i = 0; i < answerParts.Count; i++)
		{
			answerIDs[i] = (((ProblemPart)answerParts[i]).GetUniqueID());
		}
		return answerIDs;
	}
	
	/// <summary>
	/// Gets the answer parts given so far.
	/// </summary>
	/// <returns>The answer parts associated with this answer.</returns>
	public ProblemPart[] GetAnswerParts()
	{
		return (ProblemPart[])answerParts.ToArray(typeof(ProblemPart));
	}
	
	/// <summary>
	/// Gets the answer part of the given index.  You might use this if you want to review past
	/// or future correct or given answers parts.
	/// You can also iterate through answer parts using foreach.
	/// </summary>
	/// <param name="index">Index.</param>
	public ProblemPart GetAnswerPart(int index)
	{
		return (ProblemPart)answerParts [index];
	}
	
	public System.Collections.IEnumerator GetEnumerator()
	{
		foreach(ProblemPart part in answerParts)
		{
			yield return part;
		}
	}
	
	/// <summary>
	/// Returns the number of answer parts current associated with this answer.
	/// </summary>
	public int AnswerPartsCount()
	{
		return answerParts.Count;
	}
}
