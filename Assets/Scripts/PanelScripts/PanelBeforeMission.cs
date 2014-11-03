using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class PanelBeforeMission : MonoBehaviour {

	public GameObject TextInfo, ButtonStartMission, BuildingsList, TextMissionQuery;
	public GameObject PanelMinigame, PanelTopBar;

	private Mission Mission;

	public void MissionBriefing(Mission m){
		PanelTopBar.SetActive(false);
		Mission = m;

		if (m.FailureQueries.Count > 0) {
			AchievQuery failureQuery = m.FailureQueries[0];
			TextMissionQuery.GetComponent<Text>().text = "Mission: " + m.Name + "\n Destroy all buildings before " + failureQuery.ScoreType + " " + failureQuery.Sign.Text() + " " + failureQuery.Value;
		} else if (m.SuccessQueries.Count > 0) {
			AchievQuery successQuery = m.SuccessQueries[0];
			TextMissionQuery.GetComponent<Text>().text = "Win when " + successQuery.ScoreType + " " + successQuery.Sign.Text() + " " + successQuery.Value;
		}

		TextInfo.GetComponent<Text>().text = m.BeforeMissionText == "" ? m.BeforeMissionBuildings[1].Description : m.BeforeMissionText;
		
		List<List<BuildingTemplate>> b = new List<List<BuildingTemplate>>();
		b.Add(m.BeforeMissionBuildings);
		BuildingsList.GetComponent<ScrollableList>().Build(b, null);

		ButtonStartMission.GetComponent<Button>().onClick.RemoveAllListeners();
		ButtonStartMission.GetComponent<Button>().onClick.AddListener(() => { StartMinigameListener(m); });
	}

	private void StartMinigameListener(Mission m){
		StartCoroutine( StartMinigame(m) );
	}

	private IEnumerator StartMinigame(Mission m) {

		gameObject.SetActive(false);
		PanelMinigame.SetActive(true);

		List<Listener<ScoreType, float>> scoreListeners = new List<Listener<ScoreType, float>>();
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

		//ButtonStartMission.GetComponentInChildren<Text>().text = "Preparing mission...";
		PanelMinigame.GetComponent<PanelMinigame>().PrepareGame(m, scoreListeners);
		PanelTopBar.SetActive(true);
		yield return null;
		
		gameObject.SetActive(false);
	}

}
