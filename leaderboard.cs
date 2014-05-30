using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
public class lbEntry
{
	public int memberID = -1;
	public int rank = -1;
	public double score = 0;
	public string name = "";
	public lbEntry(int id, int r,double s,string n)
	{
		memberID = id;
		rank = r;
		score = s;
		name = n;
	}
}
public class leaderboard
{
	public const int LISTMAX = 20;
	public static ArrayList entries = new ArrayList();
	public static lbEntry localPlayer = null;
	public leaderboard(){}
	public static bool ready()
	{
		if (entries.Count > 0)
			return true;
		return false;
	}
	public static void clearEntry()
	{
		entries.Clear ();
	}
	public static void addEntry(string memberID, string rank, string score,string member_data)
	{
		entries.Add (new lbEntry(Convert.ToInt32(memberID),Convert.ToInt32(rank),Convert.ToDouble(score),getPlayerName(member_data)));
	}
	public static string getPlayerName(string data)
	{
		StringBuilder builder = new StringBuilder ();
		int i = data.IndexOf ("playername") + 13;
		for(int j=i;j<data.Length-2;j++)
		{
			builder.Append(data[j]);
		}
		//print (builder.ToString());
		return builder.ToString();
	}
	public static void debugPrint()
	{
		//print ("capacity " + entries.Capacity);
		for (int i=0; i<entries.Count; i++) 
		{
			//print(i+" " + ((lbEntry)entries[i]).memberID + " rank " + ((lbEntry)entries[i]).rank + " name " + ((lbEntry)entries[i]).name + " score " + ((lbEntry)entries[i]).score);
		}
	}
}