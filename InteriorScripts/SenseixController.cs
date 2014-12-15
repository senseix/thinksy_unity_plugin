using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Threading;
using System.ComponentModel;

namespace Senseix { 

	static class SenseixController {
		private static bool inSession = false;
		private static volatile string accessToken = "";
		private static volatile string deviceID;
		private static bool isSignedIn = false;
		public static volatile string authToken; 
		public const int ACCESS_TOKEN_LENGTH = 64;
		private static IList<Message.Leaderboard.PlayerData> currentLeaderboard;
		private static Message.Player.PlayerListResponse currentPlayerList;
		private static Message.Player.Player currentPlayer;

		static public ArrayList GetCurrentPlayerList()
		{
			ArrayList returnList = new ArrayList ();
			if (currentPlayerList == null)
			{
				//Debug.Log("no current Player list.  maybe not connected.");
				return returnList;
			}

			for (int i = 0; i < currentPlayerList.PlayerCount; i++)
			{
				returnList.Add(currentPlayerList.GetPlayer(i));
			}
			return returnList;
		}

		static public void SetCurrentPlayerList(Message.Player.PlayerListResponse newPlayerList)
		{
			currentPlayerList = newPlayerList;
		}

		static private void SetCurrentPlayer(Message.Player.Player newPlayer)
		{
			currentPlayer = newPlayer;
		}

		static public void SelectPlayer(Senseix.Message.Player.Player selectedPlayer)
		{
			SetCurrentPlayer (selectedPlayer);
			RegisterPlayer (selectedPlayer);
			//ProblemKeeper.CreateSeedFileIfNeeded ();
		}

		static public Message.Player.Player GetCurrentPlayer()
		{
			return currentPlayer;
		}

		static public bool GetSessionState()
		{
			return inSession;
		}
		static public void SetSessionState(bool state)
		{
			ProblemKeeper.SetOnline (state);
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
			if (authToken != null)
				return authToken;
			else
				return "you don't need to see my identification";
		}
		static public string GetCurrentPlayerID()
		{
			if (currentPlayer == null)
				return "no current Player";
			return currentPlayer.PlayerId;
		}
		static public void SetToPlayerWithID(string newPlayerID)
		{
			foreach(Message.Player.Player Player in GetCurrentPlayerList())
			{
				if (Player.PlayerId == newPlayerID)
				{
					SetCurrentPlayer(Player);
				}
			}
		}


		public static bool InitializeSenseix (string newAccessToken) { 
			SetSessionState (true);
			deviceID = SystemInfo.deviceUniqueIdentifier;

			accessToken = newAccessToken; 
			if (CheckAccessToken() == -1) {
				throw new Exception("The Senseix Token you have provided is not of a valid length, please register at developer.Senseix.com to create a valid key");
			}

			//Creates a temporary account based on device id
			//returns an auth token. This is Syncronous.
			RegisterDevice ();
//			Debug.Log ("got past register device");
		  	ListPlayers ();
			RegisterAllPlayers ();
			//ProblemKeeper.CreateSeedFileIfNeeded ();
			return GetSessionState ();
		}

		static public void ListPlayers()
		{
			Message.Request.ListPlayers ();
		}

		//this assumes that there is at least one Player always.
		static public void RegisterAllPlayers()
		{
			ArrayList Players = GetCurrentPlayerList ();
			foreach (Message.Player.Player Player in Players)
			{
				RegisterPlayer(Player);
			}
			if (Players.Count > 0) SetCurrentPlayer (Players [0] as Message.Player.Player);
		}

		static public void PullLeaderboard(uint pageNumber, uint pageSize)
		{
			Message.Request.LeaderboardPage (pageNumber, Senseix.Message.Leaderboard.SortBy.NONE, pageSize);
		}

		static public void SetLeaderboardPlayers(IList<Message.Leaderboard.PlayerData> PlayerList)
		{
			foreach (Message.Leaderboard.PlayerData Player in PlayerList)
			{
				Debug.Log("Player " + Player.Name + " has score " + Player.Score);
			}
			currentLeaderboard = PlayerList;
		}

		static public IList<Message.Leaderboard.PlayerData> GetCurrentLeaderboard()
		{
			if (currentLeaderboard == null)
			{
				return new Message.Leaderboard.PlayerData[0];
			}
			return currentLeaderboard;
		}

		static public void RegisterDevice()
		{
			Message.Request.RegisterDevice(SystemInfo.deviceName);
		}
		
		static public void VerifyGame(string verificationCode)
		{
			Message.Request.VerifyGame (verificationCode);
		}

		static private void RegisterPlayer(Message.Player.Player Player)
		{
			Message.Request.RegisterPlayer (Player.PlayerId);
		}


		static public Senseix.Message.Problem.ProblemData.Builder PullProblem()
		{
			return ProblemKeeper.GetProblem ();
		}

		static public bool CheckAnswer(Message.Problem.ProblemData.Builder Problem, Answer answer)
		{
			return ProblemKeeper.CheckAnswer(Problem, answer);
		}

		static public bool SubmitAnswer(Message.Problem.ProblemData.Builder Problem, Answer answer, bool correct)
		{
			return ProblemKeeper.SubmitAnswer(Problem, answer, correct);
		}
	

		public static void UpdateCurrentPlayerScore (UInt32 score)
		{
			Message.Request.UpdatePlayerScore (GetCurrentPlayerID(), score);
		}

		public static void SetSignedIn(bool newIsSignedIn)
		{
			isSignedIn = newIsSignedIn;
		}

		public static bool IsSignedIn()
		{
			return isSignedIn;
		}

		public static void SignOutParent()
		{
			Message.Request.SignOutParent ();
		}

		public static void PushProblems(Queue Problems)
		{
			Message.Request.PostProblems(GetCurrentPlayerID(), Problems);
		}

		public static void GetEncouragements()
		{
			Message.Request.GetEncouragements (GetCurrentPlayerID ());
		}

		//public static byte[] DecodeServerBytes(Google.ProtocolBuffers.ByteString serverBytes)
		//{
		//	string base64string = serverBytes.ToStringUtf8 ();
		//	byte[] decodedBytes = System.Convert.FromBase64String (base64string);
		//	return decodedBytes;
		//}
	}
}