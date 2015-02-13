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
		static public bool ParseResponse(byte[] responseBytes)
		{

			/*	
				case Constant.MessageType.RegisterParent:
					if (reply.Status == Constant.Status.FAILURE) 
					{
						UnityEngine.Debug.Log ("Register parent failed.");
					}
					if(reply.HasParentRegistration && reply.ParentRegistration.IsInitialized) 
					{
						SenseixSession.SetAndSaveAuthToken(reply.ParentRegistration.AuthToken);
					} 
					else 
					{	
						SenseixSession.SetSessionState(false);
						UnityEngine.Debug.Log("Can't find key from result");
						return false;
					}
					break;

				case Constant.MessageType.SignInParent:
					if (reply.Status == Constant.Status.FAILURE) 
					{
						throw new Exception ("We encountered a fatal failure on sign in.");
					}
					if(reply.HasParentSignIn && reply.ParentSignIn.IsInitialized) 
					{
						SenseixSession.SetAndSaveAuthToken(reply.ParentSignIn.AuthToken);
					} 
					else 
					{
						SenseixSession.SetSessionState(false);
						UnityEngine.Debug.Log("Can't find key from result");
						return false;
					}
					break;

				case Constant.MessageType.SignOutParent:
					SenseixSession.SetSessionState(false);//Duane, this seems..odd
					break;

				case Constant.MessageType.MergeParent:
					if(reply.HasParentMerge && reply.ParentMerge.IsInitialized && reply.ParentMerge.HasAuthToken)
					{
						SenseixSession.SetAndSaveAuthToken(reply.ParentRegistration.AuthToken);
					} 
					else 
					{	
						SenseixSession.SetSessionState(false);
						UnityEngine.Debug.Log("Can't find key from result");
						return false;
					}
					break;

				case Constant.MessageType.CreatePlayer:
					SenseixSession.SetSessionState(true);
//					Debug.Log("I got a response from a create Player Message");
					break;





				case Constant.MessageType.LeaderboardPage:
//					Debug.Log ("I recieved a Leaderboard page response");
					UnityEngine.Debug.Log(reply.Page.PlayerList);
					SenseixSession.SetLeaderboardPlayers(reply.Page.PlayerList);
					break;

				case Constant.MessageType.PlayerScore:
					SenseixSession.SetSessionState(true);
//					Debug.Log("I got a response from a Player score Message");
					break;

				case Constant.MessageType.PlayerRank:
					SenseixSession.SetSessionState(true);
//					Debug.Log("I got a response from a Player rank Message");
					break;

				case Senseix.Message.Constant.MessageType.EncouragementGet:
					SenseixSession.SetSessionState(true);
					//Debug.Log("I got a response from an encouragement get Message");
					break;



				default:
					throw new Exception("Response.cs recieved a MessageType that it didn't recognize.");
			*/
			return true;
		}

		static public bool ParseRegisterDeviceResponse(byte[] responseBytes)
		{
			MemoryStream stream = new MemoryStream (responseBytes);
			Device.DeviceRegistrationResponse registerDeviceResponse = 
				ProtoBuf.Serializer.Deserialize<Device.DeviceRegistrationResponse> (stream);

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
			MemoryStream stream = new MemoryStream (responseBytes);
			Player.PlayerListResponse listPlayersResponse = ProtoBuf.Serializer.Deserialize<Player.PlayerListResponse>(stream);

			SenseixSession.SetSessionState(true);
			Logger.BasicLog("I got a response from a list Player Message");
			if (listPlayersResponse.player.Count == 0)
				throw new Exception ("no players in player list");
			SenseixSession.SetCurrentPlayerList(listPlayersResponse);

			return true;
		}

		static public bool ParseRegisterPlayerResponse(byte[] responseBytes)
		{
			MemoryStream stream = new MemoryStream (responseBytes);
			Player.PlayerRegisterWithApplicationResponse registerPlayerResponse = ProtoBuf.Serializer.Deserialize<Player.PlayerRegisterWithApplicationResponse>(stream);
			SenseixSession.SetSessionState(true);
			Logger.BasicLog("I got a response from a register Player Message");

			return true;
		}
		static public bool ParseGetProblemResponse(byte[] responseBytes)
		{
			MemoryStream stream = new MemoryStream (responseBytes);
			Problem.ProblemGetResponse getProblemResponse = 
				ProtoBuf.Serializer.Deserialize<Problem.ProblemGetResponse>(stream);
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
			MemoryStream stream = new MemoryStream (responseBytes);
			Problem.ProblemPostResponse postProblemResponse = 
				ProtoBuf.Serializer.Deserialize<Problem.ProblemPostResponse>(stream);

			SenseixSession.SetSessionState(true);
			Logger.BasicLog("I got a response from a Problem post Message");
			return true;
		}

		static public bool ParseVerifyGameResponse(byte[] responseBytes)
		{
			MemoryStream stream = new MemoryStream (responseBytes);
			Device.GameVerificationResponse verifyGameResponse = 
				ProtoBuf.Serializer.Deserialize<Device.GameVerificationResponse>(stream);

			SenseixSession.SetSessionState(true);
			Logger.BasicLog("I got a response from my game verification Message");
			return true;
		}

		static public bool ParseReportBugResponse(byte[] responseBytes)
		{
			MemoryStream stream = new MemoryStream (responseBytes);
			Debug.DebugLogSubmitResponse verifyGameResponse = 
				ProtoBuf.Serializer.Deserialize<Debug.DebugLogSubmitResponse>(stream);

			SenseixSession.SetSessionState(true);
			Logger.BasicLog("I got a response from a debug log submit message.");
			return true;
		}


		static public bool ParsePlayerScoreResponse(byte[] responseBytes)
		{
			MemoryStream stream = new MemoryStream (responseBytes);
			Leaderboard.UpdatePlayerScoreResponse verifyGameResponse = 
				ProtoBuf.Serializer.Deserialize<Leaderboard.UpdatePlayerScoreResponse>(stream);

			SenseixSession.SetSessionState(true);
			Logger.BasicLog("I got a response from a Player score Message");
			return true;
		}
		
		static public bool ParsePlayerRankResponse(byte[] responseBytes)
		{
			MemoryStream stream = new MemoryStream (responseBytes);
			Leaderboard.PlayerRankResponse verifyGameResponse = 
				ProtoBuf.Serializer.Deserialize<Leaderboard.PlayerRankResponse>(stream);

			SenseixSession.SetSessionState(true);
			Logger.BasicLog("I got a response from a Player rank Message");
			return true;
		}

		static public bool ParseGetEncouragementsResponse(byte[] responseBytes)
		{
			MemoryStream stream = new MemoryStream (responseBytes);
			Encouragement.EncouragementGetResponse getEncouragementResponse = 
				ProtoBuf.Serializer.Deserialize<Encouragement.EncouragementGetResponse>(stream);
			
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
				MemoryStream stream = new MemoryStream(responseBytes);
				Debug.ServerErrorResponse serverErrorResponse = 
					ProtoBuf.Serializer.Deserialize<Debug.ServerErrorResponse>(stream);

				UnityEngine.Debug.LogError("I got a server error response.  Here is the message: " +
				                      serverErrorResponse.message);
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogWarning("Error while parsing error.  Um.");
			}
			return false;
		}
	}
}

