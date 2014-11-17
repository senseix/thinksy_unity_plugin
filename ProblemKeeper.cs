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
		private const float PULL_THRESHOLD = 1.5f;
		private const float PUSH_THRESHOLD = 2f; 
		//thresholds are when to pull push.  pull or push when
		//number of answered or waiting problems drops below
		//problems per pull over threshold
		private const string SEED_FILE_EXTENSION = ".seed";
		private static bool _onLine = false;
		public static volatile Queue newProblems = new Queue(); 
		public static volatile Queue answeredProblems = new Queue();

		static public void CopyFailsafeOver()
		{
			string failsafeFileName = "failsafe";
			string failsafeSource = System.IO.Path.Combine (Application.dataPath, "senseix_unity_plugin/" + failsafeFileName + SEED_FILE_EXTENSION);
			string failsafeDestination = System.IO.Path.Combine (Application.persistentDataPath, failsafeFileName + SEED_FILE_EXTENSION);
			System.IO.File.Copy (failsafeSource, failsafeDestination, true);
		}

		static public void CreateEmptySeedFile()
		{
			System.IO.File.Create (SeedFilePath());
		}

		static public bool SeedFileExists()
		{
			return System.IO.File.Exists (SeedFilePath());
		}

		static public void CreateSeedFileIfNeeded()
		{
			if(!ProblemKeeper.SeedFileExists())
			{
				ProblemKeeper.CreateEmptySeedFile();
			}
		}

		static private void GetProblemsFromSeed()
		{
			string seedPath = SeedFilePath();
			byte [] seedContents = System.IO.File.ReadAllBytes (seedPath);
			if (seedContents.Length == 0)
			{
				SenseixPlugin.ShowEmergencyWindow();
				throw new Exception ("The seed file is empty!");
			}
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
			System.IO.File.WriteAllBytes (SeedFilePath(), replacementBytes);
		}
		
		static public string SeedFilePath()
		{
			string playerName = SenseixController.GetCurrentPlayerID ();
			if (playerName == "no current player")
			{
				string[] files = Directory.GetFiles (Application.persistentDataPath, "*.seed");
				if (files.Length == 0)
				{
					SenseixPlugin.ShowEmergencyWindow();
				}
				return files[0];
			}
			return System.IO.Path.Combine(Application.persistentDataPath, playerName + SEED_FILE_EXTENSION);
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
			string seedPath = SeedFilePath();
			AppendStringToFile (appendMeString, seedPath);
			//Replace the seed file for this game with 
			//problems from the server
			//alg should be get N problems place N/2 
			//into seed file, when Answered > N/2 since
			//last pull to server, Pull another N - repeat
		}

//		static public void ClearSeedExceptHeader()
//		{
//			IList<string> seedLines = System.IO.File.ReadAllLines (SeedFilePath()) as IList<string>;
//			if (seedLines.Count == 0)
//				throw new Exception("Problem: there is no seed file.  It should be here: " + SeedFilePath());
//			System.IO.File.Delete (SeedFilePath());
//			string header = seedLines [0];
//			System.IO.File.WriteAllText(SeedFilePath(), header);
//		}

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
			CheckProblemPull ();
			return (senseix.message.problem.ProblemData.Builder) newProblems.Dequeue ();
		}

		static public bool CheckAnswer(message.problem.ProblemData.Builder answeredProblemData, Answer answer) 
		{
			bool correct = true;
			message.problem.ProblemPost.Builder problem = message.problem.ProblemPost.CreateBuilder ();

			//get correct answer IDs
			message.problem.AnswerIdentifier.Builder correctIDListBuilder = message.problem.AnswerIdentifier.CreateBuilder ();
			foreach(senseix.message.problem.Atom atom in answeredProblemData.Answer.AtomList)
			{
				correctIDListBuilder.AddUuid(atom.Uuid);
			}

			//check given answers against correct answers
			ArrayList answerIDStrings = answer.GetAnswerIDs ();
			for (int i = 0; i < answerIDStrings.Count; i++)
			{
				correct = correct && (correctIDListBuilder.GetUuid(i) == (string)answerIDStrings[i]);
			}
			problem.SetCorrect (correct);

			//set problem's answers to given ones
			senseix.message.problem.AnswerIdentifier.Builder givenAnswerIDs = senseix.message.problem.AnswerIdentifier.CreateBuilder ();
			foreach (string answerID in answerIDStrings)
			{
				givenAnswerIDs.AddUuid(answerID);
			}

			problem.SetProblemId (answeredProblemData.Uuid);
			problem.SetAnswerIds (givenAnswerIDs);
			AddAnsweredProblem (problem, answer);
			return correct;
		}

		static private void AddAnsweredProblem(message.problem.ProblemPost.Builder problemBuilder, Answer answer)
		{
			answeredProblems.Enqueue (problemBuilder);
			CheckAnsweredProblemPush ();
		}

		static private void CheckAnsweredProblemPush()
		{
			if (answeredProblems.Count > PROBLEMS_PER_PULL/PUSH_THRESHOLD)
			{
				SenseixController.PushProblems(answeredProblems);
				answeredProblems.Clear ();
			}
		}

		static private void CheckProblemPull()
		{
			//Right now we're just pulling more problems when we get low, but eventually we
			//want to do this with an asynchronous message
			if (GetNewProblemCount() < PROBLEMS_PER_PULL/PULL_THRESHOLD || GetNewProblemCount() < 1) 
			{
				GetProblems (); 
				Debug.Log ("pulling more problems");
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

