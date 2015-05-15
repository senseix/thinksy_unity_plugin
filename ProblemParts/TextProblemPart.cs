using System;
using System.Text;

public class TextProblemPart : ProblemPart
{
	public TextProblemPart (Senseix.Message.Atom.Atom atom) : base(atom)
	{

	}

	/// <summary>
	/// If this is represented by a string, this gets the string.
	/// You probably want to check IsString before calling this.
	/// </summary>
	public override string GetString()
	{
		byte[] decodedBytes = GetAtom().data;//Senseix.SenseixController.DecodeServerBytes (atom.Data);
		//Debug.Log ("Length of data byte string of text atom: " + decodedBytes.Length);
		return Encoding.ASCII.GetString (decodedBytes);
	}

	public override bool HasString()
	{
		return true;
	}
}