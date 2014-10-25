using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class BuildingStatus {

	private float Status;
	private GameObject GameObject;
	private Sprite[] Animation;
	private AudioClip Sound;
	private float EffectDamage;
	private float EffectTime;
	private float LastFill = 0;
	private float StrikeDamage;

	private float StrikeDamageWaiting;
	private float FillSpeed;


	public float Value {
		get { return Status; }
	}

	public BuildingStatus(GameObject gameObject, Sprite[] animation, AudioClip sound, float effectDamage, float effectTime, float strikeDamage, float fillSpeed)
    {
		GameObject = gameObject;
		Animation = animation;
		Sound = sound;
		EffectDamage = effectDamage;
		EffectTime = effectTime;
		StrikeDamage = strikeDamage;
		FillSpeed = fillSpeed;
    }

	internal void Add(float startingValue=1) {

		PlaySingleSound.SpawnSound(Sound);
	
		if (startingValue > Status){
			Status  = startingValue;
			Fill(); //so it will fill neighbours in next turn
		}
		StrikeDamageWaiting += StrikeDamage;
	}

	internal float UpdateAndGetDamage() {

		if (EffectTime > 0) {
			Status += -1 / EffectTime * Time.deltaTime;
		}
		if (Status < 0) {
			Status = 0;
		}
		
		GameObject.SetActive(Status > 0);
		if (Status > 0) {

			float progress = 1 - Status;

			Image im = GameObject.GetComponent<Image>();
			Sprite[] ss = Animation;
			im.sprite = ss[Mathf.RoundToInt(progress * (ss.Length - 1))];
		}

		float damage = (Status > 0 ?( EffectDamage * Time.deltaTime): 0) + StrikeDamageWaiting;
		StrikeDamageWaiting = 0;
		return damage;

	}

	internal void StopNow() {
		Status = 0;
	}

	public bool CanFill() {
		return Status > 0 && FillSpeed > 0 && Time.time - LastFill > FillSpeed ;
	}

	internal void Fill() {
		LastFill = Time.time;
	}

	
}
