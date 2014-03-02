using System;
using System.Collections;
using System.Collections.Generic;

	public class problem
	{
		//Each problem have their own instance
		private problem next = null;
		private problem last = null;
		private bool head = true;
		private bool tail = true;
		private int duration = 0;
		private bool skip = false;
		private int game_difficulty = 0;
		private string answerA = null;
		private string answerB = null;

		public problem ()
		{
		}
		//returned value is number of problems got succesfully pulled
		public static int pullProblem(problem instance)
		{
			/*
			Dictionary<string,object> message = new Dictionary<string,object>();
			message.Add ("access_token",senseix.getGameToken());
			message.Add ("problem_id",problem_id.ToString());
			Dictionary<string,string> result = (Dictionary<string,string>) request.sendRequest (message,messageType.MESSAGETYPE_PROBLEM_Q_PULL);

			//return result;
			*/
			return 0;
		}
		public static int pushProblem(problem instance)
		{
			/*
			Dictionary<string,object> message = new Dictionary<string,object>();
			message.Add ("access_token",senseix.getGameToken());
			//message.Add ("auth_token",);
			message.Add ("problem_id",problem_id.ToString());
			message.Add ("answer",answer);
			message.Add ("duration",duration);
			message.Add ("correctness",correctness.ToString());
			Dictionary<string,string> result = (Dictionary<string,string>) request.sendRequest (message,messageType.MESSAGETYPE_PROBLEM_Q_PULL);
			*/
			//return result;
			return 0;
		}
	}
