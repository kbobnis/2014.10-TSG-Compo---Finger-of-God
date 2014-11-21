using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SimpleJSON;
using System.Collections.Generic;

public class PanelLoadingMission : MonoBehaviour {

	public GameObject PanelMainMenu, PanelBeforeMission, TextInfo, PanelButtons;
	private MissionType MissionType;

	void Start () {
		TextInfo.GetComponent<Text>().text = "Loading mission";
		GetComponent<ButtonsInCloud>().ClonedPanelButtons.SetActive(false);
	}

	public void RestartLevels() {
		StartCoroutine(RestartLevelsCoroutine());
	}
	
	internal void LoadMission(MissionType mt, bool toRepeat) {
		StartCoroutine(LoadMissionCoroutine(mt, toRepeat));
	}

	public void ReturnToMainMenu() {
		PanelMainMenu.SetActive(true); 
		gameObject.SetActive(false); 
	}

	private IEnumerator RestartLevelsCoroutine() {
		GetComponent<ButtonsInCloud>().ClonedPanelButtons.SetActive(false);
		TextInfo.GetComponent<Text>().text = "Restarting levels...";
		yield return new WaitForSeconds(1f);
		yield return WebConnector.RestartLevels();
		StartCoroutine(LoadMissionCoroutine(MissionType.Specified, false));
	}

	private IEnumerator LoadMissionCoroutine(MissionType mt, bool toRepeat) {
		WWW www = WebConnector.LoadMission(mt, toRepeat);
		yield return www;

		if (www.error != null) {
			Debug.Log("Some errors occured: " + www.error);
			TextInfo.GetComponent<Text>().text = "Internet connection required: " + www.error;
			GetComponent<ButtonsInCloud>().ClonedPanelButtons.SetActive(true);
			GetComponent<ButtonsInCloud>().ClonedPanelButtons.GetComponent<PanelButtons>().ButtonTop.SetActive(false);
		} else {
			try {
				JSONNode n = JSONNode.Parse(www.text);
				string name = n["missionName"];
				int round = n["round"].AsInt;
				string jsonMap = WWW.UnEscapeURL( n["map"]);

				Game.Me.UserName = WWW.UnEscapeURL(n["name"]);

				List<LevelScore> scores = ParseScores(n["results"]);

				if (jsonMap == null || jsonMap == "" || jsonMap == "null") {
					TextInfo.GetComponent<Text>().text = "There are no more missions. \n\nVisit fb page for updates.";
					GetComponent<ButtonsInCloud>().ClonedPanelButtons.SetActive(true);
				} else {
					PanelBeforeMission.SetActive(true);
					PanelBeforeMission.GetComponent<PanelBeforeMission>().MissionBriefing(new Mission(jsonMap, mt, name, round, scores[3], scores));
					gameObject.SetActive(false);
				}
			} catch (System.Exception e) {
				Debug.Log("Exception: " + e);
			}
		}
	}
	private List<LevelScore> ParseScores(JSONNode resultsJson) {
		List<LevelScore> tmp = new List<LevelScore>();

		foreach (JSONNode rJson in resultsJson.Childs) {
			LevelScore ls = new LevelScore();
			if (rJson["deviceId"] != null) {
				ls.AddInfo(rJson["name"].Value, rJson["interventions"].AsInt, rJson["deviceId"].Value, rJson["time"].AsInt / 1000f, rJson["place"].AsInt);
			}
			tmp.Add(ls);
		}

		return tmp;
	}
}
