using UnityEngine;
using System.Collections;

namespace Senseix
{
	public class EncouragementDisplay : MonoBehaviour 
	{
		public Encouragement[] encouragementsByType;

		static private EncouragementDisplay singletonInstance;

		void Awake ()
		{
			singletonInstance = this;
		}
		
		public static void DisplayEncouragement(Senseix.Message.Encouragement.EncouragementData encouragementData)
		{
			singletonInstance.encouragementsByType [(int)encouragementData.Type].Display (encouragementData);
		}
	}
}