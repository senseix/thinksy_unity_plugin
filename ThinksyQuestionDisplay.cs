using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ThinksyQuestionDisplay : MonoBehaviour 
{
	public UnityEngine.UI.RawImage promptDisplay;
	public UnityEngine.UI.Text promptText;
	public UnityEngine.UI.Text answersSoFarText;
	public AdvancementAnimationPlayer advacementAnimation;
	
	private static uint displayedCategoryNumber;

	void Awake()
	{

	}

	void Start()
	{
		ThinksyQuestionDisplay.DisplayCurrentQuestion ();
	}

	public static void DisplayCurrentQuestion()
	{
		ThinksyQuestionDisplay[] instances = FindObjectsOfType<ThinksyQuestionDisplay> ();
		foreach (ThinksyQuestionDisplay questionDisplay in instances)
		{
			questionDisplay.InstanceDisplayCurrentQuestion ();
		}
	}

	private void InstanceDisplayCurrentQuestion()
	{
		DisplayProblem (ThinksyPlugin.GetMostRecentProblem());
	}

	private void DisplayProblem(Problem problemToDisplay)
	{
		DisplayImage (problemToDisplay.GetQuestionImage ());
		DisplayProblemText (problemToDisplay);
		//Debug.Log ("html: " + problemToDisplay.GetQuestionHTML ());
		DisplayAdvancementFanfareIfNeeded (problemToDisplay);
	}

	private void DisplayAdvancementFanfareIfNeeded(Problem problemToDisplay)
	{
		//Debug.Log ("new: " + problemToDisplay.GetCategoryNumber () + " old: " + displayedCategoryNumber);
		if (problemToDisplay.GetCategoryNumber() > displayedCategoryNumber)
		{
			advacementAnimation.PlayAnimation();
		}
		displayedCategoryNumber = problemToDisplay.GetCategoryNumber ();
	}

	private void DisplayProblemText(Problem problemToDisplay)
	{
		DisplayQuestionText (problemToDisplay.GetQuestion());
		DisplayAnswersText (problemToDisplay.GetGivenAnswer());
	}

	private void DisplayQuestionText(Question questionToDisplay)
	{
		foreach (ProblemPart part in questionToDisplay)
		{
			//Debug.Log("found a problem part in question");
			if (part.IsString())
				promptText.text = part.GetString();
		}
	}

	private void DisplayAnswersText (Answer answerToDisplay)
	{
		answersSoFarText.text = "";
		foreach (ProblemPart part in answerToDisplay.GetAnswerParts())
		{
			answersSoFarText.text += " " + part.GetString();
		}
	}

	private void DisplayImage(Texture2D imageToDisplay)
	{
		if (promptDisplay != null)
			promptDisplay.texture = imageToDisplay;
	}
}