using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using SimpleJSON;



public class PanelAfterMission : MonoBehaviour {

	public GameObject PanelMainMenu, TextYourScore, TextLeaderboardNames, TextLeaderboardScores, ImageYellowStraw, ButtonRepeat;
	private Mission Mission;
	private Dictionary<ScoreType, Result> ActualResults;
	private LevelScore YourScore;
	private WWW LastWWW;

	public void Prepare(Mission mission, Dictionary<ScoreType, Result> actualResults) {
		Mission = mission;
		//you can only repeat specified missions (i have no time to do backend search for last random mission)
		ButtonRepeat.SetActive(mission.MissionType == MissionType.Specified);
		if (mission.MissionType == MissionType.Random) {
			GetComponent<ButtonsInCloud>().ButtonTopText = "Quick Mission";
		}
		ActualResults = actualResults;
		TextLeaderboardNames.GetComponent<Text>().text = "";
		TextLeaderboardScores.GetComponent<Text>().text = "";

		YourScore = new LevelScore();
		YourScore.AddInfo( Game.Me.UserName, (int)actualResults[ScoreType.Interventions].Value, WebConnector.GetDeviceId(), actualResults[ScoreType.Time].Value, -1);
		LevelScore yourPrevious = mission.YourBest;
		TextYourScore.GetComponent<Text>().text = YourScore.Interventions + " interv, " + (YourScore.Time.ToString("##.##")) + " seconds";
		
		int interventionDelta = yourPrevious.Interventions - YourScore.Interventions;
		float timeDelta = yourPrevious.Time - YourScore.Time ;
		if (yourPrevious.Interventions == 0 || YourScore.Interventions < yourPrevious.Interventions || (YourScore.Interventions == yourPrevious.Interventions && YourScore.Time < yourPrevious.Time)) {
			TextYourScore.GetComponent<Text>().text += "\n (YOUR RECORD";
			if (yourPrevious.Interventions == 0) {
				TextYourScore.GetComponent<Text>().text += ", first try"; 
			} else {
				if (interventionDelta > 0) {
					TextYourScore.GetComponent<Text>().text += " by " + interventionDelta + " interv";
				} else {
					TextYourScore.GetComponent<Text>().text += " by " +timeDelta.ToString("##.##") + " seconds";
				}
			} 
			TextYourScore.GetComponent<Text>().text += ")";
		} else {
			TextYourScore.GetComponent<Text>().text += "\n WORSE THAN YOUR RECORD BY ";
			if (interventionDelta < 0) {
				TextYourScore.GetComponent<Text>().text += -interventionDelta + " interv";
			} else {
				TextYourScore.GetComponent<Text>().text += (-timeDelta).ToString("##.##") + " seconds";
			}
		}

		TextLeaderboardNames.GetComponent<Text>().text = "Loading";
		TextLeaderboardScores.GetComponent<Text>().text = "scores";
		ImageYellowStraw.SetActive(false);
		UpdateText(Mission.BestScores);
	}

	private void UpdateText(List<LevelScore> scores) {
		ImageYellowStraw.SetActive(true);
		TextLeaderboardNames.GetComponent<Text>().text = "";
		TextLeaderboardScores.GetComponent<Text>().text = "";

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

		}

		string tmp2 = TextLeaderboardNames.GetComponent<Text>().text;
		TextLeaderboardNames.GetComponent<Text>().text = tmp2.Substring(0, tmp2.Length - 1);
		tmp2 = TextLeaderboardScores.GetComponent<Text>().text;
		TextLeaderboardScores.GetComponent<Text>().text = tmp2.Substring(0, tmp2.Length - 1);
	}

	public void RepeatLevel() {
		PanelMainMenu.SetActive(true);
		PanelMainMenu.GetComponent<PanelMainMenu>().StartMission(Mission.MissionType, true);
		gameObject.SetActive(false);

	}

	public void UpdateText(WWW www) {
		LastWWW = www;
		string text = "";
		if (www.error != null) {
			text = www.error;
		} else {
			JSONNode n = JSONNode.Parse(www.text);
			if (n["resultsHere"].AsBool) {
				List<LevelScore> scores = ParseScores(n["results"]);
				UpdateText(scores);
			}
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

	private List<LevelScore> ParseScores(JSONNode n) {
		List<LevelScore> scores = new List<LevelScore>();

		
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

	public void ShowMainMenu() {
		gameObject.SetActive(false);
		PanelMainMenu.SetActive(true);
	}

	public void QuickNextMission() {
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

public class LevelScore {

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
