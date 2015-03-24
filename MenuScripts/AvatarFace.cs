using UnityEngine;
using System.Collections;

namespace Senseix
{
	public class AvatarFace : MonoBehaviour 
	{
		public UnityEngine.UI.Image overrideButtonFace;

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
				ReplaceAvatar(avatarPath);
			}
		}

		public void MouseEnter()
		{
			string avatarPath = Senseix.SenseixSession.GetCurrentAvatarPath ();
			if (avatarPath != "")
			{
				ReplaceAvatar(avatarPath+"_o");
			}
		}

		public void MouseExit()
		{
			InstanceUpdateButtonFace ();
		}

		private void ReplaceAvatar(string avatarPath)
		{
			//Debug.Log (avatarPath);
			Texture2D newImage = Resources.Load<Texture2D> (avatarPath);
			//Debug.Log (newImage == null);
			Sprite newSprite = Sprite.Create(newImage, 
			                                 new Rect(0f, 0f, newImage.width, newImage.height),
			                                 new Vector2(0.5f, 0.5f));
			overrideButtonFace.sprite = newSprite;
		}
	}
}
