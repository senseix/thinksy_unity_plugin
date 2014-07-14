using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class senseixVideo:MonoBehaviour
{
	//public MovieTexture movTexture;
	public WWW page;
	public static bool web_ready = false;
	public static bool html_ready = false;
	public static UniWebView webview = null;
	public senseixVideo ()
	{
		
	}
	public static int initSSXVidieo(string tag)
	{
		/*
		using(StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/test.html"))
		{
			sw.Write("<script src=\""+"http://katana-staging-426549454.us-east-1.elb.amazonaws.com/p/101/sp/10100/embedIframeJs/uiconf_id/23448275/partner_id/101?autoembed=true&entry_id=0_cpcv4vuo&playerId=kaltura_player_1405117970&cache_st=1405117970&width=400&height=333"+"\"></script>");
			sw.Flush();
			sw.Close();
		}
		*/
		
		webview = GameObject.FindGameObjectWithTag("web").GetComponent("UniWebView") as UniWebView;
		webview.SetShowSpinnerWhenLoading(false);
		//print(webb.insets.top.ToString());
		webview.insets = new UniWebViewEdgeInsets(5,5,5,5);

		return 0;
	}
	//This function will load web, but will not show it or display it, you need to call showVideo
	public static int loadVideo(string Url)
	{
		mimicHtml(Url);
		webview.url = "file:///"+Application.persistentDataPath + "/test.html";
		webview.Load ();
		return 0;
	}
	public static int showVideo()
	{
		webview.Show();	
		return 0;
	}
	public static int hideVideo()
	{
		if(!web_ready || webview == null)
		{
			return -1;
		}
		webview.Hide();
		return 0;
	}
	public static void mimicHtml(string Url)
	{

		using(StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/test.html"))
		{
			sw.Write("<script src=\""+Url+"\"></script>");
			sw.Flush();
			sw.Close();
		}
		/*
		using(StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/test.html"))
		{
			sw.Write("<script src=\""+ Url + "\"></script>");
			sw.Flush();
			sw.Close();
		}
		html_ready = true;
		*/
	}
}

