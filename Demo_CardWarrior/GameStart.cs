using UnityEngine;
using System.Collections;
using System.Threading;
/// <summary>
/// Game Start Button Event
/// </summary>
public class GameStart : MonoBehaviour
{
    GameObject gameManager;
	void Start () {
        gameManager = GameObject.Find("GameManager");
		Thread.Sleep (10);
	}

    // Game Start Button Event
    void OnClick()
    {
        gameManager.SendMessage("StartGame", SendMessageOptions.DontRequireReceiver);
        Destroy(this);
    }
	
	void Update () {
	
	}
}
