using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SimpleJSON;

public enum MissionStatus {
	Failure, Success, NotYetDetermined
}

public class Mission{

	public List<List<BuildingType>> Buildings = new List<List<BuildingType>>();
	public List<AchievQuery> SuccessQueries = new List<AchievQuery>();
	public List<AchievQuery> FailureQueries = new List<AchievQuery>();
	
	public string BeforeMissionText = "";
	public BuildingType BeforeMissionBuilding;

	private int Number;

	public static int GetCompletedMission() {
		return PlayerPrefs.GetInt("CompletedMission", 0);
	}

	public void Accomplished(MissionStatus ms, Dictionary<ScoreType, Result> actualResults) {

		if (ms == MissionStatus.Success) {
			PlayerPrefs.SetInt("CompletedMission", Number);
		}
	}

	public static void ResetMission() {
		PlayerPrefs.DeleteKey("CompletedMission");
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


	public static Mission LoadMission(int number) {
		//load json
		string map = Resources.Load<TextAsset>("Maps/map"+number).text;
		JSONNode n = JSONNode.Parse(map);

		//there is only one child
		JSONNode buildingsLayerJson = n["layers"].Childs.ElementAt(0);

		Dictionary<int, BuildingType> intToBuildingMap = new Dictionary<int, BuildingType>();
		intToBuildingMap.Add(1, BuildingType.Wood);
		intToBuildingMap.Add(2, BuildingType.Stone);
		intToBuildingMap.Add(3, BuildingType.Block);
		intToBuildingMap.Add(4, BuildingType.WaterTower);
		intToBuildingMap.Add(5, BuildingType.ElectricTower);
		intToBuildingMap.Add(6, BuildingType.GasStation);
		intToBuildingMap.Add(7, BuildingType.Destroyed);

		int width = buildingsLayerJson["width"].AsInt;
		int height = buildingsLayerJson["height"].AsInt;

		Mission m = new Mission();
		m.FailureQueries.Add(new AchievQuery(ScoreType.Interventions, Sign.Equal, n["properties"]["Interventions"].AsInt+1));
		m.BeforeMissionText = n["properties"]["BeforeText"].ToString().Trim(new Char[]{'"'});
		m.SuccessQueries.Add(new AchievQuery(ScoreType.Population, Sign.Equal, 0));
		
		int buildingInt = n["properties"]["BeforeImage"].AsInt;
		BuildingType bt = BuildingType.None;
		if (intToBuildingMap.ContainsKey(buildingInt)){
			bt = intToBuildingMap[buildingInt];
		}
		m.BeforeMissionBuilding = bt;
		
		m.Number = number;

		int i=0; 
		List<BuildingType> bRow = null;
		foreach (JSONNode tile in buildingsLayerJson["data"].Childs) {
			if (i % width == 0){
				if (i > 0) { //we want to add the first row, not the zero row
					m.Buildings.Add(bRow);
				}
				bRow = new List<BuildingType>();
			}
			bRow.Add(intToBuildingMap[tile.AsInt]);
			i++;
		}
		m.Buildings.Add(bRow);

		return m;
	}

	public static Mission MissionWithRandom() {
		Mission m = new Mission();

		int w = 5;
		int h = 7;

		for (int i = 0; i < h; i++) {
			List<BuildingType> bRow = new List<BuildingType>();
			for (int j = 0; j < w; j++) {
				bRow.Add(BuildingTypeMethods.RandomBuilding());
			}
			m.Buildings.Add(bRow);
		}
		m.SuccessQueries.Add(new AchievQuery(ScoreType.Population, Sign.Equal, 0));
		m.BeforeMissionBuilding = BuildingTypeMethods.RandomBuilding();
		m.BeforeMissionText = m.BeforeMissionBuilding.Description();

		return m;
	}



}
