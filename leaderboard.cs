using System;

	public class leaderboard
	{
		private int playOldRank = 0;
		private int playNewRank = 0;	//Every time update the New and copy the old New to Old, this is the rank for player
		private int playOldScore = 0;
		private int playNewScore = 0;
		private int rankNum = 10;
		private string gameToken = null;
		private lightUser[] usrOnBoard= new lightUser[15];
		public leaderboard ()
		{
		}
		public int initLeaderboard(string gameToken, int rankNum = 10)
		{
			if (playNewRank == 0) { // not initialized
				this.rankNum = rankNum;		//probably name rankNum as list size would be better
				this.gameToken = gameToken;
				//So far only support 10
				//Array.Resize (usrOnBoard,rankNum);
				return pullLeaderboard ();
			} else {
				//initialized
				return -3;
			}
		}
		public int pullLeaderboard()
		{
			playOldRank = playNewRank;
			playOldScore = playNewScore;

			//SEND REQUEST
			senseix.pushMyInfo ();
			playNewRank = senseix.getMyScore();
			playNewScore = senseix.getMyRank();
			/*
			respond result = request.sendRequest (null,messageType.MESSAGETYPE_LEADERBOARD_PULL);
			if ((int)result.DataField ["error"] == 0)
				usrOnBoard = (lightUser[]) result.DataField["payload"];

			else
				return (int)result.DataField ["error"];
			*/
			return -4;
		}
		public lightUser[] showBoard()
		{
			return usrOnBoard;
		}
		public int getPlayerOldRank()
		{
			return playOldRank;
		}
		public int getPlayerRank()
		{
			return playNewRank;
		}
		public int getPlayerOldScore()
		{
			return playOldScore;
		}
		public int getPlayerScore()
		{
			return playNewScore;
		}
	}