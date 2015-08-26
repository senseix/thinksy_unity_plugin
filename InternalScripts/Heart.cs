using UnityEngine;
using System.Collections;

public static class Heart
{
	private static uint heartbeatInterval = 1401;

	public static void Beat(string gameAccessToken)
	{
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
}