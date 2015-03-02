using UnityEngine;
using System.Collections;

namespace Senseix
{

	public class ConnectionIndicator : MonoBehaviour {

		public GameObject warningBox;
		public UnityEngine.UI.Text warningText;

		private static ConnectionIndicator singletonInstance;

		private static ConnectionIndicator GetSingletonInstance()
		{
			if (singletonInstance == null)
			{
				singletonInstance = FindObjectOfType<ConnectionIndicator>();
			}
			return singletonInstance;
		}

		// Use this for initialization
		void Start () 
		{
			singletonInstance = this;
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

		public static void SetWaitingIndication(bool show)
		{
			GetSingletonInstance().InstanceSetWaitingIndication (show);
		}

		private void InstanceSetWaitingIndication(bool show)
		{
			warningText.text = "Communicating...";
			warningBox.SetActive (show);
		}
	}
}
