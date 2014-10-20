using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SimpleJSON;

public class PanelLoadingMission : MonoBehaviour {

	public GameObject PanelMainMenu, PanelBeforeMission, TextInfo, ButtonContinue;

	// Use this for initialization
	void Start () {
		TextInfo.GetComponent<Text>().text = "Loading mission";
		ButtonContinue.SetActive(false);
	}
	
	internal void LoadMission(MissionType mt) {
		StartCoroutine(LoadMissionCoroutine(mt));
	}

	private IEnumerator LoadMissionCoroutine(MissionType mt) {

		WWW www = WebConnector.LoadMission(mt);
		yield return www;

		if (www.error != null) {
			Debug.Log("Some errors occured: " + www.error);
			TextInfo.GetComponent<Text>().text = "Error occured: " + www.error;
			ButtonContinue.SetActive(true);
			ButtonContinue.GetComponentInChildren<Text>().text = "Return to main menu";
			ButtonContinue.GetComponent<Button>().onClick.AddListener(() => {
				PanelMainMenu.SetActive(true);
				gameObject.SetActive(false);
			});

		} else {

			try {
				TextInfo.GetComponent<Text>().text = "Mission loaded ";

				JSONNode n = JSONNode.Parse(www.text);

				int number = n["number"].AsInt;
				TextInfo.GetComponent<Text>().text = n["map"];
				string jsonMap = WWW.UnEscapeURL( n["map"]);

				ButtonContinue.SetActive(true);
				ButtonContinue.GetComponent<Button>().onClick.AddListener(() => {
					try {
						PanelBeforeMission.SetActive(true);
						PanelBeforeMission.GetComponent<PanelBeforeMission>().MissionBriefing(new Mission(jsonMap, mt, number));
						gameObject.SetActive(false);
					} catch (System.Exception e) {
						Debug.Log("Exception: " + e);
					}
				});
			} catch (System.Exception e) {
				Debug.Log("Exception: " + e);
			}

		}
	}
}
