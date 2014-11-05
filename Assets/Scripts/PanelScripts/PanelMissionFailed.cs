using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PanelMissionFailed : MonoBehaviour {

	public GameObject TextTip, PanelMainMenu, PanelButtons;
	private Mission Mission;

	void Awake() {
		PanelButtons.SetActive(true); // this is a template, don't touch it.
		GameObject go = PanelButtons.GetComponent<PanelButtons>().CopyMeIn(gameObject);
		PanelButtons.SetActive(false); // this is a template, don't touch it.

		go.GetComponent<PanelButtons>().ButtonTopText.GetComponent<Text>().text = "Repeat mission";
		go.GetComponent<PanelButtons>().ButtonTop.GetComponent<Button>().onClick.AddListener(() => { RepeatMission(); });

		go.GetComponent<PanelButtons>().ButtonBottomText.GetComponent<Text>().text = "Main Menu";
		go.GetComponent<PanelButtons>().ButtonBottom.GetComponent<Button>().onClick.AddListener(() => { MainMenu(); });

	}

	public void Prepare(Mission mission, Dictionary<ScoreType, Result> actualResults) {
		Mission = mission;
		TextTip.GetComponent<Text>().text = "Watch out your interventions. \n"+mission.TipText;
	}

	private void MainMenu() {
		PanelMainMenu.SetActive(true);
		gameObject.SetActive(false);
	}

	private void RepeatMission() {
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
