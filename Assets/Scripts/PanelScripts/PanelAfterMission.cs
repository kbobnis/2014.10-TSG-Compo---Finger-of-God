using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using SimpleJSON;



public class PanelAfterMission : MonoBehaviour {

	public GameObject PanelMainMenu, TextYourScore, TextLeaderboardNames, TextLeaderboardScores, PanelButtons;
	private Mission Mission;
	private Dictionary<ScoreType, Result> ActualResults;
	private LevelScore YourScore;
	private WWW LastWWW;

	public void Prepare(Mission mission, Dictionary<ScoreType, Result> actualResults, WWW www) {
		Mission = mission;
		ActualResults = actualResults;
		TextLeaderboardNames.GetComponent<Text>().text = "";
		TextLeaderboardScores.GetComponent<Text>().text = "";

		YourScore = new LevelScore();
		YourScore.AddInfo( Game.Me.UserName, (int)actualResults[ScoreType.Interventions].Value, WebConnector.GetDeviceId(), actualResults[ScoreType.Time].Value, -1);
		TextYourScore.GetComponent<Text>().text = YourScore.Interventions + " interv, " + (YourScore.Time.ToString("##.##")) + " seconds";

		UpdateText(www);
	}

	void Awake() {

		PanelButtons.SetActive(true); // this is a template, don't touch it.
		GameObject go = PanelButtons.GetComponent<PanelButtons>().CopyMeIn(gameObject);
		PanelButtons.SetActive(false); // this is a template, don't touch it.

		go.GetComponent<PanelButtons>().ButtonTopText.GetComponent<Text>().text = "Next Mission";
		go.GetComponent<PanelButtons>().ButtonTop.GetComponent<Button>().onClick.AddListener(() => { QuickNextMission(); });

		go.GetComponent<PanelButtons>().ButtonBottomText.GetComponent<Text>().text = "Main Menu";
		go.GetComponent<PanelButtons>().ButtonBottom.GetComponent<Button>().onClick.AddListener(() => { ShowMainMenu(); });
		go.GetComponent<PanelButtons>().AfterSettingsClose = () => { UpdateText(LastWWW); };
	}

	private void UpdateText(WWW www) {
		LastWWW = www;
		string text = "";
		if (www.error != null) {
			text = www.error;
		} else {
			TextYourScore.GetComponent<Text>().text = YourScore.Interventions + " interv, " + (YourScore.Time.ToString("##.##")) + " seconds";
			TextLeaderboardNames.GetComponent<Text>().text = "";
			TextLeaderboardScores.GetComponent<Text>().text = "";
			List<LevelScore> scores = ParseScores(www.text);

			foreach (LevelScore tmp in scores) {
				if (tmp == null || tmp.DeviceId == null) {
					TextLeaderboardNames.GetComponent<Text>().text += "---\n";
					TextLeaderboardScores.GetComponent<Text>().text += "---\n";
				} else {
					string name = tmp.Name;
					//in case we changed the name in settings and want this to apply without another useless server call
					if (tmp.DeviceId == WebConnector.GetDeviceId()) {
						name = Game.Me.UserName;
					}
					TextLeaderboardNames.GetComponent<Text>().text += tmp.Place + ". " + name + "\n";
					TextLeaderboardScores.GetComponent<Text>().text += "" + tmp.Interventions + " INTERV, " + tmp.Time.ToString("##.##") + " s\n";
				}

				if (tmp.DeviceId == WebConnector.GetDeviceId() && tmp.Interventions == YourScore.Interventions && tmp.Time.ToString("##.##") == YourScore.Time.ToString("##.##")) {
					TextYourScore.GetComponent<Text>().text += " (YOUR RECORD)";
				}
			}

			string tmp2 = TextLeaderboardNames.GetComponent<Text>().text;
			TextLeaderboardNames.GetComponent<Text>().text = tmp2.Substring(0, tmp2.Length - 1);
			tmp2 = TextLeaderboardScores.GetComponent<Text>().text;
			TextLeaderboardScores.GetComponent<Text>().text = tmp2.Substring(0, tmp2.Length - 1);
				
		}
	}

	private List<LevelScore> FilterOnlyYourNeighbours(List<LevelScore> scores, string yourDeviceId) {
		List<LevelScore> tmp = new List<LevelScore>();
		
		for(int i=0; i < scores.Count; i++){
			LevelScore actual = scores[i];

			if (actual.DeviceId == yourDeviceId) {
				tmp.Add(i-2>=0?scores[i-2]:null);
				tmp.Add(i-1>=0?scores[i-1]:null);
				tmp.Add(actual);
				tmp.Add(i+1<scores.Count?scores[i+1]:null);
				tmp.Add(i+2<scores.Count?scores[i + 2]:null);
				break;
			}
		}
		return tmp;
	}

	private List<LevelScore> FilterTop4(List<LevelScore> scores, string yourDeviceId) {
		List<LevelScore> tmp = new List<LevelScore>();
		bool foundYou = false;
		LevelScore you = null;

		for (int i = 0; i < scores.Count; i++) {
			LevelScore actual = scores[i];

			if (actual.DeviceId == yourDeviceId) {
				tmp.Add(i - 2 >= 0 ? scores[i - 2] : null);
				tmp.Add(i - 1 >= 0 ? scores[i - 1] : null);
				tmp.Add(actual);
				tmp.Add(i + 1 < scores.Count ? scores[i + 1] : null);
				tmp.Add(i + 2 < scores.Count ? scores[i + 2] : null);
				break;
			}
		}
		return tmp;
	}

	private List<LevelScore> ParseScores(string jsonText) {
		List<LevelScore> scores = new List<LevelScore>();

		JSONNode n = JSONNode.Parse(jsonText);
		int i = 1;
		foreach (JSONNode tile in n.Childs) {
			
			LevelScore ls = new LevelScore();
			if (tile["deviceId"] != null){
				string name = tile["name"];
				int interv = tile["interventions"].AsInt;
				float time = tile["time"].AsInt/1000f;
				string deviceId = tile["deviceId"];
				int place = tile["place"].AsInt;
				ls.AddInfo(name, interv, deviceId, time, place);
			}
			scores.Add(ls);
		}
		return scores;

	}

	private void ShowMainMenu() {
		gameObject.SetActive(false);
		PanelMainMenu.SetActive(true);
	}

	private void QuickNextMission() {
		PanelMainMenu.SetActive(true);
		switch (Mission.MissionType) {
			case global::MissionType.Specified:
				PanelMainMenu.GetComponent<PanelMainMenu>().StartMissionSpecified();
				break;
			case global::MissionType.Random:
				PanelMainMenu.GetComponent<PanelMainMenu>().StartMissionRandom();
				break;
			default: throw new UnityEngine.UnityException("There is no button for mission type: " + Mission.MissionType);
		}
		gameObject.SetActive(false);
	}

}

class LevelScore {

	int _Place;
	string _Name;
	int _Interventions;
	float _Time;
	string _DeviceId;

	public string DeviceId {
		get { return _DeviceId; }
	}

	public int Place {
		get { return _Place; }
	}

	public string Name {
		get { return _Name; }
	}

	public int Interventions {
		get { return _Interventions; }
	}

	public float Time {
		get { return _Time; }
	}

	public void AddInfo(string name, int interv, string deviceId, float time, int place) {
		_Name = name;
		_Interventions = interv;
		_DeviceId = deviceId;
		_Time = time;
		_Place = place;
	}

}
