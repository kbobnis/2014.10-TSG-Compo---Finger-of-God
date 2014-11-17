using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public delegate void JustDo();

public class PanelLoading : MonoBehaviour {

	public GameObject GameObjectTextLoading, GameObjectTextTap;

	public JustDo JustDo;
	public string TextTop {
		set {
			GameObjectTextLoading.GetComponent<Text>().text = value;
		}
	}
	public string TextTap {
		set {
			GameObjectTextTap.GetComponent<Text>().text = value;
			GetComponent<EventTrigger>().enabled = value != "";
		}
	}

	public void Continue() {
		if (JustDo != null) {
			JustDo();
		}
	}
}
