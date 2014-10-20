using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using SimpleJSON;

public class PanelAfterMission : MonoBehaviour {

	public GameObject PanelMainMenu, ButtonMainMenu, ButtonQuickNextMission, TextUsersResults, TextMissionResult, InputFieldYourName;
	private int Delay = 1;
	private float TimeEndMission;
	private Mission Mission;

	void OnEnable() {
		TimeEndMission = 0;
		ButtonMainMenu.SetActive(false);
		ButtonQuickNextMission.SetActive(false);
	}

	void Update() {
		if (TimeEndMission > 0 && TimeEndMission < Time.time - Delay) {
			ButtonMainMenu.SetActive(true);
			ButtonQuickNextMission.SetActive(true);
		}
	}

	public void Prepare(Mission mission, Dictionary<ScoreType, Result> ActualResults) {
		Mission = mission;
		StartCoroutine(SaveMissionCoroutine(mission, ActualResults));

		TextUsersResults.GetComponent<Text>().text = "Loading scores";
		//because i can not access children of inactive gameobject, lol.
		ButtonQuickNextMission.SetActive(true);
		switch(mission.GetStatus(ActualResults)){
			case MissionStatus.Failure: 
				TextMissionResult.GetComponent<Text>().text = "Mission failed";
				ButtonQuickNextMission.GetComponentInChildren<Text>().text = "Repeat mission";
				TimeEndMission = Time.time;
				break;
			case MissionStatus.Success:
				TextMissionResult.GetComponent<Text>().text = "Mission success";
				ButtonQuickNextMission.GetComponentInChildren<Text>().text = mission.MissionType==global::MissionType.Specified?"Next mission":"Next quick game";
				TimeEndMission = Time.time;
				break;
		}
		ButtonQuickNextMission.SetActive(false);
	}

	private IEnumerator SaveMissionCoroutine(Mission mission, Dictionary<ScoreType, Result> ActualResults) {

		WWW www = mission.SaveGame(ActualResults);
		yield return www;

		string text = "";
		if (www.error != null) {
			Debug.Log("Some errors occured: " + www.error);
			text = www.error;
		} else {
			Debug.Log("Mission saved: " + www.text);
			//text = www.text;
			JSONNode n = JSONNode.Parse(www.text);

			foreach (JSONNode tile in n.Childs) {
				string name = tile["name"];
				string interv = tile["interventions"];
				float time = tile["time"].AsInt / 1000;
				text += "Name: " + name + ", interv: " + interv + ", time: " + time + "\n";
			}
		}

		
		TextUsersResults.GetComponent<Text>().text = text;
	}

	public void ShowMainMenu() {
		gameObject.SetActive(false);
		PanelMainMenu.SetActive(true);
	}

	public void QuickNextMission() {
		PanelMainMenu.SetActive(true);
		switch (Mission.MissionType) {
			case global::MissionType.Specified:
				PanelMainMenu.GetComponent<PanelMainMenu>().StartMissionSpecified();
				break;
			case global::MissionType.Random:
				PanelMainMenu.GetComponent<PanelMainMenu>().StartMissionRandom();
				break;
			default: throw new UnityEngine.UnityException("There is no button for mission type: " + Mission.MissionType);
		}
		gameObject.SetActive(false);
	}
}
