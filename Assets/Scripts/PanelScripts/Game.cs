using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using SimpleJSON;
using System.Linq;

public class Game : MonoBehaviour {

	public GameObject PanelMainMenu, PanelBeforeMission, PanelAfterMission, PanelLoadingMission, PanelTopBar, PanelLoading;

	public string UserName = "Anonymous";
	public Dictionary<int, BuildingTemplate> BuildingTemplates = new Dictionary<int, BuildingTemplate>();
	public List<Element> TouchPowers = new List<Element>();

	public static Game Me;

	void Awake () {
		Me = this;
		Game.Me.GetComponent<GoogleAnalyticsV3>().StartSession();

		SayLoading();
		StartCoroutine(LoadXml());
		
	}

	void OnApplicatoinPause(bool pauseStatus) {
		if (pauseStatus == false) {
			GetComponent<GoogleAnalyticsV3>().StopSession();
		} else {
			GetComponent<GoogleAnalyticsV3>().StartSession();
		}
	}

	private IEnumerator LoadXml() {
		WWW www = WebConnector.GetInitialData();
		yield return www;

		JSONNode n = JSONNode.Parse(www.text);

		XmlDocument model = new XmlDocument();
		string modelString = WWW.UnEscapeURL(n["model"]);
		model.LoadXml( modelString );
		
		XmlNode defaultsXml = model.GetElementsByTagName("defaults")[0];

		Dictionary<StatType, Dictionary<Element, float>> defaultStats = ParseStats(defaultsXml.ChildNodes);

		foreach (XmlNode buildingXml in model.GetElementsByTagName("building")) {

			Dictionary<StatType, Dictionary<Element, float>> thisStats = ParseStats(buildingXml.ChildNodes);
			foreach(StatType st in defaultStats.Keys.ToList()){
				foreach (Element el in defaultStats[st].Keys.ToList()) {
					if (!thisStats.ContainsKey(st)) {
						thisStats.Add(st, defaultStats[st]);
					}
					if (!thisStats[st].ContainsKey(el)) {
						thisStats[st].Add(el, defaultStats[st][el]);
					}
				}
			}
			int id = int.Parse( buildingXml.Attributes["id"].Value );
			string name = buildingXml.Attributes["name"].Value;
			int population = int.Parse( buildingXml.Attributes["population"].Value );
			string imagePath = buildingXml.Attributes["image"].Value;
			string imageDPath = buildingXml.Attributes["imageDestroyed"].Value;
			float health = float.Parse( buildingXml.Attributes["health"].Value == "" ? "1" : buildingXml.Attributes["health"].Value);

			BuildingTemplates.Add(id, new BuildingTemplate(id, name, population, health, imagePath, imageDPath, thisStats));
		}

		foreach (XmlNode power in model.GetElementsByTagName("power")) {
			TouchPowers.Add((Element)power.Attributes["elId"].Value);
		}

		PanelLoading.GetComponent<PanelLoading>().Ready();
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

	private void SayLoading() {
		PanelMainMenu.SetActive(false);
		PanelBeforeMission.SetActive(false);
		PanelAfterMission.SetActive(false);
		PanelLoadingMission.SetActive(false);
		PanelTopBar.SetActive(false);
		PanelLoading.SetActive(true);
		PanelLoading.GetComponent<PanelLoading>().SetLoading("Loading ", () => { ReadyToPlay(); });
	}

	private void ReadyToPlay() {
		PanelMainMenu.SetActive(true);
		PanelBeforeMission.SetActive(false);
		PanelAfterMission.SetActive(false);
		PanelLoadingMission.SetActive(false);
		PanelTopBar.SetActive(true);
	}

}
