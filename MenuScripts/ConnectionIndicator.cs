using UnityEngine;
using System.Collections;

namespace sensei
{

	public class ConnectionIndicator : MonoBehaviour {

		public GameObject warningText;

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
			warningText.SetActive (!Senseix.SenseixController.GetSessionState ());
		}
	}
}
