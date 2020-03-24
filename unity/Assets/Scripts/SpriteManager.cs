using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public delegate void LoadSprite(Sprite s);

public class SpriteManager : MonoBehaviour{

	public static Dictionary<string, Sprite[]> ElementPerBuildingSprites = new Dictionary<string, Sprite[]>();
	public static Dictionary<Element, string> DefaultEffectPaths = new Dictionary<Element, string>();

	private static List<KeyValuePair<string, LoadSprite>> SpritesToLoad = new List<KeyValuePair<string, LoadSprite>>();
	private static Dictionary<string, int> Retries = new Dictionary<string, int>();
	private static bool DownloadingNow = false;
	private static Dictionary<string, Sprite> LoadedSprites = new Dictionary<string, Sprite>();

	void Start() {
		
		DefaultEffectPaths.Add(Element.Fire, "Images/effects/wood/fire");
		DefaultEffectPaths.Add(Element.SmokeAfterFire, "Images/effects/smokeAfterFire");
		DefaultEffectPaths.Add(Element.Electricity, "Images/electric");
		DefaultEffectPaths.Add(Element.Crush, "Images/effects/crush");
		DefaultEffectPaths.Add(Element.Water, "Images/water");
		DefaultEffectPaths.Add(Element.Die, "Images/effects/gasStation/die");

		ElementPerBuildingSprites.Add(DefaultEffectPaths[Element.Fire], Resources.LoadAll<Sprite> (DefaultEffectPaths[Element.Fire]));
		ElementPerBuildingSprites.Add(DefaultEffectPaths[Element.SmokeAfterFire], Resources.LoadAll<Sprite>(DefaultEffectPaths[Element.SmokeAfterFire]));
		ElementPerBuildingSprites.Add(DefaultEffectPaths[Element.Electricity], Resources.LoadAll<Sprite>(DefaultEffectPaths[Element.Electricity]));
		ElementPerBuildingSprites.Add(DefaultEffectPaths[Element.Crush], Resources.LoadAll<Sprite>(DefaultEffectPaths[Element.Crush]));
		ElementPerBuildingSprites.Add(DefaultEffectPaths[Element.Water], Resources.LoadAll<Sprite>(DefaultEffectPaths[Element.Water]));

		ElementPerBuildingSprites.Add("Images/effects/wood/electricity", Resources.LoadAll<Sprite>("Images/effects/wood/electricity"));
		ElementPerBuildingSprites.Add("Images/effects/gasStation/die", Resources.LoadAll<Sprite>("Images/effects/gasStation/die"));

	}

	public static void LoadAsynchronous(string path, LoadSprite ls) {
		if (LoadedSprites.ContainsKey(path)) {
			ls(LoadedSprites[path]);
		} else {
			SpritesToLoad.Add(new KeyValuePair<string, LoadSprite>(path, ls));
		}
	}

	void Update() {
		if (SpritesToLoad.Count > 0) {
			KeyValuePair<string, LoadSprite> kvp = SpritesToLoad[0];
			
			if (LoadedSprites.ContainsKey(kvp.Key)) {
				kvp.Value(LoadedSprites[kvp.Key]);
				SpritesToLoad.Remove(kvp);
			} else if (DownloadingNow == false){
				DownloadingNow = true;
				StartCoroutine(LoadFromUrl(kvp.Key, kvp.Value));
				SpritesToLoad.Remove(kvp);
			}
		}
	}

	private IEnumerator LoadFromUrl(string path, LoadSprite ls){
		string url =  Config.Server + path;
    
        WWW www = new WWW(url);
		
        yield return www;
		if (www.error != null) {
			//will retry
			if (!Retries.ContainsKey(path)){
				Retries.Add(path, 0);
			}
			Retries[path]++;

			if (Retries[path] < 3) {
				Debug.Log("Error " + www.error + ", when downloading " + path + ", retries: " + Retries[path]);
				SpritesToLoad.Add(new KeyValuePair<string, LoadSprite>(path, ls));
			}
		} else {
			Sprite s = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
			s.texture.filterMode = FilterMode.Point;
			ls(s);
		}

		DownloadingNow = false;
	}

}
