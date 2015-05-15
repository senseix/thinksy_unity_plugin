using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ThinksyQuestionDisplay : MonoBehaviour 
{
	public GameObject questionTextPrefab;
	public GameObject questionImagePrefab;
	public Transform questionParent;
	[Range(0f, 0.5f)]
	public float indentMultipleChoices = 0.15f;
	[Range(0f, 0.03f)]
	public float spaceImageAreas = 0.01f;
	public int imagesPerRow = 10;

	private GameObject[] richTextAreas = new GameObject[0];
	private static uint displayedCategoryNumber;

	void Awake()
	{
		if (imagesPerRow <= 0)
			Debug.LogWarning ("imagesPerRow should not be zero or negative");
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
		InvokeCategoryAdvancementIfNeeded (problemToDisplay);
	}

	private void InvokeCategoryAdvancementIfNeeded(Problem problemToDisplay)
	{
		//Debug.Log ("new: " + problemToDisplay.GetCategoryNumber () + " old: " + displayedCategoryNumber);
		if (problemToDisplay.GetCategoryNumber() > displayedCategoryNumber)
		{
			ThinksyEvents.InvokeCategoryAdvancement();
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
		float xSpacePerImage = (float)1/Mathf.Clamp((float)question.GetMaximumQuestionPartRepeated(), 1, imagesPerRow);
		float ySpacePerImage = (float)1 / Mathf.Ceil((float)question.GetMaximumQuestionPartRepeated() 
		                                             / (float)imagesPerRow);

		int row = 0;
		bool previousAreaWasMultipleChoice = false;

		if (textAreaCount == 0)
		{
			Debug.LogWarning("I got a problem with no question atoms.");
			return;//throw new UnityException ("I got a problem with no question atoms.");
		}
		richTextAreas = new GameObject[textAreaCount];

		for (int i = 0; i < textAreaCount; i++)
		{
			GameObject newArea = Instantiate(questionTextPrefab) as GameObject;
			ProblemPart problemPart = problemToDisplay.GetQuestion().GetQuestionPart(i);

			if (newArea.GetComponent<RectTransform>() == null || 
			    newArea.GetComponent<Text>() == null )
				throw new UnityException("richTextArea must have a rect transform and Text");

			if (problemPart.HasString())
			{
				newArea.GetComponent<Text>().text += problemPart.GetString();
			}

			float indentedX = 0f;
			if (previousAreaWasMultipleChoice)
				indentedX = indentMultipleChoices;

			newArea.GetComponent<RectTransform>().SetParent(questionParent);
			PositionRectTransform(newArea.GetComponent<RectTransform>(),
			                      indentedX,
			                      1 - (row + 1) * ySpacePerArea,
			                      1,
			                      1 - row * ySpacePerArea);

			if (problemPart.HasSprite())
			{
				//squish this a little bit to provide space between image groups
				newArea.GetComponent<RectTransform>().anchorMin += new Vector2(0f, spaceImageAreas);
				newArea.GetComponent<RectTransform>().anchorMax -= new Vector2(0f, spaceImageAreas);


				for (int j = 0; j < problemPart.TimesRepeated(); j++)
				{
					GameObject newImage = Instantiate(questionImagePrefab) as GameObject;
					newImage.GetComponent<RectTransform>().SetParent(newArea.transform);
					int imageColumn = j % imagesPerRow;
					int imageRow = Mathf.FloorToInt((float)j / (float)imagesPerRow);
					PositionRectTransform(newImage.GetComponent<RectTransform>(),
					                      (imageColumn * xSpacePerImage),
					                      1 - (imageRow + 1) * ySpacePerImage,
					                      ((imageColumn + 1) * xSpacePerImage),
					                      1 - imageRow * ySpacePerImage);

					newImage.GetComponent<Image>().sprite = problemPart.GetSprite();
				}
			}

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

	private void PositionRectTransform(RectTransform positionMe, float minX, float minY, float maxX, float maxY)
	{
		positionMe.anchorMin = new Vector2(minX,
		                                   minY);
		positionMe.anchorMax = new Vector2(maxX,
		                                   maxY);
		positionMe.offsetMin = new Vector2(0f, 0f);
		positionMe.offsetMax = new Vector2(0f, 0f);
		positionMe.localScale = new Vector3(1f, 1f, 1f);
	}
}