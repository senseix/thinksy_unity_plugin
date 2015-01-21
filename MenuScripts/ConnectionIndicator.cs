using UnityEngine;
using System.Collections;

namespace sensei
{

	public class ConnectionIndicator : MonoBehaviour {

		public GameObject warningBox;
		public UnityEngine.UI.Text warningText;

		// Use this for initialization
		void Start () 
		{
			if (ThinksyPlugin.IsInOfflineMode())
			{
				warningText.text = "Offline mode";
			}
		}
		
		// Update is called once per frame
		void Update () {


			bool indicateOffline = !Senseix.SenseixSession.GetSessionState ();
			//if (indicateOffline)
			//	Debug.Log ("Showing offline indicator");
			warningBox.SetActive (indicateOffline);
		}
	}
}
