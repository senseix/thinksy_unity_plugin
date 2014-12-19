using UnityEngine;
using System.Collections;
using System.IO;

public class Logger : MonoBehaviour 
{
	private const string logFileName = "SenseiXLog.txt";

	void OnEnable() 
	{
		Application.RegisterLogCallback(HandleLog);
	}

	void OnDisable() 
	{
		Application.RegisterLogCallback(null);
	}

	void HandleLog(string logString, string stackTrace, LogType type) 
	{
		string logPath = Path.Combine(Application.persistentDataPath, logFileName);

		string writeString = "logString:\n" + logString + "\nstackTrace\n" 
						+ stackTrace + "\ntype:\n" + type.ToString ();

		File.AppendAllText (logPath, writeString);
	}
}