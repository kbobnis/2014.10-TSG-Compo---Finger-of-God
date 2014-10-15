using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public enum Element{
	Crush, SmallCrush, Water, Electricity, Fire, SmokeAfterFire
}

public enum BuildingType {
	None,
	Wood,
	Stone,
	Block,
	WaterTower,
	ElectricTower,
	GasStation,
	Destroyed
}

public class Building : MonoBehaviour{

	private Dictionary<Element, float> StrikeDamage = new Dictionary<Element, float>();
	private Dictionary<Element, float> EffectDamage = new Dictionary<Element, float>();
	private Dictionary<Element, float> EffectTime = new Dictionary<Element, float>();
	private Dictionary<Element, Side> AfterHit = new Dictionary<Element, Side> ();
	private Dictionary<Element, float> Statuses = new Dictionary<Element, float>();
	private Dictionary<Element, float> FillSpeed = new Dictionary<Element, float> ();
	private Dictionary<Element, float> LastFill = new Dictionary<Element, float> ();
	private Dictionary<Element, GoQuestion> FillRequirement = new Dictionary<Element, GoQuestion>();
	private Dictionary<Element, float> ContaminateDelta = new Dictionary<Element, float> ();

	public Dictionary<Side, Building> Neighbours = new Dictionary<Side, Building>();

	public List<Listener<ScoreType, int>> Listeners = new List<Listener<ScoreType, int>>();

	private int StartingPopulation, _Population, LastCheckedPopulation;
	private float _Health = 1f;
	private int ImageNumberFromAtlas;

	public GameObject GameObjectGroundLevel, GameObjectBuilding;
	public GameObject GameObjectFire, GameObjectWaterLevel, GameObjectSmokeAfterFire, GameObjectElectricity, GameObjectCrush, GameObjectHealthBar;

	private Dictionary<Element, GameObject> ElGO = new Dictionary<Element, GameObject> ();
	private Vector3 startingPos;


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

