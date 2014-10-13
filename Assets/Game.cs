using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public enum Side{
	Up, Down, Left, Right, Center
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

public delegate bool GoQuestion(GameObject go);

public class Game : MonoBehaviour {

	public GameObject TextPopulation, TextInterventions;
	public GameObject PanelStartGame, PanelMainScreen, PanelMainScreenPrefab;
	private bool Preparing;

	private List<List<GameObject>> Buildings = new List<List<GameObject>> ();

	public static Game Me;

	private void Clean(){

		foreach(List<GameObject> l in Buildings){
			foreach(GameObject go in l){
				Destroy(go);
			}
		}
		Buildings.Clear();
	}

	public void Start(){
	}

	// Use this for initialization
	public void Prepare(){
		Clean ();
		Preparing = true;
		TextInterventions.GetComponent<NumberShower> ().Number = 0;
		PanelMainScreen.SetActive (true);
		PanelStartGame.SetActive (false);

		Me = this;

		ScrollableList sl = PanelMainScreen.GetComponentInChildren<ScrollableList> ();
		int columns = sl.columnCount;

		sl.ElementsToPut.Clear ();
		List<GameObject> buildingsRow = new List<GameObject>();
		for (int i=0; i < sl.ElementSize; i++) {

			if (i % columns == 0 && i > 0){
				Buildings.Add(buildingsRow);
				buildingsRow = new List<GameObject>();
			}
			GameObject newItem = Instantiate(sl.itemPrefab) as GameObject;
			newItem.SetActive(true);
			Building b = newItem.GetComponent<Building>();

			int ticket = Mathf.RoundToInt( Random.value * 8 );

			if (ticket == 0){
				b.CreateGasStation();
			}

			if (ticket == 1){
				b.CreateWaterSilo();
			} 

			if (ticket == 2){
				b.CreateElectricityTower();
			} 

			if (ticket > 2 && ticket <= 5){
				b.CreateStone1(); 
			} 

			if (ticket > 5 && ticket < 9){
				b.CreateWood1();
			}


			b.Listeners.Add(TextPopulation.GetComponent<NumberShower>());
			sl.ElementsToPut.Add(newItem);
			b.Inform();

			buildingsRow.Add(newItem);

		}
		Buildings.Add (buildingsRow);
		sl.Prepare ();
		sl.itemPrefab.SetActive (false);
		
		//create neighbours -> optimization
		int x = 0;
		foreach (List<GameObject> cols in Buildings) {
			int y = 0;
			foreach (GameObject bi in cols) {
				Building bu = bi.GetComponent<Building>();

				if (Buildings.Count > x + 1) {
					bu.Neighbours.Add(Side.Right, Buildings[x + 1][y].GetComponent<Building>());
				}
				
				if (Buildings[x].Count > y + 1) {
					bu.Neighbours.Add(Side.Down, Buildings[x][y+1].GetComponent<Building>());
				}
				
				if (x - 1 >= 0) {
					bu.Neighbours.Add(Side.Left, Buildings[x - 1][y].GetComponent<Building>());
				}
				
				if (y - 1 >= 0){
					bu.Neighbours.Add(Side.Up, Buildings[x][y - 1].GetComponent<Building>());
				}
				y++;	
			}
			x++;	
		}
		
		
		Preparing = false;
	}



	void Update(){
		if (TextPopulation.GetComponent<NumberShower> ().Number == 0 && !Preparing) {
			PanelStartGame.SetActive(true);
			PanelMainScreen.SetActive (false);
		}
	}
	
	public void PointerEnter(UnityEngine.EventSystems.BaseEventData baseEvent) {
		UnityEngine.EventSystems.PointerEventData p = baseEvent as UnityEngine.EventSystems.PointerEventData;
		if (p != null){
			GameObject go = p.pointerEnter;
			ButtonTouched(go);
		}
	}

	public GameObject GetNeighbour(GameObject g, Side s){
		if (s == Side.Center) {
			return g;
		}
		GameObject tmp2 = null;
		if (g.GetComponent<Building>().Neighbours.ContainsKey(s)) {
			tmp2 = g.GetComponent<Building>().Neighbours[s].gameObject;
		}
		return tmp2;
	}

	public void CataclysmTo(Element el, GameObject g, Side s){
		GameObject tmp = GetNeighbour (g, s);
		if (tmp != null) {
			tmp.GetComponent<Building>().TreatWith(el);
		}
	}




	public void TreatNeighboursWith(GameObject go, Element e, float startingValue=0, GoQuestion goq=null){
		if (startingValue >= 1 || go == null) {
			return ; //we don't want to waste time for dead startingValue
		}

		GameObject left = GetNeighbour (go, Side.Left);
		if (left != null && (goq == null || goq(left))){
			left.GetComponent<Building> ().TreatWith (e, startingValue);
		}
		GameObject right = GetNeighbour (go, Side.Right);
		if (right != null && (goq == null || goq(right))){
			right.GetComponent<Building> ().TreatWith (e, startingValue);
		}
		
		GameObject up = GetNeighbour (go, Side.Up);
		if (up != null && (goq == null || goq(up))){
			up.GetComponent<Building> ().TreatWith (e, startingValue);
		}
		
		GameObject down = GetNeighbour (go, Side.Down);
		if (down != null && (goq == null || goq(down))){
			down.GetComponent<Building> ().TreatWith (e, startingValue);
		}
	}

	public void ButtonTouched(GameObject go){
		TextInterventions.GetComponent<NumberShower> ().AddNumber (1);

		try{
			go.GetComponent<Building>().TreatWith(Element.Crush);
			//TreatNeighboursWith(go, Element.SmallCrush);
		}catch(System.Exception e){
			Debug.Log("[ButtonTouched] exception: " + e);
		}
	}

}
