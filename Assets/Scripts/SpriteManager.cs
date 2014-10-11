using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpriteManager{


	public static Dictionary<BuildingType, Sprite> BuildingSprites = new Dictionary<BuildingType, Sprite>();

	static SpriteManager() {

		Sprite[] _buildings = Resources.LoadAll<Sprite>("Images/buildings_vert");

		for(int i=0; i < _buildings.Length; i++){
			Sprite s = _buildings[i];
			BuildingType bt = BuildingTypeMethod.FromInt(i);

			if (bt != BuildingType.Empty){
				BuildingSprites.Add(bt, s);
			}
		}
	}
}
