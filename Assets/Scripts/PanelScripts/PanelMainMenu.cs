using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;



public class PanelMainMenu : MonoBehaviour {

	public GameObject PanelLoadingMission, PanelButtons;

	void Awake() {
		
		PanelButtons.SetActive(true); // this is a template, don't touch it.
		GameObject go = PanelButtons.GetComponent<PanelButtons>().CopyMeIn(gameObject);
		PanelButtons.SetActive(false); // this is a template, don't touch it.

		go.GetComponent<PanelButtons>().ButtonTopText.GetComponent<Text>().text = "Missions";
		go.GetComponent<PanelButtons>().ButtonTop.GetComponent<Button>().onClick.AddListener(() => { StartMissionSpecified(); });

		go.GetComponent<PanelButtons>().ButtonBottomText.GetComponent<Text>().text = "Quick game";
		go.GetComponent<PanelButtons>().ButtonBottom.GetComponent<Button>().onClick.AddListener(() => { StartMissionRandom(); });
		go.GetComponent<PanelButtons>().ShowClouds(false);
	}

	public void StartMissionSpecified() {
		StartMission(MissionType.Specified);
	}

	public void StartMission(MissionType mt) {
		PanelLoadingMission.SetActive(true);
		PanelLoadingMission.GetComponent<PanelLoadingMission>().LoadMission(mt);
		gameObject.SetActive(false);
	}

	public void StartMissionRandom() {
		StartMission(MissionType.Random);
	}

}
