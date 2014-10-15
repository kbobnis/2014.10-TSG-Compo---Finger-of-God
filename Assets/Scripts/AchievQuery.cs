using UnityEngine;
using System.Collections;

using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;
using System.Collections.Generic;


public enum ScoreType {
	Interventions, Population
}
public static class ScoreTypeMethods {
	public static string HumanName(this ScoreType s) {
		switch (s) {
			default: return s.ToString();
		}
	}
}

public enum Sign {
	SmallerEqual, Equal, BiggerEqual, NoMatter, Smaller, Bigger
}
public static class SignMethods {
	public static bool IsMet(this Sign s1, float v1, float v2) {
		switch (s1) {
			case Sign.BiggerEqual:
				return v1 >= v2;
			case Sign.Equal:
				return v1 == v2;
			case Sign.NoMatter:
				return true;
			case Sign.SmallerEqual:
				return v1 <= v2;
			case Sign.Smaller:
				return v1 < v2;
			case Sign.Bigger:
				return v1 > v2;
			default:
				throw new UnityException("There is no sign: " + s1);
		}
	}

	public static string Text(this Sign s1) {
		switch (s1) {
			case Sign.BiggerEqual:
				return "bigger or equal";
			case Sign.SmallerEqual:
				return "smaller or equal";
			case Sign.Equal:
				return "equal";
			case Sign.NoMatter:
				return "no matter";
			case Sign.Smaller:
				return "smaller";
			case Sign.Bigger:
				return "bigger";
			default:
				throw new UnityException("There is no sign: " + s1);
		}
	}

}
public class Result {
	protected ScoreType _ScoreType;
	protected int _Value;

	public ScoreType ScoreType {
		get { return _ScoreType; }
	}
	public int Value {
		get { return _Value; }
		set { _Value = value;  }
	}

	public Result(ScoreType scoreType, int value) {
		_ScoreType = scoreType;
		_Value = value;
	}
}
public class AchievQuery : Result {
	private Sign _Sign;

	public Sign Sign {
		get { return _Sign; }
	}

	public AchievQuery(ScoreType scoreType, Sign sign, int value): base(scoreType, value) {
		_Sign = sign;
	}

	public bool IsTheSameType(Result r) {
		return ScoreType.Equals(r.ScoreType);
	}
	public bool CanAccept(Result r) {
		if (!IsTheSameType(r)) {
			return true;
		}

		return Sign.IsMet(r.Value, Value);
	}

}

