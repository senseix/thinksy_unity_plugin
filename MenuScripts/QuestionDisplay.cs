using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace senseix
{
	public class QuestionDisplay : MonoBehaviour 
	{
		public static QuestionDisplay singletonInstance;

		public UnityEngine.UI.Image promptDisplay;
		public UnityEngine.UI.Text promptText;

		void Awake()
		{
			singletonInstance = this;
		}

		public void UpdateDisplay()
		{
			promptText.text = SenseixPlugin.GetMostRecentProblemHTML();
		}
	}
}