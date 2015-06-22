using System;

public class NumberProblemPart : ProblemPart
{
	public NumberProblemPart (Senseix.Message.Atom.Atom atom) : base(atom)
	{
	}

	public override bool HasString ()
	{
		return true;
	}

	public override string GetString ()
	{
		return System.Convert.ToString(GetAtom().numberValue);
	}

	public override bool HasInteger()
	{
		return true;
	}

	public override int GetInteger ()
	{
		return GetAtom ().numberValue;
	}
}