using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;

public class problem
{
	//Each problem have their own instance
	public string content = "Null";
	public string category = "Null";
	public string level = "Null";
	public int problemID = 0;
	public string answer = "Null";
	public problem()
	{
	}
	public problem (string content,string category,string level,int ID,string anwser = null)
	{
		this.content=content;
		this.category=category;
		this.level=level;
		this.problemID = ID;
		this.answer = anwser;
	}
	public int mathResult()
	{
		StringBuilder tmpStr = new StringBuilder ();
		int Result = 0;
		int lastSign = 1;
		for (int i=0; i<this.content.Length; i++) 
		{
			int parseResult = parseChar((char)content[i]);
			//MonoBehaviour.print(parseResult + " " + content[i]);
			if(parseResult == -1)
				return -1;
			else
			{
				if(parseResult == 0)
					tmpStr.Append(content[i]);
				if(parseResult != 0)
				{
					int tmpNum = Convert.ToInt32(tmpStr.ToString());
					tmpStr.Remove(0,tmpStr.Length);
					if(lastSign == 1){
						//MonoBehaviour.print("plus" + Result + "+" + tmpNum);
						Result += tmpNum;

					}
					else if(lastSign == 2)
						Result -= tmpNum;
					else if(lastSign == 3)
						Result *= tmpNum;
					else if(lastSign == 4)
						Result /= tmpNum;
					if(parseResult == 9)
						return Result;
					lastSign = parseResult;
					//MonoBehaviour.print("last sign "+lastSign);
				}
			}
		}
		return -1;
	}
	public static int parseChar(char chrct)
	{
		if (chrct.Equals('0') || chrct.Equals('1') || chrct.Equals('2')|| chrct.Equals('3') || chrct.Equals('4')
		    || chrct.Equals('5') || chrct.Equals('6') || chrct.Equals('7') || chrct.Equals('8') || chrct.Equals('9')) 
		{
			return 0;
		}
		if(chrct.Equals('+'))
			return 1;
		if(chrct.Equals('-'))
			return 2;
		if(chrct.Equals('*'))
			return 3;
		if(chrct.Equals('/'))
			return 4;
		if(chrct.Equals('='))
			return 9;
		return -1;
	}
	//returned value is number of problems got succesfully pulled
}
