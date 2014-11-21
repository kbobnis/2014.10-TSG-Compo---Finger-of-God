using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class BuildingStatus {

	private float _Status;
	private GameObject GameObject;
	private Sprite[] Animation;
	private Sprite[] AnimationWhenDead;
	private AudioClip Sound;
	private float EffectDamage;
	private float EffectTime;
	private float LastFill = 0;
	private float StrikeDamage;
	private float StrikeDamageWaiting;
	private float FillSpeed;
	private bool ResizeFromDown;

	private bool _IsSource;

	public float Value {
		get { return Status; }
	}

	public BuildingStatus(GameObject gameObject, Sprite[] animation, AudioClip sound, float effectDamage, float effectTime, float strikeDamage, float fillSpeed, Sprite[] animationWhenDead, bool resizeFromDown)
    {
		GameObject = gameObject;
		Animation = animation;
		Sound = sound;
		EffectDamage = effectDamage;
		EffectTime = effectTime;
		StrikeDamage = strikeDamage;
		FillSpeed = fillSpeed;
		AnimationWhenDead = animationWhenDead;
		ResizeFromDown = resizeFromDown;
    }

	private float Status {
		set { 
			if (value <= 0) {
				_IsSource = false;
			}
			_Status = value;
		}
		get { return _Status;  }
	}

	internal float Add(float startingValue, bool triggerPower) {

		PlaySingleSound.SpawnSound(Sound);
	
		if (startingValue > Status){
			Status  = startingValue;
			FilledNow(); //so it will fill neighbours in next turn
		}
		StrikeDamageWaiting += StrikeDamage;
		_IsSource = triggerPower;
		return StrikeDamage;
	}

	internal bool IsSource() {
		return _IsSource;
	}

	internal float UpdateAndGetDamage(bool isAlive) {

		if (EffectTime > 0) {
			Status += -1 / EffectTime * Time.deltaTime;
		} else {
			Status = 0;
		}
		if (Status < 0) {
			Status = 0;

		}
		
		GameObject.SetActive( Status > 0);
		if (Status > 0) {

			float progress = 1 - Status;

			Image im = GameObject.GetComponent<Image>();
			Sprite[] ss = Animation;
			if (isAlive == false) {
				ss = AnimationWhenDead;
			}
			Sprite s = im.sprite = ss[Mathf.RoundToInt(progress * (ss.Length - 1))];
			if (ResizeFromDown) {
				Rect before = GameObject.GetComponent<RectTransform>().rect;
				float scale = before.width / s.rect.width;
				float targetHeight = scale * s.rect.height;
				Vector2 oldOffsetMax = GameObject.GetComponent<RectTransform>().offsetMax;
				GameObject.GetComponent<RectTransform>().offsetMax = new Vector2(oldOffsetMax.x, targetHeight);
			}
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

	internal void FilledNow() {
		LastFill = Time.time;
	}




}
