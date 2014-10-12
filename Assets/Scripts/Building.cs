using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class Building : MonoBehaviour{

	private Dictionary<Cataclysm, float> StrikeDamage = new Dictionary<Cataclysm, float>();
	private Dictionary<Cataclysm, float> EffectDamage = new Dictionary<Cataclysm, float>();
	private Dictionary<Cataclysm, float> EffectTime = new Dictionary<Cataclysm, float>();
	private Dictionary<Cataclysm, float> StatusesProgress = new Dictionary<Cataclysm, float>();

	public List<Listener> Listeners = new List<Listener>();

	private int StartingPopulation, _Population, LastCheckedPopulation;
	private float Health = 1f;
	private int ImageNumberFromAtlas;

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
			GetComponentInChildren<Text>().gameObject.SetActive(false);
			return;
		}


		List<Cataclysm> bss = StatusesProgress.Keys.ToList();
		foreach (Cataclysm status in bss) {
			float value = StatusesProgress[status];
			if (value < 1){
				Health -= EffectDamage[status] * Time.deltaTime;
				if (Health < 0){
					Health = 0;
				}
				StatusesProgress[status] += 1 / EffectTime[status] * Time.deltaTime;
			}
		}

		string text = "";
		bss = StatusesProgress.Keys.ToList();
		foreach (Cataclysm status in bss) {
			float value = StatusesProgress[status];
			if (value >= 1) {
				StatusesProgress.Remove(status);
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
		StrikeDamage.Add (Cataclysm.Crush, 0.1f);
		StrikeDamage.Add (Cataclysm.Fire, 0.6f);
		StrikeDamage.Add (Cataclysm.Strike, 0.6f);
		StrikeDamage.Add (Cataclysm.Whirlwind, 0.6f);
		
		EffectDamage.Add (Cataclysm.Crush, 2f);
		EffectDamage.Add (Cataclysm.Fire, 0.1f);
		EffectDamage.Add (Cataclysm.Strike, 0.1f);
		EffectDamage.Add (Cataclysm.Whirlwind, 0.1f);
		
		EffectTime.Add (Cataclysm.Crush, 0.5f);
		EffectTime.Add (Cataclysm.Fire, 1f);
		EffectTime.Add (Cataclysm.Strike, 1f);
		EffectTime.Add (Cataclysm.Whirlwind, 1f);
		
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
		StrikeDamage.Add (Cataclysm.Crush, 0.6f);
		StrikeDamage.Add (Cataclysm.Fire, 0.3f);
		StrikeDamage.Add (Cataclysm.Strike, 0.6f);
		StrikeDamage.Add (Cataclysm.Whirlwind, 0.6f);
		
		EffectDamage.Add (Cataclysm.Crush, 0.1f);
		EffectDamage.Add (Cataclysm.Fire, 0.05f);
		EffectDamage.Add (Cataclysm.Strike, 0.1f);
		EffectDamage.Add (Cataclysm.Whirlwind, 0.1f);
		
		EffectTime.Add (Cataclysm.Crush, 0.5f);
		EffectTime.Add (Cataclysm.Fire, 1f);
		EffectTime.Add (Cataclysm.Strike, 0.1f);
		EffectTime.Add (Cataclysm.Whirlwind, 1f);
		
		ImageNumberFromAtlas = 2;
		UpdateImage ();
		StartingPopulation = _Population = 2000;
	}

	public void CreateTavern(){
		Clean ();
		StrikeDamage.Add (Cataclysm.Crush, 0.1f);
		StrikeDamage.Add (Cataclysm.Fire, 0.1f);
		StrikeDamage.Add (Cataclysm.Strike, 0.1f);
		StrikeDamage.Add (Cataclysm.Whirlwind, 0.1f);
		
		EffectDamage.Add (Cataclysm.Crush, 0.1f);
		EffectDamage.Add (Cataclysm.Fire, 0.3f);
		EffectDamage.Add (Cataclysm.Strike, 0.1f);
		EffectDamage.Add (Cataclysm.Whirlwind, 0.1f);
		
		EffectTime.Add (Cataclysm.Crush, 0.2f);
		EffectTime.Add (Cataclysm.Fire, 0.1f);
		EffectTime.Add (Cataclysm.Strike, 0.1f);
		EffectTime.Add (Cataclysm.Whirlwind, 0.1f);
		
		ImageNumberFromAtlas = 3;
		UpdateImage ();
		StartingPopulation = _Population = 4000;
	}

	public void TreatWith(Cataclysm cataclysm){
		if (Health > 0){
			PlaySingleSound.SpawnSound (cataclysm.GetSound (), gameObject.transform.position);
			if (!StrikeDamage.ContainsKey(cataclysm)) {
				throw new UnityException("THere is no " + cataclysm + ", in strike damage ");
			}
			Health -= StrikeDamage [cataclysm];
			if (!StatusesProgress.ContainsKey(cataclysm)){
				StatusesProgress.Add(cataclysm, 0);
			}
		}
	}

}
