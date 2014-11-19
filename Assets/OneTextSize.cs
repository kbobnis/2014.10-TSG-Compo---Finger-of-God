using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OneTextSize : MonoBehaviour {

	private float LastAspect;

	private int ArbitraryTextSize;
	private int ActualTextSize;

	void Awake() {
		ArbitraryTextSize = GetComponent<Text>().fontSize;
	}

	// Update is called once per frame
	void Update () {
		if (LastAspect != AspectRatioKeeper.ActualScale) {
			LastAspect = AspectRatioKeeper.ActualScale;
			ActualTextSize = (int)( ArbitraryTextSize * LastAspect );
			GetComponent<Text>().fontSize = ActualTextSize;
		}
	}
}
