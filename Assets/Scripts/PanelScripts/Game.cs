using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using SimpleJSON;
using System.Linq;
using System;

public class Game : MonoBehaviour {

	public static Game Me;
	public GameObject PanelMainMenu, PanelBeforeMission, PanelAfterMission, PanelLoadingMission, PanelLoading, PanelMissionFailed, PanelMinigame, PanelSettings;
	public string UserName = "Anonymous";
	public Dictionary<int, BuildingTemplate> BuildingTemplates = new Dictionary<int, BuildingTemplate>();
	public List<Element> TouchPowers = new List<Element>();
	
	void Awake () {
		Me = this;
		//https://github.com/kayy/BundleVersionChecker
		GetComponent<GoogleAnalyticsV3>().bundleVersion = new TrackedBundleVersion().current.version;
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
		PanelLoading.SetActive(true);
		PanelMissionFailed.SetActive(false);
		PanelMinigame.SetActive(false);
		PanelSettings.SetActive(false);

		PanelLoading.GetComponent<PanelLoading>().TextTop = "Loading data";
		PanelLoading.GetComponent<PanelLoading>().TextTap = "";
		StartCoroutine(LoadDataCoroutine());
	}

	private IEnumerator LoadDataCoroutine() {
		WWW www = null;
		try {
			 www = WebConnector.GetInitialData();
		} catch (Exception e) {
			Debug.Log("exception: " + e);
		}
		yield return www;

		if (!string.IsNullOrEmpty(www.error)) {
			Debug.Log("www error: " + www.error);

			PanelLoading.GetComponent<PanelLoading>().JustDo = () => { PrepareLoading(); };
			PanelLoading.GetComponent<PanelLoading>().TextTop = "Internet connection required. (" + www.error + ")";
			PanelLoading.GetComponent<PanelLoading>().TextTap = "Tap to retry";
		} else {
			JSONNode n = JSONNode.Parse(www.text);

			if (HasGoodVersion(n)) {
				ParseInitialData(n);
			} else {
				PanelLoading.GetComponent<PanelLoading>().JustDo = () => { Application.OpenURL("https://play.google.com/store/apps/details?id=com.kprojekt.fingerofgod"); };
				string headlines = n["thisVersionHeadlines"].Value;
				PanelLoading.GetComponent<PanelLoading>().TextTop = "There is new version with: " + headlines;
				PanelLoading.GetComponent<PanelLoading>().TextTap = "Tap to update";
			}
		}
	}

	private bool HasGoodVersion(JSONNode n) {
		float appBundleVersion = float.Parse( new TrackedBundleVersion().current.version );
		float backendRequiresVersion = float.Parse(n["requiredVersion"].Value);
		return appBundleVersion >= backendRequiresVersion;
	}

	private void ParseInitialData(JSONNode n) {
		XmlDocument model = new XmlDocument();
		UserName = n["userName"].Value;
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
			
			Dictionary<Element, Sprite[]> effects = new Dictionary<Element,Sprite[]>();
			string fireEffectPath = buildingXml.Attributes["fireEffect"].Value == "" ? SpriteManager.DefaultEffectPaths[Element.Fire] : buildingXml.Attributes["fireEffect"].Value;
			effects.Add(Element.Fire, SpriteManager.ElementPerBuildingSprites[fireEffectPath]);

			string waterEffectPath = buildingXml.Attributes["waterEffect"].Value == "" ? SpriteManager.DefaultEffectPaths[Element.Water] : buildingXml.Attributes["waterEffect"].Value;
			effects.Add(Element.Water, SpriteManager.ElementPerBuildingSprites[waterEffectPath]);

			string electricityEffectPath = buildingXml.Attributes["electricityEffect"].Value == "" ? SpriteManager.DefaultEffectPaths[Element.Electricity] : buildingXml.Attributes["electricityEffect"].Value;
			effects.Add(Element.Electricity, SpriteManager.ElementPerBuildingSprites[electricityEffectPath]);

			string smokeAfterFireffectPath = buildingXml.Attributes["smokeAfterFireEffect"].Value == "" ? SpriteManager.DefaultEffectPaths[Element.SmokeAfterFire]: buildingXml.Attributes["smokeAfterFireEffect"].Value;
			effects.Add(Element.SmokeAfterFire, SpriteManager.ElementPerBuildingSprites[smokeAfterFireffectPath]);

			string crushEffectPath = buildingXml.Attributes["crushEffect"].Value == "" ? SpriteManager.DefaultEffectPaths[Element.Crush] : buildingXml.Attributes["crushEffect"].Value;
			effects.Add(Element.Crush, SpriteManager.ElementPerBuildingSprites[crushEffectPath]);

			string dieEffectPath = buildingXml.Attributes["dieEffect"].Value == "" ? SpriteManager.DefaultEffectPaths[Element.Die] : buildingXml.Attributes["dieEffect"].Value;
			effects.Add(Element.Die, SpriteManager.ElementPerBuildingSprites[dieEffectPath]);

			BuildingTemplates.Add(id, new BuildingTemplate(id, name, population, health, imagePath, imageDPath, thisStats, effects));
		}

		foreach (XmlNode power in model.GetElementsByTagName("power")) {
			TouchPowers.Add((Element)power.Attributes["elId"].Value);
		}

		PanelLoading pl = PanelLoading.GetComponent<PanelLoading>();
		PanelLoading.SetActive(false);
		PanelMainMenu.SetActive(true);
		PanelBeforeMission.SetActive(false);
		PanelAfterMission.SetActive(false);
		PanelLoadingMission.SetActive(false);
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


	internal void MinigameFinished(Dictionary<ScoreType, Result> ActualResults, Mission Mission) {
		StartCoroutine(WaitingForResultsCoroutine(Mission.SaveGame(ActualResults), Mission, ActualResults));
	}

	private IEnumerator WaitingForResultsCoroutine(WWW www, Mission Mission, Dictionary<ScoreType, Result> ActualResults) {
		MissionStatus ms = Mission.GetStatus(ActualResults);

		yield return new WaitForSeconds(2f);
		if (ms == MissionStatus.Success) {
			PanelAfterMission.SetActive(true);
			PanelAfterMission.GetComponent<PanelAfterMission>().Prepare(Mission, ActualResults);
		} else {
			PanelMissionFailed.SetActive(true);
			PanelMissionFailed.GetComponent<PanelMissionFailed>().Prepare(Mission, ActualResults);
		}
		PanelMinigame.SetActive(false);
		yield return www;
		if (ms == MissionStatus.Success) {
			PanelAfterMission.GetComponent<PanelAfterMission>().UpdateText(www);
		}
	}

}
