using UnityEngine;
using System.Collections;
using System.IO;

namespace Senseix
{
	public class Logger : MonoBehaviour 
	{
		private const string logFileName = "SenseiXLog.txt";
		private const int deleteThreshhold = 1000;

		void Start()
		{
			//SenseixPlugin.ShowEmergencyWindow ("Testing");
		}

		void OnEnable() 
		{
			Application.logMessageReceived += HandleLog;
		}

		void OnDisable() 
		{
			Application.logMessageReceived -= HandleLog;
		}

		void HandleLog(string logString, string stackTrace, LogType type) 
		{
			string writeString = "logString:\n" + logString + "\nstackTrace\n" 
							+ stackTrace + "\ntype:\n" + type.ToString ();

			LogText(writeString);

			if (type == LogType.Exception || type == LogType.Error)
			{
				StartCoroutine(Senseix.SenseixSession.SubmitBugReport ("Automatic submission of non-routine unity log: "));
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

		public static void BasicLog(string extraLog)
		{
			//Debug.Log (extraLog);
			LogText(System.Environment.NewLine + "--- BASIC LOG ---" + 
								System.Environment.NewLine + extraLog + System.Environment.NewLine);
		}

		private static void LogText(string logString)
		{
			File.AppendAllText (GetLogPath(), logString);
			if (new System.IO.FileInfo (GetLogPath ()).Length > deleteThreshhold)
				File.Delete (GetLogPath ());
			UnityEngine.iOS.Device.SetNoBackupFlag (GetLogPath ());
		}
	}
}