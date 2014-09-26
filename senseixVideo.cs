using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class senseixVideo:MonoBehaviour
{
	//public MovieTexture movTexture;
	private class LOADTYPE
	{
		public const int NOT_DECIDED = 0;
		public const int LOAD_FROM_JS = 1;
		public const int LOAD_FROM_WEB = 2;
	};
	public const int COUNTDOWN_LOAD_TO_PLAY = 1000;
	public const int COUNTDOWN_PLAY_TO_PAUSE = 300;
	public static int current_timer = 0;
	public WWW page;
	public static bool web_ready = false;
	public static bool html_ready = false;
	public static bool played = false;
	public static bool paused = false;
	public static UniWebView webview = null;
	public const string TESTURL = "http://katana-staging.senseix.com/tiny/t3n50";
	public static int loadType = LOADTYPE.NOT_DECIDED;
	public static bool allowToDisplay = false;
	public static bool displayed = false;
	public static bool loaded = false;

	void Update()
	{
		if(loaded)
		{
			if(!played && !paused)
			{
				if(current_timer < COUNTDOWN_LOAD_TO_PLAY)
				{
					current_timer++;
				}
				else
				{
					//reached end of count down and begin to play/pause video
					current_timer = 0;
					//webview.EvaluatingJavaScript("document.getElementById('kaltura_player_ifp').contentDocument.getElementById('kaltura_player').play();");
					//
					played = true;
				}
			}
			else if(played && !paused)
			{
				if(current_timer < COUNTDOWN_PLAY_TO_PAUSE)
				{
					current_timer++;
				}
				else
				{
					//reached end of count down and begin to play/pause video
					current_timer = 0;
					//webview.EvaluatingJavaScript("document.getElementById('kaltura_player_ifp').contentDocument.getElementById('kaltura_player').pause();");
					paused = true;
				}
			}
			else if(allowToDisplay && !displayed)
			{
				webview.Show();
				displayed = true;
			}
		}
	}
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
		webview.insets = new UniWebViewEdgeInsets(5,5,300,5);
		
		return 0;
	}
	//This function will load web, but will not show it or display it, you need to call showVideo
	public static int loadVideoFromJS(string Url)
	{
		loadType = LOADTYPE.LOAD_FROM_JS;
		mimicHtml(Url);
		webview.url = "file:///"+Application.persistentDataPath + "/test.html";
		
		webview.Load ();
		webview.EvaluatingJavaScript("document.getElementById('kaltura_player_1405117970_ifp').contentDocument.getElementById('kaltura_player_1405117970').play();");
		//webview.EvaluatingJavaScript("document.getElementById('kaltura_player_1405117970_ifp').contentDocument.getElementById('kaltura_player_1405117970').pause();");
		return 0;
	}
	public static int loadVideoFromWeb(string Url)
	{
		loadType = LOADTYPE.LOAD_FROM_WEB;
		webview.url = Url;
		print(webview.url);
		webview.Load ();
		//webview.EvaluatingJavaScript("document.getElementById('kaltura_player_ifp').contentDocument.getElementById('kaltura_player').play();");
		loaded = true;
		return 0;
	}
	public static int showVideo()
	{
		webview.Show();	
		//webview.EvaluatingJavaScript("var a = document.getElementById('kaltura_player_ifp').width; alert(a);");
		//webview.EvaluatingJavaScript("alert(\"aaaa\");");
		return 0;
	}
	public static void testPlay()
	{
		//webview.EvaluatingJavaScript("alert(document.getElementById('kaltura_player_ifp').id);");
		//webview.EvaluatingJavaScript("alert(1);");
		webview.EvaluatingJavaScript("document.getElementById('kaltura_player_ifp').contentDocument.getElementById('kaltura_player').play();");
	}
	public static void testPause()
	{
		//webview.EvaluatingJavaScript("alert(document.getElementById('kaltura_player_ifp').id);");
		//webview.EvaluatingJavaScript("alert(1);");
		webview.EvaluatingJavaScript("document.getElementById('kaltura_player_ifp').contentDocument.getElementById('kaltura_player').pause();");
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
	public static void js()
	{
		
		//webview.EvaluatingJavaScript("alert(1);");
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

