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
			MemoryStream seedStream = new MemoryStream (seedContents);
			if (seedContents.Length == 0)
			{
				ThinksyPlugin.ShowEmergencyWindow("The seed file is empty! (" + seedPath + ")");
				throw new Exception ("The seed file is empty!");
			}
			//Message.Problem.ProblemGetResponse problemGet = Message.Problem.ProblemGetResponse.ParseFrom (seedContents);
			//Message.Problem.ProblemGetResponse problemGet = ProtoBuf.Serializer.Deserialize<Message.Problem.ProblemGetResponse> (seedStream);

			ThinksyProtosSerializer customSerializer = new ThinksyProtosSerializer ();
			Message.Problem.ProblemGetResponse problemGet = 
				customSerializer.Deserialize(seedStream, null, typeof(Message.Problem.ProblemGetResponse))
					as Message.Problem.ProblemGetResponse;


			for (int i = 0; i < problemGet.problems.Count; i++)
			{
				Message.Problem.ProblemData problem = problemGet.problems[i];
				ProblemKeeper.AddProblemsToProblemQueue(problem);
			}
		}

		static public void ReplaceQueue(Message.Problem.ProblemGetResponse reply)
		{
			ProblemKeeper.ReplaceSeed (reply);
			ProblemKeeper.DrainProblems ();
			ProblemKeeper.GetProblemsFromSeed ();
		}

		static private void ReplaceSeed(Message.Problem.ProblemGetResponse reply)
		{
			Logger.BasicLog ("Replacing seed file.");
			MemoryStream stream = new MemoryStream ();
			ThinksyProtosSerializer customSerializer = new ThinksyProtosSerializer ();
			customSerializer.Serialize (stream, reply);
			
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
			string[] files = Directory.GetFiles (Application.persistentDataPath, "*" + SEED_FILE_EXTENSION);

			foreach (string filename in files)
			{ //test overrides everything
				if (filename.Contains("test"))
				{
					return filename;
				}
			}

			//next priority is player specific
			string playerSeedPath = PlayerSeedPath ();
			if (File.Exists(playerSeedPath))
			{
				return playerSeedPath;
			}

			//then failsafe or anything else
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
			ThinksyProtosSerializer customSerializer = new ThinksyProtosSerializer ();
			customSerializer.Serialize (stream, ProblemData);
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

		public static void AddProblemsToProblemQueue (Message.Problem.ProblemData problem) 
		{
			//Debug.Log ("Added a Problem to queue, queue length now " + newProblems.Count);
			newProblems.Enqueue (problem);
		}

		static public Senseix.Message.Problem.ProblemData GetProblem()
		{
			CheckProblemPull ();
			if (newProblems.Count == 0)
			{
				GetProblemsFromSeed();
			}
			if (newProblems.Count == 0)
				ThinksyPlugin.ShowEmergencyWindow ("We ran out of problems.  That really shouldn't happen!");
			return (Senseix.Message.Problem.ProblemData) newProblems.Dequeue ();
		}

		static public bool SubmitAnswer(Message.Problem.ProblemData answeredProblemData, Answer answer, bool correct) 
		{
			Message.Problem.ProblemPost problem = new Message.Problem.ProblemPost();
			problem.correct = correct;
			
			//set Problem's answers to given ones
			string[] answerIDStrings = answer.GetAnswerIDs ();
			Senseix.Message.Problem.AnswerIdentifier givenAnswerIDs = new Senseix.Message.Problem.AnswerIdentifier();
			foreach (string answerID in answerIDStrings)
			{
				givenAnswerIDs.uuid.Add(answerID);
			}
			
			problem.problem_id = answeredProblemData.uuid;
			problem.answer_ids = givenAnswerIDs;
			problem.player_id = SenseixSession.GetCurrentPlayerID ();
			problem.answered_at_unix_time = UnixTimeNow ();
			AddAnsweredProblem (problem, answer);
			return correct;
		}

		static private ulong UnixTimeNow()
		{
			TimeSpan unixTime = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
			return (ulong)unixTime.TotalSeconds;
		}

		static public bool CheckAnswer(Message.Problem.ProblemData answeredProblemData, Answer answer) 
		{
			bool correct = true;
			//get correct answer IDs
			Message.Problem.AnswerIdentifier correctIDList = new Message.Problem.AnswerIdentifier ();
			foreach(Senseix.Message.Atom.Atom atom in answeredProblemData.answer.answers)
			{
				correctIDList.uuid.Add(atom.uuid);
			}

			//bail out if we have the wrong number of answers
			List<string> answerIDStrings = new List<string>(answer.GetAnswerIDs ());
			if (answerIDStrings.Count != correctIDList.uuid.Count)
				return false;

			//sort the lists to eliminate order discrepencies
			correctIDList.uuid.Sort ();
			answerIDStrings.Sort ();

			//check given answers against correct answers
			for (int i = 0; i < answerIDStrings.Count; i++)
			{
				correct = correct && (correctIDList.uuid[i] == (string)answerIDStrings[i]);
			}

			return correct;
		}

		static private void AddAnsweredProblem(Message.Problem.ProblemPost ProblemBuilder, Answer answer)
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
				PushAllProblems();
			}
		}


		public static void PushAllProblems()
		{
			if (answeredProblems.Count == 0)
				return;
			Logger.BasicLog ("Pushing current queue of " + answeredProblems.Count + " problems.");
			Senseix.Message.Request.GetSingletonInstance().StartCoroutine(
				Message.Request.PostProblems(Senseix.SenseixSession.GetCurrentPlayerID(), answeredProblems));
			answeredProblems.Clear ();
		}

		static private void CheckProblemPull()
		{
			//Debug.Log ("get new problem count:　" + GetNewProblemCount ());
			if (GetNewProblemCount() < 3)
			{
				if (SenseixSession.GetSessionState())
				{
					PullNewProblems();
				}
				//Debug.Log ("pulling more Problems");
			}
		}

		static public void PullNewProblems()
		{
			SenseixSession.GetProblems (PROBLEMS_PER_PULL);
		}

		static public void DrainProblems()
		{
			int problemsDrained = 0;
			while(newProblems.Count > 0)//don't give it a little wiggle room...
			{
				newProblems.Dequeue();
				problemsDrained++;
			}
			Logger.BasicLog (problemsDrained + " problems drained.");
		}

		static public void DeleteAllSeeds()
		{
			string[] files = Directory.GetFiles (Application.persistentDataPath, "*" + SEED_FILE_EXTENSION);
			
			foreach (string filename in files)
			{
				string filepath = System.IO.Path.Combine(Application.persistentDataPath, filename);
				System.IO.File.Delete(filepath);
			}
		}
    }
}