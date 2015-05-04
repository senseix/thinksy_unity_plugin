using System;
using System.Text;

public class TextProblemPart : ProblemPart
{
	public TextProblemPart (Senseix.Message_v2.Atom.Atom atom) : base(atom)
	{

	}

	/// <summary>
	/// If this is represented by a string, this gets the string.
	/// You probably want to check IsString before calling this.
	/// </summary>
	public override string GetString()
	{
		return GetAtom().text_atom.text;
	}

	public override bool HasString()
	{
		return true;
	}
}