// ------------------------------------------------------------------------------
//Responsible for passing Messages to and receiving from the server. 
//We sacrifice some overlap of code here for readibility for the 
//end user. 
//In general, any Messages related to authentication/user management
//are done syncronously, everything to do with updating/pulling 
//Problems is done on a separate thread. 
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Threading;

namespace Senseix.Message {
	//API URLS
	//api-staging.Senseix.com

	public struct PostRequestParameters
	{
		public RequestHeader.Builder hdr_request;
		public Constant.MessageType msgType;
		public string url;
		public WWW recvResult;
	}

	public class Request : MonoBehaviour
	{
        const string ENCRYPTED = "http://";
		const string SERVER_URL = "192.168.1.8:3000/";
		const string API_VERSION = "v1";
		const string GENERIC_HDR = ENCRYPTED + SERVER_URL + API_VERSION;
		const string Parent_HDR = GENERIC_HDR + "/parents/";
		const string Player_HDR = GENERIC_HDR + "/players/";
		const string Problem_HDR = GENERIC_HDR + "/problems/";
		const string Leaderboard_HDR = GENERIC_HDR + "/applications/leaderboard/";

		//External urls
		public const string WEBSITE_URL = ENCRYPTED + SERVER_URL;
		public const string DEVICES_WEBSITE_HDR = WEBSITE_URL + "/devices/";
		public const string ENROLL_GAME_URL = DEVICES_WEBSITE_HDR + "new";

		//Requests related to Parent management 
		const string REGISTER_DEVICE_URL = Parent_HDR + "create_device";
		const string VERIFY_GAME_URL = Parent_HDR + "game_verification";
		const string REGISTER_Parent_URL = Parent_HDR + "register";
		const string EDIT_Parent_URL = Parent_HDR + "edit";
		const string SIGN_IN_Parent_URL = Parent_HDR + "sign_in";
		const string SIGN_OUT_Parent_URL = Parent_HDR + "sign_out";
		const string MERGE_Parent_URL = Parent_HDR + "merge";

		//Requests related to Player management
		const string CREATE_Player_URL = Player_HDR + "create";
		const string LIST_Player_URL = Player_HDR + "list_players";
		const string REGISTER_Player_WITH_GAME_URL = Player_HDR + "register_player_with_game";

		//Requests related to Problems
		const string GET_Problem_URL = Problem_HDR + "index";
		const string POST_Problem_URL = Problem_HDR + "update";

		//Requests related to Leaderboards
		const string GET_Leaderboard_PAGE_URL = Leaderboard_HDR + "page";
		const string GET_Player_RANK_URL = Leaderboard_HDR + "player";
		const string UPDATE_Player_SCORE_URL = Leaderboard_HDR + "update_player_score";

		public static ArrayList activeRequests = new ArrayList();

		public static void CheckResults()
		{
			if (!SenseixController.GetSessionState ())
				return;

			ArrayList removeUsRequests = new ArrayList ();

			foreach (PostRequestParameters parameters in activeRequests)
			{
				if (parameters.recvResult.isDone)
				{
					try
					{
						HandleResult(parameters.recvResult, parameters.msgType);
					}
					catch (Google.ProtocolBuffers.InvalidProtocolBufferException)
					{
						Debug.LogWarning ("A pending SenseiX request had a protobufs error" +
						           " so I'm just going to get rid of it." + 
						           "  What's one request, right?");
					}
					removeUsRequests.Add(parameters);
				}
			}

			foreach (PostRequestParameters parameters in removeUsRequests)
			{
				activeRequests.Remove(parameters);
			}
		}

		public static void SyncronousPostRequest(object parametersObject)
		{
			PostRequestParameters parameters = (PostRequestParameters)parametersObject;
			SyncronousPostRequest (parameters.recvResult, ref parameters.hdr_request, parameters.msgType, parameters.url);
		}

		public static WWW SetUpRecvResult(ref RequestHeader.Builder hdr_request, string url)
		{
			byte[] bytes;
			MemoryStream stream = new MemoryStream ();
			Dictionary<string, string> mods = new Dictionary<string, string>();
			mods.Add("Content-Type", "application/protobuf");
			hdr_request.BuildPartial().WriteTo (stream);
			bytes = stream.ToArray();
			stream.Close();
			WWW recvResult = new WWW (url, bytes, mods);
			return recvResult;
		}

