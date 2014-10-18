using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace Senseix { 

	class ProblemWorker:MonoBehaviour {
		private volatile bool _shouldStop;
		private static bool _onLine = false;
		private static bool pullMoreProblems = true; 
		public static volatile Queue newProblems = new Queue(); 
		public static volatile Queue answeredProblems = new Queue(); 

	
		private int ReplaceProblemSeed()
		{
			return 0;
			//Replace the seed file for this game with 
			//problems from the server
			//alg should be get N problems place N/2 
			//into seed file, when Answered > N/2 since
			//last pull to server, Pull another N - repeat
		}
		public int GetNewProblemCount () { 
		//	print ("Duane, problem count is" + newProblems.Count);
			return newProblems.Count;
		}
		public int GetAnsweredProblemCount () {
			return answeredProblems.Count;
		}

		public static void AddProblemsToProblemQueue (Message.Problem.ProblemData.Builder problem) {
			newProblems.Enqueue (problem);
		}

		//Request more problems from the server
		//and add them to the end of our problem queue
		public void GetServerProblems () {
			
			if (_onLine == false) {
				//1. This case should be coming from the seed file
				//2. this currently is BAD logic...
				for (int j = 1; j < 10; j++) { 
					for (int i = 1; i < 10; i++) {
						Message.Problem.ProblemData.Builder prob = Message.Problem.ProblemData.CreateBuilder ();
						prob.SetQuestion (i.ToString () + "+" + j.ToString ());
						prob.SetAnswer ((i + j).ToString ());
						prob.SetProblemId ((j+100+i).ToString());
						newProblems.Enqueue (prob);
					}
				}
			} else {
				Message.Request.GetProblems (SenseixController.playerID, 30); 
				ReplaceProblemSeed ();
			}
		}
		public void PushServerProblems () { 
			Message.Request.PostProblems (SenseixController.playerID, answeredProblems);
		}

		public Senseix.Message.Problem.ProblemData.Builder GetProblem()
		{
			//Hopefully we never hit this case
			if (GetNewProblemCount () < 1) {
					GetServerProblems (); 
			}
			return (Senseix.Message.Problem.ProblemData.Builder) newProblems.Dequeue ();
		}
		//This is a public function exposed to the developer, it is used to check an answer 
		//of the problem. If it returns true the developer should pop the next problem
		public bool CheckAnswer(Message.Problem.ProblemData.Builder problem, string answer) 
		{
			answeredProblems.Enqueue (answer);
			if (problem.Answer == answer) {
				problem.SetAnswer (answer);
				return true;
			} else {
				problem.SetAnswer (answer);
				return false; 
			}
		}

		//Main worker thread for problems
		public void DoWork()
		{
			while (!_shouldStop)
			{
				if(GetNewProblemCount() < 10) 
					GetServerProblems();

				if (_onLine == true) {
					if(GetAnsweredProblemCount() > 10)
						PushServerProblems();
				}
			}
		}

		public void SetOnline(bool state)
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
				GetServerProblems();
			}
			else
			{
				_onLine = state;
			}
		}
		public void RequestStop()
		{
			_shouldStop = true;
		}
    }
}

