using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NumberShower : MonoBehaviour, Listener<ScoreType, float> {

	public string Prefix = "none";
	private float Number;
	public bool Minus;
	public ScoreType ScoreType;

	// Use this for initialization
	void Start () {
		Actualize();
	}

	public void Inform(ScoreType st, float delta){
		if (st == ScoreType) {
			AddNumber (delta);
		}
		Actualize();
	}

	public void Clear(ScoreType st) {
		if (st == ScoreType) {
			Number = 0;
		}
		Actualize();
	}
	
	private void AddNumber(float number){
		Number += (Minus?-1:1) * number;
	}

	private void Actualize() {
		GetComponent<Text>().text = Prefix + Number;
	}
}
