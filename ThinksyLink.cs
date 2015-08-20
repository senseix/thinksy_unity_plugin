using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ThinksyLink : MonoBehaviour
{
	public string gameAccessToken = null; 	
								//this is your developer access token obtained from 
								//the Senseix website.

	private static ThinksyLink singletonInstance;
	
	private const int reconnectRetryInterval = 3000;
	private static uint heartbeatInterval = 1401;

	static private ThinksyLink GetSingletonInstance()
	{
		if (singletonInstance == null)
		{
			singletonInstance = FindObjectOfType<ThinksyLink>();
		}
		if (singletonInstance == null)
		{
			throw new Exception("There is no Thinksy Link");
		}
		return singletonInstance;
	}

	void OnApplicationFocus(bool isFocused)
	{
		if (isFocused) StaticReinitialize ();
	}

	
	void Awake()
	{	
		if (gameAccessToken == null || gameAccessToken == "")
			throw new Exception ("Please enter a game access token.");
	}

	void Update()
	{
		if (!Senseix.SenseixSession.GetSessionState() && Time.frameCount%reconnectRetryInterval == 0)
		{
			Debug.Log ("Attempting to reconnect...");
			StartCoroutine(Senseix.SenseixSession.InitializeSenseix(gameAccessToken));
		}
		if (Senseix.SenseixSession.GetSessionState() && Time.frameCount%heartbeatInterval == 0 &&  Time.frameCount != 0)
		{
			Senseix.Logger.BasicLog("Getting encouragements...");
			Senseix.SenseixSession.Heartbeat();
		}
	}

	public static void NewHeartbeatTiming(uint newTiming)
	{
		if (newTiming < 100)
			return;
		heartbeatInterval = newTiming;
	}

	public static void StaticReinitialize()
	{
		GetSingletonInstance().Reinitialize ();
	}

	/// <summary>
	/// Resends all the server communication involved in initializing the game.
	/// </summary>
	public void Reinitialize()
	{
		StartCoroutine(Senseix.SenseixSession.LimitedInitializeSenseix (gameAccessToken));
	}

	public static void SetAccessToken(string newAccessToken)
	{
		GetSingletonInstance().gameAccessToken = newAccessToken;
	}
}