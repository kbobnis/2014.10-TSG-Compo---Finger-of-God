using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BuildingClicked : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//gameObject.GetComponent<Button> ().onClick.AddListener (ButtonClickedEvent);

		EventTrigger et = gameObject.GetComponent<EventTrigger> ();

		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerEnter;
		entry.callback = new EventTrigger.TriggerEvent();
		UnityEngine.Events.UnityAction<BaseEventData> callback = 
			new UnityEngine.Events.UnityAction<BaseEventData> (EnterEventMethod);
		entry.callback.AddListener (callback);
		et.delegates.Add (entry);
	}

	public void EnterEventMethod(UnityEngine.EventSystems.BaseEventData baseEvent) {
		Debug.Log(baseEvent.selectedObject.name + " triggered an event!");
		ButtonClickedEvent ();
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void ButtonClickedEvent(){
		//GameObject.Find ("Game").GetComponent<Game> ().ButtonClicked (gameObject);
	}

}
