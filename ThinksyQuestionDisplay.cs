using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ThinksyQuestionDisplay : MonoBehaviour 
{
	public GameObject richTextArea;
	public AdvancementAnimationPlayer advacementAnimation;

	private GameObject[] richTextAreas = new GameObject[0];
	private static uint displayedCategoryNumber;

	void Awake()
	{

	}

	void Start()
	{
		ThinksyQuestionDisplay.DisplayCurrentQuestion ();
	}

	/// <summary>
	/// Displays ThinksyPlugin.GetMostRecentProblem() on ALL Question displays.
	/// </summary>
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

	/// <summary>
	/// Displays the problem on this question display only.
	/// </summary>
	/// <param name="problemToDisplay">Problem to display.</param>
	public void DisplayProblem(Problem problemToDisplay)
	{
		ClearRichTextAreas ();
		PopulateRichTextAreas (problemToDisplay);
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

	private void ClearRichTextAreas()
	{
		foreach(GameObject textArea in richTextAreas)
		{
			Destroy(textArea);
		}
		richTextAreas = new GameObject[0];
	}

	private void PopulateRichTextAreas(Problem problemToDisplay)
	{
		Question question = problemToDisplay.GetQuestion ();
		int textAreaCount = question.GetQuestionPartCount ();

		float ySpacePerArea = 1f / (float)(textAreaCount - question.GetMultipleChoiceLetterCount());

		int row = 0;
		bool previousAreaWasMultipleChoice = false;

		if (textAreaCount == 0)
			return;//throw new UnityException ("I got a problem with no question atoms.");

		for (int i = 0; i < textAreaCount; i++)
		{
			GameObject newArea = Instantiate(richTextArea) as GameObject;
			ProblemPart problemPart = problemToDisplay.GetQuestion().GetQuestionPart(i);

			if (newArea.GetComponent<RectTransform>() == null || newArea.GetComponent<Text>() == null)
				throw new UnityException("richTextArea must have a rect transform and Text");

			if (problemPart.IsString())
			{
				newArea.GetComponent<Text>().text += problemPart.GetString();
			}

			if (problemPart.IsImage())
			{
				//display repeated number of images.  write this.
				newArea.GetComponent<Text>().text += problemPart.TimesRepeated(); //this is not it
			}

			float indentedX = 0f;
			if (previousAreaWasMultipleChoice)
				indentedX = 0.1f;

			newArea.GetComponent<RectTransform>().SetParent(gameObject.transform);
			newArea.GetComponent<RectTransform>().anchorMin = new Vector2(indentedX,
			                                                             row * ySpacePerArea);
			newArea.GetComponent<RectTransform>().anchorMax = new Vector2(1,
			                                                             (row + 1) * ySpacePerArea);
			newArea.GetComponent<RectTransform>().offsetMin = new Vector2(0f, 0f);
			newArea.GetComponent<RectTransform>().offsetMax = new Vector2(0f, 0f);
			newArea.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);

			richTextAreas[i] = newArea;

			if (!problemPart.IsMultipleChoiceLetter())
			{
				row++;
				previousAreaWasMultipleChoice = false;
			}
			else
			{
				previousAreaWasMultipleChoice = true;
			}
		}
	}
}