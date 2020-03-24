using UnityEngine;
using System.Collections;

public class UpdateChecker : MonoBehaviour {

	public float LastUpdate;

	public float LastUpdatesPerSecond;
	public float LastBiggestDeltaOfSecond;

	public float UpdatesPerSecond;
	public float BiggestDeltaOfThisSecond;
	public int ThisSecond;
		 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	internal void UpdateIsNow() {
		
		int thisSecond = (int)Time.time;
		if (thisSecond != ThisSecond) {
			ThisSecond = thisSecond;
			LastUpdatesPerSecond = UpdatesPerSecond;
			UpdatesPerSecond = 0;
			LastBiggestDeltaOfSecond = BiggestDeltaOfThisSecond;
			BiggestDeltaOfThisSecond = 0;
			Debug.Log("last updates per second: " + LastUpdatesPerSecond + ", last biggest delta: " + LastBiggestDeltaOfSecond);
		}

		float now = Time.time;
		float delta = now - LastUpdate;
		UpdatesPerSecond++;
		if (delta > BiggestDeltaOfThisSecond) {
			BiggestDeltaOfThisSecond = delta;
		}
		LastUpdate = now;

	}
}
