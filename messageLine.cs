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
		MonoBehaviour.print("Added pack to list" + pack.messageType);
		packList.Add (pack);
		//MonoBehaviour.print("after adding count is "+packList.Count);
	}
	public void scanMessages()
	{
		if (packList.Count == 0) 
		{
			//MonoBehaviour.print("messageLine count " + count);
			return;
		}
		else
		{
			//MonoBehaviour.print("messageLine count not empty " + count);
		}
		pagePack tmpPack = null;
		for(int i=0;i<packList.Count;i++)
		{
			MonoBehaviour.print("message line debug " + "count "+ packList.Count +"current " + i);
			tmpPack = (pagePack)packList[i];
			if(!tmpPack.wwwPage.isDone && string.IsNullOrEmpty(tmpPack.wwwPage.error))
			{
				//MonoBehaviour.print("Not ready");
				continue;
			}
			else if(!senseix.inSession && senseixMenuManager.offlineProblemLoadedInSenseix)
			{
				
				string tmp = null;
				switch(tmpPack.messageType)
				{
				case messageType.MESSAGETYPE_PROBLEM_PULL:
				{
					Dictionary<string,string> command = new Dictionary<string, string>();
					Dictionary<string,object> result = null;
					container decoder = new container();
					StringBuilder tmpBuilder = new StringBuilder();
					//FIXME: so far we just get strings with index 0
					tmp = (string)senseixMenuManager.cachedProblemStr[0];
					//MonoBehaviour.print("======got message=====  "+tmp);
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
						senseixGameManager.enqueProblem(new problem(tester["content"],tester["category"],tester["level"],Convert.ToInt32 (tester["id"]),tester["answer"]));
					}
					packList.Remove(tmpPack);
					break;
					}
				case messageType.MESSAGETYPE_PROBLEM_PUSH:
					//tmp = tmpPack.wwwPage.text;
					//MonoBehaviour.print("===push "+tmp);
					packList.Remove(tmpPack);
					break;
				case messageType.MESSAGETYPE_PLAYLIST_PULL:
				{
					//packList.Remove(tmpPack);
					break;
				}
				default:
					break;
				}
			}
			else
			{
				//MonoBehaviour.print("ready and checking");
				string tmp = null;
				if (!string.IsNullOrEmpty (tmpPack.wwwPage.error))
				{
					
					MonoBehaviour.print(tmpPack.wwwPage.error);
					int length = tmpPack.wwwPage.size;
					StringBuilder builder = new StringBuilder();
					//Set plugin status if there is error
					if(tmpPack.wwwPage.error.Equals("401"))
					{
						senseix.setPluginStatus(statusType.AUTH_FAILED);
					}
					else if(tmpPack.wwwPage.error.Equals("422"))
					{
						senseix.setPluginStatus(statusType.INTERNAL_ERROR);
					}
					else
					{
						senseix.setPluginStatus(statusType.OTHER_ERROR);
					}
					
					for(int j = 0;j<length;j++)
					{
						builder.Append((char)tmpPack.wwwPage.bytes[i]);
					}
					MonoBehaviour.print("!!!!Found error " + builder.ToString());
					packList.Remove(tmpPack);
					continue;
				}
				else
				{
					//MonoBehaviour.print("Everything is good");
					senseix.resetPluginStatus(); //There is not error
					switch(tmpPack.messageType)
					{
					case messageType.MESSAGETYPE_PROBLEM_PULL:
					{	
						Dictionary<string,string> command = new Dictionary<string, string>();
						Dictionary<string,object> result = null;
						container decoder = new container();
						StringBuilder tmpBuilder = new StringBuilder();
						tmp = tmpPack.wwwPage.text;
						//MonoBehaviour.print("======got message=====  "+tmp);
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
							senseixGameManager.enqueProblem(new problem(tester["content"],tester["category"],tester["level"],Convert.ToInt32 (tester["id"]),tester["answer"]));
						}
						packList.Remove(tmpPack);
						break;
					}
					case messageType.MESSAGETYPE_PLAYLIST_PULL:
					{
						Dictionary<string,string> command = new Dictionary<string, string>();
						Dictionary<string,object> result = null;
						container decoder = new container();
						StringBuilder tmpBuilder = new StringBuilder();
						tmp = tmpPack.wwwPage.text;
						MonoBehaviour.print("======got message===== MESSAGETYPE_PLAYLIST_PULL  "+tmp);
						//tmpBuilder.Append("{\"problems\":\"");
						/*
						tmpBuilder.Append(tmp);
						//tmpBuilder.Append("\"}");
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
							//Dictionary<string,string> tester = (Dictionary<string,string>)first.Dequeue();
							//senseixGameManager.enqueProblem(new problem(tester["content"],tester["category"],tester["level"],Convert.ToInt32 (tester["id"]),tester["answer"]));
						}
						*/
						packList.Remove(tmpPack);
						
						break;
					}
					case messageType.MESSAGETYPE_PROBLEM_PUSH:
						tmp = tmpPack.wwwPage.text;
						//MonoBehaviour.print("===push "+tmp);
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

