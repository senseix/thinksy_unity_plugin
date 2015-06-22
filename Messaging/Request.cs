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
		//static string ENCRYPTED = "http://";
        static string ENCRYPTED = "https://";
		//static string SERVER_URL = "192.168.1.2:3000/";
		//static string SERVER_URL = "api-erudite.thinksylearn.com/";
		static string SERVER_URL = "api.thinksylearn.com/";
		static string STAGING_SERVER_URL = "api-staging.thinksylearn.com/";
		static string API_VERSION = "v1";
		static string GENERIC_HDR = ENCRYPTED + SERVER_URL + API_VERSION;
		static string PARENT_HDR = GENERIC_HDR + "/devices/";
		static string PLAYER_HDR = GENERIC_HDR + "/players/";
		static string PROBLEM_HDR = GENERIC_HDR + "/problems/";
		static string LEADERBOARD_HDR = GENERIC_HDR + "/applications/leaderboard/";
		static string DEBUG_HDR = GENERIC_HDR + "/debug/";

		//Requests related to Parent management 
		static string REGISTER_DEVICE_URL = PARENT_HDR + "create_device";
		static string VERIFY_GAME_URL = PARENT_HDR + "game_verification";
		static string SEND_PARENT_EMAIL_URL = PARENT_HDR + "send_parent_email";

		//Requests related to Player management
		static string LIST_PLAYER_URL = PLAYER_HDR + "list_players";
		static string REGISTER_PLAYER_WITH_GAME_URL = PLAYER_HDR + "register_player_with_game";
		static string GET_ENCOURAGEMENT_URL = PLAYER_HDR + "get_encouragements";
		static string LIST_ITEMS_URL = PLAYER_HDR + "list_items";

		//Requests related to Problems
		static string GET_PROBLEM_URL = PROBLEM_HDR + "index";
		static string SPECIFIED_GET_PROBLEM_URL = PROBLEM_HDR + "specified_index";
		static string POST_PROBLEM_URL = PROBLEM_HDR + "update";

		//Requests related to Leaderboards
		//static string GET_LEADERBOARD_PAGE_URL = LEADERBOARD_HDR + "page";
		static string GET_PLAYER_RANK_URL = LEADERBOARD_HDR + "player";
		static string UPDATE_PLAYER_SCORE_URL = LEADERBOARD_HDR + "update_player_score";

		//Requests related to Debugging
		static string DEBUG_LOG_SUBMIT_URL = DEBUG_HDR + "debug_log_submit";

		//External urls
		public const string ENROLL_GAME_URL = "https://parent.thinksylearn.com/devices/link_game";
		public const string ENROLL_GAME_STAGING_URL = "https://parent-staging.thinksylearn.com/devices/link_game";

		private static Request singletonInstance = null;
		private static int secretStagingCounter = 1;
		private static bool secretStagingMode = false;
		
		public static Request GetSingletonInstance()
		{
			if (singletonInstance == null)
			{
				singletonInstance = FindObjectOfType<Request>();
			}
			return singletonInstance;
		}

		public static IEnumerator CoroutinePostRequest(object parametersObject)
		{
			PostRequestParameters parameters = (PostRequestParameters)parametersObject;
			yield return GetSingletonInstance().StartCoroutine(
				CoroutinePostRequest (parameters.recvResult, parameters.serializableRequest, parameters.responseHandler, parameters.url));
		}

		public static IEnumerator CoroutinePostRequest(ProtoBuf.IExtensible serializableRequest, ResponseHandlerDelegate responseHandler, string url, bool isGet)
		{
			//UnityEngine.Debug.Log ("set up recv result");
			WWW recvResult = SetUpRecvResult(serializableRequest, url, isGet);

			yield return GetSingletonInstance().StartCoroutine(
				CoroutinePostRequest (recvResult, serializableRequest, responseHandler, url));
		}
		
		public static IEnumerator CoroutinePostRequest(WWW recvResult, ProtoBuf.IExtensible serializableRequest, ResponseHandlerDelegate responseHandler, string url)
		{
			if (!SenseixSession.GetSessionState())
			{
				yield break;
			}
			//UnityEngine.Debug.Log ("wait for request");
			yield return GetSingletonInstance().StartCoroutine(WaitForRequest (recvResult));
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
			//UnityEngine.Debug.Log ("Serializing request");
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
					string logString = "parsing a server message resulted in this error: " + e.Message;
					Logger.BasicLog(logString);
					UnityEngine.Debug.LogWarning(logString);
					Response.ParseServerErrorResponse(responseBytes);
				}
			}
			else
			{
				string logString = "A SenseiX message (Handler: " + resultHandler.Method.Name + ") had an error.  " + 
					"Most likely internet connectivity issues.";
				UnityEngine.Debug.LogWarning (logString);
				if (secretStagingMode)
				{
					ThinksyPlugin.ShowEmergencyWindow(logString);
				}
				SenseixSession.SetSessionState (false);
			}
			return;
		}

		static private IEnumerator WaitForRequest(WWW recvResult)
		{
			//UnityEngine.Debug.Log ("entering wait for request");
			while(!recvResult.isDone && string.IsNullOrEmpty(recvResult.error))
			{
				ConnectionIndicator.SetWaitingIndication (true);
				yield return null;
			}
			//UnityEngine.Debug.Log ("exiting waiting for request");
			ConnectionIndicator.SetWaitingIndication (false);
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
				if (secretStagingMode)
				{
					ThinksyPlugin.ShowEmergencyWindow (recvResult.error);
				}
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

		/// <summary>
		/// Registers the device with the Senseix server, allows a temporary account to be created
		/// and the Player to begin playing without logging in. Once an account is registered
		/// or created the temporary account is transitioned into a permanent one.  
		/// </summary>
		static public IEnumerator RegisterDevice()
		{
			string deviceNameInformation = SystemInfo.deviceName;

			//UnityEngine.Debug.Log ("building request");
			Device.DeviceRegistrationRequest newDevice = new Device.DeviceRegistrationRequest();
			newDevice.information = (deviceNameInformation);
			//UnityEngine.Debug.Log ("Device id:　" + SenseixSession.GetDeviceID ());
			newDevice.device_id =(SenseixSession.GetDeviceID());
			
			Logger.BasicLog("register device going off to " + REGISTER_DEVICE_URL);
			yield return GetSingletonInstance().StartCoroutine(
				CoroutinePostRequest (newDevice, Response.ParseRegisterDeviceResponse, REGISTER_DEVICE_URL, false));
		}

		/// <summary>
		/// Adds the temporary verification code to the server.  When the user enters the verificationCode
		/// on the Senseix website now, it will be able to link this game with the user's account.
		/// </summary>
		static public IEnumerator VerifyGame(string verificationCode)
		{
			Device.GameVerificationRequest newVerification = new Device.GameVerificationRequest();
			newVerification.verification_token = verificationCode;
			newVerification.udid = (SenseixSession.GetDeviceID ());

			//Debug.Log ("going off to " + VERIFY_GAME_URL);
			//Debug.Log (hdr_request.GameVerification.Udid);
			//Debug.Log (hdr_request.GameVerification.VerificationToken);
			//Debug.Log (hdr_request.AccessToken);
			yield return GetSingletonInstance().StartCoroutine(
				CoroutinePostRequest (newVerification, Response.ParseVerifyGameResponse, VERIFY_GAME_URL, false));
		}

		static public IEnumerator ListPlayers ()
		{
			//UnityEngine.Debug.Log ("Auth Token: " + SenseixSession.GetAuthToken());

			Player.PlayerListRequest listPlayer = new Player.PlayerListRequest();

			//UnityEngine.Debug.Log ("list players");
			yield return GetSingletonInstance().StartCoroutine(
				CoroutinePostRequest (listPlayer, Response.ParseListPlayerResponse, LIST_PLAYER_URL, true));
		}

		/// <summary>
		/// This should be called each time a new Player is selected.
		/// It will add this game to a list of played games for the Player and 
		/// add them to things like the game's Leaderboard.
		/// </summary>
		static public IEnumerator RegisterPlayer (string player_id) 
		{
			Player.PlayerRegisterWithApplicationRequest regPlayer = new Player.PlayerRegisterWithApplicationRequest();
			regPlayer.player_id = (player_id);

			//Debug.Log(hdr_request.AccessToken);
			//Debug.Log(hdr_request.AuthToken);
			//Debug.Log(hdr_request.PlayerRegisterWithApplication.PlayerId);
			//Debug.Log ("register Player going off to " + REGISTER_Player_WITH_GAME_URL);

			//UnityEngine.Debug.Log ("register player going off to " + REGISTER_PLAYER_WITH_GAME_URL);
			yield return GetSingletonInstance().StartCoroutine(
				CoroutinePostRequest (regPlayer, Response.ParseRegisterPlayerResponse, REGISTER_PLAYER_WITH_GAME_URL, false));

		}

	

		static public IEnumerator GetProblems (string player_id, UInt32 count) 
		{

			//UnityEngine.Debug.Log ("get problems");

			Problem.ProblemGetRequest getProblem = new Problem.ProblemGetRequest();
			getProblem.problem_count = (count);
			getProblem.player_id = (player_id);

			Logger.BasicLog("Get Problems request going off to " + GET_PROBLEM_URL);

			if (SenseixSession.GetAuthToken () == "you don't need to see my identification")
				yield break;
			yield return GetSingletonInstance().StartCoroutine(
				CoroutinePostRequest (getProblem, Response.ParseGetProblemResponse, GET_PROBLEM_URL, false));

		}

		static public IEnumerator GetSpecifiedProblems (string player_id, LearningAction specifyingLearningAction, UInt32 count)
		{
			//UnityEngine.Debug.Log ("get problems");
			
			Problem.SpecifiedProblemGetRequest getProblem = new Problem.SpecifiedProblemGetRequest();
			getProblem.problem_count = (count);
			getProblem.player_id = (player_id);
			getProblem.specifying_learning_action = specifyingLearningAction.GetProto ();
			
			Logger.BasicLog("Specified Get Problems request going off to " + GET_PROBLEM_URL);
			
			if (SenseixSession.GetAuthToken () == "you don't need to see my identification")
				yield break;

			yield return GetSingletonInstance().StartCoroutine(
				CoroutinePostRequest (getProblem, Response.ParseGetSpecifiedProblemResponse, GET_PROBLEM_URL, false));	
		}

		static public IEnumerator GetEncouragements (string player_id) 
		{
			Encouragement.EncouragementGetRequest getEncouragements = new Encouragement.EncouragementGetRequest();
			getEncouragements.player_id = (player_id);
			
			yield return GetSingletonInstance().StartCoroutine(
				CoroutinePostRequest (getEncouragements, Response.ParseGetEncouragementsResponse, GET_ENCOURAGEMENT_URL, false));
		}	

		/// <summary>
		/// Posts a list of Problems that have been answered or skipped by the Player to the server. This is mainly 
		/// for internal use/developers should not have to worry about this. 
		/// </summary>
		static public IEnumerator PostProblems (string PlayerId, Queue problems) 
		{
			problems = new Queue (problems);



			Problem.ProblemPostRequest postProblem = new Problem.ProblemPostRequest();

			while (problems.Count > 0) {
				Senseix.Message.Problem.ProblemPost addMeProblem = (Senseix.Message.Problem.ProblemPost)problems.Dequeue();
				postProblem.problem.Add (addMeProblem);
			}

				
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
				//UnityEngine.Debug.Log ("Post Problems request going off to " + POST_PROBLEM_URL);
				for (int i = 0; i < postProblem.problem.Count; i++)
				{
					Senseix.Message.Problem.ProblemPost problemPost = postProblem.problem[i];
					SetPlayerForProblemIfNeeded(ref problemPost);
					postProblem.problem[i] = problemPost;
					//UnityEngine.Debug.Log(postProblem.problem[0].correct);
				}
				yield return GetSingletonInstance().StartCoroutine(
					CoroutinePostRequest (postProblem, Response.ParsePostProblemResponse, POST_PROBLEM_URL, false));
				//UnityEngine.Debug.Log("Post post");
			}
		}	


		private static void WriteRequestToCache(PostRequestParameters parameters)
		{
			ProtoBuf.IExtensible serializableRequest = parameters.serializableRequest;
			MemoryStream stream = new MemoryStream ();
			ThinksyProtosSerializer customSerializer = new ThinksyProtosSerializer ();
			customSerializer.Serialize (stream, serializableRequest);
			byte[] bytes = stream.ToArray();
			string directoryPath = Path.Combine (Application.persistentDataPath, "post_cache/");
			//Debug.Log (fileCount);
			if (!Directory.Exists(directoryPath))
			    Directory.CreateDirectory(directoryPath);
			string fileCount = (Directory.GetFiles (directoryPath).Length + 1).ToString ();
			string filePath = Path.Combine (directoryPath, fileCount + ProblemKeeper.SEED_FILE_EXTENSION);
			System.IO.File.WriteAllBytes (filePath, bytes);
			SenseixSession.DoFileFlagging(filePath);
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
			//Debug.Log ("Leaderboard page request going off to " + GET_Leaderboard_PAGE_URL);
			SyncronousPostRequest (ref hdr_request, Constant.MessageType.LeaderboardPage, GET_LEADERBOARD_PAGE_URL);
			*/
		}
		/// <summary>
		/// Pushes a Players score to the Leaderboard, this is dependent on the developer to take care of what
	    /// "score" really means in their application.  
		/// </summary>
		static public IEnumerator UpdatePlayerScore (string playerId, UInt32 score)
	    {

			Leaderboard.UpdatePlayerScoreRequest lbScore = new Leaderboard.UpdatePlayerScoreRequest();
			lbScore.player_id = (playerId);
			lbScore.player_score = (score);

			//UnityEngine.Debug.Log ("update player score");

			yield return GetSingletonInstance().StartCoroutine(
				CoroutinePostRequest(lbScore, Response.ParsePlayerScoreResponse, UPDATE_PLAYER_SCORE_URL, false));

		}

		static public IEnumerator ListPlayerItems(string playerId)
		{
			
			Player.ListPlayerItemsRequest listItemsRequest = new Player.ListPlayerItemsRequest ();

			listItemsRequest.player_id = playerId;

			//UnityEngine.Debug.Log ("List items going off to " + LIST_ITEMS_URL);
			yield return GetSingletonInstance().StartCoroutine(
				CoroutinePostRequest(listItemsRequest, Response.ParseListItemsResponse, LIST_ITEMS_URL, false));
		}

		/// <summary>
		/// Stores a Players rank and Players surrounding it based on the call preferences. 
		/// By default we only return the Players rank and score. 	
		/// </summary>
		static public IEnumerator GetPlayerRank ( string PlayerId) 
		{
			UInt32 surroundingUsers = 0;
			Leaderboard.SortBy sortBy = Leaderboard.SortBy.NONE;
			UInt32 pageSize = 25;

		    Leaderboard.PlayerRankRequest rank = new Leaderboard.PlayerRankRequest();
			rank.count = (surroundingUsers);	
			rank.page_size =(pageSize);
			rank.player_id = (SenseixSession.GetCurrentPlayerID());
			rank.sort_by = (sortBy);

			//UnityEngine.Debug.Log ("get player rank");

			yield return GetSingletonInstance().StartCoroutine(
				CoroutinePostRequest(rank, Response.ParsePlayerRankResponse, GET_PLAYER_RANK_URL, false));
		}

		static public IEnumerator SubmitProblemPostCache()
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
				ThinksyProtosSerializer customSerializer = new ThinksyProtosSerializer ();
				Problem.ProblemPostRequest problemPostRequest = customSerializer.Deserialize (stream, 
				                                                                              null, 
				                                                                              typeof(Problem.ProblemPostRequest)) as Problem.ProblemPostRequest;
				for (int i = 0; i < problemPostRequest.problem.Count; i++)
				{
					Problem.ProblemPost problemPost = problemPostRequest.problem[i];
					SetPlayerForProblemIfNeeded(ref problemPost);
					problemPostRequest.problem[i] = (problemPost);
					//UnityEngine.Debug.Log(problemPostBuilder.PlayerId);
				}

				yield return GetSingletonInstance().StartCoroutine(
					CoroutinePostRequest(problemPostRequest, Response.ParsePostProblemResponse, POST_PROBLEM_URL, false));
				File.Delete(fileName);
			}
		}

		static public IEnumerator SendParentEmail(string recruitmentEmail)
		{
			UnityEngine.Debug.Log ("Sending recruitment email to " + recruitmentEmail);

			Device.SendParentEmailRequest sendEmail = new Device.SendParentEmailRequest ();
			sendEmail.email = recruitmentEmail;

			yield return GetSingletonInstance().StartCoroutine(
				CoroutinePostRequest(sendEmail, Response.ParseSendParentEmailResponse, SEND_PARENT_EMAIL_URL, false));
		}

		static public IEnumerator BugReport(string deviceID, string report)
		{
			Debug.DebugLogSubmitRequest debugLogSubmit = new Debug.DebugLogSubmitRequest();
			debugLogSubmit.debug_log = report;
			debugLogSubmit.device_id = deviceID;

			UnityEngine.Debug.Log ("Submitting bug report.");
			yield return GetSingletonInstance().StartCoroutine(
				CoroutinePostRequest(debugLogSubmit, Response.ParseReportBugResponse, DEBUG_LOG_SUBMIT_URL, false));
		}

		static private void SetPlayerForProblemIfNeeded(ref Senseix.Message.Problem.ProblemPost problemPostBuilder)
		{
			if (problemPostBuilder.player_id == "no current player")
			{
				problemPostBuilder.player_id = (SenseixSession.GetCurrentPlayerID());
			}
			if (problemPostBuilder.player_id == "no current player")
			{
				UnityEngine.Debug.LogWarning("I'm sending a problem to the server with no player ID.");
			}
		}

		public void SecretStagingSwap(int tapOrder)
		{
			if (tapOrder != secretStagingCounter)
			{
				secretStagingCounter = 1;
			}
			else
			{
				secretStagingCounter++;
			}
			UnityEngine.Debug.Log(secretStagingCounter);
			if (secretStagingCounter != 5)
				return;
			secretStagingCounter = 1;
			UnityEngine.Debug.Log ("Super secret staging strike!");
			//Handheld.Vibrate ();
			StagingStrike.Boom ();
			SERVER_URL = STAGING_SERVER_URL;
			//API URLS
			GENERIC_HDR = ENCRYPTED + SERVER_URL + API_VERSION;
			PARENT_HDR = GENERIC_HDR + "/devices/";
			PLAYER_HDR = GENERIC_HDR + "/players/";
			PROBLEM_HDR = GENERIC_HDR + "/problems/";
			LEADERBOARD_HDR = GENERIC_HDR + "/applications/leaderboard/";
			DEBUG_HDR = GENERIC_HDR + "/debug/";
			
			//Requests related to Parent management 
			REGISTER_DEVICE_URL = PARENT_HDR + "create_device";
			VERIFY_GAME_URL = PARENT_HDR + "game_verification";
			SEND_PARENT_EMAIL_URL = PARENT_HDR + "send_parent_email";

			//Requests related to Player management
			LIST_PLAYER_URL = PLAYER_HDR + "list_players";
			REGISTER_PLAYER_WITH_GAME_URL = PLAYER_HDR + "register_player_with_game";
			GET_ENCOURAGEMENT_URL = PLAYER_HDR + "get_encouragements";
			LIST_ITEMS_URL = PLAYER_HDR + "list_items";
			
			//Requests related to Problems
			GET_PROBLEM_URL = PROBLEM_HDR + "index";
			SPECIFIED_GET_PROBLEM_URL = PROBLEM_HDR + "specified_index";
			POST_PROBLEM_URL = PROBLEM_HDR + "update";
			
			//Requests related to Leaderboards
			//GET_LEADERBOARD_PAGE_URL = LEADERBOARD_HDR + "page";
			GET_PLAYER_RANK_URL = LEADERBOARD_HDR + "player";
			UPDATE_PLAYER_SCORE_URL = LEADERBOARD_HDR + "update_player_score";
			
			//Requests related to Debugging
			DEBUG_LOG_SUBMIT_URL = DEBUG_HDR + "debug_log_submit";

			SenseixSession.SetAndSaveAuthToken ("");
			SenseixSession.SetCurrentPlayerList (null);
			//ThinksyPlugin.SetAccessToken("95df4f0f98585ef3679e774878080b7d57e8bb0b5cf9190f866628a4dc497e73");
			//use same token from production
			secretStagingMode = true;
			ThinksyPlugin.StaticReinitialize ();
		}

		public static bool IsInSecretStagingMode()
		{
			return secretStagingMode;
		}

		public static string GetEnrollGameURL()
		{
			if (IsInSecretStagingMode ())
								return ENROLL_GAME_STAGING_URL;
			return ENROLL_GAME_URL;
		}
	}
}