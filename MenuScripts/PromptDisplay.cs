using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace senseix
{
	public class PromptDisplay : MonoBehaviour 
	{
		public static PromptDisplay singletonInstance;

		public Text[] textDisplays;
		public Image[] imageDisplays;


		void Awake()
		{
			singletonInstance = this;
		}

		public void UpdateDisplay()
		{
			Problem currentProblem = SenseixPlugin.GetMostRecentProblem ();

			int textsSoFar = 0;
			int imagesSoFar = 0;

			foreach(ProblemPart part in currentProblem.GetPrompt())
			{
				if (part.IsString())
				{
					textDisplays[textsSoFar].text = "";
					textDisplays[textsSoFar].text += part.GetString();
					textsSoFar++;
				}
				if (part.IsImage())
				{
					Debug.Log("I found an image!");
					//Texture2D thisImage = part.GetImage();
					//GUI.DrawTexture(new Rect(0, 0, 100f, 100f), thisImage);
				}
			}
		}
	}
}