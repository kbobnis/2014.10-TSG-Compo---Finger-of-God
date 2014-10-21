using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {

	public GameObject PanelMainMenu, PanelBeforeMission, PanelAfterMission, PanelLoadingMission;

	public string UserName = "Anonymous2";

	public static Game Me;

	// Use this for initialization
	void Start () {
		Me = this;
		PanelMainMenu.SetActive(true);
		PanelBeforeMission.SetActive(false);
		PanelAfterMission.SetActive(false);
		PanelLoadingMission.SetActive(false);
	}

}
