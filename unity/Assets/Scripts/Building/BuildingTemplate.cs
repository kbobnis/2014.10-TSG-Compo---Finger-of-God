using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BuildingTemplate {
	private readonly int Id;
	public readonly string Name;
	public readonly int Population;
	public readonly float StartingHealth;
	private Sprite _Image;
	private Sprite _ImageDestroyed;
	public Dictionary<Element, Sprite[]> Effects = new Dictionary<Element,Sprite[]>();
	public readonly Dictionary<StatType, Dictionary<Element, float>> Stats;
	
	public BuildingTemplate(int id, string name, int population, float startingHealth, string imagePath, string imageDPath, Dictionary<StatType, Dictionary<Element, float>> stats, Dictionary<Element, Sprite[]> effects) {
		Id = id;
		Name = name;
		Population = population;
		SpriteManager.LoadAsynchronous(imagePath, (Sprite s)=> {
			_Image = s;
		});
		SpriteManager.LoadAsynchronous(imageDPath, (Sprite s) => {
			_ImageDestroyed = s;
		});

		Effects = effects;
		Stats = stats;
		StartingHealth = startingHealth;
	}

	public string Description {
		get { return Name + " has " + Population + " people in it.";  }
	}

	public Sprite Image {
		get { return _Image; }
	}

	public Sprite ImageD{
		get { return _ImageDestroyed; }
	}
	
}
