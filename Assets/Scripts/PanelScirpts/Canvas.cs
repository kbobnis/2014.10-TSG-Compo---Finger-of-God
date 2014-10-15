using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Canvas : MonoBehaviour, Listener<MissionStatus, bool> {

	public static Canvas Me;

	public GameObject PanelMinigame, PanelTopBar, PanelMainMenu;

	// Use this for initialization
	void Start () {
		Me = this;
	}
	
	public void PleaseStartMinigame(Mission m){
		PanelMinigame.SetActive(true);

		List<Listener<ScoreType, int>> scoreListeners = new List<Listener<ScoreType, int>>();
		scoreListeners.Add(PanelTopBar.GetComponent<PanelTopBar>().TextPopulation.GetComponent<NumberShower>());
		scoreListeners.Add(PanelTopBar.GetComponent<PanelTopBar>().TextInterventions.GetComponent<NumberShower>());

		List<Listener<MissionStatus, bool>> missionStatusListeners = new List<Listener<MissionStatus, bool>>();
		missionStatusListeners.Add(this);
		PanelMinigame.GetComponent<Minigame>().PrepareGame(m, scoreListeners, missionStatusListeners);
	}

	public void Clear(MissionStatus t) {
		throw new System.Exception("Clear mission status is not needed");
	}

	public void Inform(MissionStatus t, bool delta) {
		Debug.Log("Mission status update: " + t + " with: " + delta);
		PanelMainMenu.gameObject.SetActive(true);
	}
}
