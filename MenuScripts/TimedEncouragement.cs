using UnityEngine;
using System.Collections;

namespace Senseix
{
	public class TimedEncouragement : Encouragement 
	{
		public float displayTime;

		private float startTime;

		void OnEnable()
		{
			startTime = Time.timeSinceLevelLoad;
		}

		void Update () 
		{
			if (Time.timeSinceLevelLoad - displayTime > startTime)
			{
				gameObject.SetActive(false);
			}
		}

		public override void Display(Senseix.Message.Encouragement.EncouragementData encouragementData)
		{
			gameObject.SetActive (true);
		}
	}
}