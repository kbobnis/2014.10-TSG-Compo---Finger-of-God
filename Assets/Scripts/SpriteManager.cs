using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public delegate void LoadSprite(Sprite s);

public class SpriteManager : MonoBehaviour{

	public static Dictionary<Element, Sprite[]> ElementSprites = new Dictionary<Element, Sprite[]>();

	private static List<KeyValuePair<string, LoadSprite>> SpritesToLoad = new List<KeyValuePair<string, LoadSprite>>();
	private static Dictionary<string, int> Retries = new Dictionary<string, int>();
	private static bool DownloadingNow = false;
	private static Dictionary<string, Sprite> LoadedSprites = new Dictionary<string, Sprite>();

	public static Sprite MainMenuBg;


	void Start() {

		ElementSprites.Add(Element.Fire, Resources.LoadAll<Sprite> ("Images/fire"));
		ElementSprites.Add (Element.Water, Resources.LoadAll<Sprite> ("Images/water"));
		ElementSprites.Add (Element.SmokeAfterFire, Resources.LoadAll<Sprite> ("Images/smokeAfterFire"));
		ElementSprites.Add (Element.Electricity, Resources.LoadAll<Sprite> ("Images/electric"));
		ElementSprites.Add (Element.Crush, Resources.LoadAll<Sprite> ("Images/explosion"));

		MainMenuBg = Resources.Load<Sprite>("Images/failed/mission_failed_bg_small");
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
