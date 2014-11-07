using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class PanelBeforeMission : MonoBehaviour {

	public GameObject TextInfo, BuildingsList, TextMissionQuery;
	public GameObject PanelMinigame, PanelTopBar, PanelMainMenu, PanelButtons;

	private Mission Mission;

	void Awake() {
		PanelButtons.SetActive(true); // this is a template, don't touch it.
		GameObject go = PanelButtons.GetComponent<PanelButtons>().CopyMeIn(gameObject);
		PanelButtons.SetActive(false); // this is a template, don't touch it.

		go.GetComponent<PanelButtons>().ButtonTopText.GetComponent<Text>().text = "Start Mission";
		go.GetComponent<PanelButtons>().ButtonTop.GetComponent<Button>().onClick.AddListener(() => { StartMinigameListener(); });

		go.GetComponent<PanelButtons>().ButtonBottomText.GetComponent<Text>().text = "Main menu";
		go.GetComponent<PanelButtons>().ButtonBottom.GetComponent<Button>().onClick.AddListener(() => { ReturnToMainMenu(); });
	}


	public void ReturnToMainMenu() {
		PanelMainMenu.SetActive(true);
		gameObject.SetActive(false);
	}

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

	}

	private void StartMinigameListener(){
		StartCoroutine( StartMinigame() );
	}

	private IEnumerator StartMinigame() {

		PanelMinigame.SetActive(true);

		List<Listener<ScoreType, float>> scoreListeners = new List<Listener<ScoreType, float>>();
		scoreListeners.Add(PanelTopBar.GetComponent<PanelTopBar>().TextPopulation.GetComponent<NumberShower>());
		scoreListeners.Add(PanelTopBar.GetComponent<PanelTopBar>().TextInterventions.GetComponent<NumberShower>());

		if (Mission.FailureQueries.Count > 0) {
			AchievQueryShower failureQuery = PanelTopBar.GetComponent<PanelTopBar>().TextMissionFailure.GetComponent<AchievQueryShower>();
			failureQuery.AchievQuery = Mission.FailureQueries[0];
			scoreListeners.Add(failureQuery);
		}

		if (Mission.SuccessQueries.Count > 0) {
			AchievQueryShower successQuery = PanelTopBar.GetComponent<PanelTopBar>().TextMissionSuccess.GetComponent<AchievQueryShower>();
			successQuery.AchievQuery = Mission.SuccessQueries[0];
			scoreListeners.Add(successQuery);
		}

		//ButtonStartMission.GetComponentInChildren<Text>().text = "Preparing mission...";
		PanelMinigame.GetComponent<PanelMinigame>().PrepareGame(Mission, scoreListeners);
		PanelTopBar.SetActive(true);
		yield return null;
		gameObject.SetActive(false);
	}

}
