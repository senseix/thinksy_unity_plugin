using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
namespace Senseix.Message 
{
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
			StudentSelection.UpdateStudentSelection ();

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
			UnityEngine.Debug.Log("register player response");

			return true;
		}
		static public bool ParseGetProblemResponse(byte[] responseBytes)
		{
			Problem.ProblemGetResponse getProblemResponse = 
				Deserialize (responseBytes, typeof(Problem.ProblemGetResponse)) as Problem.ProblemGetResponse;
			//Debug.Log("has message: " + reply.HasMessage);
			//Debug.Log("message length: " + reply.Message.Length);
			//Debug.Log("has problem get: " + reply.HasProblemGet);
			if (getProblemResponse.problems.Count != ProblemKeeper.PROBLEMS_PER_PULL)
				Logger.BasicLog("How wude.  I asked for " + ProblemKeeper.PROBLEMS_PER_PULL + " problems, but I only got " + getProblemResponse.problems.Count);
			if (getProblemResponse.problems.Count == 0)
			{
				throw new Exception ("no problems in problem response.");
			}

			ProblemKeeper.ReplaceQueue(getProblemResponse);

			return true;
		}

		static public bool ParseGetSpecifiedProblemResponse(byte[] responseBytes)
		{
			Problem.SpecifiedProblemGetResponse getProblemResponse = 
				Deserialize (responseBytes, typeof(Problem.SpecifiedProblemGetResponse)) as Problem.SpecifiedProblemGetResponse;

			if (getProblemResponse.problems.Count == 0)
			{
				throw new Exception ("no problems in specified problem response.");
			}
			
			global::Problem[] problems = new global::Problem[getProblemResponse.problems.Count];

			for (int i = 0; i < getProblemResponse.problems.Count; i++)
			{
				Senseix.Message.Problem.ProblemData problemData = getProblemResponse.problems[i];
				problems[i] = new global::Problem(problemData);
			}

			ThinksyEvents.InvokeSpecifiedProblemsReceived (problems);
			
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
			
			Logger.BasicLog ("I got an encouragement get response with " + getEncouragementResponse.encouragement_data.Count + " encouragements.");

			foreach (Encouragement.EncouragementData encouragementData in getEncouragementResponse.encouragement_data)
			{
				ProblemPart[] encouragementParts = new ProblemPart[encouragementData.encouragement_atoms.Count];
				for (int i = 0; i < encouragementParts.Length; i++)
				{
					encouragementParts[i] = ProblemPart.CreateProblemPart(encouragementData.encouragement_atoms[i]);
				}
				ThinksyEvents.InvokeEncouragementReceived(encouragementParts);
			}

			ThinksyPlugin.NewHeartbeatTiming (getEncouragementResponse.frames_per_heartbeat);
			if (getEncouragementResponse.force_pull)
				ProblemKeeper.PullNewProblems ();

			return true;
		}

		static public bool ParseListItemsResponse(byte[] responseBytes)
		{
			Player.ListPlayerItemsResponse listItemsResponse = 
				Deserialize (responseBytes, typeof(Player.ListPlayerItemsResponse)) as Player.ListPlayerItemsResponse;
			
			Logger.BasicLog ("I got an items list response with " + listItemsResponse.item_atoms.Count + " items");

			//foreach(Message.Atom.Atom atom in listItemsResponse.item_atoms)
			//{
				//UnityEngine.Debug.Log(atom.filename);
			//}

			ProblemPart[] items = new ProblemPart[listItemsResponse.item_atoms.Count];
			for (int i = 0; i < listItemsResponse.item_atoms.Count; i++)
			{
				items[i] = ProblemPart.CreateProblemPart(listItemsResponse.item_atoms[i]);
			}

			ItemsDisplay.SetItemsToDisplay (items);

			return true;
		}

		static public bool ParseSendParentEmailResponse(byte[] responseBytes)
		{
			Device.SendParentEmailResponse sendEmailResponse =
				Deserialize (responseBytes, typeof(Device.SendParentEmailResponse)) as Device.SendParentEmailResponse;

			if (sendEmailResponse == null)
				throw new Exception ("Parsing the response failed (resulting in null)");
			
			SenseixSession.SetSessionState(true);
			Logger.BasicLog("I got a response from a send parent email message");

			return true;
		}

		static public bool ParseServerErrorResponse(byte[] responseBytes)
		{
			Debug.ServerErrorResponse serverErrorResponse = 
				Deserialize(responseBytes, typeof(Debug.ServerErrorResponse)) as Debug.ServerErrorResponse;
		
			if (serverErrorResponse.message.Length == 0)
			{
				return false;
			}

			string logString = "I got a server error response.  Here is the message: " +
				serverErrorResponse.message;

			if (Request.IsInSecretStagingMode())
			{
				ThinksyPlugin.ShowEmergencyWindow (logString);
			}

			UnityEngine.Debug.LogError(logString);
			return true;
		}

		static private object Deserialize(byte[] responseBytes, Type typeToDeserialize)
		{
			MemoryStream stream = new MemoryStream(responseBytes);
			ThinksyProtosSerializer customSerializer = new ThinksyProtosSerializer ();
			return customSerializer.Deserialize(stream, null, typeToDeserialize);
		}
	}
}