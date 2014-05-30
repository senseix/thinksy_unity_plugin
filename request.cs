using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

	public class request:MonoBehaviour
	{
		//Should be const string that stand for our server
		//private static identity localUsr = new identity();
		public static Dictionary<String,respond> requests = new Dictionary<String,respond>();
		//The function to increase and renew transaction id is needed
		private static int transactionID = 0;
		private static Dictionary<int,int> TIDnTYPE= new Dictionary<int,int>(); //<TID,TYPE>
		public request ()
		{
		}
		/*
		public static string sendRequestMT(WWW recvResult)
		{
			if(message.postget == 0)
				return "ERROR: POST or GET not specified";
			if(message.postget == 1)
			{
				//print("[DEBUG] Raw message sent out:" + message.buffer);
				string tmp = null;

				while(!recvResult.isDone && string.IsNullOrEmpty(recvResult.error))
				{
					//display some waiting sign
				}
				if (!string.IsNullOrEmpty (recvResult.error))
					tmp = recvResult.error + message.buffer.ToString();
				else
					tmp = recvResult.text;
				//return (tmp+ " "+message.buffer.ToString());
				return tmp;
			}
			else if(message.postget == 2)
			{
				string tmp = null;
				while(!recvResult.isDone && string.IsNullOrEmpty(recvResult.error))
				{
					//display some waiting sign
				}
				if (!string.IsNullOrEmpty (recvResult.error))
					tmp = "error";
				else
					tmp = recvResult.text;
				return tmp;
			}
			return "ERROR: No branch got hit";
		}
		*/
		public static container prepareRequest(Dictionary<string,string> dictionary,int type)
		{
			string url = null;
			int postget = 0;	//post is 1 and get is 2
			Dictionary<string,object> data = new Dictionary<string, object>();
			var post = new WWWForm();
			
			container message = new container ();
			string deviceID = SystemInfo.deviceUniqueIdentifier;
			
			switch(type)
			{
				/*
				case messageType.MESSAGETYPE_USER_ADD:
					url = messageType.MESSAGETYPE_USER_ADD_URL;
					//data.Add ("url", url);
					data.Add ("email", message ["email"]);
					data.Add ("name", message ["name"]);
					data.Add ("password", message ["password"]);
					data.Add ("udid",message["udid"]);
					break;
				*/

			case messageType.MESSAGETYPE_COACH_SIGN_UP:
				url = messageType.MESSAGETYPE_COACH_SIGN_UP_URL;
				postget = 1;
				message.init ();
				
				message.addFieldDeli ();
				message.append ("access_token=");
				message.append (dictionary["access_token"]);
				
				message.addFieldDeli ();
				message.append ("coach");
				message.addKeyDeli ();
				message.append ("email");
				message.addValueDeli ();
				message.append (dictionary["email"]);
				
				message.addFieldDeli ();
				message.append ("coach");
				message.addKeyDeli ();
				message.append ("name");
				message.addValueDeli ();
				message.append (dictionary["name"]);
				
				message.addFieldDeli ();
				message.append ("coach");
				message.addKeyDeli ();
				message.append ("password");
				message.addValueDeli ();
				message.append (dictionary["password"]);
				
				message.addFieldDeli ();
				message.append ("device");
				message.addKeyDeli ();
				message.append ("udid");
				message.addValueDeli ();
				message.append (dictionary["udid"]);
				
				break;
			case messageType.MESSAGETYPE_COACH_SIGN_IN:
				url = messageType.MESSAGETYPE_COACH_SIGN_IN_URL;
				postget = 1;
				message.init ();
				
				message.addFieldDeli ();
				message.append ("access_token=");
				message.append (dictionary["access_token"]);
				
				message.addFieldDeli ();
				message.append ("coach");
				message.addKeyDeli ();
				message.append ("email");
				message.addValueDeli ();
				message.append (dictionary["login"]);
				
				message.addFieldDeli ();
				message.append ("coach");
				message.addKeyDeli ();
				message.append ("password");
				message.addValueDeli ();
				message.append (dictionary["password"]);

				message.addFieldDeli ();
				message.append ("device");
				message.addKeyDeli ();
				message.append ("udid");
				message.addValueDeli ();
				message.append (dictionary["udid"]);
				
				
				break;
			case messageType.MESSAGETYPE_COACH_SIGN_OUT:
				url = messageType.MESSAGETYPE_COACH_SIGN_OUT_URL;
				
				postget = 1;
				
				message.init ();
				
				message.append ("&_method=DELETE");
				
				message.addFieldDeli ();
				message.append ("access_token=");
				message.append (dictionary["access_token"]);
				
				message.addFieldDeli ();
				message.append ("auth_token=");
				message.append (dictionary["auth_token"]);
				
				break;
			case messageType.MESSAGETYPE_DEVEL_SIGN_IN:
				url = messageType.MESSAGETYPE_DEVEL_SIGN_IN_URL;
				postget = 1;
				
				message.init ();
				
				message.addFieldDeli ();
				message.append ("developer");
				message.addKeyDeli ();
				message.append ("email");
				message.addValueDeli ();
				message.append (dictionary["login"]);
				
				message.addFieldDeli ();
				message.append ("developer");
				message.addKeyDeli ();
				message.append ("password");
				message.addValueDeli ();
				message.append (dictionary["password"]);
				
				break;
			case messageType.MESSAGETYPE_DEVEL_SIGN_OUT:
				url = messageType.MESSAGETYPE_DEVEL_SIGN_OUT_URL;
				
				postget = 1;
				
				message.init ();
				
				message.append ("&_method=DELETE");
				
				message.addFieldDeli ();
				message.append ("auth_token=");
				message.append (dictionary["auth_token"]);
				
				
				
				break;
			case messageType.MESSAGETYPE_PLAYER_CREATE:
				url = messageType.MESSAGETYPE_PLAYER_CREATE_URL;
				postget = 1;
				
				message.init ();
				
				message.addFieldDeli ();
				message.append ("auth_token=");
				message.append (dictionary["auth_token"]);
				
				message.addFieldDeli ();
				message.append ("access_token=");
				message.append (dictionary["access_token"]);
				
				message.addFieldDeli ();
				message.append ("device");
				message.addKeyDeli ();
				message.append ("udid");
				message.addValueDeli ();
				message.append (deviceID);
				
				message.addFieldDeli ();
				message.append ("player");
				message.addKeyDeli ();
				message.append ("name");
				message.addValueDeli ();
				message.append (dictionary["name"]);
				
				
				
				break;
			case messageType.MESSAGETYPE_PLAYER_INDEX:
				url = messageType.MESSAGETYPE_PLAYER_INDEX_URL;
				postget = 2;
				message.append(messageType.MESSAGETYPE_PLAYER_INDEX_URL);
				message.append("?");
				message.init ();
				
				Hashtable header = new Hashtable();
				message.addFieldDeli ();
				message.append ("access_token=");
				message.append (dictionary["access_token"]);
				
				message.addFieldDeli ();
				message.append ("auth_token=");
				message.append (dictionary["auth_token"]);
				
				break;
			case messageType.MESSAGETYPE_PROBLEM_PULL:
				url = messageType.MESSAGETYPE_PROBLEM_PULL_URL;
				
				postget = 2;
				message.append(messageType.MESSAGETYPE_PROBLEM_PULL_URL);
				message.append("?");
				message.init ();
				
				message.addFieldDeli ();
				message.append ("auth_token=");
				message.append (dictionary["auth_token"]);
				
				message.addFieldDeli ();
				message.append ("access_token=");
				message.append (dictionary["access_token"]);
				
				message.addFieldDeli ();
				message.append ("player_id=");
				message.append (dictionary["player_id"]);
				
				message.addFieldDeli ();
				message.append ("problem_set");
				message.addKeyDeli ();
				message.append ("count");
				message.addValueDeli ();
				message.append (dictionary["count"]);
				
				message.addFieldDeli ();
				message.append ("problem_set");
				message.addKeyDeli ();
				message.append ("category");
				message.addValueDeli ();
				message.append (dictionary["category"]);

				message.addFieldDeli ();
				message.append ("problem_set");
				message.addKeyDeli ();
				message.append ("subcategory");
				message.addValueDeli ();
				message.append ("Addition");
				
				message.addFieldDeli ();
				message.append ("problem_set");
				message.addKeyDeli ();
				message.append ("level");
				message.addValueDeli ();
				message.append (dictionary["level"]);
				/*
					message.addFieldDeli ();
					message.append ("device");
					message.addKeyDeli ();
					message.append ("udid");
					message.addValueDeli ();
					message.append (deviceID);
					*/	
				break;
			case messageType.MESSAGETYPE_PROBLEM_PUSH:
				print ("Received problem push request");
				url = messageType.MESSAGETYPE_PROBLEM_PUSH_URL;
				postget = 1;
				
				message.init ();
				message.append("&_method=PUT");
				message.addFieldDeli ();
				message.append ("auth_token=");
				message.append (dictionary["auth_token"]);
				
				message.addFieldDeli ();
				message.append ("access_token=");
				message.append (dictionary["access_token"]);
				
				message.addFieldDeli ();
				message.append ("player_id=");
				message.append (dictionary["player_id"]);
				
				message.addPushQDeli();
				message.append("problem_id");
				message.addValueDeli();
				message.append(dictionary["problem_id"]);
				
				message.addPushQDeli();
				message.append("duration");
				message.addValueDeli();
				message.append(dictionary["duration"]);
				
				message.addPushQDeli();
				message.append("correct");
				message.addValueDeli();
				message.append(dictionary["correct"]);
				
				message.addPushQDeli();
				message.append("tries");
				message.addValueDeli();
				message.append(dictionary["tries"]);
				
				message.addPushQDeli();
				message.append("game_difficulty");
				message.addValueDeli();
				message.append(dictionary["game_difficulty"]);
				
				
				
				message.addPushQDeli();
				message.append("answer");
				message.addValueDeli();
				message.append(dictionary["answer"]);
				//message.append("&player_id=40&answer_set%5B%5D%5Bproblem_id%5D=13&answer_set%5B%5D%5Bduration%5D=2&answer_set%5B%5D%5Bcorrect%5D=false&answer_set%5B%5D%5Btries%5D=1&answer_set%5B%5D%5Bgame_difficulty%5D=2&answer_set%5B%5D%5Banswer%5D=5");
				
				break;
			case messageType.MESSAGETYPE_LEADERBOARD_PULL:
				url = messageType.MESSAGETYPE_LEADERBOARD_PULL_URL;
				postget = 2;
				message.append(messageType.MESSAGETYPE_LEADERBOARD_PULL_URL);
				message.append("?");
				message.init ();
				/*
				message.addFieldDeli ();
				message.append ("auth_token=");
				message.append (dictionary["auth_token"]);
				*/
				message.addFieldDeli ();
				message.append ("access_token=");
				message.append (dictionary["access_token"]);
				
				message.addFieldDeli ();
				message.append ("page=");
				message.append (dictionary["page"]);
				/*
					message.addFieldDeli ();
					message.append ("id=");
					message.append (dictionary["id"]);
					*/
				break;
			case messageType.MESSAGETYPE_MECHANIC_POST:
				url = messageType.MESSAGETYPE_MECHANIC_POST_URL;
				postget = 1;
				
				message.init ();
				
				message.addFieldDeli ();
				message.append ("access_token=");
				message.append (dictionary["access_token"]);
				
				message.addFieldDeli ();
				message.append ("auth_token=");
				message.append (dictionary["auth_token"]);
				
				message.addFieldDeli ();
				message.append ("player_id");
				message.addKeyDeli();
				message.addValueDeli();
				message.append (dictionary["player_id"]);
				
				message.addFieldDeli ();
				message.append ("data=");
				
				message.append(dictionary["data"]);
				
				break;
			case messageType.MESSAGETYPE_MECHANIC_GET:
				url = messageType.MESSAGETYPE_MECHANIC_GET_URL;
				postget = 2;
				message.append(url);
				message.append("?");
				message.init ();
				
				message.addFieldDeli ();
				message.append ("auth_token=");
				message.append (dictionary["auth_token"]);
				
				message.addFieldDeli ();
				message.append ("access_token=");
				message.append (dictionary["access_token"]);
				
				message.addFieldDeli ();
				message.append ("player_id=");
				message.append (dictionary["player_id"]);
				
				break;	
			case messageType.MESSAGETYPE_FRIEND_PULL:
				url = messageType.MESSAGETYPE_FRIEND_PULL_URL;
				postget = 2;
				message.append(url);
				message.append("?");
				message.init ();
				
				message.addFieldDeli ();
				message.append ("auth_token=");
				message.append (dictionary["auth_token"]);
				
				message.addFieldDeli ();
				message.append ("access_token=");
				message.append (dictionary["access_token"]);
				
				break;
			}
			if (postget == 0)
				return null;
			message.url = url;
			message.postget = postget;
			return message;
		}
		public static string sendRequest(Dictionary<string,string> dictionary,int type)
		{
			string url = null;
			int postget = 0;	//post is 1 and get is 2
			Dictionary<string,object> data = new Dictionary<string, object>();
			var post = new WWWForm();
			
			container message = new container ();
			string deviceID = SystemInfo.deviceUniqueIdentifier;
			
			switch(type)
			{
			/*
			case messageType.MESSAGETYPE_USER_ADD:
				url = messageType.MESSAGETYPE_USER_ADD_URL;
				//data.Add ("url", url);
				data.Add ("email", message ["email"]);
				data.Add ("name", message ["name"]);
				data.Add ("password", message ["password"]);
				data.Add ("udid",message["udid"]);
				break;
			*/
			case messageType.MESSAGETYPE_COACH_PUSH_UID:
				url = messageType.MESSAGETYPE_COACH_SIGN_IN_URL;
				postget = 1;
				message.init ();
				
				message.addFieldDeli ();
				message.append ("access_token=");
				message.append (dictionary["access_token"]);
				
			//	message.addFieldDeli ();
			//	message.append ("coach");
				
				message.addFieldDeli ();
				message.append ("device");
				message.addKeyDeli ();
				message.append ("udid");
				message.addValueDeli ();
				message.append (dictionary["udid"]);
				
				break;
			case messageType.MESSAGETYPE_COACH_SIGN_UP:
				url = messageType.MESSAGETYPE_COACH_SIGN_UP_URL;
				postget = 1;
				message.init ();
				
				message.addFieldDeli ();
				message.append ("access_token=");
				message.append (dictionary["access_token"]);

				message.addFieldDeli ();
				message.append ("coach");
				message.addKeyDeli ();
				message.append ("email");
				message.addValueDeli ();
				message.append (dictionary["email"]);

				message.addFieldDeli ();
				message.append ("coach");
				message.addKeyDeli ();
				message.append ("name");
				message.addValueDeli ();
				message.append (dictionary["name"]);
				
				message.addFieldDeli ();
				message.append ("coach");
				message.addKeyDeli ();
				message.append ("password");
				message.addValueDeli ();
				message.append (dictionary["password"]);
				
				message.addFieldDeli ();
				message.append ("device");
				message.addKeyDeli ();
				message.append ("udid");
				message.addValueDeli ();
				message.append (dictionary["udid"]);
				
				break;
			case messageType.MESSAGETYPE_COACH_SIGN_IN:
				url = messageType.MESSAGETYPE_COACH_SIGN_IN_URL;
				postget = 1;
				message.init ();
				
				message.addFieldDeli ();
				message.append ("access_token=");
				message.append (dictionary["access_token"]);

				message.addFieldDeli ();
				message.append ("coach");
				message.addKeyDeli ();
				message.append ("email");
				message.addValueDeli ();
				message.append (dictionary["login"]);

				message.addFieldDeli ();
				message.append ("coach");
				message.addKeyDeli ();
				message.append ("password");
				message.addValueDeli ();
				message.append (dictionary["password"]);

				message.addFieldDeli ();
				message.append ("device");
				message.addKeyDeli ();
				message.append ("udid");
				message.addValueDeli ();
				message.append (dictionary["udid"]);
				

				break;
			case messageType.MESSAGETYPE_COACH_SIGN_OUT:
				url = messageType.MESSAGETYPE_COACH_SIGN_OUT_URL;
				
				postget = 1;
				
				message.init ();
				
				message.append ("&_method=DELETE");
				
				message.addFieldDeli ();
				message.append ("access_token=");
				message.append (dictionary["access_token"]);
				
				message.addFieldDeli ();
				message.append ("auth_token=");
				message.append (dictionary["auth_token"]);

				break;
			case messageType.MESSAGETYPE_DEVEL_SIGN_IN:
				url = messageType.MESSAGETYPE_DEVEL_SIGN_IN_URL;
				postget = 1;
				
				message.init ();
				
				message.addFieldDeli ();
				message.append ("developer");
				message.addKeyDeli ();
				message.append ("email");
				message.addValueDeli ();
				message.append (dictionary["login"]);

				message.addFieldDeli ();
				message.append ("developer");
				message.addKeyDeli ();
				message.append ("password");
				message.addValueDeli ();
				message.append (dictionary["password"]);
				
				break;
			case messageType.MESSAGETYPE_DEVEL_SIGN_OUT:
				url = messageType.MESSAGETYPE_DEVEL_SIGN_OUT_URL;
				
				postget = 1;
				
				message.init ();
				
				message.append ("&_method=DELETE");

				message.addFieldDeli ();
				message.append ("auth_token=");
				message.append (dictionary["auth_token"]);
				
				
				
				break;
			case messageType.MESSAGETYPE_PLAYER_CREATE:
				url = messageType.MESSAGETYPE_PLAYER_CREATE_URL;
				postget = 1;
				
				message.init ();
				
				message.addFieldDeli ();
				message.append ("auth_token=");
				message.append (dictionary["auth_token"]);
				
				message.addFieldDeli ();
				message.append ("access_token=");
				message.append (dictionary["access_token"]);
					
				message.addFieldDeli ();
				message.append ("device");
				message.addKeyDeli ();
				message.append ("udid");
				message.addValueDeli ();
				message.append (deviceID);
				
				message.addFieldDeli ();
				message.append ("player");
				message.addKeyDeli ();
				message.append ("name");
				message.addValueDeli ();
				message.append (dictionary["name"]);
				
				
				
				break;
			case messageType.MESSAGETYPE_PLAYER_INDEX:
				url = messageType.MESSAGETYPE_PLAYER_INDEX_URL;
				postget = 2;
				message.append(messageType.MESSAGETYPE_PLAYER_INDEX_URL);
				message.append("?");
				message.init ();
				
				Hashtable header = new Hashtable();
				message.addFieldDeli ();
				message.append ("access_token=");
				message.append (dictionary["access_token"]);
				
				message.addFieldDeli ();
				message.append ("auth_token=");
				message.append (dictionary["auth_token"]);
				
				break;
			case messageType.MESSAGETYPE_PROBLEM_PULL:
				url = messageType.MESSAGETYPE_PROBLEM_PULL_URL;
				
				postget = 2;
				message.append(messageType.MESSAGETYPE_PROBLEM_PULL_URL);
				message.append("?");
				message.init ();
				
				message.addFieldDeli ();
				message.append ("auth_token=");
				message.append (dictionary["auth_token"]);
				
				message.addFieldDeli ();
				message.append ("access_token=");
				message.append (dictionary["access_token"]);

				message.addFieldDeli ();
				message.append ("player_id=");
				message.append (dictionary["player_id"]);
				
				message.addFieldDeli ();
				message.append ("problem_set");
				message.addKeyDeli ();
				message.append ("count");
				message.addValueDeli ();
				message.append (dictionary["count"]);
				
				message.addFieldDeli ();
				message.append ("problem_set");
				message.addKeyDeli ();
				message.append ("category");
				message.addValueDeli ();
				message.append (dictionary["category"]);

				message.addFieldDeli ();
				message.append ("problem_set");
				message.addKeyDeli ();
				message.append ("subcategory");
				message.addValueDeli ();
				message.append ("Addition");
				
				message.addFieldDeli ();
				message.append ("problem_set");
				message.addKeyDeli ();
				message.append ("level");
				message.addValueDeli ();
				message.append (dictionary["level"]);
				/*
				message.addFieldDeli ();
				message.append ("device");
				message.addKeyDeli ();
				message.append ("udid");
				message.addValueDeli ();
				message.append (deviceID);
				*/	
				break;
			case messageType.MESSAGETYPE_PROBLEM_PUSH:
				url = messageType.MESSAGETYPE_PROBLEM_PUSH_URL;
				postget = 1;
				
				message.init ();
				message.append("&_method=PUT");
				message.addFieldDeli ();
				message.append ("auth_token=");
				message.append (dictionary["auth_token"]);
				
				message.addFieldDeli ();
				message.append ("access_token=");
				message.append (dictionary["access_token"]);
				
				message.addFieldDeli ();
				message.append ("player_id=");
				message.append (dictionary["player_id"]);
				
				message.addPushQDeli();
				message.append("problem_id");
				message.addValueDeli();
				message.append(dictionary["problem_id"]);
				
				message.addPushQDeli();
				message.append("duration");
				message.addValueDeli();
				message.append(dictionary["duration"]);

				message.addPushQDeli();
				message.append("correct");
				message.addValueDeli();
				message.append(dictionary["correct"]);

				message.addPushQDeli();
				message.append("tries");
				message.addValueDeli();
				message.append(dictionary["tries"]);

				message.addPushQDeli();
				message.append("game_difficulty");
				message.addValueDeli();
				message.append(dictionary["game_difficulty"]);


				
				message.addPushQDeli();
				message.append("answer");
				message.addValueDeli();
				message.append(dictionary["answer"]);
			//message.append("&player_id=40&answer_set%5B%5D%5Bproblem_id%5D=13&answer_set%5B%5D%5Bduration%5D=2&answer_set%5B%5D%5Bcorrect%5D=false&answer_set%5B%5D%5Btries%5D=1&answer_set%5B%5D%5Bgame_difficulty%5D=2&answer_set%5B%5D%5Banswer%5D=5");
				
				break;
			case messageType.MESSAGETYPE_LEADERBOARD_PULL:
				url = messageType.MESSAGETYPE_LEADERBOARD_PULL_URL;
				postget = 2;
				message.append(messageType.MESSAGETYPE_LEADERBOARD_PULL_URL);
				message.append("?");
				message.init ();
				
				message.addFieldDeli ();
				message.append ("auth_token=");
				message.append (dictionary["auth_token"]);
				
				message.addFieldDeli ();
				message.append ("access_token=");
				message.append (dictionary["access_token"]);
				
				message.addFieldDeli ();
				message.append ("page=");
				message.append (dictionary["page"]);
				/*
				message.addFieldDeli ();
				message.append ("id=");
				message.append (dictionary["id"]);
				*/
				break;
				case messageType.MESSAGETYPE_MECHANIC_POST:
				url = messageType.MESSAGETYPE_MECHANIC_POST_URL;
				postget = 1;
				
				message.init ();
				
				message.addFieldDeli ();
				message.append ("access_token=");
				message.append (dictionary["access_token"]);

				message.addFieldDeli ();
				message.append ("auth_token=");
				message.append (dictionary["auth_token"]);
				
				message.addFieldDeli ();
				message.append ("player_id");
				message.addKeyDeli();
				message.addValueDeli();
				message.append (dictionary["player_id"]);
				
				message.addFieldDeli ();
				message.append ("data=");
				
				message.append(dictionary["data"]);

				break;
			case messageType.MESSAGETYPE_MECHANIC_GET:
				url = messageType.MESSAGETYPE_MECHANIC_GET_URL;
				postget = 2;
				message.append(url);
				message.append("?");
				message.init ();
				
				message.addFieldDeli ();
				message.append ("auth_token=");
				message.append (dictionary["auth_token"]);
				
				message.addFieldDeli ();
				message.append ("access_token=");
				message.append (dictionary["access_token"]);
				
				message.addFieldDeli ();
				message.append ("player_id=");
				message.append (dictionary["player_id"]);

				break;	
			case messageType.MESSAGETYPE_FRIEND_PULL:
				url = messageType.MESSAGETYPE_FRIEND_PULL_URL;
				postget = 2;
				message.append(url);
				message.append("?");
				message.init ();
				
				message.addFieldDeli ();
				message.append ("auth_token=");
				message.append (dictionary["auth_token"]);
				
				message.addFieldDeli ();
				message.append ("access_token=");
				message.append (dictionary["access_token"]);
				
			break;
			}
			

			if(postget == 0)
				return "ERROR: POST or GET not specified";
			if(postget == 1)
			{
				//print("[DEBUG] Raw message sent out:" + message.buffer);
				message.formBinary();
				string tmp = null;
				WWW recvResult =new WWW (url,message.binary);
				while(!recvResult.isDone && string.IsNullOrEmpty(recvResult.error))
				{
					//display some waiting sign
				}
				if (!string.IsNullOrEmpty (recvResult.error))
					tmp = recvResult.error + message.buffer.ToString();
				else
					tmp = recvResult.text;
				//return (tmp+ " "+message.buffer.ToString());
				return tmp;
			}
			else if(postget == 2)
			{
				string tmp = null;
				WWW recvResult =new WWW (message.buffer.ToString());
				//MonoBehaviour.print(message.buffer.ToString());
				while(!recvResult.isDone && string.IsNullOrEmpty(recvResult.error))
				{
				//display some waiting sign
				}
				if (!string.IsNullOrEmpty (recvResult.error))
					tmp = "error";
				else
					tmp = recvResult.text;
				return tmp;
			}
			return "ERROR: No branch got hit";
		}
		/*
		public static respond recRespond(WWW page)
		{
			//FIXME: leave this blank for details of API
				
		}
		*/
	}
