using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace Senseix { 

	static class ProblemKeeper 
	{
		private const int PROBLEMS_PER_PULL = 40;
		private const float PULL_THRESHOLD = 0.25f;
		private const float PUSH_THRESHOLD = 0.25f; 
		//thresholds are when to pull push.  pull or push when
		//number of answered or waiting Problems drops below
		//Problems per pull over threshold
		private const string SEED_FILE_EXTENSION = ".bytes";
		private static bool _onLine = false;
		public static volatile Queue newProblems = new Queue(); 
		public static volatile Queue answeredProblems = new Queue();

		static public void CopyFailsafeOver()
		{
			string failsafeFileName = "failsafe";
			string failsafeDestination = System.IO.Path.Combine (Application.persistentDataPath, failsafeFileName + SEED_FILE_EXTENSION);

			TextAsset failsafeAsset = Resources.Load <TextAsset>(failsafeFileName);
			byte[] failsafeContents = failsafeAsset.bytes;

			try
			{
				FileStream newFile = System.IO.File.Create (failsafeDestination);
				newFile.Close ();
				System.IO.File.WriteAllBytes(failsafeDestination, failsafeContents);
			}
			catch
			{
				SenseixPlugin.ShowEmergencyWindow("An error occurred while creating a failsafe seed file in " + failsafeDestination);
			}
		}

		static private void GetProblemsFromSeed()
		{
			string seedPath = SeedFilePath();
			byte [] seedContents = System.IO.File.ReadAllBytes (seedPath);
			if (seedContents.Length == 0)
			{
				SenseixPlugin.ShowEmergencyWindow("The seed file is empty! (" + seedPath + ")");
				throw new Exception ("The seed file is empty!");
			}
			Message.ResponseHeader reply = Message.ResponseHeader.ParseFrom (seedContents);

			for (int i = 0; i < reply.ProblemGet.ProblemList.Count; i++)
			{
				Message.Problem.ProblemData entry = reply.ProblemGet.ProblemList[i];
				Message.Problem.ProblemData.Builder Problem =  entry.ToBuilder();
				ProblemKeeper.AddProblemsToProblemQueue(Problem);
			}
		}

		static public void ReplaceSeed(Message.ResponseHeader reply)
		{
			Debug.Log ("Replacing seed file.");
			MemoryStream stream = new MemoryStream ();
			reply.WriteTo (stream);
			byte[] replacementBytes = stream.ToArray();
			try
			{
				FileStream newFile = System.IO.File.Create (PlayerSeedPath ());
				newFile.Close ();
			}
			catch
			{
				SenseixPlugin.ShowEmergencyWindow("An error occurred while creating a seedfile in " + PlayerSeedPath());
			}
			stream.Close ();
			System.IO.File.WriteAllBytes (SeedFilePath(), replacementBytes);
		}

		static public string PlayerSeedPath()
		{
			string PlayerName = SenseixController.GetCurrentPlayerID ();
			string playerSeedPath = System.IO.Path.Combine (Application.persistentDataPath, PlayerName + SEED_FILE_EXTENSION);
			return playerSeedPath;
		}
		
		static public string SeedFilePath()
		{
			string playerSeedPath = PlayerSeedPath ();
			if (File.Exists(playerSeedPath))
			{
				return playerSeedPath;
			}
			string[] files = Directory.GetFiles (Application.persistentDataPath, "*" + SEED_FILE_EXTENSION);
			if (files.Length == 0)
			{
				SenseixPlugin.ShowEmergencyWindow("No seed files found in " + Application.persistentDataPath);
				throw new Exception("No seed files found in " + Application.persistentDataPath);
			}
			return files[0];
		}

		static private void AppendStringToFile (string content, string filePath)
		{
			System.IO.File.AppendAllText (filePath, content);
		}
	
		static public void AddProblemToSeed(Message.Problem.ProblemData ProblemData)
		{
			MemoryStream stream = new MemoryStream ();
			ProblemData.WriteTo (stream);
			byte[] appendMeBytes = stream.ToArray();
			string appendMeString = "\n" + System.Text.Encoding.Default.GetString (appendMeBytes);
			string seedPath = SeedFilePath();
			AppendStringToFile (appendMeString, seedPath);
			stream.Close ();
			//Replace the seed file for this game with 
			//Problems from the server
			//alg should be get N Problems place N/2 
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
		//	Debug.Log ("Duane, Problem count is" + newProblems.Count);
			return newProblems.Count;
		}
		static public int GetAnsweredProblemCount () {
			return answeredProblems.Count;
		}

		public static void AddProblemsToProblemQueue (Message.Problem.ProblemData.Builder Problem) {
			//Debug.Log ("Added a Problem to queue, queue length now " + newProblems.Count);
			newProblems.Enqueue (Problem);
		}

		//Request more Problems from the server
		//and add them to the end of our Problem queue
		static public void GetProblems () 
		{
			if (_onLine)
			{
				Message.Request.GetProblems (SenseixController.GetCurrentPlayerID(), PROBLEMS_PER_PULL);
			}
			GetProblemsFromSeed();
		}
		static public void PushServerProblems () 
		{ 
			Debug.Log ("PUSH SERVER PROBLEMS");
			Message.Request.PostProblems (SenseixController.GetCurrentPlayerID(), answeredProblems);
		}

		static public Senseix.Message.Problem.ProblemData.Builder GetProblem()
		{
			CheckProblemPull ();
			return (Senseix.Message.Problem.ProblemData.Builder) newProblems.Dequeue ();
		}

		static public bool SubmitAnswer(Message.Problem.ProblemData.Builder answeredProblemData, Answer answer, bool correct) 
		{
			Message.Problem.ProblemPost.Builder problem = Message.Problem.ProblemPost.CreateBuilder ();
			problem.SetCorrect (correct);
			
			//set Problem's answers to given ones
			string[] answerIDStrings = answer.GetAnswerIDs ();
			Senseix.Message.Problem.AnswerIdentifier.Builder givenAnswerIDs = Senseix.Message.Problem.AnswerIdentifier.CreateBuilder ();
			foreach (string answerID in answerIDStrings)
			{
				givenAnswerIDs.AddUuid(answerID);
			}
			
			problem.SetProblemId (answeredProblemData.Uuid);
			problem.SetAnswerIds (givenAnswerIDs);
			AddAnsweredProblem (problem, answer);
			return correct;
		}

		static public bool CheckAnswer(Message.Problem.ProblemData.Builder answeredProblemData, Answer answer) 
		{
			bool correct = true;

			//get correct answer IDs
			Message.Problem.AnswerIdentifier.Builder correctIDListBuilder = Message.Problem.AnswerIdentifier.CreateBuilder ();
			foreach(Senseix.Message.Problem.Atom atom in answeredProblemData.Answer.AtomList)
			{
				correctIDListBuilder.AddUuid(atom.Uuid);
			}

			//bail out if we have the wrong number of answers
			string[] answerIDStrings = answer.GetAnswerIDs ();
			if (answerIDStrings.Length != correctIDListBuilder.UuidCount)
				return false;
			
			//check given answers against correct answers
			for (int i = 0; i < answerIDStrings.Length; i++)
			{
				correct = correct && (correctIDListBuilder.GetUuid(i) == (string)answerIDStrings[i]);
			}

			return correct;
		}

		static private void AddAnsweredProblem(Message.Problem.ProblemPost.Builder ProblemBuilder, Answer answer)
		{
			answeredProblems.Enqueue (ProblemBuilder);
			CheckAnsweredProblemPush ();
			Debug.Log (answeredProblems.Count + " answered problems are waiting to be set free (posted).");
		}

		static private void CheckAnsweredProblemPush()
		{
			if (answeredProblems.Count > PROBLEMS_PER_PULL*PUSH_THRESHOLD && SenseixController.GetSessionState())
			{
					SenseixController.PushProblems(answeredProblems);
					answeredProblems.Clear ();
			}
		}

		static private void CheckProblemPull()
		{
			//Right now we're just pulling more Problems when we get low, but eventually we
			//want to do this with an asynchronous Message
			if (GetNewProblemCount() < PROBLEMS_PER_PULL*PULL_THRESHOLD || GetNewProblemCount() < 1) 
			{
				GetProblems (); 
				Debug.Log ("pulling more Problems");
			}
		}

		static public void SetOnline(bool state)
		{
			if (_onLine == false && state == true) {
				_onLine = state;
				
				//Drain the current queue and go for some Problems!
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

