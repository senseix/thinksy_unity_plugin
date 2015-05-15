using UnityEngine;
using System.Collections;

public class ParentGateQuestion : MonoBehaviour 
{
	public UnityEngine.UI.Text multiplicationQuestionText;
	public UnityEngine.UI.Text continueButtonText;
	public UnityEngine.UI.InputField answerInput;
	public GameObject thisMenu;
	public GameObject nextMenu;

	private int firstNumber;
	private int secondNumber;

	void OnEnable ()
	{
		firstNumber = UnityEngine.Random.Range (0, 13);
		secondNumber = UnityEngine.Random.Range (0, 13);
		multiplicationQuestionText.text = "What is " + firstNumber + " times " + secondNumber + "?";
		continueButtonText.text = "I am an adult, continue.";
	}

	public void ContinuePressed ()
	{
		int answerNumber = -1;
		try
		{
			answerNumber = System.Convert.ToInt16(answerInput.text);
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

		thisMenu.SetActive (false);
		nextMenu.SetActive (true);
	}
}