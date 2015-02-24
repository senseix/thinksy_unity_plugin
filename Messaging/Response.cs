using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
namespace Senseix.Message {
	public delegate bool ResponseHandlerDelegate(byte[] responseBytes);

	static public class Response
	{

		static public bool ParseRegisterDeviceResponse(byte[] responseBytes)
		{
			Device.DeviceRegistrationResponse registerDeviceResponse = 
				Deserialize (responseBytes, typeof(Device.DeviceRegistrationResponse)) as Device.DeviceRegistrationResponse;


			if (registerDeviceResponse != null)
			{
				//Debug.Log("save auth token.");
				SenseixSession.SetAndSaveAuthToken(registerDeviceResponse.auth_token);
				SenseixSession.SetSessionState(true);
				Logger.BasicLog("Temporary account: " + registerDeviceResponse.is_temporary_account);
				SenseixSession.SetSignedIn(!registerDeviceResponse.is_temporary_account);
			}
			else 
			{
				SenseixSession.SetSessionState(false);
				UnityEngine.Debug.LogWarning("Uninitialized register device response");
				return false;
			}
			return true;
		}

		static public bool ParseListPlayerResponse(byte[] responseBytes)
		{
			Player.PlayerListResponse listPlayersResponse = 
				Deserialize (responseBytes, typeof(Player.PlayerListResponse)) as Player.PlayerListResponse;

			SenseixSession.SetSessionState(true);
			Logger.BasicLog("I got a response from a list Player Message");
			if (listPlayersResponse.player.Count == 0)
				throw new Exception ("no players in player list");
			SenseixSession.SetCurrentPlayerList(listPlayersResponse);

			return true;
		}

		static public bool ParseRegisterPlayerResponse(byte[] responseBytes)
		{
			Player.PlayerRegisterWithApplicationResponse registerPlayerResponse = 
				Deserialize (responseBytes, typeof(Player.PlayerRegisterWithApplicationResponse)) as Player.PlayerRegisterWithApplicationResponse;
			if (registerPlayerResponse == null)
				throw new Exception ("Player response is null");
			SenseixSession.SetSessionState(true);
			Logger.BasicLog("I got a response from a register Player Message");

			return true;
		}
		static public bool ParseGetProblemResponse(byte[] responseBytes)
		{
			Problem.ProblemGetResponse getProblemResponse = 
				Deserialize (responseBytes, typeof(Problem.ProblemGetResponse)) as Problem.ProblemGetResponse;
			//Debug.Log("has message: " + reply.HasMessage);
			//Debug.Log("message length: " + reply.Message.Length);
			//Debug.Log("has problem get: " + reply.HasProblemGet);
			if (getProblemResponse.problem.Count != ProblemKeeper.PROBLEMS_PER_PULL)
				Logger.BasicLog("How wude.  I asked for " + ProblemKeeper.PROBLEMS_PER_PULL + " problems, but I only got " + getProblemResponse.problem.Count);
			if (getProblemResponse.problem.Count == 0)
			{
				throw new Exception ("no problems in problem response.");
			}

			ProblemKeeper.ReplaceSeed(getProblemResponse);

			return true;
		}

		static public bool ParsePostProblemResponse(byte[] responseBytes)
		{
			Problem.ProblemPostResponse postProblemResponse = 
				Deserialize (responseBytes, typeof(Problem.ProblemPostResponse)) as Problem.ProblemPostResponse;

			if (postProblemResponse == null)
					throw new Exception ("Deserializing the response failed (resulted in null)");

			SenseixSession.SetSessionState(true);
			Logger.BasicLog("I got a response from a Problem post Message");
			return true;
		}

		static public bool ParseVerifyGameResponse(byte[] responseBytes)
		{
			Device.GameVerificationResponse verifyGameResponse = 
				Deserialize (responseBytes, typeof(Device.GameVerificationResponse)) as Device.GameVerificationResponse;

			if (verifyGameResponse == null)
				throw new Exception ("Parsing the response failed (resulting in null)");

			SenseixSession.SetSessionState(true);
			Logger.BasicLog("I got a response from my game verification Message");
			return true;
		}

		static public bool ParseReportBugResponse(byte[] responseBytes)
		{
			Debug.DebugLogSubmitResponse bugReportResponse = 
				Deserialize (responseBytes, typeof(Debug.DebugLogSubmitResponse)) as Debug.DebugLogSubmitResponse;

			if (bugReportResponse == null)
				throw new Exception ("Parsing the response failed (resulting in null)");

			SenseixSession.SetSessionState(true);
			Logger.BasicLog("I got a response from a debug log submit message.");
			return true;
		}


		static public bool ParsePlayerScoreResponse(byte[] responseBytes)
		{
			Leaderboard.UpdatePlayerScoreResponse playerScoreResponse = 
				Deserialize (responseBytes, typeof(Leaderboard.UpdatePlayerScoreResponse)) as Leaderboard.UpdatePlayerScoreResponse;

			if (playerScoreResponse == null)
				throw new Exception ("Parsing the response failed (resulting in null)");

			SenseixSession.SetSessionState(true);
			Logger.BasicLog("I got a response from a Player score Message");
			return true;
		}
		
		static public bool ParsePlayerRankResponse(byte[] responseBytes)
		{
			Leaderboard.PlayerRankResponse playerRankResponse = 
				Deserialize(responseBytes, typeof(Leaderboard.PlayerRankResponse)) as Leaderboard.PlayerRankResponse;

			if (playerRankResponse == null)
				throw new Exception ("Parsing the response failed (resulting in null)");

			SenseixSession.SetSessionState(true);
			Logger.BasicLog("I got a response from a Player rank Message");
			return true;
		}

		static public bool ParseGetEncouragementsResponse(byte[] responseBytes)
		{
			Encouragement.EncouragementGetResponse getEncouragementResponse = 
				Deserialize (responseBytes, typeof(Encouragement.EncouragementGetResponse)) as Encouragement.EncouragementGetResponse;
			
			Logger.BasicLog ("I got an encouragement get response with " + getEncouragementResponse.encouragementData.Count + " encouragements.");

			foreach (Encouragement.EncouragementData encouragementData in getEncouragementResponse.encouragementData)
			{
				EncouragementDisplay.DisplayEncouragement(encouragementData);
			}

			return true;
		}

		static public bool ParseServerErrorResponse(byte[] responseBytes)
		{
			try
			{
				Debug.ServerErrorResponse serverErrorResponse = 
					Deserialize(responseBytes, typeof(Debug.ServerErrorResponse)) as Debug.ServerErrorResponse;

				UnityEngine.Debug.LogError("I got a server error response.  Here is the message: " +
				                      serverErrorResponse.message);
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogWarning("Error while parsing error.  Um.  (" + e.Message + ")");
			}
			return false;
		}

		static private object Deserialize(byte[] responseBytes, Type typeToDeserialize)
		{
			MemoryStream stream = new MemoryStream(responseBytes);
			ThinksyProtosSerializer customSerializer = new ThinksyProtosSerializer ();
			return customSerializer.Deserialize(stream, null, typeToDeserialize);
		}
	}
}