	private static void TreatNeighboursWith(Building b, Element e, float startingValue = 0, GoQuestion goq = null) {
		if (startingValue >= 1 || b == null) {
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


	void Start(){
		startingPos = GameObjectBuilding.GetComponent<RectTransform>().localPosition;

		ElGO.Add (Element.Fire, GameObjectFire);
		ElGO.Add (Element.Water, GameObjectWaterLevel);
		ElGO.Add (Element.SmokeAfterFire, GameObjectSmokeAfterFire);
		ElGO.Add (Element.Electricity, GameObjectElectricity);
		ElGO.Add (Element.Crush, GameObjectCrush);

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

	private void AddStatus(Element e, float startingValue=0){
		if (!Statuses.ContainsKey(e)){
			Statuses.Add(e, startingValue);
			LastFill[e] = Time.time;
			if (SoundManager.Clips.ContainsKey (e)) {
				PlaySingleSound.SpawnSound(SoundManager.Clips[e]);
			}

		} else if (Statuses[e] > startingValue) {
			Statuses[e] = startingValue;
			LastFill[e] = Time.time;
			if (SoundManager.Clips.ContainsKey (e) && e != Element.Water) {
				PlaySingleSound.SpawnSound(SoundManager.Clips[e]);
			}
		}

	}

	private void Die(){
		PlaySingleSound.SpawnSound(SoundManager.BuildingDown);

        //if filling with water, check the ground level, only on crater water fills
		foreach (Element e in AfterHit.Keys.ToList ()) {
			TreatWith(e);
			if (e == Element.Fire) {
				TreatNeighboursWith(GetNeighbour(this, AfterHit[e]), e, 0, delegate(Building b) {
					return b.Health > 0;
				});
			}
		}
	}

	private void ExtinguishFireCheck(){
		if (Statuses.ContainsKey (Element.Water) && Statuses.ContainsKey (Element.Fire)) {
			FinishStatus(Element.Fire);
			AddStatus(Element.SmokeAfterFire);
		} 
	}

	private void FinishStatus(Element e){
		Statuses.Remove(e);

		if (e == Element.Crush ){
			AddStatus(Element.SmallCrush);
		}
		if (ElGO.ContainsKey(e)){
			ElGO[e].GetComponent<Image>().enabled = false;
		}
	}

	void Update(){

		ExtinguishFireCheck();

		List<Element> bss = Statuses.Keys.ToList();
		foreach (Element status in bss) {
			float value = Statuses[status];
			if (value < 1 ){

				if (EffectDamage.ContainsKey(status)){
					Health -= EffectDamage[status] * Time.deltaTime;
				}
				if (Statuses.ContainsKey(status) && EffectTime.ContainsKey(status)){
					float progress = Statuses[status] += 1 / EffectTime[status] * Time.deltaTime;
					if (progress > 1){
						progress = 1f;
					}
					if (SpriteManager.ElementSprites.ContainsKey(status) && ElGO.ContainsKey(status)){
						Image i = ElGO[status].GetComponent<Image>();
						i.enabled = true;
						Sprite[] ss = SpriteManager.ElementSprites[status];
						i.sprite = ss[Mathf.RoundToInt(progress * (ss.Length-1))];
					}
				}
			}
		}

		bss = Statuses.Keys.ToList();
		foreach (Element status in bss) {
			if (Statuses[status] >= 1) {
				FinishStatus(status);
			}
		}

		foreach (Element e in FillRequirement.Keys.ToList ()) {
			if (Statuses.ContainsKey(e) && Time.time - LastFill[e] > FillSpeed[e] )  {
				LastFill[e] = Time.time;
				
				float toSet = Statuses[e];
				if (ContaminateDelta.ContainsKey(e)){
					toSet += ContaminateDelta[e] ;
					if (toSet < 0){
						toSet = 0;
					}
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
			foreach(Listener<ScoreType, int> l in Listeners){
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

	public void TreatWith(Element e, float startingValue=0){
		if (Statuses.ContainsKey (e) && Statuses [e] <= startingValue) {
			return ; //no need to treat with it if already has this element
		}

		if (StrikeDamage.ContainsKey(e)) {
			if (StrikeDamage[e] > 0 && Health > 0){
				List<Element> bss = AfterHit.Keys.ToList();
				foreach (Element e2 in bss) {
					CataclysmTo(e2, this, Side.Center);
				}
			}
		}
		AddStatus(e, startingValue);
		if (StrikeDamage.ContainsKey(e)){
			Health -= StrikeDamage [e];
		}
	}


	internal void CreateFromTemplate(BuildingType bt) {

		switch (bt) {
			case BuildingType.ElectricTower:
				EffectDamage.Add (Element.Fire, 0.2f);
				EffectDamage.Add (Element.Water, 0.1f);
				EffectTime.Add (Element.Fire, 3f);
				AfterHit.Add (Element.Electricity, Side.Center);
				ImageNumberFromAtlas = 41;
				StartingPopulation = _Population = 0;
				break;
			case BuildingType.Wood:
				EffectDamage.Add(Element.Fire, 0.43f);
				EffectDamage.Add(Element.Water, 0.1f);
				EffectTime.Add(Element.Fire, 2.4f);
				ImageNumberFromAtlas = 38;
				StartingPopulation = _Population = 1000;
				break;
			case BuildingType.WaterTower:
				EffectDamage.Add (Element.Fire, 0.2f);
				EffectDamage.Add (Element.Water, 0.1f);
				EffectTime.Add (Element.Fire, 2.4f);
				AfterHit.Add (Element.Water, Side.Center);
				ImageNumberFromAtlas = 40;
				break;
			case BuildingType.GasStation:
				EffectDamage.Add (Element.Fire, 0.6f);
				EffectDamage.Add (Element.Water, 0.1f);
				EffectTime.Add (Element.Fire, 2f);
				AfterHit.Add (Element.Fire, Side.Center);
				ImageNumberFromAtlas = 39;
				break;
			case BuildingType.Stone:
				EffectDamage.Add (Element.Fire, 0.2f);
				EffectDamage.Add (Element.Water, 0.1f);
				EffectTime.Add (Element.Fire, 2.4f);
				ImageNumberFromAtlas = 37;
				StartingPopulation = _Population = 1000;
				break;
			case BuildingType.Destroyed:
				_Health = 0;
				ImageNumberFromAtlas = 37;
				break;
			case BuildingType.Block:
				EffectDamage.Add(Element.Fire, 0);
				EffectDamage.Add(Element.Water, 0);
				ImageNumberFromAtlas = 22;
				break;
		}

		StrikeDamage.Add(Element.Crush, 1f);

		EffectDamage.Add (Element.Crush, 0.00f);
		EffectDamage.Add (Element.Electricity, 0.6f);
		EffectDamage.Add(Element.SmokeAfterFire, 0.5f);

		EffectTime.Add (Element.SmokeAfterFire, 1.5f);
		EffectTime.Add (Element.Crush, 1f);
		EffectTime.Add (Element.Electricity, 1f);
		EffectTime.Add (Element.Water, 5f);

		FillSpeed.Add (Element.Electricity, 0.05f);
		FillSpeed.Add (Element.Water, 0.1f);
		FillSpeed.Add (Element.Fire, 2.5f);

		FillRequirement.Add(Element.Electricity, delegate(Building b){
			return b.Statuses.ContainsKey(Element.Water) && Statuses.ContainsKey(Element.Water);
		});

		FillRequirement.Add(Element.Water, delegate(Building b){
			return 
				(
					(
						(	 //side up check if is in water
							GetNeighbour(b, Side.Up) != null &&
							GetNeighbour(b, Side.Up).Statuses.ContainsKey(Element.Water) &&
							GetNeighbour(b, Side.Up).Statuses[Element.Water] > 0 &&
							GetNeighbour(b, Side.Up).Health == 0
						)
						||
						(	 //side down check if is in water
							GetNeighbour(b, Side.Down) != null &&
							GetNeighbour(b, Side.Down).Statuses.ContainsKey(Element.Water) &&
							GetNeighbour(b, Side.Down).Statuses[Element.Water] > 0 &&
							GetNeighbour(b, Side.Down).Health == 0
						)
						||
						(	 //side left check if is in water
							GetNeighbour(b, Side.Left) != null &&
							GetNeighbour(b, Side.Left).Statuses.ContainsKey(Element.Water) &&
							GetNeighbour(b, Side.Left).Statuses[Element.Water] > 0  &&
							GetNeighbour(b, Side.Left).Health == 0
						)
						||
						(	 //side right check if is in water
							GetNeighbour(b, Side.Right) != null &&
							GetNeighbour(b, Side.Right).Statuses.ContainsKey(Element.Water) &&
							GetNeighbour(b, Side.Right).Statuses[Element.Water] > 0 &&
							GetNeighbour(b, Side.Right).Health == 0
						)
					)
				)
				&&
				(
					!b.Statuses.ContainsKey(Element.Water) 
					|| 
					b.Statuses[Element.Water] >  Statuses[Element.Water]
				);
		});

		FillRequirement.Add (Element.Fire, delegate(Building b) {
			return b.Health > 0;
		});

		ContaminateDelta.Add (Element.Fire, -1f);

	}

	public void Clicked(BaseEventData b) {
		foreach (Listener<ScoreType, int> l in Listeners) {
			l.Inform(ScoreType.Interventions, 1);
		}
		TreatWith(Element.Crush);
	}
}
