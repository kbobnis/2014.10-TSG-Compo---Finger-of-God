using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class PanelBeforeMission : MonoBehaviour {

	public GameObject TextInfo, Image, ButtonStartMission;
	public GameObject PanelMinigame, PanelTopBar;


	public void PleaseStartMinigame(Mission m, PanelMainMenu pmm) {
		Debug.Log("Please start minigame");
		if (m.BeforeText != null) {
			TextInfo.GetComponent<Text>().text = m.BeforeText;
		}
		ButtonStartMission.GetComponent<Button>().onClick.AddListener(() => { 
			StartMinigame(m, new List<Listener<MissionStatus, bool>>(){pmm}); 
		});
	}

	public void StartMinigame(Mission m, List<Listener<MissionStatus, bool>> missionStatusListeners) {
		gameObject.SetActive(false);
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

		PanelMinigame.GetComponent<Minigame>().PrepareGame(m, scoreListeners, missionStatusListeners);
	}
}
