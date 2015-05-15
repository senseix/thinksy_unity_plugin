using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

//A problem part represents a small piece of data
//used to construct questions and answers.

public abstract class ProblemPart
{
	Senseix.Message.Atom.Atom atom;

	public static ProblemPart CreateProblemPart(Senseix.Message.Atom.Atom newAtom)
	{
		ProblemPart newProblemPart = new TextProblemPart(newAtom);

		bool actuallyConstructed = false;
		if (newAtom.type == Senseix.Message.Atom.Atom.Type.IMAGE)
		{
			newProblemPart = new ImageProblemPart(newAtom);
			actuallyConstructed = true;
		}
		if (newAtom.type == Senseix.Message.Atom.Atom.Type.TEXT)
		{
			newProblemPart = new TextProblemPart(newAtom);
			actuallyConstructed = true;
		}
		if (!actuallyConstructed)
		{
			throw new Exception("I encountered an atom of an unimplemented type.");
		}

		return newProblemPart;
	}

	public ProblemPart(Senseix.Message.Atom.Atom newAtom)
	{
		atom = newAtom;
	}

	/// <summary>
	/// This will build a text ProblemPart.  Hot tip: numbers are actually text
	/// ProblemParts.  If the text happens to be numbers, it will be treated
	/// as such.
	/// </summary>
	public ProblemPart (string textContent)
	{
		Senseix.Message.Atom.Atom newAtom = new Senseix.Message.Atom.Atom ();

		//forced my hand
		newAtom.uuid = "Developer-side generated problem part";
		newAtom.required = false;

		//text
		newAtom.type = Senseix.Message.Atom.Atom.Type.TEXT;
		newAtom.data = Encoding.ASCII.GetBytes (textContent);
	}

	/// <summary>
	/// This will build an image ProblemPart.  The filename references only
	/// Sprites from resources.
	/// </summary>
	public ProblemPart (string filename, bool isPretty)
	{
		Senseix.Message.Atom.Atom newAtom = new Senseix.Message.Atom.Atom ();
		
		//forced my hand
		newAtom.uuid = "Developer-side generated problem part";
		newAtom.required = false;
		newAtom.data = new byte[0];
		
		//text
		newAtom.type = Senseix.Message.Atom.Atom.Type.IMAGE;
		newAtom.filename = filename;
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

	public Senseix.Message.Atom.Atom GetAtom()
	{
		return atom;
	}
}