using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PanelMinigame : MonoBehaviour, Listener<ScoreType, float> {

	public GameObject PanelAfterMission, PanelMissionFailed, TextEndMission, CanvasEndMission;

	private Mission Mission;
	private Dictionary<ScoreType, Result> ActualResults = new Dictionary<ScoreType, Result>();
	private List<Listener<ScoreType, float>> ScoreTypeListeners;

	public void PrepareGame(Mission m, List<Listener<ScoreType, float>> scoreTypeListeners) {
		CanvasEndMission.SetActive(false);

		Game.Me.GetComponent<GoogleAnalyticsV3>().LogScreen(m.MissionType.ToString());

		scoreTypeListeners.Add(this);
		ScoreTypeListeners = scoreTypeListeners;
		foreach (Listener<ScoreType, float> l in ScoreTypeListeners) {
			l.Clear(ScoreType.Interventions);
			l.Clear(ScoreType.Time);
		}

		ActualResults.Clear();
		ActualResults.Add(ScoreType.Interventions, new Result(ScoreType.Interventions, 0));
		ActualResults.Add(ScoreType.Population, new Result(ScoreType.Population, 0)); //buildings will inform about population change
		ActualResults.Add(ScoreType.Time, new Result(ScoreType.Time, 0));

		GetComponentInChildren<ScrollableList>().Build(m.Buildings, scoreTypeListeners);
		
		Mission = m;
	}

	public void Clear(ScoreType st) {
		if (st == ScoreType.Population) {
			ActualResults[st].Value = 0;
		}
	}

	void Update() {
		if (ActualResults.ContainsKey(ScoreType.Time)) {
			ActualResults[ScoreType.Time].Value += Time.deltaTime;
		}
	}

	private IEnumerator WaitingForResultsCoroutine(WWW www, Mission Mission) {

		yield return www;
		MissionStatus ms = Mission.GetStatus(ActualResults);
		if (ms == MissionStatus.Success) {
			PanelAfterMission.SetActive(true);
			PanelAfterMission.GetComponent<PanelAfterMission>().Prepare(Mission, ActualResults, www);
		} else {
			PanelMissionFailed.SetActive(true);
			PanelMissionFailed.GetComponent<Image>().enabled = false;
			PanelMissionFailed.GetComponent<PanelMissionFailed>().Prepare(Mission, ActualResults);
		}
		yield return new WaitForSeconds(3);
		//gameObject.SetActive(false);
	}

	public void Inform(ScoreType st, float delta) {
		ActualResults[st].Value += delta;

		if (Mission != null) {
			MissionStatus ms = Mission.GetStatus(ActualResults);

			if (ms == MissionStatus.Success || ms == MissionStatus.Failure) {
				//CanvasEndMission.SetActive(true);
				
				bool isSuccess = Mission.GetStatus(ActualResults) == MissionStatus.Success;
				TextEndMission.GetComponent<Text>().text = "Mission " + (isSuccess ? "completed" : "failed");
				Game.Me.GetComponent<GoogleAnalyticsV3>().LogEvent("Finished " + Mission.MissionType + " " + (isSuccess?"Success":"Failure"), Mission.Name, ActualResults[ScoreType.Interventions].Value.ToString(), (long)(ActualResults[ScoreType.Time].Value * 1000));
				StartCoroutine(WaitingForResultsCoroutine( Mission.SaveGame(ActualResults), Mission));
				Mission = null;
			}
		}
	}



}
