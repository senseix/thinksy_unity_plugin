using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using System;
//For developers, they need to call prepareProblem when they know what is the level player select.
//Then developers can use API to get problem one by one
//MT means multithread 
public class senseixGameManager:MonoBehaviour
{
	public static Queue problemQ = new Queue();
	private static string current_category = null;
	private static int current_level = 1;
	private static bool initialized = false;
	private static int pmutex = 0;

	private static bool prepareFinish = false;
	private static bool playListReady = false;
	private static bool prepareRequest = false;
	private static messageLine line = new messageLine();
	public senseixGameManager ()
	{
	}
	public static void updateScanner()
	{
		line.scanMessages ();
		/*
		if (prepareFinish)
		{
			prepareFinish = false;
		}
		if (prepareRequest) 
		{MT
			prepareProblem(4,current_category,current_level);
			prepareRequest = false;
		}
		*/
	}
	public static int prepareProblem(int count,string category,int level)
	{
		Queue newproblemQ = senseix.pullProblemQ(count,category,level);
		print ("****************************************");
		print ("is calling playlist 3");
		print ("****************************************");
		doGetPlaylist();
		current_category = category;
		//print ("Problem debug: problem prepared to tmp q" + newproblemQ.Count);
		if(newproblemQ != null)
		{
			while(newproblemQ.Count > 0)
			{
				problemQ.Enqueue(newproblemQ.Dequeue());
			}
			//print ("Problem debug: problem prepared to problemQ" + problemQ.Count);
			prepareFinish = true;
			return 0;
		}
		else
		{
			//print("Failed to pull problems");
			return -1;
		}
	}
	public static container prepareProblemMT(int count,string category,int level)
	{
		container message = senseix.pullProblemQMT(count,category,level);
		print ("DEBUG playlist " + message.ToString());
		return message;
	}
	public static container preparePlaylistMT()
	{
		container message = senseix.pullPlaylistMT();
		return message;
	}
	//if error happens, null will be returned
	public static void doGetPlaylist()
	{
		print ("****************************************");
		print ("is calling playlist 2");
		print ("****************************************");
		//if senseix is in session and if player id is already set
		if(senseix.inSession && senseix.id != 0)
		{
			container message = preparePlaylistMT();
			WWW recvResult =new WWW (message.buffer.ToString());
			//print ("added message to line");
			line.addMessage(new pagePack(messageType.MESSAGETYPE_PLAYLIST_PULL,recvResult));
		}
	}
	public static void getPlaylist()
	{
		
	}
	public static problem getProblem ()
	{
		//print ("getProblem in GameManager was called " + problemQ.Count);
		if(problemQ.Count<4 && initialized)//This means game lost connection while playing
		{
			senseix.inSession = false;
			senseixMenuManager.retrieveProblems();
			senseixMenuManager.offlineProblemLoadedInSenseix = true;
		}
		if(!initialized)
			initialized = true;
		if (problemQ.Count < 5) 
		{
			//print ("=============Going to start new thread " + problemQ.Count);
			container message = prepareProblemMT(4,current_category,current_level);
			//print (message.url);
			WWW recvResult =new WWW (message.buffer.ToString());
			//print ("added message to line");
			line.addMessage(new pagePack(messageType.MESSAGETYPE_PROBLEM_PULL,recvResult));
		}
		//print ("===QUEUE==="+problemQ.Count + " "+initialized.ToString());
		return (problem)problemQ.Dequeue();
	}
	public static void enqueProblem(problem p)
	{
		if (problemQ == null)
			problemQ = new Queue ();
		problemQ.Enqueue (p);
	}
}
