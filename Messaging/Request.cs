// ------------------------------------------------------------------------------
//Responsible for passing messages to and receiving from the server. 
//We sacrifice some overlap of code here for readibility for the 
//end user. 
//In general, any messages related to authentication/user management
//are done syncronously, everything to do with updating/pulling 
//problems is done on a separate thread. 
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

namespace senseix.message {
	//API URLS
	//api-staging.senseix.com
	static public class Request
	{
        const string ENCRYPTED = "http://";
        const string SERVER_URL = "192.168.1.2:3000/";
		const string API_VERSION = "v1";
		const string GENERIC_HDR = ENCRYPTED + SERVER_URL + API_VERSION;
		const string PARENT_HDR = GENERIC_HDR + "/parents/";
		const string PLAYER_HDR = GENERIC_HDR + "/players/";
		const string PROBLEM_HDR = GENERIC_HDR + "/problems/";
		const string LEADERBOARD_HDR = GENERIC_HDR + "/applications/leaderboard/";

		//External urls
		public const string WEBSITE_URL = ENCRYPTED + SERVER_URL;
		public const string DEVICES_WEBSITE_HDR = WEBSITE_URL + "/devices/";
		public const string ENROLL_GAME_URL = DEVICES_WEBSITE_HDR + "new";

		//Requests related to parent management 
		const string REGISTER_DEVICE_URL = PARENT_HDR + "create_device";
		const string VERIFY_GAME_URL = PARENT_HDR + "game_verification";
		const string REGISTER_PARENT_URL = PARENT_HDR + "register";
		const string EDIT_PARENT_URL = PARENT_HDR + "edit";
		const string SIGN_IN_PARENT_URL = PARENT_HDR + "sign_in";
		const string SIGN_OUT_PARENT_URL = PARENT_HDR + "sign_out";
		const string MERGE_PARENT_URL = PARENT_HDR + "merge";

		//Requests related to player management
		const string CREATE_PLAYER_URL = PLAYER_HDR + "create";
		const string LIST_PLAYER_URL = PLAYER_HDR + "list_players";
		const string REGISTER_PLAYER_WITH_GAME_URL = PLAYER_HDR + "register_player_with_game";

		//Requests related to problems
		const string GET_PROBLEM_URL = PROBLEM_HDR + "index";
		const string POST_PROBLEM_URL = PROBLEM_HDR + "update";

		//Requests related to leaderboards
		const string GET_LEADERBOARD_PAGE_URL = LEADERBOARD_HDR + "page";
		const string GET_PLAYER_RANK_URL = LEADERBOARD_HDR + "player";
		const string UPDATE_PLAYER_SCORE_URL = LEADERBOARD_HDR + "update_player_score";
	
		static public bool SyncronousPostRequest(ref RequestHeader.Builder hdr_request, constant.MessageType msgType, string url)
	    {
			ResponseHeader reply = null;
			byte[] bytes;
		
			MemoryStream stream = new MemoryStream ();
			Dictionary<string, string> mods = new Dictionary<string, string>();
			mods.Add("Content-Type", "application/protobuf");
			hdr_request.BuildPartial().WriteTo (stream);
			bytes = stream.ToArray();
			stream.Close();

			WWW recvResult = new WWW (url, bytes, mods);

			WaitForRequest (recvResult);
		
			Debug.Log ("Recv result is " + recvResult.text);
			var replyBytes = recvResult.bytes;
			reply = ResponseHeader.ParseFrom (replyBytes);

		
			return Response.ParseResponse(msgType, ref reply); 
		}
	 

		static private void WaitForRequest(WWW recvResult)
		{
			
		
			while(!recvResult.isDone && string.IsNullOrEmpty(recvResult.error))
			{
				//display some waiting sign to the user if on the main thread...
			}

			//Did we receive any response?
			if (!string.IsNullOrEmpty (recvResult.error))
			{
				if(recvResult.error.Equals(401))
				{
					Debug.Log (recvResult.error);
					SenseixController.SetSessionState(false);
				}
				else if(recvResult.error.Equals(422))
				{
					Debug.Log (recvResult.error);
					SenseixController.SetSessionState(false);
				}
				else //This probably has no message or we hit a 500...either way
					//This is a bad place to be in
				{
					Debug.Log (recvResult.error);
					SenseixController.SetSessionState(false);
			//		return -1;
				}
			}
			//Maybe set state to up here? Or parse first..hmmm
		}
		/// <summary>
		/// Registers the device with the SenseiX server, allows a temporary account to be created
		/// and the player to begin playing without logging in. Once an account is registered
		/// or created the temporary account is transitioned into a permanent one.  
		/// </summary>
		static public bool RegisterDevice(string deviceNameInformation)
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());

