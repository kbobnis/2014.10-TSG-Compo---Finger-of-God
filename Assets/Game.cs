using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public enum Cataclysm{
	Fire, Whirlwind, Crush, Strike
}
public static class ModeTypeHelper{

	public static Cataclysm ModeTypeFromString(string text){
		switch (text) {
			case "fire": return Cataclysm.Fire;
			case "whirlwind": return Cataclysm.Whirlwind;
			case "crush": return Cataclysm.Crush;
			case "strike": return Cataclysm.Strike;
		}
		throw new UnityException ("There is no mode type for text " + text);
	}

	public static AudioClip GetSound(this Cataclysm mt){
		switch (mt) {
		case Cataclysm.Crush: return SoundManager.Crush;
		case Cataclysm.Fire: return SoundManager.Fire;
		case Cataclysm.Whirlwind: return SoundManager.Huracaine;
		case Cataclysm.Strike: return SoundManager.Lightning;
		default: throw new UnityException("there is no sound for mode type: " + mt);

		}
	}


}

public class Game : MonoBehaviour {

	public Cataclysm ModeType = Cataclysm.Crush;
	public bool HasModeChanged = true;
	public GameObject ButtonCrush, ButtonFire, ButtonWhirlwind, ButtonStrike;
	public GameObject ScrollableListBuildings;
	public GameObject TextPopulation, TextCasualties;

	public static Game Me;

	// Use this for initialization
	void Start () {
		Me = this;

		ScrollableList sl = ScrollableListBuildings.GetComponent<ScrollableList> ();

		List<GameObject> buildings = new List<GameObject> ();
		for (int i=0; i < 42; i++) {
			GameObject newItem = Instantiate(sl.itemPrefab) as GameObject;
			Building b = newItem.AddComponent<Building>();

			int ticket = Mathf.RoundToInt( Random.Range(1,3));
			switch(ticket){
			case 1: b.CreateWood1(); break;
			case 2: b.CreateStone1(); break;
			default: throw new UnityException("Please initiate building");
			}

			//TextPopulation.GetComponent<NumberShower>().AddNumber(b.PopulationDelta);
			b.Listeners.Add(TextPopulation.GetComponent<NumberShower>());
			b.Inform();
			b.Listeners.Add(TextCasualties.GetComponent<NumberShower>());

			buildings.Add(newItem);

		}
		sl.columnCount = 6;
		sl.ElementsToPut = buildings;
		sl.Prepare ();
	}
	
	// Update is called once per frame
	void Update () {
		if (HasModeChanged) {

			ButtonStrike.GetComponent<Image>().color = ButtonWhirlwind.GetComponent<Image>().color = ButtonFire.GetComponent<Image>().color = ButtonCrush.GetComponent<Image>().color = Color.gray;

			Image image = null;
			switch(ModeType){
				case Cataclysm.Crush: image =  ButtonCrush.GetComponent<Image>(); break;
				case Cataclysm.Fire: image = ButtonFire.GetComponent<Image>(); break;
				case Cataclysm.Whirlwind: image = ButtonWhirlwind.GetComponent<Image>(); break;
				case Cataclysm.Strike: image = ButtonStrike.GetComponent<Image>(); break;
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

		//UnityEngine.EventSystems.BaseEventData p = baseEvent;
		UnityEngine.EventSystems.PointerEventData p = baseEvent as UnityEngine.EventSystems.PointerEventData;
		if (p != null){
			GameObject go = p.pointerEnter;
			ButtonTouched(go);
		}
	}

	public void ButtonTouched(GameObject go){
		try{
			if (go != null && go.GetComponent<Building>() != null){
				go.GetComponent<Building>().TreatWith(ModeType);
			}
		} catch(System.Exception e){
			Debug.Log("exception4: " + e);
		}
	}

}
