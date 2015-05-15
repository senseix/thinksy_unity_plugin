using UnityEngine;
using System.Collections;

namespace Senseix
{

	public class ConnectionIndicator : MonoBehaviour {

		public GameObject warningBox;
		public UnityEngine.UI.Image warningImage;

		public Sprite offlineSprite;
		public Sprite communicatingSprite;

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
			if (ThinksyPlugin.IsInTestingMode())
			{
				warningImage.sprite = offlineSprite;
				//warningText.text = "Offline mode";
			}
		}
		
		// Update is called once per frame
		void Update () {
			bool indicateOffline = !Senseix.SenseixSession.GetSessionState ();
			//if (indicateOffline)
			//	Debug.Log ("Showing offline indicator");
			warningImage.sprite = offlineSprite;
			//warningText.text = "Not connected";
			warningBox.SetActive (indicateOffline);
		}

		public static void SetWaitingIndication(bool show)
		{
			GetSingletonInstance().InstanceSetWaitingIndication (show);
		}

		private void InstanceSetWaitingIndication(bool show)
		{
			warningImage.sprite = communicatingSprite;
			//warningText.text = "Communicating...";
			warningBox.SetActive (show);
		}
	}
}
