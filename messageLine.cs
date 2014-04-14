using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
public class pagePack
{
	public int messageType = 0;
	public WWW wwwPage = null;
	public pagePack(int type,WWW page)
	{
		messageType = type;
		wwwPage = page;
	}
}
public class messageLine
{
	public ArrayList packList = new ArrayList();
	public messageLine ()
	{
	}
	public void addMessage(pagePack pack)
	{
		MonoBehaviour.print("Added pack to list");
		packList.Add (pack);
		MonoBehaviour.print("after adding count is "+packList.Count);
	}
	public void scanMessages()
	{
		int count = packList.Count;
		if (count == 0) 
		{
			MonoBehaviour.print("messageLine count " + count);
			return;
		}
		pagePack tmpPack = null;
		for(int i=0;i<count;i++)
		{
			tmpPack = (pagePack)packList[i];
			if(!tmpPack.wwwPage.isDone && string.IsNullOrEmpty(tmpPack.wwwPage.error))
			{
				MonoBehaviour.print("Not ready");
				continue;
			}
			else
			{
				MonoBehaviour.print("ready and checking");
				string tmp = null;
				if (!string.IsNullOrEmpty (tmpPack.wwwPage.error))
				{
					MonoBehaviour.print(tmpPack.wwwPage.error);
					MonoBehaviour.print("Found error");
					packList.Remove(tmpPack);
					continue;
				}
				else
				{
					MonoBehaviour.print("Everything is good");
					switch(tmpPack.messageType)
					{
					case messageType.MESSAGETYPE_PROBLEM_PULL:
						Dictionary<string,string> command = new Dictionary<string, string>();
						Dictionary<string,object> result = null;
						container decoder = new container();
						StringBuilder tmpBuilder = new StringBuilder();
						tmp = tmpPack.wwwPage.text;

						tmpBuilder.Append("{\"problems\":\"");
						tmpBuilder.Append(tmp);
						tmpBuilder.Append("\"}");
						decoder.append(tmpBuilder.ToString());
						decoder.formBinary();
						result = decoder.formObjectDictionary();
						if (result == null)
						{
							packList.Remove(tmpPack);
							continue;
						}
						if (!result.ContainsKey ("objects"))
						{
							packList.Remove(tmpPack);
							continue;
						}
						Queue first = (Queue)result["objects"];
						if (first.Count == 0)
						{
							packList.Remove(tmpPack);
							continue;
						}
						while(first.Count != 0)
						{
							Dictionary<string,string> tester = (Dictionary<string,string>)first.Dequeue();
							senseixGameManager.enqueProblem(new problem(tester["content"],tester["category"],tester["level"],Convert.ToInt32 (tester["id"])));
						}
						packList.Remove(tmpPack);
						break;
					case messageType.MESSAGETYPE_PROBLEM_PUSH:
						tmp = tmpPack.wwwPage.text;
						MonoBehaviour.print(tmp);
						packList.Remove(tmpPack);
						break;
					default:
						break;
					}
				}
			}
		}
	}
}

