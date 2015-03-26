using UnityEngine;
using System.Collections;

namespace Senseix
{

	public abstract class Encouragement : MonoBehaviour 
	{
		public abstract void Display(Senseix.Message.Encouragement.EncouragementData encouragementData);
	}

}