			parent.DeviceRegistrationRequest.Builder newDevice = parent.DeviceRegistrationRequest.CreateBuilder ();
			newDevice.SetInformation (deviceNameInformation);
			newDevice.SetDeviceId (SenseixController.GetDeviceID());

			hdr_request.SetDeviceRegistration(newDevice);
			Debug.Log ("register device going off to " + REGISTER_DEVICE_URL);
			return SyncronousPostRequest (ref hdr_request, constant.MessageType.RegisterDevice, REGISTER_DEVICE_URL);
		}

		/// <summary>
		/// Adds the temporary verification code to the server.  When the user enters the verificationCode
		/// on the SenseiX website now, it will be able to link this game with the user's account.
		/// </summary>
		static public bool VerifyGame(string verificationCode)
		{
			Debug.Log ("henry's first message...");

			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());

			parent.GameVerificationRequest.Builder newVerification = parent.GameVerificationRequest.CreateBuilder ();
			newVerification.SetVerificationToken (verificationCode);
			newVerification.SetUdid (SenseixController.GetDeviceID ());

			hdr_request.SetGameVerification(newVerification);
			Debug.Log ("going off to " + VERIFY_GAME_URL);
			Debug.Log (hdr_request.GameVerification.Udid);
			Debug.Log (hdr_request.GameVerification.VerificationToken);
			Debug.Log (hdr_request.AccessToken);
			return SyncronousPostRequest (ref hdr_request, constant.MessageType.GameVerification, VERIFY_GAME_URL);
		}
		
	     /// <summary>
	     /// Registers a Parent with the SenseiX server and results in a auth_token for the current session.
	    /// Name is an optional parameter for the server (it can be blank). 
	    /// </summary>
		static public bool RegisterParent (string email,string name,string password)
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();
			parent.ParentRegistrationRequest.Builder newParent = parent.ParentRegistrationRequest.CreateBuilder ();
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());
			newParent.SetDeviceId(SenseixController.GetDeviceID());
			newParent.SetEmail(email);
			newParent.SetConfirmationPassword(password);
			newParent.SetName(name);
			newParent.SetPassword(password);
			hdr_request.SetParentRegistration(newParent);		

			return SyncronousPostRequest (ref hdr_request, constant.MessageType.RegisterParent, REGISTER_PARENT_URL);
		}

		/// <summary>
		/// Logs in a parent given a password and email for that account. Sets the authentication token for the user
		/// allowing for additional API calls such as pulling in additional problems, player management, etc.
		/// </summary>
		static public bool SignInParent (string email, string password)
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();
			parent.ParentSignInRequest.Builder signInParent = parent.ParentSignInRequest.CreateBuilder ();
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());
			signInParent.SetDeviceId(SenseixController.GetDeviceID());
			signInParent.SetEmail(email);
			signInParent.SetPassword(password);
			hdr_request.SetParentSignIn(signInParent);

			return SyncronousPostRequest (ref hdr_request,  constant.MessageType.SignInParent, SIGN_IN_PARENT_URL);	
		}

		
		/// <summary>
		/// Logs a user out of the SenseiX platform. This will result in an action having to be taken by the user if they attempt to 
		/// login with anoter account (each device is tied to a single account)	
		/// </summary>
		static public bool SignOutParent()
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());
			hdr_request.SetAuthToken(SenseixController.GetAuthToken ());

			senseix.message.parent.ParentSignOutRequest.Builder parentSignOutBuilder = parent.ParentSignOutRequest.CreateBuilder ();
			parentSignOutBuilder.SetDeviceId (SenseixController.GetDeviceID ());
			hdr_request.SetParentSignOut(parentSignOutBuilder);

			Debug.Log ("sign out parent going off to " + SIGN_OUT_PARENT_URL);
			return SyncronousPostRequest (ref hdr_request, constant.MessageType.SignOutParent, SIGN_OUT_PARENT_URL);
		}
		   
		/// <summary>
		/// Edit a users profile i.e. change password, name, email, etc. The current password is required to be re-entered
		/// here for added security. 
		/// </summary>
		static public bool ParentEditProfile (string email,string name,string password, string new_password, string confirmation_password)
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());
			hdr_request.SetAuthToken(SenseixController.GetAuthToken());
			parent.ParentEditRequest.Builder editParent = parent.ParentEditRequest.CreateBuilder();
			editParent.SetDeviceId(SenseixController.GetDeviceID());
			editParent.SetEmail(email);
			editParent.SetPassword (password);
			editParent.SetNewPassword(new_password);
			editParent.SetName(name);
			hdr_request.SetParentEdit(editParent);      
			return SyncronousPostRequest (ref hdr_request, constant.MessageType.EditParent, EDIT_PARENT_URL);
		}

		/// <summary>
		/// This is a specialized function that is called when a player has played on a device with a temporary account
		/// and now has a real account, we give the parent the option to either, create a new player, merge the data with 
		/// a player they already have or just delete the current data.  
		/// </summary>
		static public bool ParentAccntResolution (string email, string password, parent.ParentMergeRequest.Types.Decision decision, string player_id, string name) 
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();   
			hdr_request.SetAuthToken(SenseixController.GetAccessToken());
			hdr_request.SetAccessToken (SenseixController.GetDeviceID());
			parent.ParentMergeRequest.Builder mergeParent = parent.ParentMergeRequest.CreateBuilder();

			mergeParent.SetNewPlayerName (name);
			mergeParent.SetEmail (email);
			mergeParent.SetPlayerId (player_id);
			mergeParent.SetPassword (password);
			mergeParent.SetDecision (decision);
			hdr_request.SetParentMerge (mergeParent);

			return SyncronousPostRequest (ref hdr_request, constant.MessageType.MergeParent, MERGE_PARENT_URL);
		}


		/// <summary>
		/// Create a new player for this parent. This will return a player_id for future calls
		/// to the API. 
		/// </summary>
		static public bool CreatePlayer (string name) 
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();   
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());
			hdr_request.SetAuthToken(SenseixController.GetAuthToken());
			player.PlayerCreateRequest.Builder createPlayer = player.PlayerCreateRequest.CreateBuilder ();
			createPlayer.SetName (name);
			hdr_request.SetPlayerCreate (createPlayer);
			return SyncronousPostRequest (ref hdr_request, constant.MessageType.CreatePlayer, CREATE_PLAYER_URL);
		}

		/// <summary>
		/// Return a list of player names and player_id's for a parent, most likely to 
		/// pick which player should be playing the game at a given time.  
		/// </summary>
		static public bool ListPlayers () 
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();   
			hdr_request.SetAuthToken(SenseixController.GetAuthToken());
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());
			player.PlayerListRequest.Builder listPlayer = player.PlayerListRequest.CreateBuilder ();
			hdr_request.SetPlayerList (listPlayer);
			Debug.Log ("list players request going off to " + LIST_PLAYER_URL);
			return SyncronousPostRequest (ref hdr_request, constant.MessageType.ListPlayer, LIST_PLAYER_URL);
		}
		/// <summary>
		/// We have an explicit call to register a player with a game, this should be called each time a new player
	    /// is selected from the drop downlist. It will add this game to a list of played games for the player and 
		/// add them to things like the games leaderboard.
		/// </summary>
		static public bool RegisterPlayer (string player_id) 
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();   
			hdr_request.SetAuthToken(SenseixController.GetAuthToken());
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());
			player.PlayerRegisterWithApplicationRequest.Builder regPlayer = player.PlayerRegisterWithApplicationRequest.CreateBuilder ();
			regPlayer.SetPlayerId (player_id);
			hdr_request.SetPlayerRegisterWithApplication(regPlayer);
			Debug.Log(hdr_request.AccessToken);
			Debug.Log(hdr_request.AuthToken);
			Debug.Log(hdr_request.PlayerRegisterWithApplication.PlayerId);
			Debug.Log ("register player going off to " + REGISTER_PLAYER_WITH_GAME_URL);
			return SyncronousPostRequest (ref hdr_request, constant.MessageType.RegisterPlayerWithApplication, REGISTER_PLAYER_WITH_GAME_URL);
		}

	
		/// <summary>
		/// Return a list of player names and player_id's for a parent, most likely to 
		/// pick which player should be playing the game at a given time.  
		/// </summary>
		static public bool GetProblems (string player_id, UInt32 count) 
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();   
			hdr_request.SetAuthToken(SenseixController.GetAuthToken());
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());
			problem.ProblemGetRequest.Builder getProblem = problem.ProblemGetRequest.CreateBuilder ();
			getProblem.SetProblemCount (count);
			getProblem.SetPlayerId (player_id);
			hdr_request.SetProblemGet (getProblem);
			Debug.Log ("Get problems request going off to " + GET_PROBLEM_URL);
			Debug.Log (hdr_request.AuthToken);
			Debug.Log (hdr_request.AccessToken);
			Debug.Log (hdr_request.ProblemGet.ProblemCount);
			Debug.Log (hdr_request.ProblemGet.PlayerId);
			return SyncronousPostRequest (ref hdr_request, constant.MessageType.ProblemGet, GET_PROBLEM_URL);
		}	

		/// <summary>
		/// Posts a list of problems that have been answered or skipped by the player to the server. This is mainly 
		/// for internal use/developers should not have to worry about this. 
		/// </summary>
		static public bool PostProblems (string playerId, Queue problems) 
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();   
			hdr_request.SetAuthToken(SenseixController.GetAuthToken());
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());
			problem.ProblemPostRequest.Builder postProblem = problem.ProblemPostRequest.CreateBuilder ();

			while (problems.Count > 0) {
				senseix.message.problem.ProblemPost.Builder addMeProblem = (senseix.message.problem.ProblemPost.Builder)problems.Dequeue();
				addMeProblem.SetPlayerId(SenseixController.GetCurrentPlayerID());
				postProblem.AddProblem (addMeProblem);
			}
			hdr_request.SetProblemPost (postProblem);
				
			Debug.Log ("Post problems request going off to " + POST_PROBLEM_URL);
			return SyncronousPostRequest (ref hdr_request, constant.MessageType.ProblemPost, POST_PROBLEM_URL);
		}	
		/// <summary>
		/// Returns a page from the leaderboard with the request parameters, by default 25 entries are 
		/// are returned per page. 
		/// </summary>
		static public bool LeaderboardPage (UInt32 pageNumber = 1, leaderboard.SortBy sortBy = leaderboard.SortBy.NONE , UInt32 pageSize = 25) 
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();   
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());
			leaderboard.PageRequest.Builder lbPage = leaderboard.PageRequest.CreateBuilder ();
			lbPage.SetPage (pageNumber);
			lbPage.SetSortBy (sortBy);
			lbPage.SetPageSize (pageSize);
			hdr_request.SetPage (lbPage);
			Debug.Log ("leaderboard page request going off to " + GET_LEADERBOARD_PAGE_URL);
			return SyncronousPostRequest (ref hdr_request, constant.MessageType.LeaderboardPage, GET_LEADERBOARD_PAGE_URL);
		}
		/// <summary>
		/// Pushes a players score to the leaderboard, this is dependent on the developer to take care of what
	    /// "score" really means in their application.  
		/// </summary>
		static public bool UpdatePlayerScore (string playerId, UInt32 score)
	    {
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();   
			leaderboard.UpdatePlayerScoreRequest.Builder lbScore = leaderboard.UpdatePlayerScoreRequest.CreateBuilder ();
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());
			lbScore.SetPlayerId(playerId);
			lbScore.SetPlayerScore (score);

			hdr_request.SetPlayerScore(lbScore);
			return SyncronousPostRequest(ref hdr_request, constant.MessageType.PlayerScore, UPDATE_PLAYER_SCORE_URL);
		}

		/// <summary>
		/// Stores a players rank and players surrounding it based on the call preferences. 
		/// By default we only return the players rank and score. 	
		/// </summary>
		static public bool GetPlayerRank ( string playerId, UInt32 surroundingUsers = 0, leaderboard.SortBy sortBy = leaderboard.SortBy.NONE , UInt32 pageSize = 25) 
		{
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();   
	
		    leaderboard.PlayerRankRequest.Builder rank = leaderboard.PlayerRankRequest.CreateBuilder ();
			hdr_request.SetAccessToken (SenseixController.GetAccessToken());
			rank.SetCount (surroundingUsers);	
			rank.SetPageSize (pageSize);
			rank.SetPlayerId(SenseixController.GetCurrentPlayerID());
			rank.SetSortBy(sortBy);
		
			hdr_request.SetPlayerRank(rank);
			return SyncronousPostRequest(ref hdr_request, constant.MessageType.PlayerRank, GET_PLAYER_RANK_URL);
			
		}
	
	}
}



