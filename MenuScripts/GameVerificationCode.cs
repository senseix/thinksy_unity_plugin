using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Senseix
{

	public class GameVerificationCode : MonoBehaviour {

		public Text codeText;

		// Use this for initialization
		void Start () 
		{
			//ResetCode ();
		}

		void OnEnable()
		{
			ResetCode ();
		}

		public string GetCode()
		{
			//ResetCode ();
			return codeText.text;
		}

		// Update is called once per frame
		void Update () 
		{
			
		}

		public void ResetCode()
		{
			string verificationCode = GetRandomSixDigitHexNumber();
			codeText.text = verificationCode;
			StartCoroutine(SenseixSession.VerifyGame (verificationCode));
		}
		
		public string GetRandomSixDigitHexNumber()
		{
			System.Random random = new System.Random();
			int num = random.Next(1048576, 10066329);
			string hexString = num.ToString("X");
			return hexString;
		}
	}
}