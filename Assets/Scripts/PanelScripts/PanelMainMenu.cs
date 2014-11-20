using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;



public class PanelMainMenu : MonoBehaviour {

	public GameObject PanelLoadingMission;

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

	public void QuitApplication() {
		Application.Quit();
	}

}
