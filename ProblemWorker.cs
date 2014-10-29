using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace senseix { 

	static class ProblemWorker {

		private const int PROBLEM_COUNT = 40; 
		private static volatile bool _shouldStop;
		private static bool _onLine = false;
		private static bool pullMoreProblems = true; 
		public static volatile Queue newProblems = new Queue(); 
		public static volatile Queue answeredProblems = new Queue();

	
		static private int ReplaceProblemSeed()
		{
			return 0;
			//Replace the seed file for this game with 
			//problems from the server
			//alg should be get N problems place N/2 
			//into seed file, when Answered > N/2 since
			//last pull to server, Pull another N - repeat
		}
		static public int GetNewProblemCount () { 
		//	Debug.Log ("Duane, problem count is" + newProblems.Count);
			return newProblems.Count;
		}
		static public int GetAnsweredProblemCount () {
			return answeredProblems.Count;
		}

		public static void AddProblemsToProblemQueue (message.problem.ProblemData.Builder problem) {
			//Debug.Log ("Added a problem to queue, queue length now " + newProblems.Count);
			newProblems.Enqueue (problem);
		}

		//Request more problems from the server
		//and add them to the end of our problem queue
		static public void GetServerProblems () {
			
			if (_onLine == false) {
				//1. This case should be coming from the seed file
				//2. this currently is BAD logic...
				for (int j = 1; j < 10; j++) { 
					for (int i = 1; i < 10; i++) {
						message.problem.ProblemData.Builder prob = message.problem.ProblemData.CreateBuilder ();
						prob.SetQuestion (i.ToString () + "+" + j.ToString ());
						prob.SetAnswer ((i + j).ToString ());
						prob.SetProblemId ((j+100+i).ToString());
						newProblems.Enqueue (prob);

					}
				}
			} else {
				message.Request.GetProblems (SenseixController.GetCurrentPlayerID(), 6);  //should be 30
				ReplaceProblemSeed ();
			}
		}
		static public void PushServerProblems () { 
			Debug.Log ("PUSH SERVER PROBLEMS");
			message.Request.PostProblems (SenseixController.GetCurrentPlayerID(), answeredProblems);
		}

		static public senseix.message.problem.ProblemData.Builder GetProblem()
		{
			//Right now we're just pulling more problem when we run out, but eventually we
			//might want to do this with an asynchronous message
			if (GetNewProblemCount () < 1) {
					GetServerProblems (); 
					Debug.Log ("pulling more problems");
			}
			return (senseix.message.problem.ProblemData.Builder) newProblems.Dequeue ();
		}
		//This is a public function exposed to the developer, it is used to check an answer 
		//of the problem. If it returns true the developer should pop the next problem
		static public bool CheckAnswer(message.problem.ProblemData.Builder problem, string answer) 
		{
			answeredProblems.Enqueue (answer);
			if (problem.Answer == answer) {
				problem.SetAnswer (answer);
				return true;
			} else {
				//problem.SetAnswer (answer);
				return false; 
			}
		}

		//Main worker thread for problems
		static public void DoWork()
		{
			Debug.Log ("doin' work");
//			while (!_shouldStop)
//			{
//				if(GetNewProblemCount() < 10) 
//					GetServerProblems();
//
//				if (_onLine == true) {
//					if(GetAnsweredProblemCount() > 10)
//						PushServerProblems();
//				}
//			}
		}

		static public void SetOnline(bool state)
		{
			if (_onLine == false && state == true) {
				_onLine = state;
				
				//Drain the current queue and go for some problems!
				//Duane - improve this logic...we might be flipping the 
				//connection but still have a valid cache of prob.
				while(newProblems.Count > 2)//give it a little wiggle room...
				{
					newProblems.Dequeue();	
				}
			}
			else
			{
				_onLine = state;
			}
		}
		static public void RequestStop()
		{
			_shouldStop = true;
		}
    }
}

