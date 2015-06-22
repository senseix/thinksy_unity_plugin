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
		return new Senseix.Message.Problem.LearningAction ();
	}
}