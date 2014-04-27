using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class container
{
	public byte[] binary = null;
	public StringBuilder buffer = new StringBuilder();
	public int length = 0;
	public int ready = 0;
	public int postget = 0;
	public string url = null;
	public container ()
	{
	}
	public static int count(string input,char target)
	{
		char[] buffer = input.ToCharArray();
		int counter = 0;
		for(int i=0;i<input.Length;i++)
		{
			if(buffer[i] == target)
			{
				counter++;
			}
		}
		return counter;
	}
	private static string stripStringSign(string input)
	{
		if(input == null||input.Length == 0)
			return "null";
		StringBuilder builder = new StringBuilder();
		int sign = 0;
		if(input[0]!='"'||input[input.Length-1]!='"'||input[0]!='\"'||input[input.Length-1]!='\"')
			return input;
		for(int i=1;i<input.Length-1;i++)
		{
			builder.Append(input[i]);
		}
		return builder.ToString();
	}
	private static string stripStringSlashSign(string input)
	{
		if(input == null||input.Length == 0)
			return "null";
		StringBuilder builder = new StringBuilder();
		int sign = 0;
		for(int i=1;i<input.Length-2;i++)
		{
			if(input[i] != '\\')
				builder.Append(input[i]);
		}
		return builder.ToString();
	}
	public void init()
	{
		buffer.Append ("utf8=%E2%9C%93");
	}
	public void append(string str)
	{
		buffer.Append (str);
	}
	public int formBinary()
	{
		binary = new byte[buffer.Length];
		string tmpStr = buffer.ToString ();
		for(int i=0;i<buffer.Length;i++)
		{
			binary [i] = (byte)tmpStr [i];
		}
		return buffer.Length;
	}
	public void addFieldDeli()
	{
		this.append ("&");
	}
	public void addKeyDeli()
	{
		this.append ("%5B");
	}
	public void addValueDeli()
	{
		this.append ("%5D=");
	}
	public void addPushQDeli()
	{
		this.append ("&answer_set%5B%5D%5B");
	}
	public int formBinary(string result)
	{
		binary = new byte[result.Length];
		string tmpStr = result.ToString ();
		for(int i=0;i<result.Length;i++)
		{
			binary [i] = (byte)tmpStr [i];
		}
		return result.Length;
	}
	public string decodeQuestion(string result)
	{
		int slashCount = 0;
		string tmp = "\"";
		slashCount = container.count(result,'a');
		return null;
	}
	public Dictionary<string,string> formDictionary()
	{
		Dictionary<string,string> dictionary = new Dictionary<string,string>();
		int mao1 = 1, mao2 = 1, colon = 1, inMao = 0, befColon = 1,located = 0, length = buffer.Length;
		StringBuilder builder = new StringBuilder();
		string tmp = null;
		string key = null;
		string value = null;
		int bracklet = 0;
		int in_array = 0;
		for(int i=0;i<length;i++)
		{
			if(binary[i]==(byte)'{')
			{
				bracklet++;
			}
			if(binary[i]==(byte)'}')
			{
				bracklet--;
			}
			if(befColon == 1 && located ==0)
			{
				if(binary[i]==(byte)':')
				{
					located = 1;
					colon = i;
					mao2= i-1;
					//MonoBehaviour.print("KEY: "+mao1+" "+mao2+" "+colon);
					for(int j=mao1;j<=mao2;j++)
					{
						builder.Append((char)binary[j]);
					}
					key = new string(builder.ToString().ToCharArray());
					builder.Remove(0,builder.Length);
					mao1 = i+1;
					mao2 = i+1;
				}
				
			}
			else if(befColon ==1 && located ==1)
			{
				//MonoBehaviour.print((char)binary[i]);
				if(binary[i]==(byte)'[')
					in_array = 1;
				if(binary[i]==(byte)']')
					in_array = 0;
				if((binary[i]==(byte)',' && in_array == 0)||bracklet == 0)
				{			
					located = 0;
					mao2 = i-1;
					colon = i;
					//MonoBehaviour.print("VALUE: "+mao1+" "+mao2+" "+colon);
					for(int j=mao1;j<=mao2;j++)
					{
						builder.Append((char)binary[j]);
					}
					value = new string(builder.ToString().ToCharArray());
					builder.Remove(0,builder.Length);
					mao1 = i+1;
					mao2 = i+1;
					key=container.stripStringSign(key);
					value=container.stripStringSign(value);
					if(value.Length < 1)
						value = "null";
//					MonoBehaviour.print("========test decoder========");
					//MonoBehaviour.print(key+" | "+value);
					while(dictionary.ContainsKey(key))
						key = key + "a";
					dictionary.Add(key,value);
					//MonoBehaviour.print("Key " + key + " value "+dictionary[key]);
				}
			}
		}
		//if(dictionary.ContainsKey("member"))
		//MonoBehaviour.print ("Going to return dict "+"member "+dictionary["member"] + dictionary["rank"] + " " + dictionary["score"]);
		return dictionary;
	}
	public Dictionary<string,object> formObjectDictionary()
	{	
		//MonoBehaviour.print ("Trying to form obj dict");
		Dictionary<string,object> dictionary = new Dictionary<string,object>();
		Dictionary<string,string> objDictionary = null;
		Queue objQ = new Queue();
		int mao1 = 1, mao2 = 1, colon = 1, inMao = 0, befColon = 1,located = 0, length = buffer.Length;
		StringBuilder builder = new StringBuilder();
		string tmp = null;
		string key = null;
		string value = null;
		int bracklet = 0;

		for(int i=0;i<length;i++)
		{
			//MonoBehaviour.print ((char)binary[i]);
			if(binary[i]==(byte)'{')
			{
				bracklet++;
			}
			if(binary[i]==(byte)'}')
			{
				bracklet--;
			}
			if(befColon == 1 && located ==0)
			{
				if(binary[i]==(byte)':')
				{
					if(binary[i+2] == (byte)'[')
					{
						StringBuilder objBuilder = new StringBuilder();
						string objTmp = null;
						int objStart = i+1, objEnd = i+1;
						for(int j=i+3;j<length-1;j++)
							{
							if(binary[j] != (byte)'\\')
							{
								objBuilder.Append((char)binary[j]);
								//MonoBehaviour.print("append "+(char)binary[j]);
							}
							if(binary[j] == '}')
							{
								container decoder = new container();
								objBuilder.Append('}');	
								decoder.append(objBuilder.ToString());
								//MonoBehaviour.print("Going to get obj with " + decoder.buffer.ToString());
								decoder.formBinary();
								objDictionary = decoder.formDictionary();
								objQ.Enqueue(objDictionary);
								objBuilder.Remove(0,objBuilder.Length);
								j++;
								while(binary[j] != ',' && binary[j] != ']')
								{
									j++;
								}
								if(binary[j] == ']')
								{
									dictionary.Add("objects",objQ);
									//MonoBehaviour.print("return dict "+ i +" "+j);
									return dictionary;
								}
							}
						}
					}
					else
					{
						located = 1;
						colon = i;
						mao2= i-1;
						//MonoBehaviour.print("KEY: "+mao1+" "+mao2+" "+colon);
						for(int j=mao1;j<=mao2;j++)
						{
							builder.Append((char)binary[j]);
						}
						key = new string(builder.ToString().ToCharArray());
						builder.Remove(0,builder.Length);
						mao1 = i+1;
						mao2 = i+1;
					}
				}
				
			}
			else if(befColon ==1 && located ==1)
			{
				//MonoBehaviour.print((char)binary[i]);
				if(binary[i]==(byte)','||bracklet == 0)
				{			
					located = 0;
					mao2 = i-1;
					colon = i;
					//MonoBehaviour.print("VALUE: "+mao1+" "+mao2+" "+colon);
					for(int j=mao1;j<=mao2;j++)
					{
						builder.Append((char)binary[j]);
					}
					value = new string(builder.ToString().ToCharArray());
					builder.Remove(0,builder.Length);
					mao1 = i+1;
					mao2 = i+1;
					key=container.stripStringSign(key);
					value=container.stripStringSign(value);
					if(value.Length < 1)
						value = "null";
					//MonoBehaviour.print("========test decoder========");
					//MonoBehaviour.print(key+"|"+value);
					while(dictionary.ContainsKey(key))
						key = key + "a";
					dictionary.Add(key,value);
				}
			}
		}
		return dictionary;
		
	}
}
