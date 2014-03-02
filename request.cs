using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

	public class request
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

		public static string sendRequest(Dictionary<string,string> dictionary,int type)
		{
			string url = null;
			int postget = 0;	//post is 1 and get is 2
			Dictionary<string,object> data = new Dictionary<string, object>();
			var post = new WWWForm();
			
			container message = new container ();
			
			
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
				//data.Add ("url", url);
				break;
			case messageType.MESSAGETYPE_LEADERBOARD_PULL:
				url = messageType.MESSAGETYPE_LEADERBOARD_PULL_URL;
				//data.Add ("url", url);
				data.Add ("page",dictionary["page"]);
				break;
			}
			if(postget == 0)
				return "ERROR: POST or GET not specified";
			if(postget == 1)
			{
				message.formBinary();
				string tmp = null;
				WWW recvResult =new WWW (url,message.binary);
				while(!recvResult.isDone && string.IsNullOrEmpty(recvResult.error))
				{
					//display some waiting sign
				}
				if (!string.IsNullOrEmpty (recvResult.error))
					tmp = recvResult.error;
				else
					tmp = recvResult.text;
				//return (tmp+ " "+message.buffer.ToString());
				return tmp;
			}
			else
			{
				string tmp = null;
				WWW recvResult =new WWW (message.buffer.ToString());
				while(!recvResult.isDone && string.IsNullOrEmpty(recvResult.error))
				{
					//display some waiting sign
				}
				if (!string.IsNullOrEmpty (recvResult.error))
					tmp = recvResult.error;
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
