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
	public string BeforeText;

	private int Number;

	public static int GetCompletedMission() {
		return PlayerPrefs.GetInt("CompletedMission", 0);
	}

	public void Accomplished() {
		PlayerPrefs.SetInt("CompletedMission", Number);
	}

	public static void ResetMission() {
		PlayerPrefs.DeleteKey("CompletedMission");
	}

	public MissionStatus GetStatus(Dictionary<ScoreType, Result> results) {
		
		bool allSuccess = true;
		foreach (AchievQuery success in SuccessQueries) {
			if (!success.CanAccept(results[success.ScoreType])) {
				allSuccess = false;
			} 
		}
		if (allSuccess) {
			return MissionStatus.Success;
		}

		bool foundFailure = false;
		foreach (AchievQuery failure in FailureQueries) {
			if (failure.CanAccept(results[failure.ScoreType])) {
				foundFailure = true;
			}
		}
		if (foundFailure) {
			return MissionStatus.Failure;
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
		m.FailureQueries.Add(new AchievQuery(ScoreType.Interventions, Sign.Bigger, n["properties"]["Interventions"].AsInt));
		m.BeforeText = n["properties"]["BeforeText"].ToString();
		m.SuccessQueries.Add(new AchievQuery(ScoreType.Population, Sign.Equal, 0));
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
				bRow.Add(RandomBuilding());
			}
			m.Buildings.Add(bRow);
		}
		m.SuccessQueries.Add(new AchievQuery(ScoreType.Population, Sign.SmallerEqual, 0));

		return m;
	}


	private static BuildingType RandomBuilding(){
		
		BuildingType bt = BuildingType.Destroyed;

		Dictionary<BuildingType, int> chances = new Dictionary<BuildingType,int>();
		chances.Add(BuildingType.GasStation, 1);
		chances.Add(BuildingType.WaterTower, 1);
		chances.Add(BuildingType.ElectricTower, 1);
		chances.Add(BuildingType.Stone, 2);
		chances.Add(BuildingType.Wood, 2);
		chances.Add(BuildingType.Destroyed, 1);
		chances.Add(BuildingType.Block, 0);
		int sumOfChances = 0;
		foreach(int chance in chances.Values.ToList()){
			sumOfChances += chance;
		}
		int ticket = Mathf.RoundToInt(UnityEngine.Random.value * sumOfChances);
		foreach(KeyValuePair<BuildingType, int> b in chances){
			ticket -= b.Value;
			if (ticket <= 0){
				bt = b.Key;
				break;
			}
		}

		return bt;
	}
}
