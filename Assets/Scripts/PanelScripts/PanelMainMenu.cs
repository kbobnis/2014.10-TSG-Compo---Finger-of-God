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

		go.GetComponent<PanelButtons>().ButtonTopText.GetComponent<Text>().text = "Mission";
		go.GetComponent<PanelButtons>().ButtonTop.GetComponent<Button>().onClick.AddListener(() => { StartMissionSpecified(); });

		go.GetComponent<PanelButtons>().ButtonBottomText.GetComponent<Text>().text = "Quick mission";
		go.GetComponent<PanelButtons>().ButtonBottom.GetComponent<Button>().onClick.AddListener(() => { StartMissionRandom(); });
	}

	void OnEnable() {
		Game.Me.GetComponent<BackgroundHolder>().ChangeBg(SpriteManager.MainMenuBg);
	}



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
