using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class PanelBeforeMission : MonoBehaviour {

	public GameObject TextInfo, BuildingsList;
	public GameObject PanelMinigame, PanelMainMenu;

	private Mission Mission;

	public void ReturnToMainMenu() {
		PanelMainMenu.SetActive(true);
		gameObject.SetActive(false);
	}

	public void MissionBriefing(Mission m){
		Mission = m;

		TextInfo.GetComponent<Text>().text = m.BeforeMissionText == "" ? m.BeforeMissionBuildings[1].Description : m.BeforeMissionText;
		
		List<List<BuildingTemplate>> b = new List<List<BuildingTemplate>>();
		b.Add(m.BeforeMissionBuildings);
		BuildingsList.GetComponent<ScrollableList>().Build(b, null);
	}

	public void StartMinigameListener(){
		StartCoroutine( StartMinigame() );
	}

	private IEnumerator StartMinigame() {
		PanelMinigame.SetActive(true);
		PanelMinigame.GetComponent<PanelMinigame>().PrepareGame(Mission);
		yield return null;
		gameObject.SetActive(false);
	}

}
