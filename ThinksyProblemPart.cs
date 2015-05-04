using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

//A problem part represents a small piece of data
//used to construct questions and answers.

public abstract class ProblemPart
{
	Senseix.Message_v2.Atom.Atom atom;

	public static ProblemPart CreateProblemPart(Senseix.Message_v2.Atom.Atom newAtom)
	{
		ProblemPart newProblemPart = new TextProblemPart(newAtom);

		bool actuallyConstructed = false;
		if (newAtom.image_atom != null)
		{
			newProblemPart = new ImageProblemPart(newAtom);
			actuallyConstructed = true;
		}
		if (newAtom.text_atom != null)
		{
			newProblemPart = new TextProblemPart(newAtom);
			actuallyConstructed = true;
		}
		if (newAtom.number_atom != null)
		{
			newProblemPart = new NumberProblemPart(newAtom);
		}
		if (newAtom.equation_atom != null)
		{
			newProblemPart = new EquationProblemPart(newAtom);
		}
		if (!actuallyConstructed)
		{
			throw new Exception("I encountered an atom of an unimplemented type.");
		}

		return newProblemPart;
	}

	public ProblemPart(Senseix.Message_v2.Atom.Atom newAtom)
	{
		atom = newAtom;
	}

	/// <summary>
	/// This is a uuid for this problem part.  But, most likely, from your perspective,
	/// it's just a meaningless string.
	/// </summary>
	public string GetUniqueID()
	{
		return atom.uuid;
	}

	public virtual bool HasString()
	{
		return false;
	}

	public virtual bool HasInteger()
	{
		return false;
	}

	public virtual bool HasSprite()
	{
		return false;
	}

	/// <summary>
	/// Determines whether this instance is multiple choice letter.
	/// </summary>
	/// <returns><c>true</c> if this instance is multiple choice letter; otherwise, <c>false</c>.</returns>
	public bool IsMultipleChoiceLetter()
	{
		if (!HasString ())
		{
			return false;
		}
		return (GetString() == "A: " ||
		        GetString() == "B: " ||
		        GetString() == "C: " ||
		        GetString() == "D: " );
	}
	
	public virtual int GetInteger()
	{
		throw new Exception ("There is no way to represent this problem part as an integer.");
	}

	public virtual string GetString()
	{
		throw new Exception ("There is no way to represent this problem part as a string");
	}

	public virtual Sprite GetSprite()
	{
		throw new Exception ("There is no way to represent this problem part with a sprite");
	}

	public int TimesRepeated()
	{
		if (atom.repeated == 0)
			return 1;
		return atom.repeated;
	}

	public Senseix.Message_v2.Atom.Atom GetAtom()
	{
		return atom;
	}
}