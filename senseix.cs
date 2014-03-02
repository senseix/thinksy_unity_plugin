using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
/*
 * Score: the score was not calculated on user's device, what we do is pushing user's result of a question back to server, and let server do the left task
 * User Info: how can user get the info of himself? at the time when user session start?
 * SignIn: coaches sign in need access_token
*/

	public class messageType
	{
		//CLASS: messageType
		//Each message has its now message type id and api url, url may overlap
		//MESSSAGES TYPES
		public const int MESSAGETYPE_USER_ADD = 0;
		public const int MESSAGETYPE_COACH_SIGN_UP = 1;
		public const int MESSAGETYPE_COACH_SIGN_IN = 2;
		public const int MESSAGETYPE_COACH_SIGN_OUT = 3;	//info of profile
		
		public const int MESSAGETYPE_DEVEL_SIGN_IN = 4;
		public const int MESSAGETYPE_DEVEL_SIGN_OUT = 5;

		public const int MESSAGETYPE_PLAYER_CREATE = 6;
		public const int MESSAGETYPE_PLAYER_INDEX = 7;
		
		public const int MESSAGETYPE_LEADERBOARD_PULL = 21;
		public const int MESSAGETYPE_LEADERBOARD_PUSH_SCORE = 22;

		public const int MESSAGETYPE_PROBLEM_PULL = 31;
		public const int MESSAGETYPE_PROBLEM_PUSH = 32; 


		//API URLS
		public const string MESSAGETYPE_USER_ADD_URL = "http://senseix.herokuapp.com/v1/coaches";
		public const string MESSAGETYPE_COACH_SIGN_UP_URL = "http://senseix.herokuapp.com/v1/coaches/";
		public const string MESSAGETYPE_COACH_SIGN_IN_URL = "http://senseix.herokuapp.com/v1/coaches/sign_in";
		public const string MESSAGETYPE_COACH_SIGN_OUT_URL = "http://senseix.herokuapp.com/v1/coaches/sign_out";	//info of profile
		public const string MESSAGETYPE_DEVEL_SIGN_IN_URL = "http://senseix.herokuapp.com/v1/developers/sign_in";
		public const string MESSAGETYPE_DEVEL_SIGN_OUT_URL = "http://senseix.herokuapp.com/v1/developers/sign_out";
		
		public const string MESSAGETYPE_PLAYER_CREATE_URL = "http://senseix.herokuapp.com/v1/players/create";
		public const string MESSAGETYPE_PLAYER_INDEX_URL = "http://senseix.herokuapp.com/v1/players/index";
		
		public const string MESSAGETYPE_LEADERBOARD_PULL_URL = "http://senseix.herokuapp.com /v1/applications/leaderboard/:id";
		public const string MESSAGETYPE_LEADERBOARD_PUSH_SCORE_URL = "http://senseix.herokuapp.com/v1/coaches/sign_in";

		public const string MESSAGETYPE_PROBLEM_PULL_URL = "http://senseix.herokuapp.com/v1/problems";
		public const string MESSAGETYPE_PROBLEM_PUSH_URL = "http://senseix.herokuapp.com/v1/problems/update";

	}
	public class senseix: MonoBehaviour
	{
		//CLASS: senseix
		//	in each sub classes, 
		//						'get' means get the value of attribute from local
		//						'set' means set the value of  attribute in local
		//						'pull' means get updated info from server
		//						'push' means put local data to server, be careful
		public static int rankNum = 10;
		private static string authToken = null;
		private static string gameToken = null;
		private static string name = null;
		private static string email = null;
		private static string deviceId = null;
		private static leaderboard gameLeaderboard = new leaderboard();
		private static heavyUser me = new heavyUser();
		private static string utf8hdr = "utf8=%E2%9C%93";
		public senseix ()
		{
		}
		public static string getGameToken()
		{
			return senseix.gameToken;
		}
		private static void setAuthToken(string result)
		{
			senseix.authToken = new string(result.ToCharArray());
		}
		private static void setName(string result)
		{
			senseix.name = new string(result.ToCharArray());
		}
		private static void setEmail(string result)
		{
			senseix.email = new string(result.ToCharArray());
		}
		private static void setDeviceID(string result)
		{
			senseix.deviceId = new string(result.ToCharArray());
		}
		public static int coachSignUp (string email,string name,string password,string game=null)
		{
			string currentToken = null;
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,string> result = null;
			container decoder = new container();
			string deviceID = SystemInfo.deviceUniqueIdentifier;
			if (game == null) {
				if (getGameToken () == null) {
					return -1;
				} else {
					currentToken = getGameToken();
				}
			} else {
				currentToken = game;
			}
			command.Add("access_token",currentToken);
			print(currentToken+" "+email+" "+name+" "+password);
			command.Add("name",name);
			command.Add("email",email);
			command.Add("password",password);
			command.Add("udid",deviceID);
			string tmp = request.sendRequest(command,messageType.MESSAGETYPE_COACH_SIGN_UP);
			
			decoder.append(tmp);
			print(decoder.buffer);
			decoder.formBinary();
			result = decoder.formDictionary();
			if(result.ContainsKey("auth_token"))
			{
				setAuthToken(result["auth_token"]);
				setName(name);
				setEmail(email);
				setDeviceID(deviceID);
			}
			else
				print("Can't find key from result");
			print(senseix.authToken);
			
			return 0;
		}
		public static int coachLogin (string login,string password,string game=null)
		{

			string currentToken = null;
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,string> result = null;
			container decoder =new container();
			
			if (game == null) {
				if (getGameToken () == null) {
					return -1;
				} else {
					currentToken = getGameToken();
				}
			} else {
				currentToken = game;
			}
			command.Add("access_token",currentToken);
			print(currentToken+" "+login+" "+password);
			command.Add("login",login);
			command.Add("password",password);
			string tmp = request.sendRequest(command,messageType.MESSAGETYPE_COACH_SIGN_IN);
			
			decoder.append(tmp);
			print(decoder.buffer);
			decoder.formBinary();
			result = decoder.formDictionary();
			if(result.ContainsKey("auth_token"))
			{	
				setAuthToken(result["auth_token"]);
				
			}
			else
				print("Can't find key from result");
			print(senseix.authToken);
			
			return 0;

		}
		public static int coachLogout (string game=null)
		{
			string currentToken = null;
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,string> result = null;

			if (game == null) {
				if (getGameToken () == null) {
					return -1;
				} else {
					currentToken = getGameToken();
				}
			} else {
				currentToken = game;
			}
			command.Add("access_token",currentToken);
			command.Add("auth_token",senseix.authToken);
			print(request.sendRequest(command,messageType.MESSAGETYPE_COACH_SIGN_OUT));
			return 0;
		
		}
		public static int createPlayer (string playerName)
		{
			string currentToken = null;
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,string> result = null;
			if(senseix.getGameToken() == null)
				return -1;
			else 
				currentToken = senseix.getGameToken();
			command.Add("access_token",currentToken);
			command.Add("auth_token",senseix.authToken);
			command.Add("name",playerName);
			print(request.sendRequest(command,messageType.MESSAGETYPE_PLAYER_CREATE));
			return 0;
		}
		public static int getPlayer ()
		{
			string currentToken = null;
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,object> result = null;
			container decoder = new container();
			if(senseix.getGameToken() == null)
				return -1;
			else 
				currentToken = senseix.getGameToken();
			command.Add("access_token",currentToken);
			command.Add("auth_token",senseix.authToken);
			string tmp = request.sendRequest(command,messageType.MESSAGETYPE_PLAYER_INDEX);
			decoder.append(tmp);
			print(tmp);
			decoder.formBinary();
			result = decoder.formObjectDictionary();
			Queue first = (Queue)result["objects"];
			while(first.Count != 0)
			{
				Dictionary<string,string> tester = (Dictionary<string,string>)first.Dequeue();
				print(tester["name"]);
			}
			return 0;
		}
		public static int developerLogin (string login,string password,string game = null)
		{
			string currentToken = null;
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,string> result = null;
			container decoder = new container();
			if (game == null) {
				if (getGameToken () == null) {
					return -1;
				} else {
					currentToken = getGameToken();
				}
			} else {
				currentToken = game;
			}
			command.Add("access_token",currentToken);
			print(currentToken+" "+login+" "+password);
			command.Add("login",login);
			command.Add("password",password);
			string tmp = request.sendRequest(command,messageType.MESSAGETYPE_DEVEL_SIGN_IN);
			decoder.append(tmp);
			print(decoder.buffer);
			decoder.formBinary();
			result = decoder.formDictionary();
			if(result.ContainsKey("auth_token"))
			{	
				setAuthToken(result["auth_token"]);
				
			}
			else
				print("Can't find key from result");
			print(senseix.authToken);
			
			return 0;
		}
		public static int developerLogout ()
		{
			string currentToken = null;
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,string> result = null;

			command.Add("auth_token",senseix.authToken);
			print(request.sendRequest(command,messageType.MESSAGETYPE_DEVEL_SIGN_OUT));
			return 0;
		
		}
		//integer returned to know error number
		public static int initSenseix(string gameToken,int rankNum = 10)
		{
			int ret = 0;
			if (senseix.gameToken != null)
			{
				print("exit token exist");
				return -1;
			}
			senseix.gameToken = gameToken;
			senseix.rankNum = rankNum;
			
			return 0;

		}
		public static int getMyScore()
		{
			return me.getUsrScore ();
		}
		public static int getMyRank()
		{
			return me.getUsrRank ();
		}
		public static void pullMyInfo()
		{
			me.pullUsrInfo ();
		}

		public static void pushMyInfo()
		{
			me.pushUsrInfo ();
		}
		private static bool isGameTokenValid(string gameToken)
		{
			//What is rule to determine valid?

			//send REQUEST
			return true;
		}

		public static void pullProblemQ(int problem_id)
		{

		}

		public static void pushProblemA(int problem_id,string answer,string duration,bool correctness)
		{

		}
	}
	
	//some old code
	//ORIGIN BEG
	/*
	string currentToken = null;
	container message = new container ();

	if (game == null) {
		if (getGameToken () == null) {
			return -1;
		} else {
			currentToken = getGameToken();
		}
	} else {
		currentToken = game;
	}
	

	message.init ();
	message.addFieldDeli ();
	message.append ("auth_token=");
	message.append (authToken);
	
	message.addFieldDeli ();
	message.append ("access_token=");
	message.append (currentToken);

	message.addFieldDeli ();
	message.append ("player");
	message.addKeyDeli ();
	message.append ("name");
	message.addValueDeli ();
	message.append ("chengthree");

	print(message.formBinary ());
	print(message.buffer.ToString());
	string tmp = null;
	WWWForm form = new WWWForm();
	WWW recvResult =new WWW ("http://senseix.herokuapp.com/v1/players/create",message.binary);
	while(!recvResult.isDone && string.IsNullOrEmpty(recvResult.error))
	{
		//display some waiting sign
	}
	if (!string.IsNullOrEmpty (recvResult.error))
		tmp = recvResult.error+"error";
	else
		tmp = recvResult.text;
	
	print(tmp);
	*/
	//ORIGIN END