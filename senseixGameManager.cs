using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using System;
//For developers, they need to call prepareProblem when they know what is the level player select.
//Then developers can use API to get problem one by one
public class senseixGameManager:MonoBehaviour
{
	public static Queue problemQ = new Queue();
	private static string current_category = null;
	private static int current_level = 1;

	private static int pmutex = 0;

	private static bool prepareFinish = false;

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
		return message;
	}
	public static problem getProblem ()
	{
		//print ("getProblem in GameManager was called " + problemQ.Count);
		if (problemQ.Count < 5) 
		{
			print ("=============Going to start new thread " + problemQ.Count);
			container message = prepareProblemMT(4,current_category,current_level);
			//print (message.url);
			WWW recvResult =new WWW (message.buffer.ToString());
			print ("added message to line");
			line.addMessage(new pagePack(messageType.MESSAGETYPE_PROBLEM_PULL,recvResult));
		}
		print ("===QUEUE==="+problemQ.Count);
		return (problem)problemQ.Dequeue();
	}
	public static void enqueProblem(problem p)
	{
		if (problemQ == null)
			problemQ = new Queue ();
		problemQ.Enqueue (p);
	}
}
