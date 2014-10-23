using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using System.ComponentModel;

public enum Element{
	Crush, SmallCrush, Water, Electricity, Fire, SmokeAfterFire
}

public delegate bool GoQuestion(Building go);

public class Building : MonoBehaviour{

	private Dictionary<Element, BuildingStatus> Statuses = new Dictionary<Element, BuildingStatus>();
	private List<Element> AfterDeath = new List<Element>();
	private Dictionary<Element, GoQuestion> FillRequirement = new Dictionary<Element, GoQuestion>();
	private Dictionary<Element, float> ContaminateDelta = new Dictionary<Element, float>();

	private int StartingPopulation, _Population, LastCheckedPopulation;
	private float _Health = 1f;
	private int ImageNumberFromAtlas;
	private Vector3 startingPos;

	public Dictionary<Side, Building> Neighbours = new Dictionary<Side, Building>();
	public List<Listener<ScoreType, float>> Listeners = new List<Listener<ScoreType, float>>();

	public GameObject GameObjectGroundLevel, GameObjectBuilding;
	public GameObject GameObjectFire, GameObjectWaterLevel, GameObjectSmokeAfterFire, GameObjectElectricity, GameObjectCrush, GameObjectHealthBar;

	private static Building GetNeighbour(Building b, Side s) {
		if (s == Side.Center) {
			return b;
		}
		Building tmp2 = null;
		if (b.Neighbours.ContainsKey(s)) {
			tmp2 = b.Neighbours[s];
		}
		return tmp2;
	}

	private static void TreatNeighboursWith(Building b, Element e, float startingValue = 1, GoQuestion goq = null) {
		if (startingValue > 1) {
			startingValue = 1;
		}
		if (startingValue <= 0 || b == null) {
			return; //we don't want to waste time for dead startingValue
		}

		Building left = GetNeighbour(b, Side.Left);
		if (left != null && (goq == null || goq(left))) {
			left.GetComponent<Building>().TreatWith(e, startingValue);
		}
		Building right = GetNeighbour(b, Side.Right);
		if (right != null && (goq == null || goq(right))) {
			right.TreatWith(e, startingValue);
		}

		Building up = GetNeighbour(b, Side.Up);
		if (up != null && (goq == null || goq(up))) {
			up.TreatWith(e, startingValue);
		}

		Building down = GetNeighbour(b, Side.Down);
		if (down != null && (goq == null || goq(down))) {
			down.TreatWith(e, startingValue);
		}
	}

	public void CataclysmTo(Element el, Building b, Side s) {
		Building tmp = GetNeighbour(b, s);
		if (tmp != null) {
			tmp.TreatWith(el);
		}
	}

	public int PopulationDelta{
		get {
			int delta = _Population - LastCheckedPopulation;
			LastCheckedPopulation = _Population;
			return delta;
		}
	}

	public int Population {
		get { return _Population;  }
	}

	public float Health{
		get { return _Health; }
		set {
			bool wasDead = _Health <= 0;
			_Health = value; 
			if (_Health < 0){
				_Health = 0;
			}
			InformListeners();
			if (!wasDead && _Health == 0){
				Die();
			}
		}
	}

	private void Die(){
		PlaySingleSound.SpawnSound(SoundManager.BuildingDown);

		foreach (Element e in AfterDeath) {
			TreatWith(e);
		}

		foreach (Element e in Statuses.Keys.ToList ()) {
			if (e == Element.Fire && Statuses[e].Value > 0.1f) { //we don't want to hit full blown fire it is finishing
				TreatNeighboursWith(this, e, 1, (Building b) => { return b.Health > 0;  });
			}
		}
	}

	private void ExtinguishFireCheck(){
		if (Statuses[Element.Water].Value > 0 && Statuses[Element.Fire].Value > 0) {
			Statuses[Element.Fire].StopNow();
			Statuses[Element.SmokeAfterFire].Add();
		}
	}

