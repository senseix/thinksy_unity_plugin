using System;

public class LearningAction
{
	private Senseix.Message.Problem.LearningAction learningActionProto;

	public LearningAction(Senseix.Message.Problem.LearningAction newLearningActionProto)
	{
		learningActionProto = newLearningActionProto;
	}

	public Senseix.Message.Problem.LearningAction GetProto()
	{
		return learningActionProto;
	}

	public ProblemPart GetLearningActionPartByName(string name)
	{
		for (int i = 0; i < GetProto().atoms.Count; i++)
		{
			Senseix.Message.Atom.Atom atom = GetProto().atoms[i];
			if (atom.description.Equals(name))
			{
				return GetLearningActionPartByIndex(i);
			}
		}
		throw new Exception ("There is no problem part by that name in this learning action.");
	}

	public ProblemPart GetLearningActionPartByIndex(int index)
	{
		if (index < 0 || index > GetProto().atoms.Count)
		{
			throw new Exception("That is out of range.  There are " + GetProto().atoms.Count + " problem parts.");
		}
		return ProblemPart.CreateProblemPart(GetProto().atoms[index]);
	}
}