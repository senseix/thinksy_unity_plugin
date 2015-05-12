using System;

public abstract class LearningAction
{
	public static LearningAction CreateLearningAction(Senseix.Message.Problem.LearningAction learningActionProto)
	{
		if (learningActionProto == null)
			throw new Exception ("You are trying to create a learning action from null");

		LearningAction createdLearningAction = null;
		if (learningActionProto.count_action != null)
		{
			return new CountAction(learningActionProto.count_action);
		}
		if (createdLearningAction == null)
		{
			throw new Exception("");
		}
		return createdLearningAction;
	}

	public CountAction GetCountLearningAction()
	{
		if (!IsCountLearningAction())
			throw new Exception("There is no count learning action associated with this problem.  " +
			                    "Consider checking HasCountLearningAction() before calling GetCountLearningAction()");
		return (CountAction)this;
	}
	
	public virtual bool IsCountLearningAction()
	{
		return false;
	}

	public LearningAction ()
	{
		
	}
}