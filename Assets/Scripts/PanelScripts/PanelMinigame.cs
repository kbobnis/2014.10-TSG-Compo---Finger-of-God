using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PanelMinigame : MonoBehaviour, Listener<ScoreType, float> {

	public GameObject PanelAfterMission;

	private Mission Mission;
	private Dictionary<ScoreType, Result> ActualResults = new Dictionary<ScoreType, Result>();
	private List<Listener<ScoreType, float>> ScoreTypeListeners;

	public void PrepareGame(Mission m, List<Listener<ScoreType, float>> scoreTypeListeners) {

		Debug.Log("logging screen.");
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

		ScrollableList sl = GetComponentInChildren<ScrollableList>();
		int columns = sl.columnCount;
		foreach (GameObject go in sl.ElementsToPut) {
			Destroy(go);
		}
		sl.ElementsToPut.Clear();
		sl.itemPrefab.SetActive(true);

		List<List<Building>> buildings = new List<List<Building>>();

		foreach (List<BuildingTemplate> row in m.Buildings) {
			List<Building> buildingsRow = new List<Building>();
			foreach (BuildingTemplate bt in row) {
				GameObject newItem = Instantiate(sl.itemPrefab) as GameObject;

				Building b = newItem.GetComponent<Building>();
				b.CreateFromTemplate(bt);
				b.Listeners = scoreTypeListeners;
				b.InformListeners();

				sl.ElementsToPut.Add(newItem);
				buildingsRow.Add(b);
			}
			buildings.Add(buildingsRow);
		}

		sl.Prepare();

		//create neighbours -> optimization
		int x = 0;
		foreach (List<Building> cols in buildings) {
			int y = 0;
			foreach (Building bi in cols) {
				Building bu = bi.GetComponent<Building>();

				if (buildings.Count > x + 1) {
					bu.Neighbours.Add(Side.Right, buildings[x + 1][y]);
				}

				if (buildings[x].Count > y + 1) {
					bu.Neighbours.Add(Side.Down, buildings[x][y + 1]);
				}

				if (x - 1 >= 0) {
					bu.Neighbours.Add(Side.Left, buildings[x - 1][y]);
				}

				if (y - 1 >= 0) {
					bu.Neighbours.Add(Side.Up, buildings[x][y - 1]);
				}
				y++;
			}
			x++;
		}

		sl.itemPrefab.SetActive(false);
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

	public void Inform(ScoreType st, float delta) {
		ActualResults[st].Value += delta;

		if (Mission != null) {
			MissionStatus ms = Mission.GetStatus(ActualResults);

			if (ms == MissionStatus.Failure || ms == MissionStatus.Success) {
				PanelAfterMission.SetActive(true);
				PanelAfterMission.GetComponent<PanelAfterMission>().Prepare(Mission, ActualResults);
				gameObject.SetActive(false);
				Mission = null;
			}
		}
	}



}
