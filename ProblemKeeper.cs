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
		private const int PROBLEMS_PER_PULL = 6;
		private const string SEED_FILE_NAME = "seed.proto";
		private static bool _onLine = false;
		public static volatile Queue newProblems = new Queue(); 
		public static volatile Queue answeredProblems = new Queue();

		static public void CreateEmptySeedFile()
		{
			System.IO.File.Create (FilePath (SEED_FILE_NAME));
		}

		static public bool SeedFileExists()
		{
			return System.IO.File.Exists (FilePath (SEED_FILE_NAME));
		}

		static private void GetProblemsFromSeed()
		{
			string seedPath = FilePath (SEED_FILE_NAME);
			byte [] seedContents = System.IO.File.ReadAllBytes (seedPath);
			if (seedContents.Length == 0)
				throw new Exception ("The seed file is empty!");
			message.ResponseHeader reply = message.ResponseHeader.ParseFrom (seedContents);

			for (int i = 0; i < reply.ProblemGet.ProblemList.Count; i++)
			{
				message.problem.ProblemData entry = reply.ProblemGet.ProblemList[i];
				message.problem.ProblemData.Builder problem =  entry.ToBuilder();
				ProblemKeeper.AddProblemsToProblemQueue(problem);
			}
		}

		static public void ReplaceSeed(message.ResponseHeader reply)
		{
			Debug.Log ("Replacing seed file.");
			MemoryStream stream = new MemoryStream ();
			reply.WriteTo (stream);
			byte[] replacementBytes = stream.ToArray();
			System.IO.File.WriteAllBytes (FilePath(SEED_FILE_NAME), replacementBytes);
		}
		
		static public string FilePath(string fileName)
		{
			return System.IO.Path.Combine(Application.persistentDataPath, fileName);
		}

		static private void AppendStringToFile (string content, string filePath)
		{
			System.IO.File.AppendAllText (filePath, content);
		}
	
		static public void AddProblemToSeed(message.problem.ProblemData problemData)
		{
			MemoryStream stream = new MemoryStream ();
			problemData.WriteTo (stream);
			byte[] appendMeBytes = stream.ToArray();
			string appendMeString = "\n" + System.Text.Encoding.Default.GetString (appendMeBytes);
			string seedPath = FilePath (SEED_FILE_NAME);
			AppendStringToFile (appendMeString, seedPath);
			//Replace the seed file for this game with 
			//problems from the server
			//alg should be get N problems place N/2 
			//into seed file, when Answered > N/2 since
			//last pull to server, Pull another N - repeat
		}

		static public void ClearSeedExceptHeader()
		{
			IList<string> seedLines = System.IO.File.ReadAllLines (FilePath(SEED_FILE_NAME)) as IList<string>;
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
			if (_onLine)
			{
				message.Request.GetProblems (SenseixController.GetCurrentPlayerID(), PROBLEMS_PER_PULL);
			}
			GetProblemsFromSeed();
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
			if (GetNewProblemCount() < PROBLEMS_PER_PULL/1.5 || GetNewProblemCount() < 1) 
			{
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

