using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public enum Element{
	Crush, SmallCrush, Water, Electricity, Fire, Explosion
}

public static class ElementMethods{

	public static AudioClip GetSound(this Element s){
		switch (s) {
			case Element.Crush: return SoundManager.Crush; break;
			case Element.SmallCrush: return SoundManager.Huracaine; break;
			default: Debug.Log("There is no sound for " + s); return null;
		}
	}
}

public class Building : MonoBehaviour{

	private Dictionary<Element, float> StrikeDamage = new Dictionary<Element, float>();
	private Dictionary<Element, float> EffectDamage = new Dictionary<Element, float>();
	private Dictionary<Element, float> EffectTime = new Dictionary<Element, float>();
	private Dictionary<Element, float> StatusesProgress = new Dictionary<Element, float>();

	public List<Listener> Listeners = new List<Listener>();

	private int StartingPopulation, _Population, LastCheckedPopulation;
	private float Health = 1f;
	private int ImageNumberFromAtlas;

	public GameObject GameObjectExplosion, GameObjectFire;

	public int PopulationDelta{
		get {
			int delta = _Population - LastCheckedPopulation;
			LastCheckedPopulation = _Population;
			return delta;
		}
	}

	void Update(){

		if (Health <= 0) {
			UpdateImage();
		}

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

				if (status == Element.Explosion){
					Image i = GameObjectExplosion.GetComponent<Image>();
					i.enabled = true;
					i.sprite = SpriteManager.Explosions[(int)(progress * 14)]; // 0 - 14
				}
				if (status == Element.Fire){
					Image i = GameObjectFire.GetComponent<Image>();
					i.enabled = true;
					i.sprite = SpriteManager.Fires[(int)(progress * SpriteManager.Fires.Length -1)];
				}
			}
		}

		string text = "";
		bss = StatusesProgress.Keys.ToList();
		foreach (Element status in bss) {
			float value = StatusesProgress[status];
			if (value >= 1) {
				StatusesProgress.Remove(status);
				if (status == Element.Explosion){
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
		StrikeDamage.Add (Element.Explosion, 0.2f);

		EffectDamage.Add (Element.Fire, 0.1f);
		EffectDamage.Add (Element.Electricity, 0.1f);
		EffectDamage.Add (Element.Water, 0.01f);
		
		EffectTime.Add (Element.Fire, 5f);
		EffectTime.Add (Element.Electricity, 1f);
		EffectTime.Add (Element.Water, 10f);

		ImageNumberFromAtlas = 1;
		UpdateImage ();
		StartingPopulation = _Population = 1000;
	}

	private void UpdateImage(){
		Sprite s = SpriteManager.BuildingSprites [ImageNumberFromAtlas];
		if (Health <= 0){
			s = SpriteManager.BuildingSpritesDestroyed[ImageNumberFromAtlas];
		}

		GetComponent<Image> ().sprite = s;
	}

	public void CreateStone1(){
		Clean ();
		StrikeDamage.Add (Element.Crush, 0.1f);
		StrikeDamage.Add (Element.SmallCrush, 0.06f);
		StrikeDamage.Add (Element.Explosion, 0.2f);
		
		EffectDamage.Add (Element.Fire, 0.1f);
		EffectDamage.Add (Element.Electricity, 0.1f);
		EffectDamage.Add (Element.Water, 0.05f);
		
		EffectTime.Add (Element.Fire, 2f);
		EffectTime.Add (Element.Electricity, 1f);
		EffectTime.Add (Element.Water, 20f);
		
		ImageNumberFromAtlas = 2;
		UpdateImage ();
		StartingPopulation = _Population = 2000;
	}

	public void CreateTavern(){
		Clean ();
		StrikeDamage.Add (Element.Crush, 0.1f);
		StrikeDamage.Add (Element.SmallCrush, 0.06f);
		StrikeDamage.Add (Element.Explosion, 0.2f);
		
		EffectDamage.Add (Element.Fire, 0.1f);
		EffectDamage.Add (Element.Electricity, 0.1f);
		EffectDamage.Add (Element.Water, 0.05f);
		
		EffectTime.Add (Element.Fire, 2f);
		EffectTime.Add (Element.Electricity, 1f);
		EffectTime.Add (Element.Water, 20f);
		
		ImageNumberFromAtlas = 3;
		UpdateImage ();
		StartingPopulation = _Population = 4000;
	}

	public void TreatWith(Element e){
		if (Health > 0){
			PlaySingleSound.SpawnSound (e.GetSound (), gameObject.transform.position);
			if (StrikeDamage.ContainsKey(e)) {

				Health -= StrikeDamage [e];
				if (EffectDamage.ContainsKey(e) && !StatusesProgress.ContainsKey(e)){
					StatusesProgress.Add(e, 0);
				}
			}
		}
	}

}
