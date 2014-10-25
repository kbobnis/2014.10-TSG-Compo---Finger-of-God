using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public sealed class Element {

	private readonly string Name;

	public static readonly Element Water = new Element("water");
	public static readonly Element Crush = new Element("crush");
	public static readonly Element Electricity = new Element("electricity");
	public static readonly Element Fire = new Element("fire");
	public static readonly Element SmokeAfterFire = new Element("smokeAfterFire");

	private Element(string name) {
		Name = name;
	}

	public override string ToString() {
		return Name;
	}

	//this is high standards, dudes
	public static explicit operator Element(string el) {
		if (el == Element.Water.Name){
			return Water;
		} else if (el == Element.Crush.Name){
			return Crush;
		} else if (el == Element.Electricity.Name){
			return Electricity;
		} else if (el == Element.Fire.Name){
			return Fire;
		} else if (el == Element.SmokeAfterFire.Name){
			return SmokeAfterFire;
		}
		throw new Exception("There is no element for name: " + el);
	}
}
