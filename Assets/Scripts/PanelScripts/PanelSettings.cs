using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class PanelSettings : MonoBehaviour {

	public GameObject InputFieldTextName, ButtonMusic, ButtonSounds, TextVibrations, ButtonVibrations;
	
	public Sprite SpriteOn, SpriteOff;


	void Awake() {

		ButtonMusic.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
		ButtonMusic.GetComponent<Toggle>().onValueChanged.AddListener((bool on) => { 
			ButtonMusic.GetComponent<Image>().sprite = on ? SpriteOn : SpriteOff;
			SoundManager.EnableMusic(on);
		});
		ButtonMusic.GetComponent<Toggle>().isOn = SoundManager.IsMusicEnabled();
		
		ButtonSounds.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
		ButtonSounds.GetComponent<Toggle>().onValueChanged.AddListener((bool on) => { 
			ButtonSounds.GetComponent<Image>().sprite = on ? SpriteOn : SpriteOff;
			SoundManager.EnableSounds(on);
		});
		ButtonSounds.GetComponent<Toggle>().isOn = SoundManager.AreSoundsEnabled();

		

		ButtonVibrations.SetActive(false);
		TextVibrations.SetActive(false);
		ButtonVibrations.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
		ButtonVibrations.GetComponent<Toggle>().onValueChanged.AddListener((bool on) => { ButtonVibrations.GetComponent<Image>().sprite = on ? SpriteOn : SpriteOff; });
	}

	void Start() {
		InputFieldTextName.GetComponent<InputField>().value = Game.Me.UserName;
	}

	void Update() {
		GetComponent<ButtonsInCloud>().ClonedPanelButtons.GetComponent<PanelButtons>().ButtonTop.SetActive(Game.Me.UserName != InputFieldTextName.GetComponent<InputField>().value);
	}

	public void SaveSettings() {
		StartCoroutine(SaveSettingsCoroutine());
	}

	private IEnumerator SaveSettingsCoroutine() {
		Game.Me.PanelLoading.SetActive(true);
		Game.Me.PanelLoading.GetComponent<PanelLoading>().TextTop = "Saving name...";

		WWW www = null;
		string newName = InputFieldTextName.GetComponent<InputField>().value;
		if (newName.Length == 0) {
			newName = "Empty";
		}
		if (newName.Length > 9) {
			newName = newName.Substring(0, 9);
		}
		newName = Regex.Replace(newName, "\\W", "");
		InputFieldTextName.GetComponent<InputField>().value = newName;
		if (newName != Game.Me.UserName) {
			www = WebConnector.ChangeName(newName);
			Game.Me.UserName = newName; 
		}

		yield return new WaitForSeconds(1f);
		yield return www;
		Game.Me.PanelLoading.SetActive(false);
	}

	public void Back() {
		gameObject.SetActive(false);
	}

	public void OpenFb() {
		Application.OpenURL("https://www.facebook.com/pages/Wyspian-Studios/1541988342704621");
	}

}
