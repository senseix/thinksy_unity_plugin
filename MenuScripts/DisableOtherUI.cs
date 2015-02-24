using UnityEngine;
using System.Collections;

public class DisableOtherUI : MonoBehaviour {
	
	public bool pauseWhenMenuOpen = true;
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
		if (pauseWhenMenuOpen)
			Time.timeScale = 0;
	}

	void OnDisable()
	{
		SetOtherCanvasActive(true);
		if (pauseWhenMenuOpen)
			Time.timeScale = 1;
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
			if (thisCanvas != null &&
			    thisCanvas.gameObject != null &&
			    thisCanvas.gameObject != this.gameObject)
			{
				thisCanvas.gameObject.SetActive(isActive);
			}
		}
	}
}
