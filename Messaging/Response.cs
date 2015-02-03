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
			Device.DeviceRegistrationResponse registerDeviceResponse = 
				Device.DeviceRegistrationResponse.ParseFrom (responseBytes);

			if (registerDeviceResponse.IsInitialized) 
			{	
				//Debug.Log("save auth token.");
				SenseixSession.SetAndSaveAuthToken(registerDeviceResponse.AuthToken);
				SenseixSession.SetSessionState(true);
				Logger.BasicLog("Temporary account: " + registerDeviceResponse.IsTemporaryAccount);
				SenseixSession.SetSignedIn(!registerDeviceResponse.IsTemporaryAccount);
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
				Player.PlayerListResponse.ParseFrom (responseBytes);

			SenseixSession.SetSessionState(true);
			Logger.BasicLog("I got a response from a list Player Message");
			if (listPlayersResponse.PlayerCount == 0)
				throw new Exception ("no players in player list");
			SenseixSession.SetCurrentPlayerList(listPlayersResponse);

			return true;
		}

		static public bool ParseRegisterPlayerResponse(byte[] responseBytes)
		{
			Player.PlayerRegisterWithApplicationResponse registerPlayerResponse = 
				Player.PlayerRegisterWithApplicationResponse.ParseFrom (responseBytes);
			SenseixSession.SetSessionState(true);
			Logger.BasicLog("I got a response from a register Player Message");

			return true;
		}
		static public bool ParseGetProblemResponse(byte[] responseBytes)
		{
			Problem.ProblemGetResponse getProblemResponse = 
				Problem.ProblemGetResponse.ParseFrom (responseBytes);
			//Debug.Log("has message: " + reply.HasMessage);
			//Debug.Log("message length: " + reply.Message.Length);
			//Debug.Log("has problem get: " + reply.HasProblemGet);
			if (getProblemResponse.ProblemCount != ProblemKeeper.PROBLEMS_PER_PULL)
				Logger.BasicLog("How wude.  I asked for " + ProblemKeeper.PROBLEMS_PER_PULL + " problems, but I only got " + getProblemResponse.ProblemCount);
			if (getProblemResponse.ProblemCount == 0)
			{
				throw new Exception ("no problems in problem response.");
			}

			ProblemKeeper.ReplaceSeed(getProblemResponse);

			return true;
		}

		static public bool ParsePostProblemResponse(byte[] responseBytes)
		{
			Problem.ProblemPostResponse postProblemResponse = 
				Problem.ProblemPostResponse.ParseFrom (responseBytes);

			SenseixSession.SetSessionState(true);
			Logger.BasicLog("I got a response from a Problem post Message");
			return true;
		}

		static public bool ParseVerifyGameResponse(byte[] responseBytes)
		{
			Device.GameVerificationResponse verifyGameResponse = 
				Device.GameVerificationResponse.ParseFrom (responseBytes);

			SenseixSession.SetSessionState(true);
			Logger.BasicLog("I got a response from my game verification Message");
			return true;
		}

		static public bool ParseReportBugResponse(byte[] responseBytes)
		{
			Debug.DebugLogSubmitResponse verifyGameResponse = 
				Debug.DebugLogSubmitResponse.ParseFrom (responseBytes);

			SenseixSession.SetSessionState(true);
			Logger.BasicLog("I got a response from a debug log submit message.");
			return true;
		}


		static public bool ParsePlayerScoreResponse(byte[] responseBytes)
		{
			Leaderboard.UpdatePlayerScoreResponse verifyGameResponse = 
				Leaderboard.UpdatePlayerScoreResponse.ParseFrom (responseBytes);

			SenseixSession.SetSessionState(true);
			Logger.BasicLog("I got a response from a Player score Message");
			return true;
		}
		
		static public bool ParsePlayerRankResponse(byte[] responseBytes)
		{
			Leaderboard.PlayerRankResponse verifyGameResponse = 
				Leaderboard.PlayerRankResponse.ParseFrom (responseBytes);

			SenseixSession.SetSessionState(true);
			Logger.BasicLog("I got a response from a Player rank Message");
			return true;
		}

		static public bool ParseGetEncouragementsResponse(byte[] responseBytes)
		{
			Encouragement.EncouragementGetResponse getEncouragementResponse = 
				Encouragement.EncouragementGetResponse.ParseFrom (responseBytes);
			
			Logger.BasicLog ("I got an encouragement get response with " + getEncouragementResponse.EncouragementDataCount + " encouragements.");

			foreach (Encouragement.EncouragementData encouragementData in getEncouragementResponse.EncouragementDataList)
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
					Debug.ServerErrorResponse.ParseFrom (responseBytes);

				UnityEngine.Debug.LogError("I got a server error response.  Here is the message: " +
				                      serverErrorResponse.Message);
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogWarning("Error while parsing error.  Um.");
			}
			return false;
		}
	}
}

