using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PanelMissionFailed : MonoBehaviour {

	public GameObject TextTip, PanelMainMenu;
	private Mission Mission;

	public void Prepare(Mission mission, Dictionary<ScoreType, Result> actualResults) {
		Mission = mission;
		TextTip.GetComponent<Text>().text = "Watch out your interventions. \n"+mission.TipText;
	}

	public void MainMenu() {
		PanelMainMenu.SetActive(true);
		gameObject.SetActive(false);
	}

	public void RepeatMission() {
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