	void FixedUpdate(){
		//check if prepared already
		if (Statuses.Count == 0) {
			return;
		}

		ExtinguishFireCheck();

		foreach (Element status in Statuses.Keys.ToList()) {
			Health -= Statuses[status].UpdateAndGetDamage();
		}

		foreach (Element e in FillRequirement.Keys.ToList()) {

			if (Statuses[e].CanFill()) {
				Statuses[e].Fill(); //so it won't fill too quickly
				float toSet = Statuses[e].Value;
				if (ContaminateDelta.ContainsKey(e)) {
					toSet += ContaminateDelta[e];
				}
				TreatNeighboursWith(this, e, toSet, FillRequirement[e]);
			}
		}
		UpdateImage();
	}

	public void InformListeners(){
		_Population = (int)(Health * StartingPopulation);
		int d = PopulationDelta;
		if (d != 0 ) {
			foreach(Listener<ScoreType, float> l in Listeners){
				l.Inform(ScoreType.Population, d);
			}
		}
	}

	private void UpdateImage(){
		Sprite s = SpriteManager.BuildingSprites [ImageNumberFromAtlas];
		if (Health <= 0){
			s = SpriteManager.BuildingSpritesDestroyed[ImageNumberFromAtlas];
		}
		Sprite actualSprite = GameObjectBuilding.GetComponent<Image> ().sprite;
		if (actualSprite != s){
			GameObjectBuilding.GetComponent<Image> ().sprite = s;
		}

		//update health bar
		GameObjectHealthBar.GetComponent<Image> ().fillAmount = Health;
		GameObjectHealthBar.transform.parent.gameObject.SetActive (Health > 0);
	}

	public void TreatWith(Element e, float startingValue=1){
		//Debug.Log("Treating with: " + e + ", starting value: " + startingValue);
		Statuses[e].Add(startingValue);
	}

