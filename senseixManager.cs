//------------------------------------------------------------------------------
//This is the layer that right under game and above senseixGameManager
//1. game
//2. senseixManager
//   *provide direct interface that can call API in senseix to signup, signin, signout, pull players
//   *decide when to cache or push cached problems/answers
//   *decide when to pull streaming and what to pull
//3. senseixGameManager
//   *problems preparing
//   *problems pulling when number in a low level
//4. senseix
//   *+add functionality to pull streaming
//5. request
//6. container
//------------------------------------------------------------------------------
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class senseixManager:MonoBehaviour
{
	private static int insession = 0;
	public static ArrayList playerA = null;
	public senseixManager ()
	{
	}
	static public int initSenseixManager(string access_token)
	{
		int ret = 0;
		senseix.initSenseix (access_token);
		ret = senseix.coachUidPush ();
		if (ret == 0)
		{
			senseix.readProblemFromStr();

			senseix.inSession = true;
			playerA = senseix.getPlayerA();
			//here should be showing profile selecting
			/*
			if(playerA.Count == 1)
			{
				senseix.id=((heavyUser)playerA[0]).id;
				print ("player is" + senseix.id);
			}
			*/
			senseixMenuManager.storeProblems();
		}
		else
			senseix.inSession = false;

		return ret;
	}
	static public int levelDecider()
	{
		return 1;	
	}
	static public bool networkConnected()
	{
		return true;
	}
	static public ArrayList getPlayers()
	{
		playerA = senseix.getPlayerA();
		if (playerA == null || playerA.Count < 1)
			return null;
		return playerA;
	}

}


