using UnityEngine;
using System.Collections;

namespace Senseix
{
	public class EncouragementDisplay : MonoBehaviour 
	{
		void OnEnable()
		{
			ThinksyEvents.onEncouragementReceived += EncouragementReceived;
		}

		void OnDisable()
		{
			ThinksyEvents.onEncouragementReceived -= EncouragementReceived;
		}

		private void EncouragementReceived(ProblemPart[] encouragmentParts)
		{
			Debug.Log ("I received an encouragement with " + encouragmentParts.Length + " parts");
		}
	}
}