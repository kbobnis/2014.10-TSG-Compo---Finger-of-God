using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {

	public GameObject PanelMainMenu, PanelBeforeMission, PanelAfterMission, PanelLoadingMission;

	// Use this for initialization
	void Start () {
		PanelMainMenu.SetActive(true);
		PanelBeforeMission.SetActive(false);
		PanelAfterMission.SetActive(false);
		PanelLoadingMission.SetActive(false);
	}

}
