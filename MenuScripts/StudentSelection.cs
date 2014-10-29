using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace senseix
{
	public class StudentSelection : MonoBehaviour 
	{

		public Text playerNameText;

		private ArrayList availablePlayers;
		private int currentPlayerIndex = 0;

		// Use this for initialization
		void Start () 
		{
		
		}
		
		// Update is called once per frame
		void Update () 
		{
		
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
			currentPlayerIndex = studentIndex % availablePlayers.Count;
			if (currentPlayerIndex < 0)
								currentPlayerIndex = availablePlayers.Count + currentPlayerIndex;
			SenseixController.RegisterPlayer (GetCurrentPlayer ());
			SenseixController.SetCurrentPlayer (GetCurrentPlayer ());
			SetName ();
		}

		public senseix.message.player.Player GetCurrentPlayer()
		{
			return availablePlayers [currentPlayerIndex] as senseix.message.player.Player;
		}

		public void SetName()
		{
			message.player.Player newPlayer = availablePlayers [currentPlayerIndex] as message.player.Player;
			string newName = newPlayer.Name;
			playerNameText.text = newName;
		}

		public void PullAvailablePlayers()
		{
			SenseixController.ListPlayers ();
			availablePlayers = SenseixController.GetCurrentPlayerList ();
		}
	}
}
