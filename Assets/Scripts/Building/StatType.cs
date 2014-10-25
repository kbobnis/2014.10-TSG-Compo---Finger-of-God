using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * Type safe enum pattern!
 */
public sealed class StatType {

	private readonly String Name;

	public static readonly StatType ContaminateDelta = new StatType("contaminateDelta");
	public static readonly StatType StrikeDamage = new StatType("strikeDamage");
	public static readonly StatType EffectDamage = new StatType("effectDamage");
	public static readonly StatType EffectTime = new StatType("effectTime");
	public static readonly StatType FillSpeed = new StatType("fillSpeed");
	public static readonly StatType AfterDeath = new StatType("afterDeath");

	private StatType(string name) {
		Name = name;
	}

	public override string ToString() {
		return Name;
	}

	public static explicit operator StatType(string st) {
		if (st == StatType.ContaminateDelta.Name){
			return ContaminateDelta;
		} else if(st == StatType.StrikeDamage.Name){
			return StrikeDamage;
		} else if (st == StatType.EffectDamage.Name){
			return EffectDamage;
		} else if (st == StatType.EffectTime.Name){
			return EffectTime;
		} else if (st == StatType.FillSpeed.Name){
			return FillSpeed;
		} else if (st == StatType.AfterDeath.Name) {
			return AfterDeath;
		} 
		throw new Exception("There is no statType for name: " + st);
	}
}