		public static void SyncronousPostRequest(ref RequestHeader.Builder hdr_request, Constant.MessageType msgType, string url)
		{
			WWW recvResult = SetUpRecvResult (ref hdr_request, url);
			SyncronousPostRequest (recvResult, ref hdr_request, msgType, url);
		}

		public static void SyncronousPostRequest(WWW recvResult, ref RequestHeader.Builder hdr_request, Constant.MessageType msgType, string url)
	    {
//			Debug.Log ("Wait for Request:");
			if (!SenseixController.GetSessionState())
			{
				return;
			}
			WaitForRequest (recvResult);
			HandleResult (recvResult, msgType);
		}

		public static void HandleResult(WWW recvResult, Constant.MessageType msgType)
		{
			ResponseHeader reply = null;
			byte[] replyBytes = new byte[0];
			if (NetworkErrorChecking(recvResult))
			{
				//happy
				replyBytes = recvResult.bytes;
				//Debug.Log ("Recv result is " + recvResult.bytes.Length + " bytes long");
			}
			else
			{
				Debug.LogWarning ("A SenseiX message had an error.  " + 
				                  "Most likely internet connectivity issues.");
				SenseixController.SetSessionState (false);
			}
			
			if (replyBytes.Length == 0)
			{
				//Debug.Log("Bytes empty");
				SenseixController.SetSessionState (false);
				return;
			}

			reply = ResponseHeader.ParseFrom (replyBytes);
			Response.ParseResponse(msgType, ref reply); 
		}

		static private void WaitForRequest(WWW recvResult)
		{
			while(!recvResult.isDone && string.IsNullOrEmpty(recvResult.error))
			{
				//display dancing ninjas
			}
		}

		/// <summary>
		/// Returns false for an error, or true for no errors.
		/// </summary>
		static public bool NetworkErrorChecking(WWW recvResult)
		{
			//Did we receive any response?
			if (!string.IsNullOrEmpty (recvResult.error))
			{
				Debug.LogWarning (recvResult.error);
				SenseixController.SetSessionState(false);
				if(recvResult.error.Equals(401))
				{
					//This is probably a problem with an auth token
				}
				else if(recvResult.error.Equals(422))
				{
					//This is probably a server side error
				}
				else
				{
					//This is probably a 500.
					//This is a bad place in which to be.
				}
				return false;
			}
			return true;
		}

		static public void NonblockingPostRequest(PostRequestParameters parameters)
		{
			activeRequests.Add (parameters);
		}

		static public void NonblockingPostRequest(ref RequestHeader.Builder hdr_request, Constant.MessageType msgType, string url)
		{
			PostRequestParameters parameters = new PostRequestParameters ();
			parameters.hdr_request = hdr_request;
			parameters.msgType = msgType;
			parameters.url = url;
			parameters.recvResult = SetUpRecvResult (ref hdr_request, url);
			NonblockingPostRequest (parameters);
		}

		/// <summary>
		/// Registers the device with the Senseix server, allows a temporary account to be created
		/// and the Player to begin playing without logging in. Once an account is registered
		/// or created the temporary account is transitioned into a permanent one.  
		/// </summary>
		static public void RegisterDevice(string deviceNameInformation)
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());

			Parent.DeviceRegistrationRequest.Builder newDevice = Parent.DeviceRegistrationRequest.CreateBuilder ();
			newDevice.SetInformation (deviceNameInformation);
			newDevice.SetDeviceId (SenseixController.GetDeviceID());

