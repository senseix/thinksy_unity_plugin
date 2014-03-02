using System;
using System.Collections;
using System.Collections.Generic;

	public class heavyUser: user
	{
		private string usrHash = "N/A";
		private string moreInfoOne = "N/A";
		private string moreInfoTWO = "N/A";
		private int moreInfoTHR = 0;
		private int moreInfoFOR = 0;
		private bool moreInfoFIV = false;

		public heavyUser ()
		{
			//Just keep the default value
		}

		public heavyUser (string id,string name,int rank,int score, string arg1, string arg2, int arg3, int arg4, bool arg5)
		{
			usrID = id;
			usrNAME = name;
			usrRank = rank;
			usrScore = score;

			moreInfoOne = arg1;
			moreInfoTWO = arg2;
			moreInfoTHR = arg3;
			moreInfoFOR = arg4;
			moreInfoFIV = arg5;
		}
		public virtual string getmoreInfoOne()
		{
			return moreInfoOne;
		}
		public virtual string getmoreInfoTWO()
		{
			return moreInfoTWO;
		}
		public virtual int getmoreInfoTHR()
		{
			return moreInfoTHR;
		}
		public virtual int getmoreInfoFOR()
		{
			return moreInfoFOR;
		}
		public virtual bool getmoreInfoFIV()
		{
			return moreInfoFIV;
		}
	}

