using UnityEngine;
using System.Collections;

namespace Senseix
{
	public class TextEncouragement : TimedEncouragement 
	{
		public override void Display(Senseix.Message.Encouragement.EncouragementData encouragementData)
		{
			base.Display (encouragementData);
			if (encouragementData.data != null)
				GetComponent<UnityEngine.UI.Text> ().text += encouragementData.data;
			else
				UnityEngine.Debug.LogWarning("We are trying to display a text encouragement, but it doesn't have data");
		}

		void OnDisable()
		{
			GetComponent<UnityEngine.UI.Text> ().text = "";
		}
	}
}