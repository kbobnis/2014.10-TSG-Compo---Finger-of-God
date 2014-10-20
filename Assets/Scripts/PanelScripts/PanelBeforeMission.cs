using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class PanelBeforeMission : MonoBehaviour {

	public GameObject TextInfo, ButtonStartMission, PanelPrefabHolder, BuildingPrefab, TextMissionQuery;
	public GameObject PanelMinigame, PanelTopBar;

	public void MissionBriefing(Mission m){
		for (int i = 0; i < PanelPrefabHolder.transform.childCount; i++) {
			Destroy(PanelPrefabHolder.transform.GetChild(i).gameObject);
		}
		PanelPrefabHolder.transform.DetachChildren();

		if (m.FailureQueries.Count > 0) {
			AchievQuery failureQuery = m.FailureQueries[0];
			TextMissionQuery.GetComponent<Text>().text = "\nLose when " + failureQuery.ScoreType + " " + failureQuery.Sign.Text() + " " + failureQuery.Value;
		} else if (m.SuccessQueries.Count > 0) {
			AchievQuery successQuery = m.SuccessQueries[0];
			TextMissionQuery.GetComponent<Text>().text = "Win when " + successQuery.ScoreType + " " + successQuery.Sign.Text() + " " + successQuery.Value;
		}

		BuildingType showBuilding = m.BeforeMissionBuilding;
		if (showBuilding == BuildingType.None) {
			showBuilding = BuildingTypeMethods.RandomBuilding();
		}

		TextInfo.GetComponent<Text>().text = showBuilding.Description();

		GameObject newItem = Instantiate(BuildingPrefab) as GameObject;
		newItem.SetActive(true);
		newItem.transform.parent = PanelPrefabHolder.transform;

		RectTransform r = PanelPrefabHolder.GetComponent<RectTransform>();
		RectTransform itemR = newItem.GetComponent<RectTransform>();
		itemR.offsetMin = new Vector2(-r.rect.width / 2, -r.rect.height / 2);
		itemR.offsetMax = new Vector2(r.rect.width / 2, r.rect.height / 2);
		Building b = newItem.GetComponent<Building>();
		b.CreateFromTemplate(showBuilding);
		
		ButtonStartMission.GetComponent<Button>().onClick.AddListener(() => {
			StartMinigame(m);
		});

	}

	private void StartMinigame(Mission m) {

		try {
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

			PanelMinigame.GetComponent<PanelMinigame>().PrepareGame(m, scoreListeners);
			gameObject.SetActive(false);
		} catch (System.Exception e) {
			Debug.Log("Exceptin: " + e);
		}
	}

}
