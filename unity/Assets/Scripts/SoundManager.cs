using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour{

	public static Dictionary<Element, AudioClip> Clips = new Dictionary<Element, AudioClip> ();
	public static AudioClip Ambient;

	private static bool SoundsEnabled;

	static SoundManager(){
		Clips.Add (Element.Electricity, Resources.Load<AudioClip> ("Sounds/piorun"));
		Clips.Add (Element.Crush, null);//Resources.Load<AudioClip> ("Sounds/budynek"));
		Clips.Add (Element.Fire, Resources.Load<AudioClip> ("Sounds/ogien"));
		Clips.Add (Element.Water, Resources.Load<AudioClip> ("Sounds/water"));
		Clips.Add (Element.Die, Resources.Load<AudioClip> ("Sounds/building_down_short"));
		Clips.Add (Element.SmokeAfterFire, Resources.Load<AudioClip> ("Sounds/extinguishFire"));

		Ambient = Resources.Load<AudioClip> ("Sounds/muzyczka");
		Game.Me.GetComponent<AudioSource>().enabled = PlayerPrefs.GetInt("MusicOn", 1)==1;
		SoundsEnabled = PlayerPrefs.GetInt("SoundsOn", 1) == 1;
	}

	public static void EnableMusic(bool on){
		Game.Me.GetComponent<AudioSource>().enabled = on;
		PlayerPrefs.SetInt("MusicOn", on ? 1 : 0);
	}

	public static bool IsMusicEnabled() {
		return Game.Me.GetComponent<AudioSource>().enabled;
	}

	public static void EnableSounds(bool on){
		SoundsEnabled = on;
		PlayerPrefs.SetInt("SoundsOn", on ? 1 : 0);
	}

	public static bool AreSoundsEnabled() {
		return SoundsEnabled;
	}
}
