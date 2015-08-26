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
			Reinitialize();
		}
		Heart.Beat (gameAccessToken);
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