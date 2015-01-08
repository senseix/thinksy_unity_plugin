using UnityEngine;
using System.Collections;
using System.IO;

namespace Senseix
{
	public class Logger : MonoBehaviour 
	{
		private const string logFileName = "SenseiXLog.txt";

		void Start()
		{
			SenseixPlugin.ShowEmergencyWindow ("Testing");
		}

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
			string logPath = GetLogPath();

			string writeString = "logString:\n" + logString + "\nstackTrace\n" 
							+ stackTrace + "\ntype:\n" + type.ToString ();

			File.AppendAllText (logPath, writeString);

			if (type == LogType.Exception || type == LogType.Error)
			{
				Senseix.SenseixSession.SubmitBugReport ("Automatic submission of non-routine unity log: ");
			}
		}

		static string GetLogPath()
		{
			return Path.Combine (Application.persistentDataPath, logFileName);
		}

		public static string GetCurrentLog()
		{
			string logPath = GetLogPath();
			string currentLog = File.ReadAllText (logPath);
			return currentLog;
		}
	}
}
