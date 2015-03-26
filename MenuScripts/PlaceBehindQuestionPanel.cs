using UnityEngine;
using System.Collections;

public class PlaceBehindQuestionPanel : MonoBehaviour {

	public GameObject questionPanel;

	// Use this for initialization
	void Start () {
		this.transform.position = 
			Camera.main.ScreenToWorldPoint(new Vector3(questionPanel.transform.position.x, 
			                                           questionPanel.transform.position.y, 
			                                           10));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
