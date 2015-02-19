using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Threading;

namespace Senseix.Message 
{
	public struct PostRequestParameters
	{
		public ProtoBuf.IExtensible serializableRequest;
		public ResponseHandlerDelegate responseHandler;
		public string url;
		public WWW recvResult;
		public bool isGet;
	}

	public class Request : MonoBehaviour
	{
		//API URLS
		//api-staging.Senseix.com
        const string ENCRYPTED = "https://";
		const string SERVER_URL = "api.thinksylearn.com/";
		const string API_VERSION = "v1";
		const string GENERIC_HDR = ENCRYPTED + SERVER_URL + API_VERSION;
		const string PARENT_HDR = GENERIC_HDR + "/devices/";
		const string PLAYER_HDR = GENERIC_HDR + "/players/";
		const string PROBLEM_HDR = GENERIC_HDR + "/problems/";
		const string LEADERBOARD_HDR = GENERIC_HDR + "/applications/leaderboard/";
		const string DEBUG_HDR = GENERIC_HDR + "/debug/";

		//External urls
		public const string WEBSITE_URL = "https://parent.thinksylearn.com/parents/sign_up";
		public const string DEVICES_WEBSITE_HDR = WEBSITE_URL + "/devices/";
		public const string ENROLL_GAME_URL = "https://parent.thinksylearn.com/devices/new";

		//Requests related to Parent management 
		const string REGISTER_DEVICE_URL = PARENT_HDR + "create_device";
		const string VERIFY_GAME_URL = PARENT_HDR + "game_verification";
		const string REGISTER_PARENT_URL = PARENT_HDR + "register";
		const string EDIT_PARENT_URL = PARENT_HDR + "edit";
		const string SIGN_IN_PARENT_URL = PARENT_HDR + "sign_in";
		const string SIGN_OUT_PARENT_URL = PARENT_HDR + "sign_out";
		const string MERGE_PARENT_URL = PARENT_HDR + "merge";

		//Requests related to Player management
		const string CREATE_PLAYER_URL = PLAYER_HDR + "create";
		const string LIST_PLAYER_URL = PLAYER_HDR + "list_players";
		const string REGISTER_PLAYER_WITH_GAME_URL = PLAYER_HDR + "register_player_with_game";
		const string GET_ENCOURAGEMENT_URL = PLAYER_HDR + "get_encouragements";

		//Requests related to Problems
		const string GET_PROBLEM_URL = PROBLEM_HDR + "index";
		const string POST_PROBLEM_URL = PROBLEM_HDR + "update";

		//Requests related to Leaderboards
		const string GET_LEADERBOARD_PAGE_URL = LEADERBOARD_HDR + "page";
		const string GET_PLAYER_RANK_URL = LEADERBOARD_HDR + "player";
		const string UPDATE_PLAYER_SCORE_URL = LEADERBOARD_HDR + "update_player_score";

		//Requests related to Debugging
		const string DEBUG_LOG_SUBMIT_URL = DEBUG_HDR + "debug_log_submit";

		public static ArrayList activeRequests = new ArrayList();