			hdr_request.SetDeviceRegistration(newDevice);
			//Debug.Log ("register device going off to " + REGISTER_DEVICE_URL);
			SyncronousPostRequest (ref hdr_request, Constant.MessageType.RegisterDevice, REGISTER_DEVICE_URL);
		}

		/// <summary>
		/// Adds the temporary verification code to the server.  When the user enters the verificationCode
		/// on the Senseix website now, it will be able to link this game with the user's account.
		/// </summary>
		static public void VerifyGame(string verificationCode)
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());

			Parent.GameVerificationRequest.Builder newVerification = Parent.GameVerificationRequest.CreateBuilder ();
			newVerification.SetVerificationToken (verificationCode);
			newVerification.SetUdid (SenseixController.GetDeviceID ());

			hdr_request.SetGameVerification(newVerification);
			//Debug.Log ("going off to " + VERIFY_GAME_URL);
			//Debug.Log (hdr_request.GameVerification.Udid);
			//Debug.Log (hdr_request.GameVerification.VerificationToken);
			//Debug.Log (hdr_request.AccessToken);
			SyncronousPostRequest (ref hdr_request, Constant.MessageType.GameVerification, VERIFY_GAME_URL);
		}
		
	     /// <summary>
	     /// Registers a Parent with the Senseix server and results in a auth_token for the current session.
	    /// Name is an optional parameter for the server (it can be blank). 
	    /// </summary>
		static public void RegisterParent (string email,string name,string password)
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();
			Senseix.Message.Parent.ParentRegistrationRequest.Builder newParent = Parent.ParentRegistrationRequest.CreateBuilder ();
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());
			newParent.SetDeviceId(SenseixController.GetDeviceID());
			newParent.SetEmail(email);
			newParent.SetConfirmationPassword(password);
			newParent.SetName(name);
			newParent.SetPassword(password);
			hdr_request.SetParentRegistration(newParent);		

			SyncronousPostRequest (ref hdr_request, Constant.MessageType.RegisterParent, REGISTER_Parent_URL);
		}

		/// <summary>
		/// Logs in a Parent given a password and email for that account. Sets the authentication token for the user
		/// allowing for additional API calls such as pulling in additional Problems, Player management, etc.
		/// </summary>
		static public void SignInParent (string email, string password)
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();
			Parent.ParentSignInRequest.Builder signInParent = Parent.ParentSignInRequest.CreateBuilder ();
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());
			signInParent.SetDeviceId(SenseixController.GetDeviceID());
			signInParent.SetEmail(email);
			signInParent.SetPassword(password);
			hdr_request.SetParentSignIn(signInParent);

			SyncronousPostRequest (ref hdr_request,  Constant.MessageType.SignInParent, SIGN_IN_Parent_URL);	
		}

		
		/// <summary>
		/// Logs a user out of the Senseix platform. This will result in an action having to be taken by the user if they attempt to 
		/// login with anoter account (each device is tied to a single account)	
		/// </summary>
		static public void SignOutParent()
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());
			hdr_request.SetAuthToken(SenseixController.GetAuthToken ());

			Senseix.Message.Parent.ParentSignOutRequest.Builder ParentSignOutBuilder = Parent.ParentSignOutRequest.CreateBuilder ();
			ParentSignOutBuilder.SetDeviceId (SenseixController.GetDeviceID ());
			hdr_request.SetParentSignOut(ParentSignOutBuilder);

			//Debug.Log ("sign out Parent going off to " + SIGN_OUT_Parent_URL);
			SyncronousPostRequest (ref hdr_request, Constant.MessageType.SignOutParent, SIGN_OUT_Parent_URL);
		}
		   
		/// <summary>
		/// Edit a users profile i.e. change password, name, email, etc. The current password is required to be re-entered
		/// here for added security. 
		/// </summary>
		static public void ParentEditProfile (string email,string name,string password, string new_password, string confirmation_password)
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());
			hdr_request.SetAuthToken(SenseixController.GetAuthToken());
			Parent.ParentEditRequest.Builder editParent = Parent.ParentEditRequest.CreateBuilder();
			editParent.SetDeviceId(SenseixController.GetDeviceID());
			editParent.SetEmail(email);
			editParent.SetPassword (password);
			editParent.SetNewPassword(new_password);
			editParent.SetName(name);
			hdr_request.SetParentEdit(editParent);      
			SyncronousPostRequest (ref hdr_request, Constant.MessageType.EditParent, EDIT_Parent_URL);
		}

		/// <summary>
		/// This is a specialized function that is called when a Player has played on a device with a temporary account
		/// and now has a real account, we give the Parent the option to either, create a new Player, merge the data with 
		/// a Player they already have or just delete the current data.  
		/// </summary>
		static public void ParentAccntResolution (string email, string password, Parent.ParentMergeRequest.Types.Decision decision, string Player_id, string name) 
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();   
			hdr_request.SetAuthToken(SenseixController.GetAccessToken());
			hdr_request.SetAccessToken (SenseixController.GetDeviceID());
			Parent.ParentMergeRequest.Builder mergeParent = Parent.ParentMergeRequest.CreateBuilder();

			mergeParent.SetNewPlayerName (name);
			mergeParent.SetEmail (email);
			mergeParent.SetPlayerId (Player_id);
			mergeParent.SetPassword (password);
			mergeParent.SetDecision (decision);
			hdr_request.SetParentMerge (mergeParent);

			SyncronousPostRequest (ref hdr_request, Constant.MessageType.MergeParent, MERGE_Parent_URL);
		}


		/// <summary>
		/// Create a new Player for this Parent. This will return a Player_id for future calls
		/// to the API. 
		/// </summary>
		static public void CreatePlayer (string name) 
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();   
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());
			hdr_request.SetAuthToken(SenseixController.GetAuthToken());
			Player.PlayerCreateRequest.Builder createPlayer = Player.PlayerCreateRequest.CreateBuilder ();
			createPlayer.SetName (name);
			hdr_request.SetPlayerCreate (createPlayer);
			SyncronousPostRequest (ref hdr_request, Constant.MessageType.CreatePlayer, CREATE_Player_URL);
		}

		/// <summary>
		/// Return a list of Player names and Player_id's for a Parent, most likely to 
		/// pick which Player should be playing the game at a given time.  
		/// </summary>
		static public void ListPlayers () 
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();   
			hdr_request.SetAuthToken(SenseixController.GetAuthToken());
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());
			Player.PlayerListRequest.Builder listPlayer = Player.PlayerListRequest.CreateBuilder ();
			hdr_request.SetPlayerList (listPlayer);
			//Debug.Log ("list Players request going off to " + LIST_Player_URL);
			SyncronousPostRequest (ref hdr_request, Constant.MessageType.ListPlayer, LIST_Player_URL);
		}
		/// <summary>
		/// We have an explicit call to register a Player with a game, this should be called each time a new Player
	    /// is selected from the drop downlist. It will add this game to a list of played games for the Player and 
		/// add them to things like the games Leaderboard.
		/// </summary>
		static public void RegisterPlayer (string Player_id) 
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();   
			hdr_request.SetAuthToken(SenseixController.GetAuthToken());
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());
			Player.PlayerRegisterWithApplicationRequest.Builder regPlayer = Player.PlayerRegisterWithApplicationRequest.CreateBuilder ();
			regPlayer.SetPlayerId (Player_id);
			hdr_request.SetPlayerRegisterWithApplication(regPlayer);
