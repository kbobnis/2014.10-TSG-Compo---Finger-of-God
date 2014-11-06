using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BackgroundHolder : MonoBehaviour {

	private Sprite ActualBg;
	private float ActualScale;

	internal void ScaleChanged(float scale) {
		ActualScale = scale;
		UpdateMe();
	}

	internal void ChangeBg(Sprite newBg) {
		ActualBg = newBg;
		UpdateMe();
	}

	private void UpdateMe() {
		GetComponent<Image>().sprite = ActualBg;
		RectTransform rt = GetComponent<RectTransform>();
		rt.offsetMin = new Vector2(-ActualBg.rect.width / 2 * ActualScale, -ActualBg.rect.height / 2 * ActualScale);
		rt.offsetMax = new Vector2(ActualBg.rect.width / 2 * ActualScale, ActualBg.rect.height / 2 * ActualScale);
	}
}
