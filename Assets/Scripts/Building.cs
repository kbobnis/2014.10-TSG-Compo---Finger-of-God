using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public enum Element{
	Crush, SmallCrush, Water, Electricity, Fire
}

public static class ElementMethods{

	public static AudioClip GetSound(this Element s){
		switch (s) {
			case Element.Crush: return SoundManager.Crush; break;
			//case Element.SmallCrush: return SoundManager.Huracaine; break;
			case Element.Fire: return SoundManager.Fire; break;
			default: Debug.Log("There is no sound for " + s); return null;
		}
	}
}

public class Building : MonoBehaviour{

	private Dictionary<Element, float> StrikeDamage = new Dictionary<Element, float>();
	private Dictionary<Element, float> EffectDamage = new Dictionary<Element, float>();
	private Dictionary<Element, float> EffectTime = new Dictionary<Element, float>();
	private Dictionary<Element, float> StatusesProgress = new Dictionary<Element, float>();
	private Dictionary<Element, Side> AfterHit = new Dictionary<Element, Side> ();

	public List<Listener> Listeners = new List<Listener>();

	private int StartingPopulation, _Population, LastCheckedPopulation;
	private float _Health = 1f;
	private int ImageNumberFromAtlas;

	public GameObject GameObjectExplosion, GameObjectFire, GameObjectGroundLevel, GameObjectWaterLevel, GameObjectBuilding;
	private Vector3 startingPos;

	void Start(){
		startingPos = GameObjectBuilding.GetComponent<RectTransform>().localPosition;
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

	private void Die(){
	}

	void Update(){

		if (Health <= 0) {
			UpdateImage();
		}

		bool isSmallCrush=false, isBigCrush=false;
		List<Element> bss = StatusesProgress.Keys.ToList();
		foreach (Element status in bss) {
			float value = StatusesProgress[status];
			if (value < 1){
				Health -= EffectDamage[status] * Time.deltaTime;
				if (Health < 0){
					Health = 0;
				}
				float progress = StatusesProgress[status] += 1 / EffectTime[status] * Time.deltaTime;
				if (progress > 1){
					progress = 1;
				}

				if (status == Element.Crush){
					//Image i = GameObjectExplosion.GetComponent<Image>();
					//i.enabled = true;
					//i.sprite = SpriteManager.Explosions[(int)(progress * 14)]; // 0 - 14
				}
				if (status == Element.Fire){
					Image i = GameObjectFire.GetComponent<Image>();
					i.enabled = true;
					i.sprite = SpriteManager.Fires[(int)(progress * SpriteManager.Fires.Length -1)];
				}
				if (status == Element.Crush){
					isBigCrush = true;
				}
				if (status == Element.SmallCrush){
					isSmallCrush = true;
				}
			}
		}

		Sprite groundLevel = SpriteManager.GroundLevels [0];
		float posY = 0;
		if (isBigCrush) {
			groundLevel = SpriteManager.GroundLevels[2];
			posY = -15f;
		} else if(!isBigCrush && isSmallCrush){
			groundLevel = SpriteManager.GroundLevels[1];
			posY = -7.5f;
		}
		GameObjectBuilding.GetComponent<RectTransform>().localPosition = new Vector3(startingPos.x, startingPos.y + posY, 0);
		GameObjectGroundLevel.GetComponent<Image>().sprite = groundLevel;


		string text = "";
		bss = StatusesProgress.Keys.ToList();
		foreach (Element status in bss) {
			float value = StatusesProgress[status];
			if (value >= 1) {
				StatusesProgress.Remove(status);
				if (status == Element.Crush){
					GameObjectExplosion.GetComponent<Image>().enabled = false;
				}
				if (status == Element.Fire){
					GameObjectFire.GetComponent<Image>().enabled = false;
				}
			} else {
				text += status.ToString()+", " + value+". ";
			}
		}
		text = "health: " + Health +". "+ text;
		GetComponentInChildren<Text> ().text = text;

		Inform ();

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
		StatusesProgress.Clear ();
		_Population = 0;
	}


	public void CreateWood1(){
		Clean ();
		StrikeDamage.Add (Element.Crush, 0.1f);
		StrikeDamage.Add (Element.SmallCrush, 0.06f);

		EffectDamage.Add (Element.Fire, 0.1f);
		EffectDamage.Add (Element.Electricity, 0.1f);
		EffectDamage.Add (Element.Water, 0.01f);
		EffectDamage.Add (Element.Crush, 0.00f);
		EffectDamage.Add (Element.SmallCrush, 0.00f);

		EffectTime.Add (Element.Fire, 5f);
		EffectTime.Add (Element.Electricity, 1f);
		EffectTime.Add (Element.Water, 10f);
		EffectTime.Add (Element.Crush, 25f);
		EffectTime.Add (Element.SmallCrush, 15f);

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
		EffectDamage.Add (Element.Water, 0.05f);
		EffectDamage.Add (Element.Crush, 0.00f);
		EffectDamage.Add (Element.SmallCrush, 0.00f);
		
		EffectTime.Add (Element.Fire, 2f);
		EffectTime.Add (Element.Electricity, 1f);
		EffectTime.Add (Element.Water, 20f);
		EffectTime.Add (Element.Crush, 25f);
		EffectTime.Add (Element.SmallCrush, 15f);
		
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
		EffectDamage.Add (Element.Crush, 0.1f);
		EffectDamage.Add (Element.SmallCrush, 0.00f);

		EffectTime.Add (Element.Fire, 2f);
		EffectTime.Add (Element.Electricity, 1f);
		EffectTime.Add (Element.Water, 20f);
		EffectTime.Add (Element.Crush, 25f);
		EffectTime.Add (Element.SmallCrush, 15f);

		AfterHit.Add (Element.Fire, Side.Center);
		
		ImageNumberFromAtlas = 39;
		UpdateImage ();
		StartingPopulation = _Population = 4000;
	}

	public void TreatWith(Element e){

		PlaySingleSound.SpawnSound (e.GetSound (), gameObject.transform.position);
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
		if (EffectDamage.ContainsKey(e) && !StatusesProgress.ContainsKey(e)){
			if ( e == Element.Fire && Health == 0){
				//dont fire up dead buildings

			} else {
				StatusesProgress.Add(e, 0);
			}
		}
		if (StrikeDamage.ContainsKey(e)){
			Health -= StrikeDamage [e];
		}

	}

}
