using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class SpriteManager{


	public static List<Sprite> BuildingSprites;
	public static List<Sprite> BuildingSpritesDestroyed;
	public static Sprite[] Explosions;
	public static Sprite[] Fires;
	public static Sprite[] GroundLevels;

	static SpriteManager() {

		BuildingSprites = Resources.LoadAll<Sprite>("Images/buildings_vert").ToList();
		BuildingSprites.Add (Resources.Load<Sprite> ("Images/gas_station"));

		BuildingSpritesDestroyed = Resources.LoadAll<Sprite>("Images/buildings_vert_destroyed").ToList();
		BuildingSpritesDestroyed.Add (Resources.Load<Sprite> ("Images/gas_station_destroyed"));

		Explosions = Resources.LoadAll<Sprite> ("Images/explosion");
		Fires = Resources.LoadAll<Sprite> ("Images/fire");
		GroundLevels = Resources.LoadAll<Sprite> ("Images/groundLevels");
	}
}
