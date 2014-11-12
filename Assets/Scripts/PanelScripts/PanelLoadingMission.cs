using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SimpleJSON;

public class PanelLoadingMission : MonoBehaviour {

	public GameObject PanelMainMenu, PanelBeforeMission, TextInfo, PanelButtons;
	private MissionType MissionType;
	private GameObject MyPanelButtons;

	void Start () {
		TextInfo.GetComponent<Text>().text = "Loading mission";
	}

	void Awake() {

		PanelButtons.SetActive(true); // this is a template, don't touch it.
		GameObject go = PanelButtons.GetComponent<PanelButtons>().CopyMeIn(gameObject);
		MyPanelButtons = go;
		PanelButtons.SetActive(false); // this is a template, don't touch it.

		go.GetComponent<PanelButtons>().ButtonTopText.GetComponent<Text>().text = "Restart Levels";
		go.GetComponent<PanelButtons>().ButtonTop.GetComponent<Button>().onClick.AddListener(() => { StartCoroutine(RestartLevelsCoroutine()); });

		go.GetComponent<PanelButtons>().ButtonBottomText.GetComponent<Text>().text = "Main Menu";
		go.GetComponent<PanelButtons>().ButtonBottom.GetComponent<Button>().onClick.AddListener(() => { PanelMainMenu.SetActive(true); gameObject.SetActive(false); });
		go.SetActive(false);
	}
	
	internal void LoadMission(MissionType mt) {
		StartCoroutine(LoadMissionCoroutine(mt));
	}

	private IEnumerator RestartLevelsCoroutine() {
		MyPanelButtons.SetActive(false);
		TextInfo.GetComponent<Text>().text = "Restarting levels...";
		yield return new WaitForSeconds(1f);
		yield return WebConnector.RestartLevels();
		StartCoroutine(LoadMissionCoroutine(MissionType.Specified));
	}

	private IEnumerator LoadMissionCoroutine(MissionType mt) {
		WWW www = WebConnector.LoadMission(mt);
		yield return www;

		if (www.error != null) {
			Debug.Log("Some errors occured: " + www.error);
			TextInfo.GetComponent<Text>().text = "Internet connection required: " + www.error;
			MyPanelButtons.SetActive(true);
			MyPanelButtons.GetComponent<PanelButtons>().ButtonTop.SetActive(false);
		} else {
			try {
				JSONNode n = JSONNode.Parse(www.text);
				string name = n["missionName"];
				int round = n["round"].AsInt;
				string jsonMap = WWW.UnEscapeURL( n["map"]);
				Game.Me.UserName = WWW.UnEscapeURL(n["name"]);
				if (jsonMap == null || jsonMap == "" || jsonMap == "null") {
					TextInfo.GetComponent<Text>().text = "There are no more missions. \n\nVisit fb page for updates.";
					MyPanelButtons.SetActive(true);
				} else {
					PanelBeforeMission.SetActive(true);
					PanelBeforeMission.GetComponent<PanelBeforeMission>().MissionBriefing(new Mission(jsonMap, mt, name, round));
					gameObject.SetActive(false);
				}
			} catch (System.Exception e) {
				Debug.Log("Exception: " + e);
			}
		}
	}
}
