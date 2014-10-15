using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NumberShower : MonoBehaviour, Listener<ScoreType, int> {

	public string Prefix = "none";
	private int Number;
	public bool Minus;
	public ScoreType ScoreType;

	// Use this for initialization
	void Start () {
		Actualize();
	}

	public void Inform(ScoreType st, int delta){
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
	
	private void AddNumber(int number){
		Number += (Minus?-1:1) * number;
	}

	private void Actualize() {
		GetComponent<Text>().text = Prefix + ": " + Number;
	}
}
