using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace senseix
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

		public void UpdateDisplay()
		{
			UpdateQuestionText ();
			UpdateQuestionImage ();
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