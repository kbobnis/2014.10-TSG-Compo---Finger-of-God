using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public enum ModeType{
	Fire, Whirlwind, Crush
}
public static class ModeTypeHelper{

	public static ModeType ModeTypeFromString(string text){
		switch (text) {
			case "fire": return ModeType.Fire;
			case "whirlwind": return ModeType.Whirlwind;
			case "crush": return ModeType.Crush;
		}
		return ModeType.Fire;
	}

}

public class Game : MonoBehaviour {

	public ModeType ModeType = ModeType.Crush;
	public bool HasModeChanged = true;
	public GameObject ButtonCrush, ButtonFire, ButtonWhirlwind;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (HasModeChanged) {

			ButtonWhirlwind.GetComponent<Image>().color = ButtonFire.GetComponent<Image>().color = ButtonCrush.GetComponent<Image>().color = Color.gray;

			Image image = null;
			switch(ModeType){
				case ModeType.Crush: image =  ButtonCrush.GetComponent<Image>(); break;
				case ModeType.Fire: image = ButtonFire.GetComponent<Image>(); break;
				case ModeType.Whirlwind: image = ButtonWhirlwind.GetComponent<Image>(); break;
			}
			if (image != null){
				image.color = Color.white;
			}
			HasModeChanged = false;
		}
	}



	public void ModeChanged(string type){
		ModeType = ModeTypeHelper.ModeTypeFromString (type);
		HasModeChanged = true;
	}
}
