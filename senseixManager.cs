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
	public static string name;
	public static int id=0;
	public static string email;
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
			//senseix.readProblemFromStr();

			senseix.inSession = true;
			playerA = senseix.getPlayerA();
			//here should be showing profile selecting
			//If there is only one count, then we can directly go to get problems
			//otherwise, we need to do that after we get user id
			if(playerA.Count == 1)
			{
				senseix.id=((heavyUser)playerA[0]).id;
				id=senseix.id;
				if(senseix.id != 0)
				{
					//print ("player is" + senseix.id);
					senseixMenuManager.storeProblems(0);
					senseixGameManager.prepareProblem (5, "Mathematics", 1);
				}
			}
			
		}
		else
		{	
			ret = -1;
			senseix.inSession = false;
		}
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
	void Update()
	{
		senseixMenuManager.updateScanner ();
		senseixGameManager.updateScanner ();
	}
	static public ArrayList getPlayers()
	{
		playerA = senseix.getPlayerA();
		if (playerA == null || playerA.Count < 1)
			return null;
		return playerA;
	}
	static public ArrayList getCachedPlayers()
	{
		return playerA;
	}
	static public int selectProfile(int index)
	{
		if (senseix.playerA.Count <= index)
			return -1;
		senseix.selectProfile(index);
		name=senseix.name;
		id=senseix.id;
		email=senseix.email;
		//print ("selected: " + name + " "+id+" "+email);
		senseixMenuManager.storeProblems(0);
		senseixGameManager.prepareProblem (20, "Mathematics", 1);
		return 0;
	}
	public static string getProblem()
	{
		return senseixMenuManager.getProblem ();
	}
	public static string getAnwser()
	{
		return senseixMenuManager.getAnswer();
	}
	public static void gotAnwser(string answer)
	{
		senseixMenuManager.gotAnswer (answer);
	}
}


