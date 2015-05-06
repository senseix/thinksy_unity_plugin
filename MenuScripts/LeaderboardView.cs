using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Senseix
{
	public class LeaderboardView : MonoBehaviour {

		public Text pageNumberText;
		public int PlayersPerPage = 5;
		private uint currentPage = 1;

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void IncrementPage()
		{
			if (currentPage < 99)
				currentPage++;
			UpdateView ();
		}

		public void DecrementPage()
		{
			if (currentPage > 1)
				currentPage--;
			UpdateView ();
		}

		public void SetPageNumberText()
		{
			pageNumberText.text = currentPage.ToString ();
		}

		void OnEnable()
		{
			currentPage = 1;
			UpdateView ();
		}

		public void UpdateView()
		{
			SetPageNumberText ();
			Refresh ();
		}

		public void Refresh()
		{
			SenseixSession.PullLeaderboard (currentPage, (uint)PlayersPerPage);
			IList<Message.Leaderboard.PlayerData> leaders = SenseixSession.GetCurrentLeaderboard ();

			Text thisText = gameObject.GetComponent<Text> ();
			thisText.text = "";
			for (int i = 0; i < leaders.Count; i++)
			{
				thisText.text += (i + 1).ToString() + ". ";
				thisText.text += leaders[i].name + " Sensei - ";
				thisText.text += leaders[i].score.ToString() + " points";
				thisText.text += "\n";
			}
		}
	}
}
