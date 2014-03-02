using System;
using System.Collections;
using System.Collections.Generic;

	public class user
	{
		protected string usrID = "NULL";
		protected string usrNAME = "NULL";
		protected int usrRank = 0;
		protected int usrScore = 0;
		public user ()
		{
			//Just keep the default value
		}

		public virtual string getUsrID()
		{
			return usrID;
		}
		public virtual string getUsrNAME()
		{
			return usrNAME;
		}
		public virtual int getUsrRank()
		{
			return usrRank;
		}
		public virtual int getUsrScore()
		{
			return usrScore;
		}
		public virtual void pullUsrInfo()
		{

		}
		public virtual bool pushUsrInfo()
		{
			return false;
		}

	}
