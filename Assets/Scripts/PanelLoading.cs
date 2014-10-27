using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public delegate void JustDo();

public class PanelLoading : MonoBehaviour {

	public GameObject TextLoading, TextTap;

	private JustDo JustDo;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetLoading(string text, JustDo justDo) {
		JustDo = justDo;
		TextLoading.GetComponent<Text>().text = text;
		TextTap.SetActive(false);
		GetComponent<EventTrigger>().enabled = false;
	}

	internal void Ready() {
		Continue();
		TextTap.SetActive(true);
		GetComponent<EventTrigger>().enabled = true;
		TextLoading.GetComponent<Text>().text = "Ready to play";
	}

	public void Continue() {
		JustDo();
		gameObject.SetActive(false);
	}
}
