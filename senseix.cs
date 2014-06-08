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
	public class problemType
	{
		public const int MATH = 0;
	}
	public class messageType
	{
		//CLASS: messageType
		//Each message has its now message type id and api url, url may overlap
		//MESSSAGES TYPES
		public const int MESSAGETYPE_USER_ADD = 0;
		public const int MESSAGETYPE_COACH_SIGN_UP = 1;
		public const int MESSAGETYPE_COACH_SIGN_IN = 2;
		public const int MESSAGETYPE_COACH_SIGN_OUT = 3;	//info of profile
		public const int MESSAGETYPE_COACH_PUSH_UID = 4;	//info of profile
		
		public const int MESSAGETYPE_DEVEL_SIGN_IN = 11;
		public const int MESSAGETYPE_DEVEL_SIGN_OUT = 12;

		public const int MESSAGETYPE_PLAYER_CREATE = 21;
		public const int MESSAGETYPE_PLAYER_INDEX = 22;
		
		public const int MESSAGETYPE_LEADERBOARD_PULL = 61;

		public const int MESSAGETYPE_PROBLEM_PULL = 31;
		public const int MESSAGETYPE_PROBLEM_PUSH = 32;

		public const int MESSAGETYPE_MECHANIC_GET = 41; 
		public const int MESSAGETYPE_MECHANIC_POST = 42; 

		public const int MESSAGETYPE_FRIEND_PULL = 51; 
		public const int MESSAGETYPE_FRIEND_CREATE = 52; 
		public const int MESSAGETYPE_FRIEND_DELETE = 53;
		


		//API URLS
		public const string MESSAGETYPE_USER_ADD_URL = "http://senseix.herokuapp.com/v1/coaches";
		public const string MESSAGETYPE_COACH_SIGN_UP_URL = "http://senseix.herokuapp.com/v1/coaches/";
		public const string MESSAGETYPE_COACH_SIGN_IN_URL = "http://senseix.herokuapp.com/v1/coaches/sign_in";
		public const string MESSAGETYPE_COACH_SIGN_OUT_URL = "http://senseix.herokuapp.com/v1/coaches/sign_out";	//info of profile
		public const string MESSAGETYPE_DEVEL_SIGN_IN_URL = "http://senseix.herokuapp.com/v1/developers/sign_in";
		public const string MESSAGETYPE_DEVEL_SIGN_OUT_URL = "http://senseix.herokuapp.com/v1/developers/sign_out";
		
		public const string MESSAGETYPE_PLAYER_CREATE_URL = "http://senseix.herokuapp.com/v1/players/create";
		public const string MESSAGETYPE_PLAYER_INDEX_URL = "http://senseix.herokuapp.com/v1/players/index";
		
		public const string MESSAGETYPE_LEADERBOARD_PULL_URL = "http://senseix.herokuapp.com/v1/applications/leaderboard/show";

		public const string MESSAGETYPE_PROBLEM_PULL_URL = "http://senseix.herokuapp.com/v1/problems";
		public const string MESSAGETYPE_PROBLEM_PUSH_URL = "http://senseix.herokuapp.com/v1/problems/update";
		
		public const string MESSAGETYPE_MECHANIC_POST_URL = "http://senseix.herokuapp.com/v1/applications/mechanic";
		public const string MESSAGETYPE_MECHANIC_GET_URL = "http://senseix.herokuapp.com/v1/applications/mechanic";
		public const string MESSAGETYPE_FRIEND_PULL_URL = "http://senseix.herokuapp.com/v1/players/friends";
		public const string MESSAGETYPE_FRIEND_CREATE_URL = "http://senseix.herokuapp.com/v1/players/friends/create";
		public const string MESSAGETYPE_FRIEND_DELETE_URL = "http://senseix.herokuapp.com/v1/players/friends/delete";
			
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
		public static int id = 0;
		public static string name = null;
		public static string email = null;
		private static string deviceId = null;
		private static Queue playerQ = new Queue();
		public static ArrayList playerA = new ArrayList();
		private static Queue problemQ = new Queue();
		private static ArrayList problemA = new ArrayList();
		private static leaderboard gameLeaderboard = new leaderboard();
		private static heavyUser me = new heavyUser();
		private static string utf8hdr = "utf8=%E2%9C%93";
		public static bool inSession = false;
		public static string str1="";
		public static string str2="";
		public static string str3="";
		public senseix ()
		{
		}
		public static string getAuthToken()
		{
			return senseix.authToken;
		}
		public static string getGameToken()
		{
			return senseix.gameToken;
		}
		public static int getProfileID()
		{
			return id;
		}
		public static void setProfileID(int newID)
		{
			id = newID;
		}
		public static void setAuthToken(string result)
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
		public static void selectProfile(int index)
		{
			print ("select " + index + " count " + playerA.Count);
			name = ((heavyUser)playerA [index]).name;
			email = ((heavyUser)playerA [index]).email;
			id = ((heavyUser)playerA [index]).id;
		}
		public static int coachUidPush()
		{
			string currentToken = null;
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,string> result = null;
			container decoder = new container();
			//string deviceID = SystemInfo.deviceUniqueIdentifier;
			string deviceID = "weruh878gb";
			command.Add("access_token",getGameToken());
			command.Add("udid",deviceID);
			string tmp = request.sendRequest(command,messageType.MESSAGETYPE_COACH_PUSH_UID);
			
			decoder.append(tmp);
			print(decoder.buffer);
			decoder.formBinary();
			result = decoder.formDictionary();
			if (result.ContainsKey ("sucess")) 
			{
				if(result["sucess"].Equals("false"))
					return -1;
			}
			if(result.ContainsKey("auth_token"))
			{
				setAuthToken(result["auth_token"]);
				//setName(name);
				//setEmail(email);
				//setDeviceID(deviceID);
			}
			else
			{	
				print("Can't find key from result");
				return -2;
			}
			print(senseix.authToken);
			saveAuthToken ();
			inSession = true;
			return 0;
		}
		public static int coachSignUp (string email,string name,string password,string game=null)
		{
			string currentToken = null;
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,string> result = null;
			container decoder = new container();
			//string deviceID = SystemInfo.deviceUniqueIdentifier;
		string deviceID = "weruh878gb";
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
			if (result.ContainsKey ("sucess")) 
			{
				if(result["sucess"].Equals("false"))
					return -1;
			}
			if(result.ContainsKey("auth_token"))
			{
				setAuthToken(result["auth_token"]);
				setName(name);
				setEmail(email);
				setDeviceID(deviceID);
			}
			else
			{	
				print("Can't find key from result");
				return -2;
			}
			print(senseix.authToken);
			saveAuthToken ();
			inSession = true;
			return 0;
		}
		public static int coachLogin (string login,string password,string game=null)
		{

			string currentToken = null;
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,string> result = null;
			container decoder =new container();
			string deviceID = "weruh878gb";
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
			command.Add("udid",deviceID);
			string tmp = request.sendRequest(command,messageType.MESSAGETYPE_COACH_SIGN_IN);
			
			//DEBUG
			print ("[DEBUG] result: "+tmp);

			decoder.append(tmp);
			print(decoder.buffer);
			decoder.formBinary();
			result = decoder.formDictionary();
			if (result.ContainsKey ("sucess")) 
			{
				if(result["sucess"].Equals("false"))
					return -1;
			}
			if(result.ContainsKey("auth_token"))
			{	
				setAuthToken(result["auth_token"]);
				
			}
			else
			{	
				print("Can't find key from result");
				return -1;
			}
			print(senseix.authToken);
			saveAuthToken ();
			inSession = true;
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
			//DEBUG
			print(request.sendRequest(command,messageType.MESSAGETYPE_COACH_SIGN_OUT));
			cleanData ();
			inSession = false;
			return 0;
		
		}
		public static int createPlayer (string playerName)
		{
			string currentToken = null;
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,string> result = null;
			container decoder = new container();
			if(senseix.getGameToken() == null)
				return -1;
			else 
				currentToken = senseix.getGameToken();
			command.Add("access_token",currentToken);
			command.Add("auth_token",senseix.authToken);
			command.Add("name",playerName);
			//DEBUG
			string tmp = request.sendRequest(command,messageType.MESSAGETYPE_PLAYER_CREATE);

			if (tmp.Equals ("error")) 
			{
				print("[DEBUG] Found error in request, return -1");
				return -1;
			}

			decoder.append(tmp);
			print(decoder.buffer);
			decoder.formBinary();
			result = decoder.formDictionary();			

			if (result.ContainsKey ("sucess")) 
			{
				if(result["sucess"].Equals("false"))
					return -2;
			}
			return 0;
		}
		public static Queue getPlayer ()
		{
			string currentToken = null;
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,object> result = null;
			container decoder = new container();
			if(senseix.getGameToken() == null)
				return null;
			else 
				currentToken = senseix.getGameToken();
			command.Add("access_token",currentToken);
			command.Add("auth_token",senseix.authToken);
			string tmp = request.sendRequest(command,messageType.MESSAGETYPE_PLAYER_INDEX);
			
			if (tmp.Equals ("error")) 
			{
				print("[DEBUG] Found error in request, return -1");
				return null;
			}
			//DEBUG
			print ("[DEBUG] result: "+tmp);

			decoder.append(tmp);
			print(tmp);
			decoder.formBinary();
			result = decoder.formObjectDictionary();
			if (result == null)
				return null;
			if (!result.ContainsKey ("objects"))
				return null;
			Queue first = (Queue)result["objects"];
			if (first.Count == 0)
				return null;
			if(playerQ == null)
				playerQ = new Queue();
			else
				playerQ.Clear();
			if (playerA == null)
				playerA = new ArrayList ();
			else
				playerA.Clear();	

			while(first.Count != 0)
			{
				Dictionary<string,string> tester = (Dictionary<string,string>)first.Dequeue();
				playerQ.Enqueue(new heavyUser(tester["id"],tester["email"],tester["name"],tester["age"],tester["coach_id"],tester["created_at"],tester["updated_at"],tester["team_id"],tester["deleted_at"]));
				playerA.Add(new heavyUser(tester["id"],tester["email"],tester["name"],tester["age"],tester["coach_id"],tester["created_at"],tester["updated_at"],tester["team_id"],tester["deleted_at"]));
			}
		
			return playerQ;
		}
		public static ArrayList getPlayerA ()
		{
			string currentToken = null;
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,object> result = null;
			container decoder = new container();
			if(senseix.getGameToken() == null)
				return null;
			else 
				currentToken = senseix.getGameToken();
			command.Add("access_token",currentToken);
			command.Add("auth_token",senseix.authToken);
			string tmp = request.sendRequest(command,messageType.MESSAGETYPE_PLAYER_INDEX);
			
			if (tmp.Equals ("error")) 
			{
				print("[DEBUG] Found error in request, return -1");
				return null;
			}
			//DEBUG
			print ("[DEBUG] result: "+tmp);
			
			decoder.append(tmp);
			//print(tmp);
			decoder.formBinary();
			result = decoder.formObjectDictionary();
			if (result == null)
				return null;
			if (!result.ContainsKey ("objects"))
				return null;
			Queue first = (Queue)result["objects"];
			if (first.Count == 0)
				return null;
			if(playerQ == null)
				playerQ = new Queue();
			else
				playerQ.Clear();
			if (playerA == null)
				playerA = new ArrayList ();
			else
				playerA.Clear();	
			
			while(first.Count != 0)
			{
				Dictionary<string,string> tester = (Dictionary<string,string>)first.Dequeue();
			print ("============senseix debug "+tester["id"]);
				playerQ.Enqueue(new heavyUser(tester["id"],tester["email"],tester["name"],tester["age"],tester["coach_id"],tester["created_at"],tester["updated_at"],tester["team_id"],tester["deleted_at"]));
				playerA.Add(new heavyUser(tester["id"],tester["email"],tester["name"],tester["age"],tester["coach_id"],tester["created_at"],tester["updated_at"],tester["team_id"],tester["deleted_at"]));
			}
			
			return playerA;
		}
		public static Queue getCachedPlayerQ ()
		{
			return senseix.playerQ;
		}
		public static ArrayList getCachedPlayerA ()
		{
			return senseix.playerA;
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
		    if (tmp.Equals ("error")) 
			{
				print("[DEBUG] Found error in request, return -1");
				return -1;
			}
			//DEBUG
			print ("[DEBUG] result: "+tmp);

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
			inSession = true;
			return 0;
		}
		public static int developerLogout ()
		{
			string currentToken = null;
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,string> result = null;

			command.Add("auth_token",senseix.authToken);
			string tmp = request.sendRequest(command,messageType.MESSAGETYPE_DEVEL_SIGN_OUT);
			
			if (tmp.Equals ("error")) 
			{
				print("[DEBUG] Found error in request, return -1");
				return -1;
			}
			
			cleanData ();
			inSession = false;
			return 0;
		
		}
		//integer returned to know error number
		public static int initSenseixLM(string gameToken,int rankNum = 10)//legacy mode
		{
			int ret = 0;
			if (senseix.gameToken != null)
			{
				print("exit token exist");
				return -1;
			}
			senseix.gameToken = gameToken;
			senseix.rankNum = rankNum;
			
			if (tryLoadAuthToken () == 0 && loadProfileID() == 0) 
			{	
				inSession = true;
			}
			else
				inSession = false;
			return 0;
			
		}
		//should read cached uid instead of using cached auth token
		public static int initSenseix(string gameToken,int rankNum = 10)
		{
			
			int ret = 0;
			if (senseix.gameToken != null)
			{
				//print("exit token exist");
				return -1;
			}
			senseix.gameToken = gameToken;
			senseix.rankNum = rankNum;
			/*
			if (tryLoadAuthToken () == 0 && loadProfileID() == 0) 
			{	
				inSession = true;
			}
			else
				inSession = false;
				*/
			return 0;
			
		}
		/*
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
		*/
		public static Queue pullProblemQ(int count,string category,int level)
		{
			int player_id = senseix.id;
			string currentToken = null;
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,object> result = null;
			container decoder = new container();
			if(senseix.getGameToken() == null)
				return null;
			else 
				currentToken = senseix.getGameToken();
			command.Add("access_token",currentToken);
			command.Add("auth_token",senseix.authToken);
			command.Add("player_id",player_id.ToString());
			command.Add("count",count.ToString());
			command.Add("level",level.ToString());
			command.Add("category",category);
			string tmp = request.sendRequest(command,messageType.MESSAGETYPE_PROBLEM_PULL);
			if (tmp.Equals ("error")) 
			{
				print("[DEBUG] Found error in request, return -1");
				return null;
			}
			//DEBUG
			print ("[DEBUG] result: "+tmp);
			
			StringBuilder tmpBuilder = new StringBuilder();
			tmpBuilder.Append("{\"problems\":\"");
			tmpBuilder.Append(tmp);
			tmpBuilder.Append("\"}");
			decoder.append(tmpBuilder.ToString());
			print(tmpBuilder.ToString());
			decoder.formBinary();
			result = decoder.formObjectDictionary();
			if (result == null)
				return null;
			if (!result.ContainsKey ("objects"))
				return null;
			Queue first = (Queue)result["objects"];
			if (first.Count == 0)
				return null;
			if(problemQ == null)
				problemQ = new Queue();
			else
				problemQ.Clear();
			while(first.Count != 0)
			{
				Dictionary<string,string> tester = (Dictionary<string,string>)first.Dequeue();
				problemQ.Enqueue(new problem(tester["content"],tester["category"],tester["level"],Convert.ToInt32 (tester["id"]),tester["answer"]));
			}		
			return problemQ;			
		}
		public static int readProblemFromStr(int slot)
		{
			string slotname = "problem00";
			if (slot >= 0)
				slotname = "problem0" + slot.ToString ();
			int player_id = senseix.id;
			string currentToken = null;
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,object> result = null;
			container decoder = new container();
			if (!PlayerPrefs.HasKey (slotname))
				return -1;
			string tmp = PlayerPrefs.GetString (slotname);
			if (tmp.Equals ("error")) 
			{
				print("[DEBUG] Found error in request, return -1");
				return -2;
			}
			print ("[DEBUG] result: "+tmp);
			StringBuilder tmpBuilder = new StringBuilder();
			tmpBuilder.Append("{\"problems\":\"");
			tmpBuilder.Append(tmp);
			tmpBuilder.Append("\"}");
			decoder.append(tmpBuilder.ToString());
			print(tmpBuilder.ToString());
			decoder.formBinary();
			result = decoder.formObjectDictionary();
			if (result == null)
				return -3;
			if (!result.ContainsKey ("objects"))
				return -4;
			Queue first = (Queue)result["objects"];
			if (first.Count == 0)
				return -5;
			if(problemQ == null)
				problemQ = new Queue();
			else
				problemQ.Clear();
			while(first.Count != 0)
			{
				Dictionary<string,string> tester = (Dictionary<string,string>)first.Dequeue();
				problemQ.Enqueue(new problem(tester["content"],tester["category"],tester["level"],Convert.ToInt32 (tester["id"]),tester["answer"]));
			print("id "+tester["id"]+"content "+tester["content"]+"answer "+tester["answer"]+"category "+tester["category"]+"level "+tester["level"]);
			}		
			return 0;	
		}
		public static string pullProblemQStr(int count,string category,int level)
		{
			int player_id = senseix.id;
			string currentToken = null;
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,object> result = null;
			container decoder = new container();
			if(senseix.getGameToken() == null)
				return null;
			else 
				currentToken = senseix.getGameToken();
			command.Add("access_token",currentToken);
			command.Add("auth_token",senseix.authToken);
			command.Add("player_id",player_id.ToString());
			command.Add("count",count.ToString());
			command.Add("level",level.ToString());
			command.Add("category",category);
			print ("============debug: player_id "+player_id);
			print ("============debug:  "+player_id);
			string tmp = request.sendRequest(command,messageType.MESSAGETYPE_PROBLEM_PULL);
			if (tmp.Equals ("error")) 
			{
				print("[DEBUG] Found error in request, return -1");
				return null;
			}
			//DEBUG
			return tmp;			
		}
		public static void pushProblemA(int problem_id,int duration,bool correctness,int tries,int game_difficulty,string answer)
		{
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,object> result = null;
			container decoder = new container();
			int player_id = senseix.id;
			command.Add("access_token",senseix.getGameToken());
			command.Add("auth_token",senseix.authToken);
			command.Add("player_id",player_id.ToString());
			command.Add("answer",answer);
			command.Add("problem_id",problem_id.ToString());
			command.Add("correct",correctness.ToString());
			command.Add ("game_difficulty",game_difficulty.ToString());
			command.Add ("tries",tries.ToString());
			command.Add ("duration",duration.ToString());
			string tmp = request.sendRequest(command,messageType.MESSAGETYPE_PROBLEM_PUSH);
			if (tmp.Equals ("error")) 
			{
				print("[DEBUG] Found error in request, return -1");
				//return null;
			}
			//DEBUG
			print ("[DEBUG] result: "+tmp);
		}
		public static bool pullLeaderboard(int page)
		{
			int player_id = senseix.id;
			string currentToken = null;
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,object> result = null;
			container decoder = new container();
			if(senseix.getGameToken() == null)
				return false;
			else 
				currentToken = senseix.getGameToken();
			command.Add("access_token",currentToken);
			command.Add("auth_token",senseix.authToken);
			command.Add("page",page.ToString());
			string tmp = request.sendRequest(command,messageType.MESSAGETYPE_LEADERBOARD_PULL);
			if (tmp.Equals ("error")) 
			{
				print("[DEBUG] Found error in request, return -1");
				return false;
			}
			//DEBUG
			print ("[DEBUG] result: "+tmp);
			
			StringBuilder tmpBuilder = new StringBuilder();
			tmpBuilder.Append("{\"leaders\":\"");
			tmpBuilder.Append(tmp);
			tmpBuilder.Append("\"}");
			decoder.append(tmpBuilder.ToString());
			print(tmpBuilder.ToString());
			decoder.formBinary();
			result = decoder.formObjectDictionary();
			Queue first = (Queue)result["objects"];
			leaderboard.clearEntry ();
			while(first.Count != 0)
			{
				Dictionary<string,string> tester = (Dictionary<string,string>)first.Dequeue();
			print (tester["rank"]);
				leaderboard.addEntry(tester["member"],tester["rank"],tester["score"],tester["member_data"]);
			}	
			return true;

		}
		public static Queue getCachedProblemQ()
		{
			return problemQ;
		}
		public static bool postMessage(int player_id,string data)
		{
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,object> result = null;
			container decoder = new container();
			command.Add("access_token",senseix.getGameToken());
			command.Add("auth_token",senseix.authToken);
			command.Add("player_id",player_id.ToString());
			command.Add ("data",data);
			string tmp = request.sendRequest(command,messageType.MESSAGETYPE_MECHANIC_POST);
			if (tmp.Equals ("error")) 
			{
				print("[DEBUG] Found error in request, return -1");
				return false;
			}
			//DEBUG
			print ("[DEBUG] result: "+tmp);
			return true;
		}
		public static bool getMessage()
		{
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,object> result = null;
			container decoder = new container();
			int player_id = senseix.id;
			command.Add("access_token",senseix.getGameToken());
			command.Add("auth_token",senseix.authToken);
			command.Add("player_id",player_id.ToString());
			string tmp = request.sendRequest(command,messageType.MESSAGETYPE_MECHANIC_GET);
			if (tmp.Equals ("error")) 
			{
				//print("[DEBUG] Found error in request, return -1");
				return false;
			}
			//DEBUG
			//print ("[DEBUG] result: "+tmp);
			return true;	
		}
		public static Queue friendsPull()
		{
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,object> result = null;
			container decoder = new container();
			int player_id = senseix.id;
			command.Add("access_token",senseix.getGameToken());
			command.Add("auth_token",senseix.authToken);
			string tmp = request.sendRequest(command,messageType.MESSAGETYPE_FRIEND_PULL);
			if (tmp.Equals ("error")) 
			{
				//print("[DEBUG] Found error in request, return -1");
				return null;
			}
			//DEBUG
			//print ("[DEBUG] result: "+tmp);
			return null;	
		}
	/*
		public static bool friendsCreate(int friend_id)
		{
			return true;
		}	
		public static bool friendsCreate(int friend_id)
		{
			return true;
		}
		*/
		public static bool checkAnswer(string answerStr,problem p)
		{
			int answerInt = Convert.ToInt32 (answerStr);
			if (answerInt == p.mathResult ())
				return true;
			return false;
		}
		public static bool checkAnswer(int answerInt,problem p)
		{
			if (answerInt == p.mathResult ())
				return true;
			return false;
		}
		public static void saveProfileID()
		{
			if (id != 0) 
			{
				print ("Profile ID saved: " + id.ToString ());
				PlayerPrefs.SetInt ("data01", id);
				PlayerPrefs.Save();
			} 
			else
				print ("Failed to save profile id: local id is 0 now");
		}
		public static int loadProfileID()
		{
			if (PlayerPrefs.HasKey ("data01")) 
			{
				int cachedID = PlayerPrefs.GetInt ("data01", 0);
				if (cachedID == 0) {
						str1="Loading profile id failed: saved profile id is 0";
						print ("Loading profile id failed: saved profile id is 0");
						return -1;
				} 
				else
				{	
					str1="Loading profile failed";
					senseix.setProfileID (cachedID);
					return 0;
				}
			}
			else 
			{
				print ("There is not profile id data saved");
				return -1;
			}
		}
		private static void saveAuthToken()
		{
			if(senseix.getAuthToken() != null)
			{
				print("Data Saved: " + senseix.getAuthToken());
				PlayerPrefs.SetString("data00",senseix.getAuthToken());
				PlayerPrefs.Save();
			}
			else
			{
				print("You have not signed in.");
			}
		}
		private static int loadAuthToken()
		{
			if(PlayerPrefs.HasKey("data00"))
			{
				string authToken = PlayerPrefs.GetString("data00","null");
				senseix.setAuthToken(authToken);
				print("Loaded token: " + senseix.getAuthToken());
				return 0;
			}
			else
			{
				print ("No save found, or save data invalid.");
				return -1;
			}
		}
		private static int tryLoadAuthToken()
		{
			if (loadAuthToken() == 0) 
			{
				if(getPlayer () == null)
				{
					return -1;
				}
				return 0;
			} 
			else 
				return -2;
		}
		public static void cleanData()
		{
			print ("cleaned data");
			PlayerPrefs.DeleteAll ();
		}

		//Multi-Threading
		public static container pullProblemQMT(int count,string category,int level)
		{
			int player_id = senseix.id;
			string currentToken = null;
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,object> result = null;
			container decoder = new container();
			if(senseix.getGameToken() == null)
				return null;
			else 
				currentToken = senseix.getGameToken();
			command.Add("access_token",currentToken);
			command.Add("auth_token",senseix.authToken);
			command.Add("player_id",player_id.ToString());
			command.Add("count",count.ToString());
			command.Add("level",level.ToString());
			command.Add("category",category);
			return request.prepareRequest(command,messageType.MESSAGETYPE_PROBLEM_PULL);
		}
		public static Queue decodeProblemQMT(string recv)
		{
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,object> result = null;
			container decoder = new container();
			StringBuilder tmpBuilder = new StringBuilder();
			tmpBuilder.Append("{\"problems\":\"");
			tmpBuilder.Append(recv);
			tmpBuilder.Append("\"}");
			decoder.append(tmpBuilder.ToString());
			//print(tmpBuilder.ToString());
			decoder.formBinary();
			result = decoder.formObjectDictionary();
			if (result == null)
				return null;
			if (!result.ContainsKey ("objects"))
				return null;
			Queue first = (Queue)result["objects"];
			if (first.Count == 0)
				return null;
			if(problemQ == null)
				problemQ = new Queue();
			else
				problemQ.Clear();
			while(first.Count != 0)
			{
				Dictionary<string,string> tester = (Dictionary<string,string>)first.Dequeue();
			print ("Enqueue " + tester["answer"]);
			problemQ.Enqueue(new problem(tester["content"],tester["category"],tester["level"],Convert.ToInt32 (tester["id"]),tester["answer"]));
			}		
			
			return problemQ;	
		}
		public static container pushProblemAMT(int problem_id,int duration,bool correctness,int tries,int game_difficulty,string answer)
		{
			Dictionary<string,string> command = new Dictionary<string, string>();
			Dictionary<string,object> result = null;
			container decoder = new container();
			int player_id = senseix.id;
			command.Add("access_token",senseix.getGameToken());
			command.Add("auth_token",senseix.authToken);
			command.Add("player_id",player_id.ToString());
			command.Add("answer",answer);
			command.Add("problem_id",problem_id.ToString());
			command.Add("correct",correctness.ToString());
			command.Add ("game_difficulty",game_difficulty.ToString());
			command.Add ("tries",tries.ToString());
			command.Add ("duration",duration.ToString());
			return request.prepareRequest(command,messageType.MESSAGETYPE_PROBLEM_PUSH);
		}
		public static void decodePushProblemAMT(string recv)
		{
			
		}
		private static void sendMessage(container message)
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