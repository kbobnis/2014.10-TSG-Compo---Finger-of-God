using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public enum Element{
	Crush, SmallCrush, Water, Electricity, Fire, SmokeAfterFire
}

public class Building : MonoBehaviour{

	private Dictionary<Element, float> StrikeDamage = new Dictionary<Element, float>();
	private Dictionary<Element, float> EffectDamage = new Dictionary<Element, float>();
	private Dictionary<Element, float> EffectTime = new Dictionary<Element, float>();
	private Dictionary<Element, Side> AfterHit = new Dictionary<Element, Side> ();
	private Dictionary<Element, float> Statuses = new Dictionary<Element, float>();

	public List<Listener> Listeners = new List<Listener>();

	private int StartingPopulation, _Population, LastCheckedPopulation;
	private float _Health = 1f;
	private int ImageNumberFromAtlas;
	private const float WaterFillSpeed = 0.3f;
	private float LastWaterFill;

	public GameObject GameObjectGroundLevel, GameObjectBuilding;
	public GameObject GameObjectFire, GameObjectWaterLevel, GameObjectSmokeAfterFire;

	private Dictionary<Element, GameObject> ElGO = new Dictionary<Element, GameObject> ();
	private Vector3 startingPos;

	void Start(){
		startingPos = GameObjectBuilding.GetComponent<RectTransform>().localPosition;

		ElGO.Add (Element.Fire, GameObjectFire);
		ElGO.Add (Element.Water, GameObjectWaterLevel);
		ElGO.Add (Element.SmokeAfterFire, GameObjectSmokeAfterFire);

	}

	public int PopulationDelta{
		get {
			int delta = _Population - LastCheckedPopulation;
			LastCheckedPopulation = _Population;
			return delta;
		}
	}

	public float Health{
		get { return _Health; }
		set { 
			bool wasDead = _Health <= 0;
			_Health = value; 
			if (_Health < 0){
				_Health = 0;
			}
			if (!wasDead && _Health == 0){
				Die();
			}
		}
	}

	private void AddStatus(Element e, float startingValue=0){
		if (!Statuses.ContainsKey(e)){
			Statuses.Add(e, startingValue);
			if (e == Element.Water){
				LastWaterFill = Time.time;
			}
			if (SoundManager.Clips.ContainsKey (e)) {
				PlaySingleSound.SpawnSound(SoundManager.Clips[e]);
			} else {
				Debug.Log("There is no sound for " + e);
			}

		} else if (Statuses[e] > startingValue) {
			Statuses[e] = startingValue;
			if (e == Element.Water){
				LastWaterFill = Time.time;
			}
			if (SoundManager.Clips.ContainsKey (e)) {
				PlaySingleSound.SpawnSound(SoundManager.Clips[e]);
			} else {
				Debug.Log("There is no sound for " + e);
			}

		}

	}

