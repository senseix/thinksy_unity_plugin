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
		private static bool problemThreadActive = false;
		private static bool inSession = false;
		private static volatile string accessToken = "";
		private static volatile string deviceID;
		private static bool isSignedIn = false;
		public static volatile string playerID;
		public static volatile string authToken; 
		public const int ACCESS_TOKEN_LENGTH = 64;
		private static Message.Request request = new Message.Request();
		private static IList<Message.Leaderboard.PlayerData> currentLeaderboard;
		private static Message.Player.PlayerListResponse currentPlayerList;

		static public ArrayList GetCurrentPlayerList()
		{
			ArrayList returnList = new ArrayList ();
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

		static public bool GetSessionState()
		{
			return inSession;
		}
		static public void SetSessionState(bool state)
		{
			ProblemWorker.SetOnline (state);
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

		public static void EndLife(){
		}


		public static void InitializeSenseix (string newAccessToken) { 
			deviceID = SystemInfo.deviceUniqueIdentifier;

			accessToken = newAccessToken; 
			if (CheckAccessToken() == -1) {
				throw new Exception("The Senseix Token you have provided is not of a valid length, please register at developer.senseix.com to create a valid key");
			}

			//VerifyGame ("123456");
			//Creates a temporary account based on device id
			//returns an auth token. This is Syncronous.
			RegisterDevice ();
		
			Debug.Log ("got past register device");
		  ListPlayers ();

		}

		static public void ListPlayers()
		{
			request.ListPlayers ();
		}

		static public void RegisterAllPlayers()
		{
			ArrayList players = GetCurrentPlayerList ();
			foreach (Message.Player.Player player in players)
			{
				RegisterPlayer(player);
			}
		}

		static public void PullLeaderboard(uint pageNumber, uint pageSize)
		{
			request.LeaderboardPage (pageNumber, Senseix.Message.Leaderboard.SortBy.NONE, pageSize);
		}

		static public void SetLeaderboardPlayers(IList<Message.Leaderboard.PlayerData> playerList)
		{
			foreach (Message.Leaderboard.PlayerData player in playerList)
			{
				Debug.Log("player " + player.Name + " has score " + player.Score);
			}
			currentLeaderboard = playerList;
		}

		static public IList<Message.Leaderboard.PlayerData> GetCurrentLeaderboard()
		{
			return currentLeaderboard;
		}

		static public void RegisterDevice()
		{
			request.RegisterDevice(SystemInfo.deviceName);
		}
		
		static public void VerifyGame(string verificationCode)
		{
			request.VerifyGame (verificationCode);
		}

		static public void RegisterPlayer(Message.Player.Player player)
		{
			request.RegisterPlayer (player.PlayerId);
		}


		static public Senseix.Message.Problem.ProblemData.Builder PullProblem()
		{
			return ProblemWorker.GetProblem ();
		}

		static public bool CheckAnswer(Message.Problem.ProblemData.Builder problem, string answer)
		{
			return ProblemWorker.CheckAnswer(problem, answer);
		}

		public static bool IsSignedIn()
		{
			return isSignedIn;
		}
	}
}
