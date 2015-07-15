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
		if (!GetAtom().text.Equals(""))
			return GetAtom().text;
		else return Encoding.UTF8.GetString (GetAtom().data);
	}

	public override bool HasString()
	{
		return true;
	}
}