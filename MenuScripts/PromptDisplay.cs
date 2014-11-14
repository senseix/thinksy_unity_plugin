using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace senseix
{
	public class PromptDisplay : MonoBehaviour 
	{
		public static PromptDisplay singletonInstance;

		public UnityEngine.UI.Image promptDisplay;


		void Awake()
		{
			singletonInstance = this;
		}

		public void UpdateDisplay()
		{
			Problem currentProblem = SenseixPlugin.GetMostRecentProblem ();
			string currentProblemHTML = currentProblem.GetPromptHTML ();

			promptDisplay.material = new Material ("");
		}
	}
}