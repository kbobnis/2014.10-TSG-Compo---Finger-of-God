using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class SpriteManager{


	public static List<Sprite> BuildingSprites;
	public static List<Sprite> BuildingSpritesDestroyed;

	public static Sprite[] GroundLevels;
	public static Dictionary<Element, Sprite[]> ElementSprites = new Dictionary<Element, Sprite[]>();

	static SpriteManager() {

		BuildingSprites = Resources.LoadAll<Sprite>("Images/buildings_vert").ToList();
		BuildingSprites.Add (Resources.Load<Sprite> ("Images/gas_station"));
		BuildingSprites.Add (Resources.Load<Sprite> ("Images/silos"));

		BuildingSpritesDestroyed = Resources.LoadAll<Sprite>("Images/buildings_vert_destroyed").ToList();
		BuildingSpritesDestroyed.Add (Resources.Load<Sprite> ("Images/gas_station_destroyed"));
		BuildingSpritesDestroyed.Add (Resources.Load<Sprite> ("Images/silos_destroyed"));

		ElementSprites.Add(Element.Fire, Resources.LoadAll<Sprite> ("Images/fire"));
		GroundLevels = Resources.LoadAll<Sprite> ("Images/groundLevels");
		ElementSprites.Add (Element.Water, Resources.LoadAll<Sprite> ("Images/water"));

		ElementSprites.Add (Element.SmokeAfterFire, Resources.LoadAll<Sprite> ("Images/smokeAfterFire"));

	}
}
