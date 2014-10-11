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

	private Sprite Sprite;
	private float Health = 1f;

	void Update(){

		List<Cataclysm> bss = StatusesProgress.Keys.ToList();
		foreach (Cataclysm status in bss) {
			float value = StatusesProgress[status];
			if (value < 1){
				Health -= EffectDamage[status] * Time.deltaTime;
				StatusesProgress[status] += EffectTime[status] * Time.deltaTime;
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


		if (Health <= 0) {
			Destroy(gameObject);
		}
	}

	private void Clean(){
		StrikeDamage.Clear ();
		EffectDamage.Clear ();
		EffectTime.Clear ();
		Health = 1f;
		StatusesProgress.Clear ();
	}


	public void CreateWood1(){
		Clean ();
		StrikeDamage.Add (Cataclysm.Crush, 0.6f);
		StrikeDamage.Add (Cataclysm.Fire, 0.6f);
		StrikeDamage.Add (Cataclysm.Strike, 0.6f);
		StrikeDamage.Add (Cataclysm.Whirlwind, 0.6f);
		
		EffectDamage.Add (Cataclysm.Crush, 0.1f);
		EffectDamage.Add (Cataclysm.Fire, 0.1f);
		EffectDamage.Add (Cataclysm.Strike, 0.1f);
		EffectDamage.Add (Cataclysm.Whirlwind, 0.1f);
		
		EffectTime.Add (Cataclysm.Crush, 0.2f);
		EffectTime.Add (Cataclysm.Fire, 1f);
		EffectTime.Add (Cataclysm.Strike, 0.1f);
		EffectTime.Add (Cataclysm.Whirlwind, 0.1f);
		
		Sprite = SpriteManager.BuildingSprites [1];
		GetComponent<Image> ().sprite = Sprite;
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
		
		EffectTime.Add (Cataclysm.Crush, 0.2f);
		EffectTime.Add (Cataclysm.Fire, 0.3f);
		EffectTime.Add (Cataclysm.Strike, 0.1f);
		EffectTime.Add (Cataclysm.Whirlwind, 0.1f);
		
		Sprite = SpriteManager.BuildingSprites [2];
		GetComponent<Image> ().sprite = Sprite;
	}

	public void TreatWith(Cataclysm cataclysm){
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
