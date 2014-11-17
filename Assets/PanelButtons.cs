using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PanelButtons : MonoBehaviour {

	public GameObject ButtonTop, ButtonTopText, ButtonTopClouds, ButtonBottom, ButtonBottomText, ButtonBottomClouds, PanelSettings, ButtonSettings;
	public JustDo AfterSettingsClose;

	private bool IsPanelSettingsActive;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//sometimes we want an action after settings close
		if (IsPanelSettingsActive && !PanelSettings.activeSelf) {
			IsPanelSettingsActive = false;
			if (AfterSettingsClose != null) {
				AfterSettingsClose();
			}
		}
	}

	internal GameObject CopyMeIn(GameObject parent) {
		GameObject go = Instantiate(gameObject) as GameObject;
		go.transform.parent = parent.transform;
		go.GetComponent<RectTransform>().offsetMin = new Vector2(); //we want it to fit into anchors exactly as they were set in editor
		go.GetComponent<RectTransform>().offsetMax = new Vector2(); //we want it to fit into anchors exactly as they were set in editor
		return go;
	}

	public void ShowClouds(bool enabled){
		GetComponent<Image>().enabled = enabled;
		ButtonTopClouds.SetActive(enabled);
		ButtonBottomClouds.SetActive(enabled);
	}

	public void ShowSettings() {
		IsPanelSettingsActive = true;
		PanelSettings.SetActive(true);
	}

	public void ShowSettingsButton(bool on) {
		ButtonSettings.SetActive(on);
	}
}
