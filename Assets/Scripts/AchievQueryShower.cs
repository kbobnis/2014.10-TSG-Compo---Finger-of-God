using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AchievQueryShower : MonoBehaviour, Listener<ScoreType, int> {

	public AchievQuery AchievQuery;

	public int ActualValue;
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

	public void Inform(ScoreType t, int delta) {
		if (t == AchievQuery.ScoreType) {
			ActualValue += delta;
		}
	}
}
