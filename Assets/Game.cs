using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

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

	public static AudioClip GetSound(this ModeType mt){
		switch (mt) {
		case ModeType.Crush: return SoundManager.Crush;
		case ModeType.Fire: return SoundManager.Fire;
		case ModeType.Whirlwind: return SoundManager.Huracaine;
		default: throw new UnityException("there is no sound for mode type: " + mt);

		}
	}

}

public class Game : MonoBehaviour {

	public ModeType ModeType = ModeType.Crush;
	public bool HasModeChanged = true;
	public GameObject ButtonCrush, ButtonFire, ButtonWhirlwind;
	public GameObject ScrollableListBuildings;

	// Use this for initialization
	void Start () {

		ScrollableList sl = ScrollableListBuildings.GetComponent<ScrollableList> ();

		List<GameObject> buildings = new List<GameObject> ();
		for (int i=0; i < 130; i++) {
			GameObject newItem = Instantiate(sl.itemPrefab) as GameObject;
			Building b = newItem.AddComponent<Building>();
			b.BuildingType = BuildingTypeMethod.GetRandom();
			buildings.Add(newItem);
		}
		sl.columnCount = 9;
		sl.ElementsToPut = buildings;
		sl.Prepare ();
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

	public void PointerEnter(UnityEngine.EventSystems.BaseEventData baseEvent) {

		UnityEngine.EventSystems.PointerEventData p = baseEvent as UnityEngine.EventSystems.PointerEventData;
		GameObject go = p.pointerEnter;
		Debug.Log(" triggered an event! " + go!=null?go.GetType().ToString():"null");
		if (go != null){
			if (go.GetComponent<Building>() != null){
				go.GetComponent<Building>().DestroyWith(ModeType);
			}
		}

	}

}
