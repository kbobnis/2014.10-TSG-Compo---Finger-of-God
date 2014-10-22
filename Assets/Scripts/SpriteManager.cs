using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class SpriteManager : MonoBehaviour{


	public static List<Sprite> BuildingSprites;
	public static List<Sprite> BuildingSpritesDestroyed;

	public static Sprite[] GroundLevels;
	public static Dictionary<Element, Sprite[]> ElementSprites = new Dictionary<Element, Sprite[]>();

	private static string SpritesUrl = "assets/Images/";

	static SpriteManager() {

		BuildingSprites = Resources.LoadAll<Sprite>("Images/buildings_vert").ToList();
		BuildingSprites.Add (Resources.Load<Sprite> ("Images/gas_station"));
		BuildingSprites.Add (Resources.Load<Sprite> ("Images/silos"));
		BuildingSprites.Add (Resources.Load<Sprite> ("Images/electricityTower"));

		BuildingSpritesDestroyed = Resources.LoadAll<Sprite>("Images/buildings_vert_destroyed").ToList();
		BuildingSpritesDestroyed.Add (Resources.Load<Sprite> ("Images/gas_station_destroyed"));
		BuildingSpritesDestroyed.Add (Resources.Load<Sprite> ("Images/silos_destroyed"));
		BuildingSpritesDestroyed.Add (Resources.Load<Sprite> ("Images/electricityTower_destroyed"));

		ElementSprites.Add(Element.Fire, Resources.LoadAll<Sprite> ("Images/fire"));
		GroundLevels = Resources.LoadAll<Sprite> ("Images/groundLevels");
		ElementSprites.Add (Element.Water, Resources.LoadAll<Sprite> ("Images/water"));

		ElementSprites.Add (Element.SmokeAfterFire, Resources.LoadAll<Sprite> ("Images/smokeAfterFire"));
		ElementSprites.Add (Element.Electricity, Resources.LoadAll<Sprite> ("Images/electric"));

		ElementSprites.Add (Element.Crush, Resources.LoadAll<Sprite> ("Images/explosion"));

	}

	void Start() {
		//StartCoroutine(LoadFromUrl("silos.png", 40, BuildingSprites));
		//StartCoroutine(LoadFromUrl("silos_destroyed.png", 40, BuildingSpritesDestroyed));

	}

	/*private IEnumerator LoadFromUrl(string name, int number, List<Sprite> sprites){
		string url = WebConnector.Server + SpritesUrl + name;
    
        WWW www = new WWW(url);
        yield return www;
		sprites[number] = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
	}*/
}
