using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using SimpleJSON;

public class PanelAfterMission : MonoBehaviour {

	public GameObject PanelMainMenu, ButtonMainMenu, ButtonQuickNextMission, TextUsersResults, TextMissionResult, InputFieldYourName, TextFieldYourName, PanelYourName, TextYourScore;
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

	public void Prepare(Mission mission, Dictionary<ScoreType, Result> actualResults, WWW www) {
		Mission = mission;
		ActualResults = actualResults;
		UpdateText(www);

		ButtonQuickNextMission.SetActive(true);
		PanelYourName.SetActive(true);
		TextMissionResult.GetComponent<Text>().text = "Mission success";
		TextYourScore.GetComponent<Text>().text = "Interventions: " + actualResults[ScoreType.Interventions].Value + ", time: " + (actualResults[ScoreType.Time].Value.ToString("#.##")) + "s";
		ButtonQuickNextMission.GetComponentInChildren<Text>().text = mission.MissionType==global::MissionType.Specified?"Next mission":"Next quick game";
		TimeEndMission = Time.time;
		ButtonQuickNextMission.SetActive(false);
	}

	private void UpdateText(WWW www) {
		try {
			string text = "";
			if (www.error != null) {
				text = www.error;
			} else {

				JSONNode n = JSONNode.Parse(www.text);
				int i = 0; 
				foreach (JSONNode tile in n.Childs) {
					string name = tile["name"];
					string interv = tile["interventions"];
					string time = (tile["time"].AsInt / 1000f).ToString("#.##");
					if (tile["deviceId"].Value == WebConnector.GetDeviceId()) {
						text += "---------------------------------\n";
					}
					text += (++i) + ". Interv: " + interv + ", time: " + time + "s, name: " + name + "\n";
					if (tile["deviceId"].Value == WebConnector.GetDeviceId()) {
						text += "---------------------------------\n";
					}
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
