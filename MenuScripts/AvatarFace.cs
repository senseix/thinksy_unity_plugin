using UnityEngine;
using System.Collections;

namespace Senseix
{
	public class AvatarFace : MonoBehaviour 
	{
		public UnityEngine.UI.RawImage overrideButtonFace;

		private Texture defaultAvatarImage;

		void Awake()
		{
			defaultAvatarImage = overrideButtonFace.texture;
		}

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
			if (!SenseixSession.IsSignedIn())
			{
				overrideButtonFace.texture = defaultAvatarImage;
			}
			string avatarPath = Senseix.SenseixSession.GetCurrentAvatarPath ();
			//Debug.Log (avatarPath);
			if (avatarPath != "")
			{
				overrideButtonFace.texture = Resources.Load<Texture2D>(avatarPath);
			}
		}

		public void MouseEnter()
		{
			string avatarPath = Senseix.SenseixSession.GetCurrentAvatarPath ();
			if (avatarPath != "")
			{
				overrideButtonFace.texture = Resources.Load<Texture2D>(avatarPath+"_o");
			}
		}

		public void MouseExit()
		{
			InstanceUpdateButtonFace ();
		}
	}
}
