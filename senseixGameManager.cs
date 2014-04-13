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

	public senseixGameManager ()
	{
	}
	void Update()
	{
		if (prepareFinish)
		{
			prepareFinish = false;
		}
		if (prepareRequest) 
		{
			prepareProblem(4,current_category,current_level);
			prepareRequest = false;
		}
	}
	public static int prepareProblem(int count,string category,int level)
	{
		Queue newproblemQ = senseix.pullProblemQ(count,category,level);
		print ("Problem debug: problem prepared to tmp q" + newproblemQ.Count);
		if(newproblemQ != null)
		{
			while(newproblemQ.Count > 0)
			{
				problemQ.Enqueue(newproblemQ.Dequeue());
			}
			print ("Problem debug: problem prepared to problemQ" + problemQ.Count);
			prepareFinish = true;
			return 0;
		}
		else
		{
			print("Failed to pull problems");
			return -1;
		}
	}
	public static void problemThread()
	{
		/*while (pmutex != 0) 
		{
			Thread.Sleep(1);
			print ("Block by mutex");
		}
		pmutex = 1;
		print ("Going to prepare problem");
		prepareProblem(4,current_category,current_level);
		pmutex = 0;
		*/
		//print (senseixMenuManager.debug_menu_state);
	}
	public static problem getProblem ()
	{
		if (problemQ.Count < 5) 
		{
			prepareRequest = true;
			print ("Going to start new thread " + problemQ.Count);
		}
		return (problem)problemQ.Dequeue();
	}
}
