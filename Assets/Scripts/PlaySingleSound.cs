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

	public static void SpawnSound( AudioClip clip, Vector3 position, float volume=1f )
	{
		GameObject go = new GameObject( "sound clip: " + clip.name );
		go.transform.position = position;

		AudioSource audio = go.AddComponent<AudioSource>();
		audio.volume = volume;
		audio.clip = clip;
		//audio.rolloffMode = AudioRolloffMode.Linear;
		audio.Play();

		PlaySingleSound play_sound = go.AddComponent<PlaySingleSound>();

	}
}
