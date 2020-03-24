using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BackButtonSupport : MonoBehaviour {

	public Button.ButtonClickedEvent BackButtonAction;

	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			BackButtonAction.Invoke();
		}
	}
}