		public static void CheckResults()
		{
			if (!SenseixSession.GetSessionState ())
				return;

			ArrayList removeUsRequests = new ArrayList ();

			foreach (PostRequestParameters parameters in activeRequests)
			{
				if (parameters.recvResult.isDone)
				{
					HandleResult(parameters.recvResult, parameters.responseHandler);
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
			SyncronousPostRequest (parameters.recvResult, parameters.serializableRequest, parameters.responseHandler, parameters.url);
		}

		public static void SyncronousPostRequest(ProtoBuf.IExtensible serializableRequest, ResponseHandlerDelegate responseHandler, string url, bool isGet)
		{
			WWW recvResult = SetUpRecvResult(serializableRequest, url, isGet);
			//UnityEngine.Debug.Log ("set up recv result already");
			SyncronousPostRequest (recvResult, serializableRequest, responseHandler, url);
		}
		
		public static void SyncronousPostRequest(WWW recvResult, ProtoBuf.IExtensible serializableRequest, ResponseHandlerDelegate responseHandler, string url)
		{
			//Debug.Log ("Wait for Request:");
			if (!SenseixSession.GetSessionState())
			{
				return;
			}
			WaitForRequest (recvResult);
			//UnityEngine.Debug.Log ("handle result");
			HandleResult (recvResult, responseHandler);
		}
		
		public static WWW SetUpRecvResult(ProtoBuf.IExtensible serializableRequest, string url, bool isGet)
		{
			byte[] bytes;
			Dictionary<string, string> mods = new Dictionary<string, string>();
			mods.Add ("X-Auth-Token", SenseixSession.GetAuthToken ());
			mods.Add ("X-Access-Token", SenseixSession.GetAccessToken());
			mods.Add("Content-Type", "application/protobuf");

			MemoryStream requestMessageStream = new MemoryStream ();
			ThinksyProtosSerializer serializer = new ThinksyProtosSerializer ();
			serializer.Serialize(requestMessageStream, serializableRequest);
			bytes = requestMessageStream.ToArray();
			requestMessageStream.Close();

			WWW recvResult;
			if (!isGet)
			{
				//UnityEngine.Debug.Log("POST");
				recvResult = new WWW (url, bytes, mods);
			}
			else
			{
				//UnityEngine.Debug.Log("GET");
				recvResult = new WWW (url, null, mods);
			}

			return recvResult;
		}


		public static void HandleResult(WWW recvResult, ResponseHandlerDelegate resultHandler)
		{
			byte[] responseBytes = new byte[0];
			if (NetworkErrorChecking(recvResult))
			{
				responseBytes = recvResult.bytes;
				//UnityEngine.Debug.Log ("Recv result is " + recvResult.bytes.Length + " bytes long");
				//UnityEngine.Debug.Log ("parse response");
				try
				{
					resultHandler(responseBytes);
				}
				catch (Exception e)
				{
					Logger.BasicLog("parsing a server message resulted in this error: " + e.Message);
					Response.ParseServerErrorResponse(responseBytes);
				}
			}
			else
			{
				UnityEngine.Debug.LogWarning ("A SenseiX message had an error.  " + 
				                  "Most likely internet connectivity issues.");
				SenseixSession.SetSessionState (false);
			}

			return;
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
				UnityEngine.Debug.LogWarning (recvResult.error);
				SenseixSession.SetSessionState(false);
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

		static public void NonblockingPostRequest(ProtoBuf.IExtensible serializableRequest, ResponseHandlerDelegate responseHandler, string url, bool isGet)
		{
			PostRequestParameters parameters = new PostRequestParameters ();
			parameters.serializableRequest = serializableRequest;
			parameters.responseHandler = responseHandler;
			parameters.url = url;
			parameters.recvResult = SetUpRecvResult (serializableRequest, url, isGet);
			NonblockingPostRequest (parameters);
		}

		/// <summary>
		/// Registers the device with the Senseix server, allows a temporary account to be created
		/// and the Player to begin playing without logging in. Once an account is registered
		/// or created the temporary account is transitioned into a permanent one.  
		/// </summary>
		static public void RegisterDevice(string deviceNameInformation)
		{
			Device.DeviceRegistrationRequest newDevice = new Device.DeviceRegistrationRequest();
			newDevice.information = (deviceNameInformation);
			//UnityEngine.Debug.Log ("Device id:　" + SenseixSession.GetDeviceID ());				
			newDevice.device_id =(SenseixSession.GetDeviceID());
			
			//Debug.Log ("register device going off to " + REGISTER_DEVICE_URL);
			SyncronousPostRequest (newDevice, Response.ParseRegisterDeviceResponse, REGISTER_DEVICE_URL, false);
		}

		/// <summary>
		/// Adds the temporary verification code to the server.  When the user enters the verificationCode
		/// on the Senseix website now, it will be able to link this game with the user's account.
		/// </summary>
		static public void VerifyGame(string verificationCode)
		{
			Device.GameVerificationRequest newVerification = new Device.GameVerificationRequest();
			newVerification.verification_token = verificationCode;
			newVerification.udid = (SenseixSession.GetDeviceID ());

			//Debug.Log ("going off to " + VERIFY_GAME_URL);
			//Debug.Log (hdr_request.GameVerification.Udid);
			//Debug.Log (hdr_request.GameVerification.VerificationToken);
			//Debug.Log (hdr_request.AccessToken);
			SyncronousPostRequest (newVerification, Response.ParseVerifyGameResponse, VERIFY_GAME_URL, false);
		}

		/// <summary>
		/// Return a list of Player names and Player_id's for a Parent, most likely to 
		/// pick which Player should be playing the game at a given time.  
		/// </summary>
		static public void ListPlayers () 
		{
			//UnityEngine.Debug.Log ("Auth Token: " + SenseixSession.GetAuthToken());

			Player.PlayerListRequest listPlayer = new Player.PlayerListRequest();

			//Debug.Log ("register device going off to " + REGISTER_DEVICE_URL);
			SyncronousPostRequest (listPlayer, Response.ParseListPlayerResponse, LIST_PLAYER_URL, true);
		}
		/// <summary>
		/// We have an explicit call to register a Player with a game, this should be called each time a new Player
	    /// is selected from the drop downlist. It will add this game to a list of played games for the Player and 
		/// add them to things like the games Leaderboard.
		/// </summary>
		static public void RegisterPlayer (string player_id) 
		{

			Player.PlayerRegisterWithApplicationRequest regPlayer = new Player.PlayerRegisterWithApplicationRequest();
			regPlayer.player_id = (player_id);

			//Debug.Log(hdr_request.AccessToken);
			//Debug.Log(hdr_request.AuthToken);
			//Debug.Log(hdr_request.PlayerRegisterWithApplication.PlayerId);
			//Debug.Log ("register Player going off to " + REGISTER_Player_WITH_GAME_URL);

			//UnityEngine.Debug.Log ("register player going off to " + REGISTER_PLAYER_WITH_GAME_URL);
			SyncronousPostRequest (regPlayer, Response.ParseRegisterPlayerResponse, REGISTER_PLAYER_WITH_GAME_URL, false);

		}

	
		/// <summary>
		/// Return a list of Player names and Player_id's for a Parent, most likely to 
		/// pick which Player should be playing the game at a given time.  
		/// </summary>
		static public void GetProblems (string player_id, UInt32 count) 
		{

			//UnityEngine.Debug.Log ("get problems");

			Problem.ProblemGetRequest getProblem = new Problem.ProblemGetRequest();
			getProblem.problem_count = (count);
			getProblem.player_id = (player_id);

//			Debug.Log ("Get Problems request going off to " + GET_Problem_URL);
//			Debug.Log (hdr_request.AuthToken);
//			Debug.Log (hdr_request.AccessToken);
//			Debug.Log (hdr_request.ProblemGet.ProblemCount);
//			Debug.Log (hdr_request.ProblemGet.PlayerId);

			NonblockingPostRequest (getProblem, Response.ParseGetProblemResponse, GET_PROBLEM_URL, false);

		}

		static public void GetEncouragements (string player_id) 
		{

			Encouragement.EncouragementGetRequest getEncouragements = new Encouragement.EncouragementGetRequest();
			getEncouragements.player_id = (player_id);
			
			NonblockingPostRequest (getEncouragements, Response.ParseGetEncouragementsResponse, GET_ENCOURAGEMENT_URL, false);
		}	

		/// <summary>
		/// Posts a list of Problems that have been answered or skipped by the Player to the server. This is mainly 
		/// for internal use/developers should not have to worry about this. 
		/// </summary>
		static public void PostProblems (string PlayerId, Queue problems) 
		{
			problems = new Queue (problems);



			Problem.ProblemPostRequest postProblem = new Problem.ProblemPostRequest();

			while (problems.Count > 0) {
				Senseix.Message.Problem.ProblemPost addMeProblem = (Senseix.Message.Problem.ProblemPost)problems.Dequeue();
				SetPlayerForProblemIfNeeded(ref addMeProblem);
				postProblem.problem.Add (addMeProblem);
			}

				
			//Debug.Log ("Post Problems request going off to " + POST_PROBLEM_URL);
			if (SenseixSession.ShouldCacheProblemPosts())
			{
				PostRequestParameters queueParameters = new PostRequestParameters();
				queueParameters.serializableRequest = postProblem;
				queueParameters.responseHandler = Response.ParsePostProblemResponse;
				queueParameters.url = POST_PROBLEM_URL;
				WriteRequestToCache(queueParameters);
				//UnityEngine.Debug.Log("Cache post");
			}
			else
			{
				NonblockingPostRequest (postProblem, Response.ParsePostProblemResponse, POST_PROBLEM_URL, false);
				//UnityEngine.Debug.Log("Post post");
			}
		}	


		private static void WriteRequestToCache(PostRequestParameters parameters)
		{
			ProtoBuf.IExtensible serializableRequest = parameters.serializableRequest;
			MemoryStream stream = new MemoryStream ();
			ProtoBuf.Serializer.Serialize<ProtoBuf.IExtensible>(stream, serializableRequest);
			byte[] bytes = stream.ToArray();
			string directoryPath = Path.Combine (Application.persistentDataPath, "post_cache/");
			//Debug.Log (fileCount);
			if (!Directory.Exists(directoryPath))
			    Directory.CreateDirectory(directoryPath);
			string fileCount = (Directory.GetFiles (directoryPath).Length + 1).ToString ();
			string filePath = Path.Combine (directoryPath, fileCount + ProblemKeeper.SEED_FILE_EXTENSION);
			System.IO.File.WriteAllBytes (filePath, bytes);
		}

		/// <summary>
		/// Returns a page from the Leaderboard with the request parameters, by default 25 entries are 
		/// are returned per page. 
		/// </summary>
		static public void LeaderboardPage (UInt32 pageNumber, Leaderboard.SortBy sortBy, UInt32 pageSize) 
		{
			/*
			RequestHeader.Builder hdr_request = RequestHeader.CreateBuilder ();   
			hdr_request.SetAccessToken (SenseixSession.GetAccessToken());
			Leaderboard.PageRequest.Builder lbPage = Leaderboard.PageRequest.CreateBuilder ();
			lbPage.SetPage (pageNumber);
			lbPage.SetSortBy (sortBy);
			lbPage.SetPageSize (pageSize);
			hdr_request.SetPage (lbPage);
//			Debug.Log ("Leaderboard page request going off to " + GET_Leaderboard_PAGE_URL);
			SyncronousPostRequest (ref hdr_request, Constant.MessageType.LeaderboardPage, GET_LEADERBOARD_PAGE_URL);
			*/
		}
		/// <summary>
		/// Pushes a Players score to the Leaderboard, this is dependent on the developer to take care of what
	    /// "score" really means in their application.  
		/// </summary>
		static public void UpdatePlayerScore (string playerId, UInt32 score)
	    {

			Leaderboard.UpdatePlayerScoreRequest lbScore = new Leaderboard.UpdatePlayerScoreRequest();
			lbScore.player_id = (playerId);
			lbScore.player_score = (score);

			SyncronousPostRequest(lbScore, Response.ParsePlayerScoreResponse, UPDATE_PLAYER_SCORE_URL, false);

		}

		/// <summary>
		/// Stores a Players rank and Players surrounding it based on the call preferences. 
		/// By default we only return the Players rank and score. 	
		/// </summary>
		static public void GetPlayerRank ( string PlayerId) 
		{
			UInt32 surroundingUsers = 0;
			Leaderboard.SortBy sortBy = Leaderboard.SortBy.NONE;
			UInt32 pageSize = 25;

		    Leaderboard.PlayerRankRequest rank = new Leaderboard.PlayerRankRequest();
			rank.count = (surroundingUsers);	
			rank.page_size =(pageSize);
			rank.player_id = (SenseixSession.GetCurrentPlayerID());
			rank.sort_by = (sortBy);

			SyncronousPostRequest(rank, Response.ParsePlayerRankResponse, GET_PLAYER_RANK_URL, false);

		}

		static public void SubmitProblemPostCache()
		{
			string directoryPath = Path.Combine (Application.persistentDataPath, "post_cache/");
			if (!Directory.Exists(directoryPath))
				Directory.CreateDirectory(directoryPath);
			string[] fileNames = Directory.GetFiles (directoryPath);
			foreach (string fileName in fileNames)
			{
				//UnityEngine.Debug.Log("Submitting cache");
				byte[] bytes = System.IO.File.ReadAllBytes(fileName);
				MemoryStream stream = new MemoryStream(bytes);
				Problem.ProblemPostRequest problemPostRequest = ProtoBuf.Serializer.Deserialize<Problem.ProblemPostRequest>(stream);
				for (int i = 0; i < problemPostRequest.problem.Count; i++)
				{
					Problem.ProblemPost problemPost = problemPostRequest.problem[i];
					SetPlayerForProblemIfNeeded(ref problemPost);
					problemPostRequest.problem[i] = (problemPost);
					//UnityEngine.Debug.Log(problemPostBuilder.PlayerId);
				}

				NonblockingPostRequest(problemPostRequest, Response.ParsePostProblemResponse, POST_PROBLEM_URL, false);
				File.Delete(fileName);
			}
		}

		static public void BugReport(string deviceID, string report)
		{
			Debug.DebugLogSubmitRequest debugLogSubmit = new Debug.DebugLogSubmitRequest();
			debugLogSubmit.debug_log = report;
			debugLogSubmit.device_id = deviceID;

			UnityEngine.Debug.Log ("Submitting bug report.");
			SyncronousPostRequest(debugLogSubmit, Response.ParseReportBugResponse, DEBUG_LOG_SUBMIT_URL, false);
		}

		static private void SetPlayerForProblemIfNeeded(ref Senseix.Message.Problem.ProblemPost problemPostBuilder)
		{
			if (problemPostBuilder.player_id == "no current player")
			{
				problemPostBuilder.player_id = (SenseixSession.GetCurrentPlayerID());
			}
			if (problemPostBuilder.player_id == "no current player")
			{
				//UnityEngine.Debug.LogWarning("I'm sending a problem to the server with no player ID.");
			}
		}
	}
}