	private void Die(){
		PlaySingleSound.SpawnSound(SoundManager.BuildingDown);
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

		if (Health <= 0) {
			UpdateImage();
		}

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
						progress = 1;
					}
					if (SpriteManager.ElementSprites.ContainsKey(status) && ElGO.ContainsKey(status)){
						Image i = ElGO[status].GetComponent<Image>();
						i.enabled = true;
						Sprite[] ss = SpriteManager.ElementSprites[status];
						i.sprite = ss[(int)(progress * ss.Length -1)];
					}
				}
			}
		}


		Sprite groundLevel = SpriteManager.GroundLevels [0];
		float posY = 0;
		if (Statuses.ContainsKey(Element.Crush)) {
			groundLevel = SpriteManager.GroundLevels[2];
			posY = -15f;
		} else if(!Statuses.ContainsKey(Element.Crush) && Statuses.ContainsKey(Element.SmallCrush)){
			groundLevel = SpriteManager.GroundLevels[1];
			posY = -7.5f;
		}
		GameObjectBuilding.GetComponent<RectTransform>().localPosition = new Vector3(startingPos.x, startingPos.y + posY, 0);
		GameObjectGroundLevel.GetComponent<Image>().sprite = groundLevel;


		string text = "";
		bss = Statuses.Keys.ToList();
		foreach (Element status in bss) {
			if (Statuses[status] >= 1) {
				FinishStatus(status);
			}
		}
		GetComponentInChildren<Text> ().enabled = false;

		Inform ();

		//if filling with water, check the ground level, only on crater water fills

		if (Statuses.ContainsKey(Element.Water) && Time.time - LastWaterFill > WaterFillSpeed)  {
			LastWaterFill = Time.time;
			Game.Me.TreatNeighboursWith(gameObject, Element.Water, Statuses[Element.Water], delegate(GameObject go){
				Building b = go.GetComponent<Building>();
				return 
					(
						b.Statuses.ContainsKey(Element.Crush) 
						|| 
						b.Statuses.ContainsKey(Element.SmallCrush)
					) 
					&&
					(
						!b.Statuses.ContainsKey(Element.Water) 
						|| 
						b.Statuses[Element.Water] >  Statuses[Element.Water]
					);
			});
		}

	}

	public void Inform(){
		_Population = (int)(Health * StartingPopulation);
		int d = PopulationDelta;
		if (d != 0) {
			foreach(Listener l in Listeners){
				l.Inform((object)d);
			}
		}
	}

	private void Clean(){
		StrikeDamage.Clear ();
		EffectDamage.Clear ();
		EffectTime.Clear ();
		Health = 1f;
		Statuses.Clear ();
		_Population = 0;
	}

	private void AddConstants(){ 

		EffectDamage.Add (Element.Crush, 0.00f);
		EffectDamage.Add (Element.SmallCrush, 0.00f);

		EffectTime.Add (Element.SmokeAfterFire, 2f);

		EffectTime.Add (Element.Crush, 25f);
		EffectTime.Add (Element.SmallCrush, 15f);
	}


	public void CreateWood1(){
		Clean ();
		StrikeDamage.Add (Element.Crush, 0.1f);
		StrikeDamage.Add (Element.SmallCrush, 0.06f);

		EffectDamage.Add (Element.Fire, 0.1f);
		EffectDamage.Add (Element.Water, 0.01f);
		EffectDamage.Add (Element.Electricity, 0.1f);

		EffectTime.Add (Element.Fire, 5f);
		EffectTime.Add (Element.Water, 5f);
		EffectTime.Add (Element.Electricity, 1f);

		AddConstants ();
		ImageNumberFromAtlas = 1;
		UpdateImage ();
		StartingPopulation = _Population = 1000;
	}

	private void UpdateImage(){
		Sprite s = SpriteManager.BuildingSprites [ImageNumberFromAtlas];
		if (Health <= 0){
			s = SpriteManager.BuildingSpritesDestroyed[ImageNumberFromAtlas];
		}
		GameObjectBuilding.GetComponent<Image> ().sprite = s;
	}

	public void CreateStone1(){
		Clean ();
		StrikeDamage.Add (Element.Crush, 0.1f);
		StrikeDamage.Add (Element.SmallCrush, 0.06f);

		EffectDamage.Add (Element.Fire, 0.1f);
		EffectDamage.Add (Element.Electricity, 0.1f);
		EffectDamage.Add (Element.Water, 0.1f);

		EffectTime.Add (Element.Fire, 2f);
		EffectTime.Add (Element.Electricity, 1f);
		EffectTime.Add (Element.Water, 5f);

		AddConstants ();
		ImageNumberFromAtlas = 2;
		UpdateImage ();
		StartingPopulation = _Population = 2000;
	}

	public void CreateGasStation(){
		Clean ();
		StrikeDamage.Add (Element.Crush, 0.6f);
		StrikeDamage.Add (Element.SmallCrush, 0.06f);

		EffectDamage.Add (Element.Fire, 0.1f);
		EffectDamage.Add (Element.Electricity, 0.1f);
		EffectDamage.Add (Element.Water, 0.05f);

		EffectTime.Add (Element.Fire, 2f);
		EffectTime.Add (Element.Electricity, 1f);
		EffectTime.Add (Element.Water, 5f);

		AfterHit.Add (Element.Fire, Side.Center);

		AddConstants ();
		ImageNumberFromAtlas = 39;
		UpdateImage ();
		StartingPopulation = _Population = 4000;
	}

	public void CreateWaterSilo(){
		Clean ();
		StrikeDamage.Add (Element.Crush, 0.6f);
		StrikeDamage.Add (Element.SmallCrush, 0.06f);
		
		EffectDamage.Add (Element.Fire, 0.1f);
		EffectDamage.Add (Element.Electricity, 0.1f);
		EffectDamage.Add (Element.Water, 0.05f);

		EffectTime.Add (Element.Fire, 2f);
		EffectTime.Add (Element.Electricity, 1f);
		EffectTime.Add (Element.Water, 5f);

		AfterHit.Add (Element.Water, Side.Center);

		AddConstants ();
		ImageNumberFromAtlas = 40;
		UpdateImage ();
		StartingPopulation = _Population = 4000;
	}

	public void TreatWith(Element e, float startingValue=0){
		if (Statuses.ContainsKey (e) && Statuses [e] <= startingValue) {
			return ; //no need to treat with it
		}

		if (StrikeDamage.ContainsKey(e)) {

			if (StrikeDamage[e] > 0 && Health > 0){
				List<Element> bss = AfterHit.Keys.ToList();
				foreach (Element e2 in bss) {
					Game.Me.CataclysmTo(e2, gameObject, Side.Center);
					//splash damage with crush
					if (e == Element.Crush){
						Game.Me.TreatNeighboursWith(gameObject, e2);
					}
				}
			}
		}
		if (EffectDamage.ContainsKey(e) && (!Statuses.ContainsKey(e) || Statuses[e] > startingValue)){
			if ( e == Element.Fire && Health == 0){
				//dont start fire on dead buildings
			} else {
				AddStatus(e, startingValue);
			}
		}
		if (StrikeDamage.ContainsKey(e)){
			Health -= StrikeDamage [e];
		}

	}

}
