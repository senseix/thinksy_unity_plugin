using UnityEngine;
using System.Collections;

public class AvatarFace : MonoBehaviour 
{	
	public UnityEngine.UI.RawImage overrideButtonFace;

	void OnEnable()
	{
		InstanceUpdateButtonFace ();
	}
	
	public static void UpdateButtonFaces()
	{
		AvatarFace[] allButtonFaces = FindObjectsOfType<AvatarFace> ();
		foreach (AvatarFace buttonFace in allButtonFaces)
		{
			buttonFace.InstanceUpdateButtonFace ();
		}
	}

	private	void InstanceUpdateButtonFace()
	{
		string avatarPath = Senseix.SenseixSession.GetCurrentAvatarPath ();
		if (avatarPath != "")
		{
			overrideButtonFace.gameObject.SetActive(true);
			overrideButtonFace.texture = Resources.Load<Texture2D>(avatarPath);
		}
	}
}
