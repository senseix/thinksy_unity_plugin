using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class ProblemPart
{
	Senseix.Message.Atom.Atom atom;
	
	/// <summary>
	/// You can get problem parts from questions, answers, and distractors.
	/// If you want to create them yourself, that is great!
	/// Please e-mail SenseiX telling us about your game, and we can add
	/// features that allow you to do what you are trying to do.
	/// 
	/// Including possibly the game-side creation of problem parts.
	/// 
	/// But you can't do that yet, unless you really know what you're doing.
	/// </summary>
	public ProblemPart (Senseix.Message.Atom.Atom newAtom)
	{
		atom = newAtom;
	}
	
	/// <summary>
	/// This is a uuid for this problem part.  But, most likely, from your perspective,
	/// it's just a meaningless string.
	/// </summary>
	public string GetUniqueID()
	{
		return atom.Uuid;
	}
	
	/// <summary>
	/// If this problem part is represented using a string, this will return true.
	/// Otherwise, this will return false.
	/// </summary>
	/// <returns><c>true</c> if this instance is string; otherwise, <c>false</c>.</returns>
	public bool IsString()
	{
		return atom.Type == Senseix.Message.Atom.Atom.Types.Type.TEXT;
	}
	
	/// <summary>
	/// If this problem part is represented using an image, this will return true.
	/// Otherwise, this will return false.
	/// </summary>
	/// <returns><c>true</c> if this instance is image; otherwise, <c>false</c>.</returns>
	public bool IsImage()
	{
		return atom.Type == Senseix.Message.Atom.Atom.Types.Type.IMAGE;
	}

	/// <summary>
	/// If this problem can be represented as an integer, this will return true.
	/// Otherwise, this will return false.
	/// </summary>
	/// <returns><c>true</c> if this instance is integer; otherwise, <c>false</c>.</returns>
	public bool IsInteger()
	{
		if (!IsString ())
			return false;
		try
		{
			UnsafeGetInteger();
		}
		catch
		{
			return false;
		}
		return true;
	}

	private int UnsafeGetInteger()
	{
		return Convert.ToInt32(GetString());
	}

	/// <summary>
	/// If this is represented by an integer, gets the integer.
	/// You probably want to check IsInteger before calling this.
	/// </summary>
	public int GetInteger()
	{
		if (!IsInteger())
			throw new Exception ("This QuestionPart is not an integer.  Be sure to check IsInteger before GetInteger.");
		return UnsafeGetInteger ();
	}
	
	/// <summary>
	/// If this is represented by a string, this gets the string.
	/// You probably want to check IsString before calling this.
	/// </summary>
	public string GetString()
	{
		if (!IsString())
			throw new Exception ("This QuestionPart is not a string.  Be sure to check IsString before GetString.");
		byte[] decodedBytes = atom.Data.ToByteArray ();//Senseix.SenseixController.DecodeServerBytes (atom.Data);
		return Encoding.ASCII.GetString (decodedBytes);
	}
	
	/// <summary>
	/// If this is represented by an image, this gets the image.
	/// You probably want to check IsImage before calling this.
	/// </summary>
	public Texture2D GetImage()
	{
		if (!IsImage())
			throw new Exception ("This QuestionPart is not an image.  Be sure to check IsImage before GetImage.");
		Texture2D returnImage = new Texture2D(0, 0);
		byte[] imageBytes = atom.Data.ToByteArray ();//Senseix.SenseixController.DecodeServerBytes (atom.Data);
		returnImage.LoadImage (imageBytes);
		return returnImage;
	}
}
