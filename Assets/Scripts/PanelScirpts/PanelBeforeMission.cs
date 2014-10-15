using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class PanelBeforeMission : MonoBehaviour {

	public GameObject TextInfo, ButtonStartMission, PanelPrefabHolder, BuildingPrefab;
	public GameObject PanelMinigame, PanelTopBar;

	public void PleaseStartMinigame(Mission m, PanelMainMenu pmm) {
		Debug.Log("Please start minigame");
		for (int i = 0; i < PanelPrefabHolder.transform.childCount; i++) {
			Destroy(PanelPrefabHolder.transform.GetChild(i).gameObject);
		}
		PanelPrefabHolder.transform.DetachChildren();

		if (m.BeforeText != "") {
			TextInfo.GetComponent<Text>().text = m.BeforeText;
		} else {
			StartMinigame(m, new List<Listener<MissionStatus, bool>> { pmm });
		}

		Debug.Log("before building: " + m.BeforeBuilding);
		if (m.BeforeBuilding != BuildingType.None) {
			GameObject newItem = Instantiate(BuildingPrefab) as GameObject;
			newItem.SetActive(true);
			newItem.transform.parent = PanelPrefabHolder.transform;

			RectTransform r = PanelPrefabHolder.GetComponent<RectTransform>();
			RectTransform itemR = newItem.GetComponent<RectTransform>();
			itemR.offsetMin = new Vector2(-r.rect.width/2, -r.rect.height/2);
			itemR.offsetMax = new Vector2(r.rect.width/2, r.rect.height/2);
			Building b = newItem.GetComponent<Building>();
			b.CreateFromTemplate(m.BeforeBuilding);
			

		}
		ButtonStartMission.GetComponent<Button>().onClick.AddListener(() => { 
			StartMinigame(m, new List<Listener<MissionStatus, bool>>(){pmm}); 
		});
	}


	private void StartMinigame(Mission m, List<Listener<MissionStatus, bool>> missionStatusListeners) {
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
