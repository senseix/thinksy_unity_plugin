using UnityEngine;
using System.Collections;

public class DisableOtherUI : MonoBehaviour {
	
	public GameObject [] reenableWhenThisDisabled;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable()
	{
		SetOtherCanvasActive(false);
	}

	void OnDisable()
	{
		foreach(GameObject thisGameObject in reenableWhenThisDisabled)
		{
			thisGameObject.SetActive (true);
		}
	}

	private void SetOtherCanvasActive(bool isActive)
	{
		Object [] Canvases = Object.FindObjectsOfType <Canvas>();
		foreach(Object canvas in Canvases)
		{
			Canvas thisCanvas = canvas as Canvas;
			if (thisCanvas.gameObject != this.gameObject)
			{
				thisCanvas.gameObject.SetActive(isActive);
			}
		}
	}
}
