using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour{

	public static AudioClip Lightning, Crush, Huracaine, Fire;

	static SoundManager(){
		Lightning = Resources.Load<AudioClip> ("Sounds/piorun");
		Crush = Resources.Load<AudioClip> ("Sounds/budynek");
		Huracaine = Resources.Load<AudioClip> ("Sounds/huragan");
		Fire = Resources.Load<AudioClip> ("Sounds/ogien");
	}
}
