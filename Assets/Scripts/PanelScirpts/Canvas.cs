using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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

		if (m.FailureQueries.Count > 0) {
			AchievQueryShower failureQuery = PanelTopBar.GetComponent<PanelTopBar>().TextMissionFailure.GetComponent<AchievQueryShower>();
			failureQuery.AchievQuery = m.FailureQueries[0];
			scoreListeners.Add(failureQuery);
		}

		if (m.SuccessQueries.Count > 0) {
			AchievQueryShower successQuery = PanelTopBar.GetComponent<PanelTopBar>().TextMissionSuccess.GetComponent<AchievQueryShower>();
			successQuery.AchievQuery = m.SuccessQueries[0];
			scoreListeners.Add(successQuery);
		}


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
