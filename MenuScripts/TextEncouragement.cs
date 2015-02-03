using UnityEngine;
using System.Collections;

namespace Senseix
{
	public class TextEncouragement : TimedEncouragement 
	{
		public override void Display(Senseix.Message.Encouragement.EncouragementData encouragementData)
		{
			base.Display (encouragementData);
			if (encouragementData.HasData)
				GetComponent<UnityEngine.UI.Text> ().text = encouragementData.Data;
			else
				UnityEngine.Debug.LogWarning("We are trying to display a text encouragement, but it doesn't have data");
		}
	}
}