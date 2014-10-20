using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class WebConnector {

	private static string Server = "localhost/godsfingerserver/index-test.php?r=site";
	//private static string Server = "http://philon.pl/fingerOfGod/godsfingerserver/index.php?r=site";


	internal static WWW SendMissionAccomplished(MissionType MissionType, int Number, MissionStatus ms, Dictionary<ScoreType, Result> actualResults) {

		int interventions = (int)actualResults[ScoreType.Interventions].Value;
		float time = actualResults[ScoreType.Time].Value;

		Debug.Log("send mission accomplished");
		WWWForm form = new WWWForm();
		form.AddField("MissionType", MissionType.ToString());
		form.AddField("DeviceId", SystemInfo.deviceUniqueIdentifier);
		form.AddField("Number", Number);
		form.AddField("MissionStatus", ms.ToString());
		form.AddField("Interventions", interventions);
		form.AddField("Time", ""+ (int)( time*1000) );
		Debug.Log("sending form: " + form.data);
		Debug.Log("server: " + Server + "/save" + " form: " + form.data);
		return new WWW(Server+"/save", form);
	}

	internal static WWW LoadMission(MissionType mt) {
		WWWForm form = new WWWForm();
		form.AddField("MissionType", mt.ToString());
		form.AddField("DeviceId", SystemInfo.deviceUniqueIdentifier);
		return new WWW(Server+"/load", form);
	}
}
