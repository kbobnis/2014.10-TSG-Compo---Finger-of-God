using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class WebConnector {

	
	//public static string Server = "localhost/godsfingerserver/";
	public static string Server = "http://philon.pl/fingerOfGod/godsfingerserver/";

	//private static string Service = "index-test.php?r=site";
	private static string Service = "index.php?r=site";

	private static WWWForm PrepareForm() {
		WWWForm form = new WWWForm();
		form.AddField("DeviceId", SystemInfo.deviceUniqueIdentifier);
		return form;
	}

	internal static WWW SendMissionAccomplished(MissionType MissionType, int Number, MissionStatus ms, Dictionary<ScoreType, Result> actualResults) {

		int interventions = (int)actualResults[ScoreType.Interventions].Value;
		float time = actualResults[ScoreType.Time].Value;
		WWWForm form = PrepareForm();
		form.AddField("MissionType", MissionType.ToString());
		form.AddField("Number", Number);
		form.AddField("MissionStatus", ms.ToString());
		form.AddField("Interventions", interventions);
		form.AddField("Time", ""+ (int)( time*1000) );
		return new WWW(Server+Service+"/save", form);
	}

	internal static WWW LoadMission(MissionType mt) {
		WWWForm form = PrepareForm();
		form.AddField("MissionType", mt.ToString());
		return new WWW(Server+Service+"/load", form);
	}

	internal static WWW ChangeName(string name, Mission Mission, Dictionary<ScoreType, Result> ActualResults) {
		try {
			WWWForm form = PrepareForm();
			form.AddField("Name", name);
			form.AddField("Number", Mission.Number);
			form.AddField("MissionStatus", Mission.GetStatus(ActualResults).ToString());
			form.AddField("MissionType", Mission.MissionType.ToString());
			return new WWW(Server+Service+ "/changeNameAndGetResults", form);
		} catch (System.Exception e) {
			Debug.Log("Exception: " + e);
		}
		return null;
	}
}
