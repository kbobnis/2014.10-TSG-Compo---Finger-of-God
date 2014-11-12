using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PanelMinigame : MonoBehaviour, Listener<ScoreType, float> {

	public GameObject PanelAfterMission, PanelMissionFailed, PanelTop, ImageEndGame, TextEndGame;

	private Mission Mission;
	private Dictionary<ScoreType, Result> ActualResults = new Dictionary<ScoreType, Result>();
	private List<Listener<ScoreType, float>> ScoreTypeListeners = new List<Listener<ScoreType,float>>();

	public void PrepareGame(Mission m) {

		ImageEndGame.SetActive(false);

		ActualResults.Clear();
		ActualResults.Add(ScoreType.Interventions, new Result(ScoreType.Interventions, 0));
		ActualResults.Add(ScoreType.Population, new Result(ScoreType.Population, 0)); //buildings will inform about population change
		ActualResults.Add(ScoreType.Time, new Result(ScoreType.Time, 0));

		Game.Me.GetComponent<GoogleAnalyticsV3>().LogScreen(m.MissionType.ToString());

		ScoreTypeListeners.Clear();
		ScoreTypeListeners.Add(this);
		this.Clear(ScoreType.Interventions);
		this.Clear(ScoreType.Time);

		NumberShower nsi = PanelTop.GetComponent<PanelTop>().TextInterventions.GetComponent<NumberShower>();

		int interventions = 0;
		foreach (AchievQuery aq in m.FailureQueries) {
			if (aq.ScoreType == ScoreType.Interventions) {
				interventions = (int)aq.Value;
			}
		}
		nsi.Clear(ScoreType.Interventions);
		nsi.Inform(ScoreType.Interventions, -interventions);
		
		ScoreTypeListeners.Add(nsi);

		NumberShower nsp = PanelTop.GetComponent<PanelTop>().TextPopulation.GetComponent<NumberShower>();
		nsp.Clear(ScoreType.Population);
		ScoreTypeListeners.Add(nsp);

		GetComponentInChildren<ScrollableList>().Build(m.Buildings, ScoreTypeListeners);
		
		Mission = m;
	}

	public void Clear(ScoreType st) {
		if (st == ScoreType.Population) {
			ActualResults[st].Value = 0;
		}
	}

	void Update() {
		if (ActualResults.ContainsKey(ScoreType.Time) && Mission != null) {
			ActualResults[ScoreType.Time].Value += Time.deltaTime;
		}
	}

	private IEnumerator WaitingForResultsCoroutine(WWW www, Mission Mission) {
		ImageEndGame.SetActive(true);
		MissionStatus ms = Mission.GetStatus(ActualResults);
		TextEndGame.GetComponent<Text>().text = "Mission " + (ms == MissionStatus.Success ? "Success" : "Failure") + "\n\n Loading";

		yield return new WaitForSeconds(2f);
		yield return www;
		
		if (ms == MissionStatus.Success) {
			PanelAfterMission.SetActive(true);
			PanelAfterMission.GetComponent<PanelAfterMission>().Prepare(Mission, ActualResults, www);
		} else {
			PanelMissionFailed.SetActive(true);
			PanelMissionFailed.GetComponent<PanelMissionFailed>().Prepare(Mission, ActualResults);
		}
		gameObject.SetActive(false);
	}

	public void Inform(ScoreType st, float delta) {
		ActualResults[st].Value += delta;

		if (Mission != null) {

			MissionStatus ms = Mission.GetStatus(ActualResults);

			if (ms == MissionStatus.Success || ms == MissionStatus.Failure) {
				
				bool isSuccess = Mission.GetStatus(ActualResults) == MissionStatus.Success;
				Game.Me.GetComponent<GoogleAnalyticsV3>().LogEvent("Finished " + Mission.MissionType + " " + (isSuccess?"Success":"Failure"), Mission.Name, ActualResults[ScoreType.Interventions].Value.ToString(), (long)(ActualResults[ScoreType.Time].Value * 1000));
				StartCoroutine(WaitingForResultsCoroutine( Mission.SaveGame(ActualResults), Mission));
				Mission = null;
			}
		}
	}



}
