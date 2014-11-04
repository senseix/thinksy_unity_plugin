using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace senseix { 

	static class ProblemKeeper 
	{
		private const int PROBLEM_COUNT = 40;
		private const int PROBLEMS_PER_PULL = 6;
		private const string SEED_FILE_NAME = "seed.proto";
		private static bool _onLine = false;
		public static volatile Queue newProblems = new Queue(); 
		public static volatile Queue answeredProblems = new Queue();

		static private void GetProblemsFromSeed()
		{
			string seedPath = FilePath (SEED_FILE_NAME);
			byte [] seedContents = System.IO.File.ReadAllBytes (seedPath);
			message.ResponseHeader reply = message.ResponseHeader.ParseFrom (seedContents);
			message.Response.ParseResponse(message.constant.MessageType.ProblemGet, ref reply);
		}

		static public string FilePath(string fileName)
		{
			return System.IO.Path.Combine(Application.persistentDataPath, fileName);
		}

		//Mehrdad wrote this function.
		static byte[] GetBytes(string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		static private void AppendStringToFile (string content, string filePath)
		{
			System.IO.File.AppendAllText (filePath, content);
		}
	
		static public void AddProblemToSeed(message.problem.ProblemData problemData)
		{
			MemoryStream stream = new MemoryStream ();
			problemData.WriteTo (stream);
			string appendMe = stream.ToString();
			string seedPath = FilePath (SEED_FILE_NAME);
			AppendStringToFile (appendMe, seedPath);
			//Replace the seed file for this game with 
			//problems from the server
			//alg should be get N problems place N/2 
			//into seed file, when Answered > N/2 since
			//last pull to server, Pull another N - repeat
		}

		static public void ClearSeedExceptHeader()
		{
			IList<string> seedLines = System.IO.File.ReadLines (FilePath(SEED_FILE_NAME)) as IList<string>;
			if (seedLines.Count == 0)
				throw new Exception("Problem: there is no seed file.  It should be here: " + FilePath (SEED_FILE_NAME));
			System.IO.File.Delete (FilePath (SEED_FILE_NAME));
			string header = seedLines [0];
			System.IO.File.WriteAllText(FilePath(SEED_FILE_NAME), header);
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
		static public void GetProblems () 
		{
			_onLine = false;
			if (_onLine == false) 
			{
				GetProblemsFromSeed();
			} 
			else 
			{
				message.Request.GetProblems (SenseixController.GetCurrentPlayerID(), PROBLEMS_PER_PULL);
			}
		}
		static public void PushServerProblems () 
		{ 
			Debug.Log ("PUSH SERVER PROBLEMS");
			message.Request.PostProblems (SenseixController.GetCurrentPlayerID(), answeredProblems);
		}

		static public senseix.message.problem.ProblemData.Builder GetProblem()
		{
			//Right now we're just pulling more problems when we get low, but eventually we
			//want to do this with an asynchronous message
			if (GetNewProblemCount () < PROBLEMS_PER_PULL/3 ||　GetNewProblemCount() < 2) {
					GetProblems (); 
					Debug.Log ("pulling more problems");
			}
			return (senseix.message.problem.ProblemData.Builder) newProblems.Dequeue ();
		}
		//This is called from functions exposed to the developer, it is used to check an answer 
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
    }
}

