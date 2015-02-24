using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Senseix
{
	public class QuestionDisplay : MonoBehaviour 
	{
		private static QuestionDisplay singletonInstance;

		public UnityEngine.UI.RawImage promptDisplay;
		public UnityEngine.UI.Text promptText;
		public UnityEngine.UI.Text answersSoFarText;

		void Awake()
		{
			singletonInstance = this;
		}

		void Start()
		{
			QuestionDisplay.Update ();
		}

		public static void Update()
		{
			if (singletonInstance != null)
			{
				singletonInstance.UpdateDisplay ();
			}
		}

		public static void UpdateTextOnly()
		{
			singletonInstance.UpdateText ();
		}

		public void UpdateDisplay()
		{
			UpdateQuestionImage ();
			UpdateText ();
		}

		public void UpdateText()
		{
			UpdateQuestionText ();
			UpdateAnswersText ();
		}

		private void UpdateQuestionText()
		{
			foreach (ProblemPart part in ThinksyPlugin.GetMostRecentProblemQuestion())
			{
				if (part.IsString())
					promptText.text = part.GetString();
			}
		}

		private void UpdateQuestionImage()
		{
			if (promptDisplay != null)
				promptDisplay.texture = ThinksyPlugin.GetMostRecentProblemImage ();
		}

		private void UpdateAnswersText ()
		{
			answersSoFarText.text = "";
			foreach (ProblemPart part in ThinksyPlugin.GetMostRecentGivenAnswer().GetAnswerParts())
			{
				answersSoFarText.text += " " + part.GetString();
			}
		}
	}
}