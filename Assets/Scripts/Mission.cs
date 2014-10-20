using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SimpleJSON;
using System.Collections;

public enum MissionStatus {
	Failure, Success, NotYetDetermined
}

public enum MissionType {
	Random, Specified
}

public class Mission {

	public List<List<BuildingType>> Buildings = new List<List<BuildingType>>();
	public List<AchievQuery> SuccessQueries = new List<AchievQuery>();
	public List<AchievQuery> FailureQueries = new List<AchievQuery>();
	
	public string BeforeMissionText = "";
	public BuildingType BeforeMissionBuilding;

	private int Number;
	private MissionType _MissionType;

	public MissionType MissionType {
		get { return _MissionType;  }
	}

	public WWW SaveGame(Dictionary<ScoreType, Result> actualResults) {
		return WebConnector.SendMissionAccomplished(_MissionType, Number, GetStatus(actualResults), actualResults);
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

	public Mission(string json, MissionType mt, int number) {
		_MissionType = mt;
		Number = number;

		JSONNode n = JSONNode.Parse(json);
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


		int interventions = n["properties"]["Interventions"].AsInt;
		if (interventions > 0){
			FailureQueries.Add(new AchievQuery(ScoreType.Interventions, Sign.Equal, interventions+1));
		}
		
		BeforeMissionText = n["properties"]["BeforeText"].ToString().Trim(new Char[]{'"'});
		
		SuccessQueries.Add(new AchievQuery(ScoreType.Population, Sign.Equal, 0));
		
		int buildingInt = n["properties"]["BeforeImage"].AsInt;
		BuildingType bt = BuildingType.None;
		if (intToBuildingMap.ContainsKey(buildingInt)){
			bt = intToBuildingMap[buildingInt];
		}
		BeforeMissionBuilding = bt;
		
		int i=0; 
		List<BuildingType> bRow = null;
		foreach (JSONNode tile in buildingsLayerJson["data"].Childs) {
			if (i % width == 0){
				if (i > 0) { //we want to add the first row, not the zero row
					Buildings.Add(bRow);
				}
				bRow = new List<BuildingType>();
			}
			bRow.Add(intToBuildingMap[tile.AsInt]);
			i++;
		}
		Buildings.Add(bRow);
	}

}
