using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaySingleSound : MonoBehaviour {

	
	private static Dictionary<AudioClip, OneSound> Sounds = new Dictionary<AudioClip, OneSound>();

	private float SoundStart = 0f;
	public OneSound MySound;

	public static void SpawnSound( AudioClip clip, float pan=0){
		if (!Config.SoundsOn) {
			return;
		}
		float volume = 0.5f;
		if (clip != null){

			if (!Sounds.ContainsKey(clip)) {
				Sounds.Add(clip, new OneSound(clip, volume));
			}
			Sounds[clip].PlayAnother(pan);
		}
	}

	void Update() {
		if (SoundStart == 0f && MySound != null) {
			SoundStart = Time.realtimeSinceStartup;
		}
		if (audio != null && Time.realtimeSinceStartup - SoundStart > audio.clip.length) {
			MySound.ActuallyPlaying--;
			GameObject.Destroy(gameObject);
		}
	}
}

public class OneSound {

	private AudioClip AudioClip;
	private float Volume;
	public int ActuallyPlaying;
	private int MaxSimult = 2;
	private float MinDelay = 0.05f;
	private float LastPlay;

	public OneSound(AudioClip ac, float volume) {
		AudioClip = ac;
		Volume = volume;
	}

	public void PlayAnother(float pan) {
		if (ActuallyPlaying < MaxSimult && Time.time - MinDelay > LastPlay) {
			LastPlay = Time.time;
			ActuallyPlaying++;
			GameObject go = new GameObject("sound clip: " + AudioClip.name);
			AudioSource audio = go.AddComponent<AudioSource>();
			audio.volume = Volume;// -(Volume / MaxSimult * ActuallyPlaying);
			audio.pan = pan==0?Random.Range(-1, 1):pan;
			audio.clip = AudioClip;
			audio.panLevel = -1f;
			audio.Play();
			PlaySingleSound playSound = go.AddComponent<PlaySingleSound>();
			playSound.MySound = this;
		}
	}


}
