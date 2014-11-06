using UnityEngine;
using System.Collections;

public class AspectRatioKeeper : MonoBehaviour {

	public int DesiredW, DesiredH;
	public float ScaleWhenOffsetBiggerThan;

	private int LastWidth, LastHeight;
	private float Aspect;
	private float _ActualScale;

	public float ActualScale {
		get { return _ActualScale; }
	}


	// Use this for initialization
	void Start () {
		Aspect = DesiredW / (float)DesiredH;
		FixAspect();
	}
	
	// Update is called once per frame
	void Update () {
		FixAspect();
	}

	private void FixAspect() {
		if (Screen.width != LastWidth || Screen.height != LastHeight) {

			int w = LastWidth = Screen.width;
			int h = LastHeight = Screen.height;

			RectTransform rt = GetComponent<RectTransform>();

			float scale = w / (int)DesiredW;
			//height scale 
			if (h / DesiredH < scale) {
				scale = h / (int)DesiredH;
			}

			//if scale is smaller than 1
			if (scale == 0) {
				scale = w / (float)DesiredW;
			}

			//width scale
			float offsetW = w / (float)DesiredW - scale;
			if (offsetW < 0.01) {
				offsetW = 1;
			}
			float offsetH = h / (float)DesiredH - scale;
			if (offsetH < 0.01) {
				offsetH = 1;
			}
			Debug.Log("scale was: " + scale + " offset is: " + offsetW.ToString("#.##") + " offset h: " + offsetH.ToString("#.##"));

			//if the offset would be too big, then scale is not int
			if (false && offsetW + offsetH > ScaleWhenOffsetBiggerThan) {
				scale = w / (float)DesiredW;
				if (DesiredW * scale > w) {
					scale = w / (float)DesiredW;
				}
				if (DesiredH * scale > h && h / (float)DesiredH < scale) {
					scale = h / (float)DesiredH;
				}
			}

			int canvasW = (int)(DesiredW * scale);
			int canvasH = (int)(DesiredH * scale);
			
			_ActualScale = scale;
			Debug.Log("scale is: " + scale + " offset is: " + offsetW.ToString("#.##") + " offset h: " + offsetH.ToString("#.##"));
			rt.offsetMin = new Vector2(-canvasW/2, -canvasH/2);
			rt.offsetMax = new Vector2(canvasW/2, canvasH/2);
			Game.Me.GetComponent<BackgroundHolder>().ScaleChanged(scale);
		}
	}
}
