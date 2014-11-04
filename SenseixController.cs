using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Threading;
using System.ComponentModel;

namespace senseix { 

	static class SenseixController {
		private static bool problemThreadActive = false;
		private static bool inSession = false;
		private static volatile string accessToken = "";
		private static volatile string deviceID;
		private static bool isSignedIn = false;
		public static volatile string authToken; 
		public const int ACCESS_TOKEN_LENGTH = 64;
		private static IList<message.leaderboard.PlayerData> currentLeaderboard;
		private static message.player.PlayerListResponse currentPlayerList;
		private static message.player.Player currentPlayer;

		static public ArrayList GetCurrentPlayerList()
		{
			ArrayList returnList = new ArrayList ();
			for (int i = 0; i < currentPlayerList.PlayerCount; i++)
			{
				returnList.Add(currentPlayerList.GetPlayer(i));
			}
			return returnList;
		}

		static public void SetCurrentPlayerList(message.player.PlayerListResponse newPlayerList)
		{
			currentPlayerList = newPlayerList;
		}

		static public void SetCurrentPlayer(message.player.Player newPlayer)
		{
			currentPlayer = newPlayer;
		}

		static public message.player.Player GetCurrentPlayer()
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
			return authToken;
		}
		static public string GetCurrentPlayerID()
		{
			return currentPlayer.PlayerId;
		}
		static public void SetToPlayerWithID(string newPlayerID)
		{
			foreach(message.player.Player player in GetCurrentPlayerList())
			{
				if (player.PlayerId == newPlayerID)
				{
					SetCurrentPlayer(player);
				}
			}
		}

		public static void EndLife(){
		}


		public static void InitializeSenseix (string newAccessToken) { 
			deviceID = SystemInfo.deviceUniqueIdentifier;

			accessToken = newAccessToken; 
			if (CheckAccessToken() == -1) {
				throw new Exception("The Senseix Token you have provided is not of a valid length, please register at developer.senseix.com to create a valid key");
			}

			//Creates a temporary account based on device id
			//returns an auth token. This is Syncronous.
			RegisterDevice ();
			Debug.Log ("got past register device");
		  	ListPlayers ();
			RegisterAllPlayers ();

			if(!ProblemKeeper.SeedFileExists())
			{
				ProblemKeeper.CreateEmptySeedFile();
			}
		}

		static public void ListPlayers()
		{
			message.Request.ListPlayers ();
		}

		//this assumes that there is at least one player always.
		static public void RegisterAllPlayers()
		{
			ArrayList players = GetCurrentPlayerList ();
			foreach (message.player.Player player in players)
			{
				RegisterPlayer(player);
			}
			SetCurrentPlayer (players [0] as message.player.Player);
		}

		static public void PullLeaderboard(uint pageNumber, uint pageSize)
		{
			message.Request.LeaderboardPage (pageNumber, senseix.message.leaderboard.SortBy.NONE, pageSize);
		}

		static public void SetLeaderboardPlayers(IList<message.leaderboard.PlayerData> playerList)
		{
			foreach (message.leaderboard.PlayerData player in playerList)
			{
				Debug.Log("player " + player.Name + " has score " + player.Score);
			}
			currentLeaderboard = playerList;
		}

		static public IList<message.leaderboard.PlayerData> GetCurrentLeaderboard()
		{
			return currentLeaderboard;
		}

		static public void RegisterDevice()
		{
			message.Request.RegisterDevice(SystemInfo.deviceName);
		}
		
		static public void VerifyGame(string verificationCode)
		{
			message.Request.VerifyGame (verificationCode);
		}

		static public void RegisterPlayer(message.player.Player player)
		{
			message.Request.RegisterPlayer (player.PlayerId);
		}


		static public senseix.message.problem.ProblemData.Builder PullProblem()
		{
			return ProblemKeeper.GetProblem ();
		}

		static public bool CheckAnswer(message.problem.ProblemData.Builder problem, string answer)
		{
			return ProblemKeeper.CheckAnswer(problem, answer);
		}
	

		public static void UpdateCurrentPlayerScore (UInt32 score)
		{
			message.Request.UpdatePlayerScore (GetCurrentPlayerID(), score);
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
			message.Request.SignOutParent ();
		}

		public static void PushProblems(Queue problems)
		{
			message.Request.PostProblems(GetCurrentPlayerID(), problems);
		}
	}
}
