using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

	public class respond
	{
		public respond ()
		{
		}
		public string strDataField = null;
		//Dictionary or list of dictionary

		public static Dictionary<string,string> processWWW(WWW www)
		{
			Dictionary<string,string> DataField = new Dictionary<string, string>();
			MonoBehaviour.print (www.text);

			return DataField;
		}
	}

	public class respondTest
	{
		//Predefined
		public static lightUser[] testUsers= new lightUser[15];
		public static int userSize = 12;
		public static int myIndex = -1;

		public respondTest()
		{
			testUsers [0] = new lightUser ("Adams","00001",1,60);
			testUsers [1] = new lightUser ("Boney","00002",2,70);
			testUsers [2] = new lightUser ("Ben","00003",3,80);
			testUsers [3] = new lightUser ("David","00004",4,110);

			testUsers [4] = new lightUser ("Julie","00005",5,153);
			testUsers [5] = new lightUser ("R","00012",6,214);
			testUsers [6] = new lightUser ("Dimitry","00030",7,234);
			testUsers [7] = new lightUser ("Nickle","00023",8,311);

			testUsers [8] = new lightUser ("Roma","00011",9,461);
			testUsers [9] = new lightUser ("Zombie","00007",10,472);
			testUsers [10] = new lightUser ("Ezio","00041",11,512);
			testUsers [11] = new lightUser ("ISAAC","00032",12,554);

		}
		public static lightUser[] testTopUser(int num)
		{
			lightUser[] result = new lightUser[num];
			for(int i=0;i<num;i++)
			{
				result [i] = testUsers [i];
			}
			return result;
		}

		public static respond tester(int type,Dictionary<string,object> message)
		{
			respond testResult = new respond ();
			string url = (string)message ["url"];
			/*
			switch(type)
			{
			case messageType.MESSAGETYPE_USER_ADD:
				url = messageType.MESSAGETYPE_USER_ADD_URL;
				if (testSearch ((string)message ["usrID"]) >= 0) {
					//existing user
					//testResult.DataField.Add ("error", 1);
				} else if (userSize < 15) {
					testUsers [userSize] = new lightUser ((string)message ["usrNAME"], (string)message ["usrID"], 99, 0);
					//testResult.DataField.Add ("error", 0);
					myIndex = userSize;
					userSize++;
				} else {
					//testResult.DataField.Add ("error", 2);
				}
				break;
			case messageType.MESSAGETYPE_USER_AUTHENTICATE:
				url = messageType.MESSAGETYPE_USER_AUTHENTICATE_URL;
				int index;
				if ((index = testSearch ((string)message ["usrID"])) < 0) {
					//Not a valid user
					//testResult.DataField.Add ("error", 3);
				} else {
					myIndex = index;
				}
				break;
			case messageType.MESSAGETYPE_USER_PULL_INFO:
				url = messageType.MESSAGETYPE_USER_PULL_INFO_URL;
				if ((index = testSearch ((string)message ["usrID"])) >= 0) {
					//existing user
					//testResult.DataField.Add ("payload",testUsers[index]);
					//testResult.DataField.Add ("error",0);
				} 
			    else {
					//error 2 not existing
					//testResult.DataField.Add ("error", 2);
				}
				break;
			case messageType.MESSAGETYPE_USER_PUSH_INFO:
				url = messageType.MESSAGETYPE_USER_PUSH_INFO_URL;
				if ((index = testSearch (senseix.getMyID())) >= 0) {
					//existing user
					testUsers [index] = new lightUser (senseix.getMyID(),senseix.getMyName(),senseix.getMyRank(),senseix.getMyScore());
					//testResult.DataField.Add ("error",0);
				} 
				else {
					//error 2 not existing
					//testResult.DataField.Add ("error", 2);
				}
				break;
			case messageType.MESSAGETYPE_LEADERBOARD_PULL:
				url = messageType.MESSAGETYPE_LEADERBOARD_PULL_URL;
				//testResult.DataField.Add ("payload", testTopUser (10));
				//testResult.DataField.Add ("error",0);
				break;
			case messageType.MESSAGETYPE_LEADERBOARD_PUSH_SCORE:
				url = messageType.MESSAGETYPE_USER_PUSH_INFO_URL;
				break;
			}
			return testResult;
			*/
			return testResult;
		}

	}