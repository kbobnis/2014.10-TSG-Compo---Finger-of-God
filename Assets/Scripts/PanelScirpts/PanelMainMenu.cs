using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public delegate bool GoQuestion(Building go);

public class PanelMainMenu : MonoBehaviour {

	//for the inspector to click this
	public void ResetMission() {
		Mission.ResetMission();
	}

	public void StartMission() {
		//load actual mission;
		try {
			int number = Mission.GetCompletedMission() + 1;

			string path = "Maps/map" + number;
			if (Resources.Load<TextAsset>(path) != null) {
				Mission mission = Mission.LoadMission(number);
				Canvas.Me.PleaseStartMinigame(mission);
				gameObject.SetActive(false);
			} else {
				Debug.Log("There is no mission named: " + path);
			}

		} catch (System.Exception e) {
			Debug.Log("Exception: " + e);
		}
	}

	// Use this for initialization
	public void QuickGame() {
		try {
			Canvas.Me.PleaseStartMinigame(Mission.MissionWithRandom());
			gameObject.SetActive(false);
		} catch (System.Exception e) {
			Debug.Log("Exceptin: " + e);
		}
	}

}
