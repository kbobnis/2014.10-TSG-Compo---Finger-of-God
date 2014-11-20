using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;



public class PanelMainMenu : MonoBehaviour {

	public GameObject PanelLoadingMission;

	public void StartMissionSpecified() {
		StartMission(MissionType.Specified, false);
	}

	public void StartMission(MissionType mt, bool toRepeat) {
		PanelLoadingMission.SetActive(true);
		PanelLoadingMission.GetComponent<PanelLoadingMission>().LoadMission(mt, toRepeat);
		gameObject.SetActive(false);
	}

	public void StartMissionRandom() {
		StartMission(MissionType.Random, false);
	}

	public void QuitApplication() {
		Application.Quit();
	}


}
