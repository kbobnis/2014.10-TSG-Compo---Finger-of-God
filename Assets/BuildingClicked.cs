using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BuildingClicked : MonoBehaviour {

	// Use this for initialization
	void Start () {

		try{
		//gameObject.GetComponent<Button> ().onClick.AddListener (ButtonClickedEvent);

		EventTrigger et = gameObject.GetComponent<EventTrigger> ();

		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerDown;
		entry.callback = new EventTrigger.TriggerEvent();
		UnityEngine.Events.UnityAction<BaseEventData> callback = 
			new UnityEngine.Events.UnityAction<BaseEventData> (EnterEventMethod);
		entry.callback.AddListener (callback);
		et.delegates.Add (entry);
		} catch(System.Exception e){
			Debug.Log("exception: " + e);
		}
	}

	public void EnterEventMethod(UnityEngine.EventSystems.BaseEventData baseEvent) {
		UnityEngine.EventSystems.PointerEventData p = baseEvent as PointerEventData;
		ButtonClickedEvent ();
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void ButtonClickedEvent(){
		Game.Me.ButtonTouched (gameObject);
	}

}
