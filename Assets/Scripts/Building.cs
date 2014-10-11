using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


public enum BuildingType{
	Empty,
	Wood1=1, Stone1=2, Stone2=3, Wood2=4, Stone3=5, Wood3=6
}

public static class BuildingTypeMethod{

	public static BuildingType GetRandom(){
		return  FromInt (Mathf.RoundToInt (Random.value * 5 + 1));
	}

	public static BuildingType FromInt(int i){
		switch(i){
		case 1: return BuildingType.Wood1;
		case 2: return BuildingType.Stone1;
		case 3: return BuildingType.Stone2;
		case 4: return BuildingType.Wood2;
		case 5: return BuildingType.Stone3; 
		case 6: return BuildingType.Wood3;
		}
		return BuildingType.Empty;
	}
}

public class Building : MonoBehaviour{

	private BuildingType _BuildingType;

	public BuildingType BuildingType{
		set { 
			_BuildingType = value; 
			UpdateImage();
		}
	}

	private void UpdateImage(){
		GetComponent<Image> ().sprite = SpriteManager.BuildingSprites [_BuildingType];
	}

	public void DestroyWith(ModeType modeType){

		PlaySingleSound.SpawnSound (modeType.GetSound (), gameObject.transform.position);
		Destroy (gameObject);
	}

}
