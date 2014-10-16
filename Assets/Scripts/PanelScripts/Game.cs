using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

	public GameObject PanelMainMenu, PanelBeforeMission, PanelAfterMission;

	// Use this for initialization
	void Start () {
		PanelMainMenu.SetActive(true);
		PanelBeforeMission.SetActive(false);
		PanelAfterMission.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
