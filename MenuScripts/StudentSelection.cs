using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Senseix
{
	public class StudentSelection : MonoBehaviour 
	{

		public Text playerNameText;

		private ArrayList availablePlayerNames;
		private int currentNameIndex = 0;

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
			SetName (availablePlayerNames [currentNameIndex] as string);
		}

		public void NextStudent()
		{
			currentNameIndex = (currentNameIndex + 1) % availablePlayerNames.Count;
			SetName (availablePlayerNames [currentNameIndex] as string);
		}

		public void PreviousStudent()
		{
			currentNameIndex = (currentNameIndex - 1) % availablePlayerNames.Count;
			SetName (availablePlayerNames [currentNameIndex] as string);
		}

		public void SetName(string newName)
		{
			playerNameText.text = newName;
		}

		public void PullAvailablePlayers()
		{
			//SenseixController.ListPlayers ();
			availablePlayerNames = new ArrayList() {"Alice", "Bob", "Jorge", "Yuanyuan"};
		}
	}
}
