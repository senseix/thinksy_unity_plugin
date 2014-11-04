// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
namespace senseix.message {
	static public class Response
	{
		static public bool ParseResponse(constant.MessageType type, ref ResponseHeader reply)
		{
			if (reply == null) 
			{
				throw new Exception ("Reply was returned as null...this should not be possible");
			}

			if (!reply.IsInitialized) 
			{
				Debug.Log ("Could not find a valid reply message from server...");
				return false; 
			}

			if (reply.Status == constant.Status.FAILURE) 
			{
				Debug.Log (reply.Message);
				return false;
			}
	
			switch (type) 
			{
				case constant.MessageType.RegisterDevice:
					if (reply.HasDeviceRegistration && reply.DeviceRegistration.IsInitialized) 
					{	
						SenseixController.SetAndSaveAuthToken(reply.DeviceRegistration.AuthToken);
						SenseixController.SetSessionState(true);
						Debug.Log("ALL THE WAY FROM THE CITY OF SERVER... THE FAMED ISTEMPORARYACCOUNT" +
							"!!!!!!!!!!!!!!!!!!!!!!!: " + reply.DeviceRegistration.IsTemporaryAccount);
						SenseixController.SetSignedIn(!reply.DeviceRegistration.IsTemporaryAccount);
					} 
					else 
					{
						SenseixController.SetSessionState(false);
						Debug.Log("Can't find key from result");
						return false;
					}
					break;

				case constant.MessageType.GameVerification:
					SenseixController.SetSessionState(true);
					Debug.Log("I got a response from my game verification message");
					break;
				
				case constant.MessageType.RegisterParent:
					if (reply.Status == constant.Status.FAILURE) 
					{
						Debug.Log ("DUANE!!!! MERGE CODE REQUIRED HERE!!!");
					}
					if(reply.HasParentRegistration && reply.ParentRegistration.IsInitialized) 
					{
						SenseixController.SetAndSaveAuthToken(reply.ParentRegistration.AuthToken);
					} 
					else 
					{	
						SenseixController.SetSessionState(false);
						Debug.Log("Can't find key from result");
						return false;
					}
					break;

				case constant.MessageType.SignInParent:
					if (reply.Status == constant.Status.FAILURE) 
					{
						throw new Exception ("We encountered a fatal failure on sign in.");
					}
					if(reply.HasParentSignIn && reply.ParentSignIn.IsInitialized) 
					{
						SenseixController.SetAndSaveAuthToken(reply.ParentSignIn.AuthToken);
					} 
					else 
					{
						SenseixController.SetSessionState(false);
						Debug.Log("Can't find key from result");
						return false;
					}
					break;

				case constant.MessageType.SignOutParent:
					SenseixController.SetSessionState(false);//Duane, this seems..odd
					break;

				case constant.MessageType.MergeParent:
					if(reply.HasParentMerge && reply.ParentMerge.IsInitialized && reply.ParentMerge.HasAuthToken)
					{
						SenseixController.SetAndSaveAuthToken(reply.ParentRegistration.AuthToken);
					} 
					else 
					{	
						SenseixController.SetSessionState(false);
						Debug.Log("Can't find key from result");
						return false;
					}
					break;

				case constant.MessageType.CreatePlayer:
					SenseixController.SetSessionState(true);
					Debug.Log("I got a response from a create player message");
					break;

				case constant.MessageType.ListPlayer:
					SenseixController.SetSessionState(true);
					Debug.Log("I got a response from a list player message");
					SenseixController.SetCurrentPlayerList(reply.PlayerList);
					break;

				case constant.MessageType.RegisterPlayerWithApplication:
					SenseixController.SetSessionState(true);
					Debug.Log("I got a response from a register player message");
					break;

				case constant.MessageType.ProblemPost:
					if (reply.HasProblemPost)
					{
						Debug.Log ("Successfully posted problems to the server.");
					}
					else 
					{
						Debug.Log ("Message sent back for a problem was not as expected");
						return false;
					}
					break;
				
				case constant.MessageType.ProblemGet:
					Debug.Log("I got a response from a problem get message");
					if(reply.HasProblemGet && reply.ProblemGet.IsInitialized) 
					{
						ProblemKeeper.ReplaceSeed(reply);
					}
					else
					{
						throw new Exception("Response to ProblemGet request was empty or uninitialized");
					}
					break;

				case constant.MessageType.LeaderboardPage:
					Debug.Log ("I recieved a leaderboard page response");
					Debug.Log(reply.Page.PlayerList);
					SenseixController.SetLeaderboardPlayers(reply.Page.PlayerList);
					break;

				case constant.MessageType.PlayerScore:
					SenseixController.SetSessionState(true);
					Debug.Log("I got a response from a player score message");
					break;

				case constant.MessageType.PlayerRank:
					SenseixController.SetSessionState(true);
					Debug.Log("I got a response from a player rank message");
					break;

				default:
					throw new Exception("Response.cs recieved a MessageType that it didn't recognize.");
			}
			return true;
		}
	}
}

