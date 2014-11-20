using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonsInCloud : MonoBehaviour {

	public GameObject PanelButtons, ClonedPanelButtons;
	public string ButtonTopText, ButtonBottomText;
	public bool HideClouds;
	public bool HideSettingsButton;
	public Button.ButtonClickedEvent ButtonTopAction, ButtonBottomAction;


	// Use this for initialization
	void Awake() {

		PanelButtons.SetActive(true); // this is a template, don't touch it.
		GameObject go = PanelButtons.GetComponent<PanelButtons>().CopyMeIn(gameObject);
		ClonedPanelButtons = go;
		PanelButtons.SetActive(false); // this is a template, don't touch it.

		go.GetComponent<PanelButtons>().ButtonTopText.GetComponent<Text>().text = ButtonTopText;
		go.GetComponent<PanelButtons>().ButtonTop.GetComponent<Button>().onClick = ButtonTopAction;

		go.GetComponent<PanelButtons>().ButtonBottomText.GetComponent<Text>().text = ButtonBottomText;
		go.GetComponent<PanelButtons>().ButtonBottom.GetComponent<Button>().onClick = ButtonBottomAction;
		go.GetComponent<PanelButtons>().ShowClouds(!HideClouds);
		go.GetComponent<PanelButtons>().ShowSettingsButton(!HideSettingsButton);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
