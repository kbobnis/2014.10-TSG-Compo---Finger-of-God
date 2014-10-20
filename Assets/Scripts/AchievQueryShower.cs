using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AchievQueryShower : MonoBehaviour, Listener<ScoreType, float> {

	public AchievQuery AchievQuery;

	public float ActualValue;
	public string Prefix;

	// Use this for initialization
	void Start () {
	
	}

	void Update() {
		if (AchievQuery != null) {
			GetComponent<Text>().text = Prefix + ": " + AchievQuery.ScoreType + " " + ActualValue + " " + AchievQuery.Sign + " " + AchievQuery.Value;
		} else {
			GetComponent<Text>().text = "";
		}
	}
	
	public void Clear(ScoreType t) {
		ActualValue = 0;
	}

	public void Inform(ScoreType t, float delta) {
		if (t == AchievQuery.ScoreType) {
			ActualValue += delta;
		}
	}
}
