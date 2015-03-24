using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ThinksyQuestionDisplay : MonoBehaviour 
{
	public GameObject richTextArea;
	public GameObject imageArea;
	public AdvancementAnimationPlayer advacementAnimation;
	public float indentMultipleChoices = 0.2f;

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
		{
			Debug.LogWarning("I got a problem with no question atoms.");
			return;//throw new UnityException ("I got a problem with no question atoms.");
		}
		richTextAreas = new GameObject[textAreaCount];

		for (int i = 0; i < textAreaCount; i++)
		{
			GameObject newArea = Instantiate(richTextArea) as GameObject;
			ProblemPart problemPart = problemToDisplay.GetQuestion().GetQuestionPart(i);

			if (newArea.GetComponent<RectTransform>() == null || 
			    newArea.GetComponent<Text>() == null )
				throw new UnityException("richTextArea must have a rect transform and Text");

			if (problemPart.IsString())
			{
				newArea.GetComponent<Text>().text += problemPart.GetString();
			}

			float indentedX = 0f;
			if (previousAreaWasMultipleChoice)
				indentedX = indentMultipleChoices;

			newArea.GetComponent<RectTransform>().SetParent(gameObject.transform);
			newArea.GetComponent<RectTransform>().anchorMin = new Vector2(indentedX,
			                                                             1 - (row + 1) * ySpacePerArea);
			newArea.GetComponent<RectTransform>().anchorMax = new Vector2(1,
			                                                              1 - row * ySpacePerArea);
			newArea.GetComponent<RectTransform>().offsetMin = new Vector2(0f, 0f);
			newArea.GetComponent<RectTransform>().offsetMax = new Vector2(0f, 0f);
			newArea.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);

			if (problemPart.IsImage())
			{
				float xSpacePerImage = 1/problemPart.TimesRepeated();
				for (int j = 0; j < problemPart.TimesRepeated(); j++)
				{
					GameObject newImage = Instantiate(imageArea) as GameObject;
					newImage.GetComponent<RectTransform>().SetParent(newArea.transform);
					newImage.GetComponent<RectTransform>().anchorMin = new Vector2(j * xSpacePerImage,
					                                                              0);
					newImage.GetComponent<RectTransform>().anchorMax = new Vector2((j + 1) * xSpacePerImage,
					                                                              1);
					newImage.GetComponent<RectTransform>().offsetMin = new Vector2(0f, 0f);
					newImage.GetComponent<RectTransform>().offsetMax = new Vector2(0f, 0f);
					newImage.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);

					Texture2D partImage = problemPart.GetImage();
					//Debug.Log (newImage == null);
					Sprite newSprite = Sprite.Create(partImage, 
					                                 new Rect(0f, 0f, partImage.width, partImage.height),
					                                 new Vector2(0.5f, 0.5f));
					newImage.GetComponent<Image>().sprite = newSprite;
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
}