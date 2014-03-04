using System;
using System.Collections;
using System.Collections.Generic;

public class problem
{
	//Each problem have their own instance
	public string content = "null";
	public string category = "null";
	public string level = "null";
	public problem()
	{
	}
	public problem (string content,string category,string level)
	{
		this.content=content;
		this.category=category;
		this.level=level;
	}
	//returned value is number of problems got succesfully pulled
}
