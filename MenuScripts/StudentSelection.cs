using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Senseix
{
	public class StudentSelection : MonoBehaviour 
	{

		public Text playerNameText;

		private ArrayList availablePlayers;
		private int currentPlayerIndex = 0;

		// Use this for initialization
		void Start () 
		{
			SetFace ();
		}
		
		public static void UpdateStudentSelection()
		{
			StudentSelection[] allStudentSelections = FindObjectsOfType<StudentSelection> ();
			foreach (StudentSelection studentSelection in allStudentSelections)
			{
				studentSelection.OnEnable();
				studentSelection.SetName();
				studentSelection.SetFace();
			}
		}

		void OnEnable()
		{
			PullAvailablePlayers ();
			SetStudent (currentPlayerIndex);
		}

		public void NextStudent()
		{
			SetStudent (currentPlayerIndex + 1);
		}

		public void PreviousStudent()
		{
			SetStudent (currentPlayerIndex - 1);
		}

		public void SetStudent(int studentIndex)
		{
			if (availablePlayers.Count <= 0)
				return;

			currentPlayerIndex = studentIndex % availablePlayers.Count;
			if (currentPlayerIndex < 0)
				currentPlayerIndex = availablePlayers.Count + currentPlayerIndex;
			SenseixSession.SelectPlayer (GetCurrentPlayer ());
			SetName ();
			SetFace ();
			ThinksyPlugin.NextProblem ();
		}

		public Senseix.Message_v2.Player.Player GetCurrentPlayer()
		{
			return availablePlayers [currentPlayerIndex] as Senseix.Message_v2.Player.Player;
		}

		private void SetName()
		{
			Message_v2.Player.Player newPlayer = availablePlayers [currentPlayerIndex] as Message_v2.Player.Player;
			string newName = newPlayer.name;
			playerNameText.text = newName;
		}

		private void SetFace()
		{
			AvatarFace.UpdateButtonFaces ();
		}

		public void PullAvailablePlayers()
		{
			availablePlayers = SenseixSession.GetCurrentPlayerList ();
		}
	}
}
