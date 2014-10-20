using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public delegate bool GoQuestion(Building go);

public class PanelMainMenu : MonoBehaviour {

	public GameObject PanelLoadingMission;

	public void StartMissionSpecified() {
		PanelLoadingMission.SetActive(true);
		PanelLoadingMission.GetComponent<PanelLoadingMission>().LoadMission(MissionType.Specified);
		gameObject.SetActive(false);
	}

	public void StartMissionRandom() {
		PanelLoadingMission.SetActive(true);
		PanelLoadingMission.GetComponent<PanelLoadingMission>().LoadMission(MissionType.Random);
		gameObject.SetActive(false);
	}

}
