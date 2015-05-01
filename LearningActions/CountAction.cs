using System;

public class CountAction : LearningAction
{
	private int countFrom; //where to begin
	private int countBy; //how large one step is
	private int stepCount; //how many steps to take

	public int GetCountFrom()
	{
		return countFrom;
	}

	public int GetCountBy()
	{
		return countBy;
	}

	public int GetStepCount()
	{
		return stepCount;
	}

	public CountAction () : base()
	{

	}

	public override bool IsCountLearningAction()
	{
		return true;
	}
}