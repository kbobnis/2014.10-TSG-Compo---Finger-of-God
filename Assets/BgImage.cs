using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class BgImage : MonoBehaviour {

	public Sprite Sprite;

	void Awake() {
		if (GetComponent<Image>() != null) {
			throw new Exception("There can not be image and bgimage components added");
		}
		if (GetComponent<CanvasRenderer>() != null) {
			throw new Exception("There is no need for canvasRenderer when bgImage is added");
		}
	}

	void OnEnable() {
		Game.Me.GetComponent<BackgroundHolder>().ChangeBg(Sprite);
	}
}
