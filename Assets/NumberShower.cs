using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NumberShower : MonoBehaviour, Listener {

	public string Prefix = "none";
	public int Number;
	public bool Minus;

	// Use this for initialization
	void Start () {
	
	}

	public void Inform(object o){
		AddNumber ((int)o);
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Text> ().text = Prefix + ": " + Number;
	}

	public void AddNumber(int number){
		Debug.Log ("adding number: "+(Minus?"-":"+")+"" + number);
		Number += (Minus?-1:1) * number;
	}
}
