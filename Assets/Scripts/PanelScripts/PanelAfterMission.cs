using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using SimpleJSON;

public class PanelAfterMission : MonoBehaviour {

	public GameObject PanelMainMenu, ButtonMainMenu, ButtonQuickNextMission, TextUsersResults, TextMissionResult, InputFieldYourName, TextFieldYourName;
	private int Delay = 1;
	private float TimeEndMission;
	private Mission Mission;
	private Dictionary<ScoreType, Result> ActualResults;

	void OnEnable() {
		if (Game.Me != null && Game.Me.UserName != null){
			InputFieldYourName.GetComponent<InputField>().value = TextFieldYourName.GetComponent<Text>().text = Game.Me.UserName;
		}
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

	public void Prepare(Mission mission, Dictionary<ScoreType, Result> actualResults) {
		Mission = mission;
		ActualResults = actualResults;
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
		UpdateText(www);
	}

	private void UpdateText(WWW www) {
		try {
			string text = "";
			if (www.error != null) {
				text = www.error;
			} else {
				JSONNode n = JSONNode.Parse(www.text);
				foreach (JSONNode tile in n.Childs) {
					string name = tile["name"];
					string interv = tile["interventions"];
					float time = tile["time"].AsInt / 1000;
					text += name+ ", " + interv + " interv, " + time + "s\n";
				}
			}
			TextUsersResults.GetComponent<Text>().text = text;
		} catch (System.Exception e) {
			Debug.Log("Exception: " + e);
		}
	}

	private IEnumerator UpdateNameCoroutine(string newName) {
		WWW www = WebConnector.ChangeName(newName, Mission, ActualResults);
		yield return www;
		UpdateText(www);
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

	public void ChangeName() {
		try {
			GameObject ifyn = InputFieldYourName;
			string text = ifyn.GetComponent<InputField>().value;
			string name = text;
			StartCoroutine(UpdateNameCoroutine(name));
		} catch (System.Exception e) {
			Debug.Log("Exception: " + e);
		}
	}
}