//			Debug.Log(hdr_request.AccessToken);
//			Debug.Log(hdr_request.AuthToken);
//			Debug.Log(hdr_request.PlayerRegisterWithApplication.PlayerId);
//			Debug.Log ("register Player going off to " + REGISTER_Player_WITH_GAME_URL);
			SyncronousPostRequest (ref hdr_request, Constant.MessageType.RegisterPlayerWithApplication, REGISTER_Player_WITH_GAME_URL);
		}

	
		/// <summary>
		/// Return a list of Player names and Player_id's for a Parent, most likely to 
		/// pick which Player should be playing the game at a given time.  
		/// </summary>
		static public void GetProblems (string Player_id, UInt32 count) 
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();   
			hdr_request.SetAuthToken(SenseixController.GetAuthToken());
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());
			Problem.ProblemGetRequest.Builder getProblem = Problem.ProblemGetRequest.CreateBuilder ();
			getProblem.SetProblemCount (count);
			getProblem.SetPlayerId (Player_id);
			hdr_request.SetProblemGet (getProblem);
//			Debug.Log ("Get Problems request going off to " + GET_Problem_URL);
//			Debug.Log (hdr_request.AuthToken);
//			Debug.Log (hdr_request.AccessToken);
//			Debug.Log (hdr_request.ProblemGet.ProblemCount);
//			Debug.Log (hdr_request.ProblemGet.PlayerId);
			NonblockingPostRequest (ref hdr_request, Constant.MessageType.ProblemGet, GET_Problem_URL);
		}	

		/// <summary>
		/// Posts a list of Problems that have been answered or skipped by the Player to the server. This is mainly 
		/// for internal use/developers should not have to worry about this. 
		/// </summary>
		static public void PostProblems (string PlayerId, Queue problems) 
		{
			problems = new Queue (problems);
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();   
			hdr_request.SetAuthToken(SenseixController.GetAuthToken());
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());
			Problem.ProblemPostRequest.Builder postProblem = Problem.ProblemPostRequest.CreateBuilder ();

			while (problems.Count > 0) {
				Senseix.Message.Problem.ProblemPost.Builder addMeProblem = (Senseix.Message.Problem.ProblemPost.Builder)problems.Dequeue();
				addMeProblem.SetPlayerId(SenseixController.GetCurrentPlayerID());
				postProblem.AddProblem (addMeProblem);
			}
			hdr_request.SetProblemPost (postProblem);
				
