using UnityEngine;
using System.Collections;

public class PlaySingleSound : MonoBehaviour
{
	float sound_start = 0f;

	// Update is called once per frame
	void Update ()
	{
		if (sound_start == 0f) {
			sound_start = Time.realtimeSinceStartup;
		}
		if(audio != null && Time.realtimeSinceStartup - sound_start > audio.clip.length ) {
			GameObject.Destroy( gameObject );
		}
	}

	public static void SpawnSound( AudioClip clip, float volume=0.06f )
	{
		if (clip != null){
			GameObject go = new GameObject( "sound clip: " + clip.name );

			AudioSource audio = go.AddComponent<AudioSource>();
			audio.volume = 0f;// volume;
			audio.clip = clip;
			//audio.rolloffMode = AudioRolloffMode.Linear;
			audio.Play();

			PlaySingleSound play_sound = go.AddComponent<PlaySingleSound>();
		}
	}
}
