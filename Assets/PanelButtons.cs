using UnityEngine;
using System.Collections;

public class PanelButtons : MonoBehaviour {

	public GameObject ButtonTop, ButtonTopText, ButtonBottom, ButtonBottomText ;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	internal GameObject CopyMeIn(GameObject parent) {
		GameObject go = Instantiate(gameObject) as GameObject;
		go.transform.parent = parent.transform;
		//go.GetComponent<RectTransform>().position = new Vector3(Screen.width / 2, Screen.height * 1 / 5, 0);
		go.GetComponent<RectTransform>().offsetMin = new Vector2();
		go.GetComponent<RectTransform>().offsetMax = new Vector2();
		return go;
	}
}
