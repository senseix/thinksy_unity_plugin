using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Senseix
{
	public class QuestionDisplay : MonoBehaviour 
	{
		public static QuestionDisplay singletonInstance;

		public UnityEngine.UI.RawImage promptDisplay;
		public UnityEngine.UI.Text promptText;
		public UnityEngine.UI.Text answersSoFarText;

		void Awake()
		{
			singletonInstance = this;
		}

		public static void Update()
		{
			singletonInstance.UpdateDisplay ();
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
			foreach (ProblemPart part in SenseixPlugin.GetMostRecentProblemQuestion())
			{
				if (part.IsString())
					promptText.text = part.GetString();
			}
		}

		private void UpdateQuestionImage()
		{
			if (promptDisplay != null)
				promptDisplay.texture = SenseixPlugin.GetMostRecentProblemImage ();
		}

		private void UpdateAnswersText ()
		{
			answersSoFarText.text = "";
			foreach (ProblemPart part in SenseixPlugin.GetMostRecentGivenAnswer().GetAnswerParts())
			{
				answersSoFarText.text += " " + part.GetString();
			}
		}
	}
}