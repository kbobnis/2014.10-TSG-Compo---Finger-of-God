using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PanelAfterMission : MonoBehaviour {

	public GameObject PanelMainMenu, TextInfo, ButtonMainMenu, ButtonQuickNextMission;
	private int Delay = 1;

	private float TimeEndMission;

	public void Clear(MissionStatus t) {
		throw new System.NotImplementedException();
	}

	void OnEnable() {
		TimeEndMission = 0;
		ButtonMainMenu.SetActive(false);
		ButtonQuickNextMission.SetActive(false);
	}

	void Update() {
		if (TimeEndMission > 0 && TimeEndMission < Time.time - Delay) {
			ButtonMainMenu.SetActive(true);
			ButtonQuickNextMission.SetActive(true);
		}
	}

	public void Inform(MissionStatus t, bool delta) {
		//because i can not access children of inactive gameobject, lol.
		ButtonQuickNextMission.SetActive(true);
		switch(t){
			case MissionStatus.Failure: 
				TextInfo.GetComponent<Text>().text = "Mission failed";
				ButtonQuickNextMission.GetComponentInChildren<Text>().text = "Repeat mission";
				TimeEndMission = Time.time;
				break;
			case MissionStatus.Success:
				TextInfo.GetComponent<Text>().text = "Mission success";
				ButtonQuickNextMission.GetComponentInChildren<Text>().text = "Next mission";
				TimeEndMission = Time.time;
				break;
		}
		ButtonQuickNextMission.SetActive(false);
	}

	public void ShowMainMenu() {
		gameObject.SetActive(false);
		PanelMainMenu.SetActive(true);
	}

	public void QuickNextMission() {
		PanelMainMenu.SetActive(true);
		PanelMainMenu.GetComponent<PanelMainMenu>().StartMission();
		gameObject.SetActive(false);
	}
}
