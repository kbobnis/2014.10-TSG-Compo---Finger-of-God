using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public delegate bool GoQuestion(Building go);

public class PanelMainMenu : MonoBehaviour, Listener<MissionStatus, bool> {

	public GameObject PanelBeforeMission;

	//for the inspector to click this
	public void ResetMission() {
		Mission.ResetMission();
	}

	public void Clear(MissionStatus t) {
		throw new System.Exception("Clear mission status is not needed");
	}

	public void Inform(MissionStatus t, bool delta) {
		gameObject.SetActive(true);
		Debug.Log("Mission status update: " + t + " with: " + delta);
	}

	public void StartMission() {
		//load actual mission;
		try {
			int number = Mission.GetCompletedMission() + 1;

			string path = "Maps/map" + number;
			if (Resources.Load<TextAsset>(path) != null) {
				Mission mission = Mission.LoadMission(number);
				PanelBeforeMission.gameObject.SetActive(true);
				PanelBeforeMission.GetComponent<PanelBeforeMission>().PleaseStartMinigame(mission, this);
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
			PanelBeforeMission.gameObject.SetActive(true);
			PanelBeforeMission.GetComponent<PanelBeforeMission>().PleaseStartMinigame(Mission.MissionWithRandom(), this);
			gameObject.SetActive(false);
		} catch (System.Exception e) {
			Debug.Log("Exceptin: " + e);
		}
	}

}
