using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum BuildingType {
	None,
	Wood,
	Stone,
	Block,
	WaterTower,
	ElectricTower,
	GasStation,
	Destroyed
}

public static class BuildingTypeMethods {


	public static string Description(this BuildingType bt) {
		Building b = new Building();
		b.CreateFromTemplate(bt);
		string text = bt + " building. Has " + b.Population + " people. ";
		switch (bt) {
			case BuildingType.WaterTower: text += "After destroyed fills with water all empty spaces and sorroundings. ";
				break;
			case BuildingType.ElectricTower: text += "After destroyed shocks with electricity. Electricity travels with water. ";
				break;
			case BuildingType.GasStation: text += "After destroyed sets on fire nearby buildings. ";
				break;
			case BuildingType.Block: text += "You need more than one intervention to bring it down. It is sesceptive to elementals.";
				break;
		}
		return text;
	}


	public static BuildingType RandomBuilding() {
		BuildingType bt = BuildingType.Destroyed;
		Dictionary<BuildingType, int> chances = new Dictionary<BuildingType, int>();
		chances.Add(BuildingType.GasStation, 1);
		chances.Add(BuildingType.WaterTower, 1);
		chances.Add(BuildingType.ElectricTower, 1);
		chances.Add(BuildingType.Stone, 1);
		chances.Add(BuildingType.Wood, 1);
		chances.Add(BuildingType.Destroyed, 1);
		chances.Add(BuildingType.Block, 1);
		int sumOfChances = 0;
		foreach (int chance in chances.Values.ToList()) {
			sumOfChances += chance;
		}
		int ticket = Mathf.RoundToInt(UnityEngine.Random.value * sumOfChances);
		foreach (KeyValuePair<BuildingType, int> b in chances) {
			ticket -= b.Value;
			if (ticket <= 0) {
				bt = b.Key;
				break;
			}
		}

		return bt;
	}
}
