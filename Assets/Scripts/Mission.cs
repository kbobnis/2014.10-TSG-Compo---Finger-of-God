using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SimpleJSON;

public enum MissionStatus {
	Failure, Success, NotYetDetermined
}

public enum MissionType {
	Random, Specified
}

public class Mission {

	public List<List<BuildingTemplate>> Buildings = new List<List<BuildingTemplate>>();
	public List<AchievQuery> SuccessQueries = new List<AchievQuery>();
	public List<AchievQuery> FailureQueries = new List<AchievQuery>();
	
	public string BeforeMissionText = "";
	public string TipText = "";
	public List<BuildingTemplate> BeforeMissionBuildings = new List<BuildingTemplate>();
	public LevelScore YourBest;

	private string _Name;
	private MissionType _MissionType;
	private int _Round;
	private List<LevelScore> _BestScores;

	public List<LevelScore> BestScores {
		get { return _BestScores; }
	}
	public MissionType MissionType {
		get { return _MissionType;  }
	}

	public string Name {
		get { return _Name;  }
	}

	public WWW SaveGame(Dictionary<ScoreType, Result> actualResults) {
		int intervetions = (int)actualResults[ScoreType.Interventions].Value;
		float time = actualResults[ScoreType.Time].Value;
		bool isRecord = intervetions < YourBest.Interventions || (intervetions == YourBest.Interventions && time < YourBest.Time);
		bool getScores = isRecord;
		WWW www = WebConnector.SendMissionAccomplished(_MissionType, _Name, _Round, GetStatus(actualResults), actualResults, getScores);
		return www;
	}

	public MissionStatus GetStatus(Dictionary<ScoreType, Result> results) {

		bool foundFailure = false;
		foreach (AchievQuery failure in FailureQueries) {
			if (failure.CanAccept(results[failure.ScoreType])) {
				foundFailure = true;
			}
		}
		if (foundFailure) {
			return MissionStatus.Failure;
		}

		bool allSuccess = true;
		foreach (AchievQuery success in SuccessQueries) {
			if (!success.CanAccept(results[success.ScoreType])) {
				allSuccess = false;
			}
		}
		if (allSuccess) {
			return MissionStatus.Success;
		}

		return MissionStatus.NotYetDetermined;
	}

	public Mission(string json, MissionType mt, string name, int round, LevelScore yourBest, List<LevelScore> bestScores) {
		_MissionType = mt;
		_Name = name;
		_Round = round;
		YourBest = yourBest;
		_BestScores = bestScores;

		JSONNode n = JSONNode.Parse(json);
		JSONNode buildingsLayerJson = n["layers"].Childs.ElementAt(0);
		
		int width = buildingsLayerJson["width"].AsInt;
		int height = buildingsLayerJson["height"].AsInt;

		int interventions = n["properties"]["Interventions"].AsInt;
		if (interventions > 0){
			FailureQueries.Add(new AchievQuery(ScoreType.Interventions, Sign.Bigger, interventions));
		}
		
		BeforeMissionText = WWW.UnEscapeURL( n["properties"]["BeforeText"] );
		TipText = WWW.UnEscapeURL(n["properties"]["TipText"]);
		
		SuccessQueries.Add(new AchievQuery(ScoreType.Population, Sign.Equal, 0));

		int count = 0;
		string buildingsValue = n["properties"]["BeforeImage"].Value;
		if (!string.IsNullOrEmpty( buildingsValue) ) {
			string[] elements = buildingsValue.Split(new char[]{','});
			foreach (string el in elements) {
				int tmp = int.Parse(el);
				if (!Game.Me.BuildingTemplates.ContainsKey(tmp)) {
					throw new Exception("There is no key in building templates with name: " + tmp);
				}
				BeforeMissionBuildings.Add( Game.Me.BuildingTemplates[tmp]);
				count++;
				if (count == 3) {
					break;
				}
			}
		}
		if (BeforeMissionBuildings.Count < 3) {
			foreach(BuildingTemplate b in RandomValues(Game.Me.BuildingTemplates).Take(3 - BeforeMissionBuildings.Count)){
				BeforeMissionBuildings.Add(b);
			}
		}
		int i=0; 
		List<BuildingTemplate> bRow = null;
		foreach (JSONNode tile in buildingsLayerJson["data"].Childs) {
			if (i % width == 0){
				if (i > 0) { //we want to add the first row, not the zero row
					Buildings.Add(bRow);
				}
				bRow = new List<BuildingTemplate>();
			}
			bRow.Add(Game.Me.BuildingTemplates[tile.AsInt]);
			i++;
		}
		Buildings.Add(bRow);
	}

	public IEnumerable<TValue> RandomValues<TKey, TValue>(IDictionary<TKey, TValue> dict) {
		System.Random rand = new System.Random();
		List<TValue> values = Enumerable.ToList(dict.Values);
		int size = dict.Count;
		while (true) {
			yield return values[rand.Next(size)];
		}
	}

}
