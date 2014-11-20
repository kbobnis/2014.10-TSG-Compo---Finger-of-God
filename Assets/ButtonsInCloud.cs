using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonsInCloud : MonoBehaviour {

	public GameObject PanelButtons;
	
	//I don't want to show this attribute in inspector
	private GameObject _ClonedPanelButtons;

	//this is underlined, because i want to have an option to set in the inspector
	public string _ButtonTopText, ButtonBottomText;
	public bool HideClouds;
	public bool HideSettingsButton;
	public Button.ButtonClickedEvent ButtonTopAction, ButtonBottomAction;

	public string ButtonTopText {
		set { _ButtonTopText = value; UpdateInfo(); }
	}
	public GameObject ClonedPanelButtons {
		get { return _ClonedPanelButtons;  }
	}

	// Use this for initialization
	void Awake() {

		PanelButtons.SetActive(true); // this is a template, don't touch it.
		GameObject go = PanelButtons.GetComponent<PanelButtons>().CopyMeIn(gameObject);
		_ClonedPanelButtons = go;
		PanelButtons.SetActive(false); // this is a template, don't touch it.

		UpdateInfo();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateInfo() {
		_ClonedPanelButtons.GetComponent<PanelButtons>().SetInvokingPanel(gameObject);
		_ClonedPanelButtons.GetComponent<PanelButtons>().ButtonTopText.GetComponent<Text>().text = _ButtonTopText;
		_ClonedPanelButtons.GetComponent<PanelButtons>().ButtonTop.GetComponent<Button>().onClick = ButtonTopAction;

		_ClonedPanelButtons.GetComponent<PanelButtons>().ButtonBottomText.GetComponent<Text>().text = ButtonBottomText;
		_ClonedPanelButtons.GetComponent<PanelButtons>().ButtonBottom.GetComponent<Button>().onClick = ButtonBottomAction;
		_ClonedPanelButtons.GetComponent<PanelButtons>().ShowClouds(!HideClouds);
		_ClonedPanelButtons.GetComponent<PanelButtons>().ShowSettingsButton(!HideSettingsButton);
	}
}
