using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public enum Side{
	Up, Down, Left, Right
}

public static class SideMethods{

	public static int DeltaX(this Side s){
		switch (s) {
			case Side.Left: return -1;
			case Side.Right: return 1;
			default: return 0;
		}
	}

	public static int DeltaY(this Side s){
		switch (s) {
			case Side.Up: return -1;
			case Side.Down: return 1;
			default: return 0;
		}
	}
}

public class Game : MonoBehaviour {

	public GameObject ScrollableListBuildings;
	public GameObject TextPopulation, TextCasualties;

	private List<List<GameObject>> Buildings = new List<List<GameObject>> ();

	public static Game Me;

	// Use this for initialization
	void Start () {
		Me = this;

		ScrollableList sl = ScrollableListBuildings.GetComponent<ScrollableList> ();
		int columns = sl.columnCount = 6;

		List<GameObject> buildingsRow = new List<GameObject>();
		for (int i=0; i < 48; i++) {

			if (i % columns == 0 && i > 0){
				Buildings.Add(buildingsRow);
				buildingsRow = new List<GameObject>();
			}

			GameObject newItem = Instantiate(sl.itemPrefab) as GameObject;
			newItem.SetActive(true);
			Building b = newItem.GetComponent<Building>();

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
			sl.ElementsToPut.Add(newItem);

			buildingsRow.Add(newItem);
		}

		sl.Prepare ();

		sl.itemPrefab.SetActive (false);

		int x=0; 
		foreach (List<GameObject> bsTmp in Buildings) {
			int y=0; 
			foreach(GameObject  bTmp in bsTmp){
				Debug.Log("x: " + x + ", y: " + y + ", building: " + Buildings[x][y]);
				y++;
			}
			y++;
		}
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void PointerEnter(UnityEngine.EventSystems.BaseEventData baseEvent) {

		//UnityEngine.EventSystems.BaseEventData p = baseEvent;
		UnityEngine.EventSystems.PointerEventData p = baseEvent as UnityEngine.EventSystems.PointerEventData;
		if (p != null){
			GameObject go = p.pointerEnter;
			ButtonTouched(go);
		}
	}

	private GameObject GetNeighbour(GameObject g, Side s){

		int myX=0, myY=0;
		int i = 0;
		foreach (List<GameObject> rows in Buildings) {
			int j=0;
			foreach(GameObject b in rows){
				if (b == g){
					myX = j;
					myY = i;
				}
				j++;
			}
			i++;
		}
		GameObject tmp = null;

		if (Buildings.Count > (s.DeltaY() + myY) && Buildings[s.DeltaY() + myY].Count > (s.DeltaX() + myX)){
		 tmp = Buildings [s.DeltaY () + myY] [s.DeltaX () + myX];
		}
		return tmp;
	}

	public void ButtonTouched(GameObject go){

		try{
		go.GetComponent<Building>().TreatWith(Element.Crush);

		GameObject left = GetNeighbour (go, Side.Left);
		if (left != null){
			left.GetComponent<Building> ().TreatWith (Element.SmallCrush);
		}

		GameObject right = GetNeighbour (go, Side.Right);
		if (right != null){
			right.GetComponent<Building> ().TreatWith (Element.SmallCrush);
		}

		GameObject up = GetNeighbour (go, Side.Up);
		if (up != null){
			up.GetComponent<Building> ().TreatWith (Element.SmallCrush);
		}

		GameObject down = GetNeighbour (go, Side.Down);
		if (down != null){ 
			down.GetComponent<Building> ().TreatWith (Element.SmallCrush);
		}
		}catch(System.Exception e){
			Debug.Log("[ButtonTouched] exception: " + e);
		}
	}

}