//			Debug.Log ("Post Problems request going off to " + POST_Problem_URL);
			NonblockingPostRequest (ref hdr_request, Constant.MessageType.ProblemPost, POST_Problem_URL);
		}	
		/// <summary>
		/// Returns a page from the Leaderboard with the request parameters, by default 25 entries are 
		/// are returned per page. 
		/// </summary>
		static public void LeaderboardPage (UInt32 pageNumber = 1, Leaderboard.SortBy sortBy = Leaderboard.SortBy.NONE , UInt32 pageSize = 25) 
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();   
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());
			Leaderboard.PageRequest.Builder lbPage = Leaderboard.PageRequest.CreateBuilder ();
			lbPage.SetPage (pageNumber);
			lbPage.SetSortBy (sortBy);
			lbPage.SetPageSize (pageSize);
			hdr_request.SetPage (lbPage);
//			Debug.Log ("Leaderboard page request going off to " + GET_Leaderboard_PAGE_URL);
			SyncronousPostRequest (ref hdr_request, Constant.MessageType.LeaderboardPage, GET_Leaderboard_PAGE_URL);
		}
		/// <summary>
		/// Pushes a Players score to the Leaderboard, this is dependent on the developer to take care of what
	    /// "score" really means in their application.  
		/// </summary>
		static public void UpdatePlayerScore (string PlayerId, UInt32 score)
	    {
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();   
			Leaderboard.UpdatePlayerScoreRequest.Builder lbScore = Leaderboard.UpdatePlayerScoreRequest.CreateBuilder ();
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());
			lbScore.SetPlayerId(PlayerId);
			lbScore.SetPlayerScore (score);

			hdr_request.SetPlayerScore(lbScore);
			SyncronousPostRequest(ref hdr_request, Constant.MessageType.PlayerScore, UPDATE_Player_SCORE_URL);
		}

		/// <summary>
		/// Stores a Players rank and Players surrounding it based on the call preferences. 
		/// By default we only return the Players rank and score. 	
		/// </summary>
		static public void GetPlayerRank ( string PlayerId, UInt32 surroundingUsers = 0, Leaderboard.SortBy sortBy = Leaderboard.SortBy.NONE , UInt32 pageSize = 25) 
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();   
	
		    Leaderboard.PlayerRankRequest.Builder rank = Leaderboard.PlayerRankRequest.CreateBuilder ();
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());
			rank.SetCount (surroundingUsers);	
			rank.SetPageSize (pageSize);
			rank.SetPlayerId(SenseixController.GetCurrentPlayerID());
			rank.SetSortBy(sortBy);
		
			hdr_request.SetPlayerRank(rank);
			SyncronousPostRequest(ref hdr_request, Constant.MessageType.PlayerRank, GET_Player_RANK_URL);
			
		}
	
	}
}



