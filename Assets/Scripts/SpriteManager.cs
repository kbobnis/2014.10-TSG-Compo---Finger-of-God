using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpriteManager{


	public static Sprite[] BuildingSprites;
	public static Sprite[] BuildingSpritesDestroyed;
	public static Sprite[] Explosions;
	public static Sprite[] Fires;

	static SpriteManager() {

		BuildingSprites = Resources.LoadAll<Sprite>("Images/buildings_vert");
		BuildingSpritesDestroyed = Resources.LoadAll<Sprite>("Images/buildings_vert_destroyed");
		Explosions = Resources.LoadAll<Sprite> ("Images/explosion");
		Fires = Resources.LoadAll<Sprite> ("Images/fire");
	}
}
