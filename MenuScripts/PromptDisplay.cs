using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace senseix
{
	public class PromptDisplay : MonoBehaviour 
	{
		public Text questionDisplay;

		void Update()
		{
			UpdateDisplay ();
		}

		public void UpdateDisplay()
		{
			Problem currentProblem = SenseixPlugin.GetMostRecentProblem ();
			questionDisplay.text = "";
			foreach(ProblemPart part in currentProblem.GetPrompt())
			{
				if (part.IsString())
				{
					questionDisplay.text += part.GetString();
				}
			}
		}
	}
}