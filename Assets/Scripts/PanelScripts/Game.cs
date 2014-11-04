using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using SimpleJSON;
using System.Linq;
using System;

public class Game : MonoBehaviour {

	public static Game Me;
	public GameObject PanelMainMenu, PanelBeforeMission, PanelAfterMission, PanelLoadingMission, PanelTopBar, PanelLoading, PanelMissionFailed;
	public string UserName = "Anonymous";
	public Dictionary<int, BuildingTemplate> BuildingTemplates = new Dictionary<int, BuildingTemplate>();
	public List<Element> TouchPowers = new List<Element>();
	
	void Awake () {
		Me = this;
		Game.Me.GetComponent<GoogleAnalyticsV3>().StartSession();
		PrepareLoading();
	}

	void OnApplicatoinPause(bool pauseStatus) {
		if (pauseStatus == false) {
			GetComponent<GoogleAnalyticsV3>().StopSession();
		} else {
			GetComponent<GoogleAnalyticsV3>().StartSession();
		}
	}

	private void PrepareLoading() {
		PanelMainMenu.SetActive(false);
		PanelBeforeMission.SetActive(false);
		PanelAfterMission.SetActive(false);
		PanelLoadingMission.SetActive(false);
		PanelTopBar.SetActive(false);
		PanelLoading.SetActive(true);
		PanelMissionFailed.SetActive(false);

		PanelLoading.GetComponent<PanelLoading>().TextTop = "Loading data";
		PanelLoading.GetComponent<PanelLoading>().TextTap = "";
		StartCoroutine(LoadData());
	}

	private IEnumerator LoadData() {
		WWW www = null;
		try {
			 www = WebConnector.GetInitialData();
		} catch (Exception e) {
			Debug.Log("exception: " + e);
		}
		yield return www;

		try {
			if (!string.IsNullOrEmpty(www.error)) {
				Debug.Log("www error: " + www.error);

				PanelLoading.GetComponent<PanelLoading>().JustDo = () => { PrepareLoading(); };
				PanelLoading.GetComponent<PanelLoading>().TextTop = "Internet connection required. (" + www.error + ")";
				PanelLoading.GetComponent<PanelLoading>().TextTap = "Tap to retry";
			} else {
				ParseInitialData(www.text);
				PanelLoading pl = PanelLoading.GetComponent<PanelLoading>();
				PanelLoading.SetActive(false);
				PanelMainMenu.SetActive(true);
				PanelBeforeMission.SetActive(false);
				PanelAfterMission.SetActive(false);
				PanelLoadingMission.SetActive(false);
				PanelTopBar.SetActive(true);
			}
		} catch (Exception e) {
			Debug.Log("Exception: " + e);
		}
	}

	private void ParseInitialData(string initialDataJson) {
		JSONNode n = JSONNode.Parse(initialDataJson);

		XmlDocument model = new XmlDocument();
		string modelString = WWW.UnEscapeURL(n["model"]);
		model.LoadXml(modelString);

		XmlNode defaultsXml = model.GetElementsByTagName("defaults")[0];

		Dictionary<StatType, Dictionary<Element, float>> defaultStats = ParseStats(defaultsXml.ChildNodes);

		foreach (XmlNode buildingXml in model.GetElementsByTagName("building")) {

			Dictionary<StatType, Dictionary<Element, float>> thisStats = ParseStats(buildingXml.ChildNodes);
			foreach (StatType st in defaultStats.Keys.ToList()) {
				foreach (Element el in defaultStats[st].Keys.ToList()) {
					if (!thisStats.ContainsKey(st)) {
						thisStats.Add(st, defaultStats[st]);
					}
					if (!thisStats[st].ContainsKey(el)) {
						thisStats[st].Add(el, defaultStats[st][el]);
					}
				}
			}
			int id = int.Parse(buildingXml.Attributes["id"].Value);
			string name = buildingXml.Attributes["name"].Value;
			int population = int.Parse(buildingXml.Attributes["population"].Value);
			string imagePath = buildingXml.Attributes["image"].Value;
			string imageDPath = buildingXml.Attributes["imageDestroyed"].Value;
			float health = float.Parse(buildingXml.Attributes["health"].Value == "" ? "1" : buildingXml.Attributes["health"].Value);

			BuildingTemplates.Add(id, new BuildingTemplate(id, name, population, health, imagePath, imageDPath, thisStats));
		}

		foreach (XmlNode power in model.GetElementsByTagName("power")) {
			TouchPowers.Add((Element)power.Attributes["elId"].Value);
		}
	}

	private Dictionary<StatType, Dictionary<Element, float>> ParseStats(XmlNodeList statsXml) {
		Dictionary<StatType, Dictionary<Element, float>> stats = new Dictionary<StatType, Dictionary<Element, float>>();

		foreach (XmlNode statXml in statsXml) {
			if (statXml.NodeType == XmlNodeType.Comment || statXml.NodeType == XmlNodeType.None) {
				continue;
			}
			string typeString = statXml.Attributes["type"].Value;
			string elementString = statXml.Attributes["elId"].Value;
			string valueString = statXml.Attributes["value"].Value;

			StatType type = (StatType) typeString;
			Element element = (Element)elementString;
			float value = float.Parse(valueString);

			if (!stats.ContainsKey(type)) {
				stats.Add(type, new Dictionary<Element, float>());
			}
			if (stats[type].ContainsKey(element)) {
				throw new UnityException("You can not define two stats with the same type in one building, " + type + ", " + element);
			}
			stats[type].Add(element, value);
		}
		return stats;
	}

}
