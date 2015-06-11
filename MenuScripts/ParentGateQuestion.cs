using UnityEngine;
using System.Collections;

public class ParentGateQuestion : MonoBehaviour 
{
	public UnityEngine.UI.Text multiplicationQuestionText;
	public UnityEngine.UI.Text continueButtonText;
	public UnityEngine.UI.Text[] answerTexts;
	public GameObject thisMenu;
	public GameObject nextMenu;

	private int correctTextIndex = -1;
	private int clickedIndex = -1;
	private int firstNumber;
	private int secondNumber;

	void OnEnable ()
	{
		firstNumber = UnityEngine.Random.Range (0, 13);
		secondNumber = UnityEngine.Random.Range (0, 13);
		multiplicationQuestionText.text = "What is " + firstNumber + " times " + secondNumber + "?";
		continueButtonText.text = "I am an adult, continue.";
		clickedIndex = -1;
		correctTextIndex = UnityEngine.Random.Range (0, answerTexts.Length);
		for (int i = 0; i < answerTexts.Length; i++)
		{
			if (i == correctTextIndex)
			{
				answerTexts[i].text = System.Convert.ToString(firstNumber * secondNumber);
			}
			else
			{
				answerTexts[i].text = System.Convert.ToString(UnityEngine.Random.Range(0, 12*12));
			}
		}
	}

	public void AnswerSelected(int index)
	{
		clickedIndex = index;
	}

	public void ContinuePressed()
	{
		if (clickedIndex == -1)
		{
			continueButtonText.text = "Please select an answer above.";
			return;
		}
		AnswerGiven (answerTexts [clickedIndex].text);
	}

	private void AnswerGiven (string answer)
	{
		int answerNumber = -1;
		try
		{
			answerNumber = System.Convert.ToInt16(answer);
		}
		catch
		{
			continueButtonText.text = "Please input an integer";
			return;
		}

		if (answerNumber != firstNumber * secondNumber)
		{
			continueButtonText.text = "That's not right...";
			return;
		}

		if (answerNumber == -1)
		{
			ThinksyPlugin.ShowEmergencyWindow ("An error was encountered during the parent gate question.");
		}

		thisMenu.SetActive (false);
		nextMenu.SetActive (true);
	}
}