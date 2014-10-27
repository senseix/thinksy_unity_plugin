using UnityEngine;
using System.Collections;

public class DisableOtherUI : MonoBehaviour {
	
	private Canvas [] reenableWhenThisDisabled;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable()
	{
		BuildDisabledList ();
		SetOtherCanvasActive(false);
	}

	void OnDisable()
	{
		SetOtherCanvasActive(true);
	}

	private void BuildDisabledList()
	{
		reenableWhenThisDisabled = Object.FindObjectsOfType <Canvas>();
	}

	private void SetOtherCanvasActive(bool isActive)
	{
		foreach(Canvas canvas in reenableWhenThisDisabled)
		{
			Canvas thisCanvas = canvas as Canvas;
			if (thisCanvas.gameObject != this.gameObject)
			{
				thisCanvas.gameObject.SetActive(isActive);
			}
		}
	}
}
