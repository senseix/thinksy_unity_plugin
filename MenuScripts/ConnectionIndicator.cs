using UnityEngine;
using System.Collections;

namespace sensei
{

	public class ConnectionIndicator : MonoBehaviour {

		public GameObject warningText;

		// Use this for initialization
		void Start () {
			if (SenseixPlugin.IsInOfflineMode())
			{
				warningText.GetComponentInChildren<UnityEngine.UI.Text>().text = "Offline mode";
			}
		}
		
		// Update is called once per frame
		void Update () {
			bool indicateOffline = !Senseix.SenseixSession.GetSessionState ();
			//if (indicateOffline)
			//	Debug.Log ("Showing offline indicator");
			warningText.SetActive (indicateOffline);
		}
	}
}