	internal void CreateFromTemplate(BuildingType bt) {

		Dictionary<Element, float> strikeDamage = new Dictionary<Element, float>();
		Dictionary<Element, float> effectDamage = new Dictionary<Element, float>();
		Dictionary<Element, float> effectTime = new Dictionary<Element, float>();
		Dictionary<Element, float> fillSpeed = new Dictionary<Element, float>();

		switch (bt) {
			case BuildingType.ElectricTower:
				
				effectDamage.Add (Element.Fire, 0.3f);
				effectDamage.Add (Element.Water, 0.05f);
				effectDamage.Add(Element.Electricity, 0.6f);
				
				effectTime.Add (Element.Fire, 6f);
				
				AfterDeath.Add (Element.Electricity);
				ImageNumberFromAtlas = 41;
				StartingPopulation = _Population = 0;
				break;
			case BuildingType.Wood:
				
				effectDamage.Add(Element.Fire, 0.27f);
				effectDamage.Add(Element.Water, 0.15f);
				effectTime.Add(Element.Fire, 3.8f);
				effectDamage.Add(Element.Electricity, 0.7f);

				ImageNumberFromAtlas = 38;
				StartingPopulation = _Population = 1000;
				break;
			case BuildingType.Stone:
				
				effectDamage.Add(Element.Fire, 0.17f);
				effectDamage.Add(Element.Water, 0.17f);
				effectDamage.Add(Element.Electricity, 0.75f);

				effectTime.Add(Element.Fire, 3.8f);
				
				ImageNumberFromAtlas = 37;
				StartingPopulation = _Population = 1000;
				break;
			case BuildingType.WaterTower:
				
				effectDamage.Add (Element.Fire, 0.27f);
				effectDamage.Add (Element.Water, 0.11f);
				effectDamage.Add(Element.Electricity, 0.7f);

				effectTime.Add (Element.Fire, 3.8f);

				AfterDeath.Add (Element.Water);
				ImageNumberFromAtlas = 40;
				break;
			case BuildingType.GasStation:
				
				effectDamage.Add (Element.Fire, 1f);
				effectDamage.Add (Element.Water, 0.12f);
				effectDamage.Add(Element.Electricity, 0.75f);

				effectTime.Add(Element.Fire, 1.2f);

				AfterDeath.Add (Element.Fire);
				ImageNumberFromAtlas = 39;
				break;

			case BuildingType.Destroyed:
				_Health = 0;
				ImageNumberFromAtlas = 37;
				break;
			case BuildingType.Block:
				
				
				effectDamage.Add(Element.Fire, 0.75f);
				effectDamage.Add(Element.Water, 0.1f);

				effectTime.Add(Element.Fire, 1.8f);
				
				ImageNumberFromAtlas = 5;
				break;
		}
		strikeDamage.Add(Element.Crush, 0.85f);

		effectDamage.Add (Element.Crush, 0.00f);
		effectDamage.Add(Element.SmokeAfterFire, 0f);

		effectTime.Add (Element.SmokeAfterFire, 0.5f);
		effectTime.Add (Element.Crush, 0.3f);
		effectTime.Add (Element.Electricity, 1f);
		effectTime.Add (Element.Water, 5.5f);
		
		fillSpeed.Add (Element.Electricity, 0.09f);
		fillSpeed.Add (Element.Water, 0.15f);

		FillRequirement.Add(Element.Electricity, delegate(Building b){
			return b.Statuses[Element.Water].Value > 0 && Statuses[Element.Water].Value > 0 ;
		});

		FillRequirement.Add(Element.Water, delegate(Building b){
			return 
				(
					(
						(	 //side up check if is in water
							GetNeighbour(b, Side.Up) != null &&
							GetNeighbour(b, Side.Up).Statuses[Element.Water].Value > 0 &&
							GetNeighbour(b, Side.Up).Health == 0
						)
						||
						(	 //side down check if is in water
							GetNeighbour(b, Side.Down) != null &&
							GetNeighbour(b, Side.Down).Statuses[Element.Water].Value > 0 &&
							GetNeighbour(b, Side.Down).Health == 0
						)
						||
						(	 //side left check if is in water
							GetNeighbour(b, Side.Left) != null &&
							GetNeighbour(b, Side.Left).Statuses[Element.Water].Value > 0  &&
							GetNeighbour(b, Side.Left).Health == 0
						)
						||
						(	 //side right check if is in water
							GetNeighbour(b, Side.Right) != null &&
							GetNeighbour(b, Side.Right).Statuses[Element.Water].Value > 0 &&
							GetNeighbour(b, Side.Right).Health == 0
						)
					)
				)
				&&
				(
					b.Statuses[Element.Water].Value == 0
					|| 
					b.Statuses[Element.Water].Value <  Statuses[Element.Water].Value
				);
		});

		ContaminateDelta.Add (Element.Fire, 1f);

		startingPos = GameObjectBuilding.GetComponent<RectTransform>().localPosition;

		Dictionary<Element, GameObject> elGo = new Dictionary<Element, GameObject>();
		elGo.Add(Element.Fire, GameObjectFire);
		elGo.Add(Element.Water, GameObjectWaterLevel);
		elGo.Add(Element.SmokeAfterFire, GameObjectSmokeAfterFire);
		elGo.Add(Element.Electricity, GameObjectElectricity);
		elGo.Add(Element.Crush, GameObjectCrush);

		//initializing statuses
		foreach (Element e in new Element[] { Element.Fire, Element.Water, Element.SmokeAfterFire, Element.Electricity, Element.Crush }) {
			float fillSpeedF = fillSpeed.ContainsKey(e)?fillSpeed[e]:0;
			float strikeDamageF = strikeDamage.ContainsKey(e) ? strikeDamage[e] : 0;
			float effectDamageF = (effectDamage.ContainsKey(e) ? effectDamage[e] : 0);
			float effectTimeF = (effectTime.ContainsKey(e) ? effectTime[e] : 0);
			Statuses.Add(e, new BuildingStatus(elGo[e], SpriteManager.ElementSprites[e], SoundManager.Clips[e], effectDamageF, effectTimeF, strikeDamageF, fillSpeedF));
		}
	}

	public void Clicked(BaseEventData b) {
		foreach (Listener<ScoreType, float> l in Listeners) {
			l.Inform(ScoreType.Interventions, 1f);
		}
		TreatWith(Element.Crush);
		TreatWith(Element.Fire);
	}
}
