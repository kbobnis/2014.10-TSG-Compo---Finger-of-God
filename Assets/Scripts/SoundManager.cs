using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour{

	public static Dictionary<Element, AudioClip> Clips = new Dictionary<Element, AudioClip> ();
	public static AudioClip BuildingDown;

	static SoundManager(){
		Clips.Add (Element.Electricity, Resources.Load<AudioClip> ("Sounds/piorun"));

		Clips.Add (Element.Crush, Resources.Load<AudioClip> ("Sounds/budynek"));
		Clips.Add (Element.Fire, Resources.Load<AudioClip> ("Sounds/ogien"));
		Clips.Add (Element.Water, Resources.Load<AudioClip> ("Sounds/water"));
		Clips.Add (Element.SmokeAfterFire, Resources.Load<AudioClip> ("Sounds/extinguishFire"));

		BuildingDown = Resources.Load<AudioClip> ("Sounds/building_down");
	}
}
