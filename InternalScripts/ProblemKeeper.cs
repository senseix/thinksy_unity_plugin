using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace Senseix 
{ 
	static class ProblemKeeper 
	{
		public const int PROBLEMS_PER_PULL = 12;
		private const float PULL_THRESHOLD = 0.25f;
		private const float PUSH_THRESHOLD = 0.25f; 
		//thresholds are when to pull push.  pull or push when
		//number of answered or waiting Problems drops below
		//Problems per pull over threshold
		public const string SEED_FILE_EXTENSION = ".bytes";
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
				ThinksyPlugin.ShowEmergencyWindow("An error occurred while creating a failsafe seed file in " + failsafeDestination);
			}
		}

		static private void GetProblemsFromSeed()
		{
			string seedPath = SeedFilePath();
			byte [] seedContents = System.IO.File.ReadAllBytes (seedPath);
			if (seedContents.Length == 0)
			{
				ThinksyPlugin.ShowEmergencyWindow("The seed file is empty! (" + seedPath + ")");
				throw new Exception ("The seed file is empty!");
			}
			Message.Problem.ProblemGetResponse problemGet = Message.Problem.ProblemGetResponse.ParseFrom (seedContents);

			for (int i = 0; i < problemGet.ProblemList.Count; i++)
			{
				Message.Problem.ProblemData entry = problemGet.ProblemList[i];
				Message.Problem.ProblemData.Builder problem =  entry.ToBuilder();
				ProblemKeeper.AddProblemsToProblemQueue(problem);
			}
		}

		static public void ReplaceSeed(Message.Problem.ProblemGetResponse reply)
		{
			Logger.BasicLog ("Replacing seed file.");
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
				ThinksyPlugin.ShowEmergencyWindow("An error occurred while creating a seedfile in " + PlayerSeedPath());
			}
			stream.Close ();
			System.IO.File.WriteAllBytes (SeedFilePath(), replacementBytes);
		}

		static public string PlayerSeedPath()
		{
			string PlayerName = SenseixSession.GetCurrentPlayerID ();
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
				ThinksyPlugin.ShowEmergencyWindow("No seed files found in " + Application.persistentDataPath);
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
			if (SenseixSession.GetSessionState())
			{
				Message.Request.GetProblems (SenseixSession.GetCurrentPlayerID(), PROBLEMS_PER_PULL);
			}
			GetProblemsFromSeed();
		}
		static public void PushServerProblems () 
		{ 
			Debug.Log ("PUSH SERVER PROBLEMS");
			Message.Request.PostProblems (SenseixSession.GetCurrentPlayerID(), answeredProblems);
		}

		static public Senseix.Message.Problem.ProblemData.Builder GetProblem()
		{
			CheckProblemPull ();
			if (newProblems.Count == 0)
				ThinksyPlugin.ShowEmergencyWindow ("We ran out of problems.  That really shouldn't happen!");
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
			problem.SetPlayerId (SenseixSession.GetCurrentPlayerID ());
			problem.SetAnsweredAtUnixTime (UnixTimeNow ());
			AddAnsweredProblem (problem, answer);
			return correct;
		}

		static private ulong UnixTimeNow()
		{
			var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
			return (ulong)(timeSpan.TotalSeconds);
		}

		static public bool CheckAnswer(Message.Problem.ProblemData.Builder answeredProblemData, Answer answer) 
		{
			bool correct = true;

			//get correct answer IDs
			Message.Problem.AnswerIdentifier.Builder correctIDListBuilder = Message.Problem.AnswerIdentifier.CreateBuilder ();
			foreach(Senseix.Message.Atom.Atom atom in answeredProblemData.Answer.AtomList)
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
			//Debug.Log (answeredProblems.Count + " answered problems are waiting to be set free (posted).");
		}

		static private void CheckAnsweredProblemPush()
		{
			//Debug.Log ("Answered problems count: " + answeredProblems.Count);
			if (answeredProblems.Count > PROBLEMS_PER_PULL*PUSH_THRESHOLD)
			{
					SenseixSession.PushProblems(answeredProblems);
					answeredProblems.Clear ();
			}
		}

		static private void CheckProblemPull()
		{
			//Debug.Log ("get new problem count:　" + GetNewProblemCount ());
			if (GetNewProblemCount() < PROBLEMS_PER_PULL*PULL_THRESHOLD || GetNewProblemCount() < 1) 
			{
				GetProblems ();
				//Debug.Log ("pulling more Problems");
			}
		}

		static public void DrainProblems()
		{
			while(newProblems.Count > 2)//give it a little wiggle room...
			{
				newProblems.Dequeue();	
			}
		}
    }
}