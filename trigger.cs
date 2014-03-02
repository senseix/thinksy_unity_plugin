using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
public class trigger : MonoBehaviour {
    int times = 0;
	private bool mainWindow = true;
	private bool coachLogin = false;
	private bool developerLogin = false;
	
	public string emailText = "email";
	public string passwordText = "";
	public trigger()
	{
		senseix.initSenseix("63c4512541f5d27dd4dd12a8e5b8c0ea7d8c2f6be8e15e7718920b399bc9846f");
	}
    public void showLoading()
	{
		GUI.Box(new Rect(Screen.width/2-Screen.width/4, Screen.height/2-Screen.height/8, Screen.width/2, Screen.height/4), "Loading");
	}
	void OnGUI() {
		if(mainWindow)
		{
        	if (GUI.Button(new Rect(Screen.width/2-75, Screen.height/2-15,80,30), "Login"))
			{	
				mainWindow = false;
			}
		}
		if(!mainWindow&&!coachLogin&&!developerLogin)
		{
			if (GUI.Button(new Rect(Screen.width/2-75, Screen.height/2-15,80,30), "Coach"))
			{	
				coachLogin = true;
			}
			if (GUI.Button(new Rect(Screen.width/2-75, Screen.height/2+25,80,30), "Developer"))
			{	
				developerLogin = true;
			}
		}
		if(coachLogin)
		{
			emailText = GUI.TextField(new Rect(Screen.width/2-75, Screen.height/2-5,80,30), emailText, 25);
			passwordText = GUI.PasswordField(new Rect(Screen.width/2-75, Screen.height/2+30,80,30), passwordText, "*"[0], 25);
			if (GUI.Button(new Rect(Screen.width/2-75, Screen.height/2+65,80,30), "Login"))
			{	
				senseix.coachLogin(emailText,passwordText);
				coachLogin = false;
				emailText="email";
				passwordText="";
			}
			if (GUI.Button(new Rect(Screen.width/2-75, Screen.height/2+100,80,30), "Cancel"))
			{	
				coachLogin = false;
				emailText="email";
				passwordText="";
			}
		}
		if(developerLogin)
		{
			emailText = GUI.TextField(new Rect(Screen.width/2-75, Screen.height/2-5,80,30), emailText, 25);
			passwordText = GUI.PasswordField(new Rect(Screen.width/2-75, Screen.height/2+30,80,30), passwordText, "*"[0], 25);
			if (GUI.Button(new Rect(Screen.width/2-75, Screen.height/2+100,80,30), "Login"))
			{	
				senseix.developerLogin(emailText,passwordText);
				developerLogin = false;
				emailText="email";
				passwordText="";
			}
			if (GUI.Button(new Rect(Screen.width/2-75, Screen.height/2+65,80,30), "Cancel"))
			{	
				developerLogin = false;
				emailText="email";
				passwordText="";
			}
		}
		
		
    }
	void DoMyWindow(int windowID) {
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }
	void Update() {
		//HttpWebResponse response = null;
		
		if (Input.GetKey("0"))
		{
			senseix.initSenseix("63c4512541f5d27dd4dd12a8e5b8c0ea7d8c2f6be8e15e7718920b399bc9846f");
		}
		if (Input.GetKey("a"))
		{
			senseix.developerLogin("80644036@qq.com","password.com");
		}
		if (Input.GetKey("b"))
		{
			senseix.developerLogout();
		}
		if (Input.GetKey("c"))
		{
			senseix.coachLogin("80640000@qq.com","password.com");
        }
        if (Input.GetKey("d")){
			senseix.coachLogout();
		}
		if (Input.GetKey("e")){
			senseix.coachSignUp("90909090@qq.com","chris","password.com");
		}
		if (Input.GetKey("f")){
			senseix.getPlayer ();
		}
		
        
    }
}
