using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Threading;
using System.ComponentModel;

namespace Senseix { 

	class SenseixController:MonoBehaviour {
		private static bool problemThreadActive = false;
		private static bool inSession = false;
		private static volatile string accessToken = "";
		private static volatile string deviceID = SystemInfo.deviceUniqueIdentifier;
		private static ProblemWorker problemWorker; 
		private static Thread problemWorkerThread;
		public static volatile string playerID;
		public static volatile string authToken; 
		public const int ACCESS_TOKEN_LENGTH = 64;
		public string developerAccessToken;
		private Message.Request request = new Message.Request();


		static public bool GetSessionState()
		{
			return inSession;
		}
		static public void SetSessionState(bool state)
		{
			problemWorker.SetOnline (state);
			inSession = state;
		}

		static public void SetAndSaveAuthToken(string newAuthToken) 
		{
			authToken = newAuthToken;
		}
		static private int CheckAccessToken()
		{
			if (accessToken.Length != ACCESS_TOKEN_LENGTH) 
				return -1;
			return 0;
		}
		static public string GetAccessToken()
		{
			return accessToken;
		}
		static public string GetDeviceID()
		{
			return deviceID;
		}
		static public string GetAuthToken()
		{
			return authToken;
				}
		static public string GetPlayerID()
		{
			return playerID;
		}
		static public void SetPlayerID(string newPlayerID)
		{
			playerID = newPlayerID;
		}
		~SenseixController(){
			problemWorker.RequestStop ();
			problemWorkerThread.Join();
		}
		public SenseixController() { 
	
		}
		void Start () { 
			if (startProblemWorkerThread () < 0) {
				throw new Exception ("Failed to start the problem worker thread, Please uninstall and reinstall this game");
			}

			
			accessToken = developerAccessToken;
			if (CheckAccessToken() == -1) {
				throw new Exception("The Senseix Token you have provided is not of a valid length, please register at developer.senseix.com to create a valid key");
			}

			//Creates a temporary account based on device id
			//returns an auth token. This is Syncronous.
			request.RegisterDevice();
		}
		static private int startProblemWorkerThread()
		{
			problemWorker = new ProblemWorker();
			problemWorkerThread = new Thread(problemWorker.DoWork);
			problemWorkerThread.Start();
			while (!problemWorkerThread.IsAlive);
			return 1; 
		}
		static public Senseix.Message.Problem.ProblemData.Builder PullProblem()
		{
			return problemWorker.GetProblem ();
		}

		static public bool CheckAnswer(Message.Problem.ProblemData.Builder problem, string answer)
		{
			return problemWorker.CheckAnswer(problem, answer);
		}

		void Update()
		{

		}
	}
}