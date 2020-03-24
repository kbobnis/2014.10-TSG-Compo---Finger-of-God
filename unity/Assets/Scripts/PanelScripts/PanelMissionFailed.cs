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
		//it is a backend thing. we don't want to repeat the last successfull mission. we want to do the next one
		PanelMainMenu.GetComponent<PanelMainMenu>().StartMission(Mission.MissionType, false);
		gameObject.SetActive(false);
	}
}
