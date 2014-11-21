using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScrollableList : MonoBehaviour
{
    public GameObject itemPrefab;
	public bool FitToSize;

	private int columnCount = 1;
	private List<GameObject> ElementsToPut = new List<GameObject>();

	private void Prepare(){

		RectTransform panelRect = gameObject.GetComponent<RectTransform>();

		int itemCount = ElementsToPut.Count;

		int rowCount = itemCount / columnCount;
		if (rowCount > 0 && itemCount % columnCount > 0) {
				rowCount++;
		}

		float prefabW = itemPrefab.GetComponent<RectTransform>().rect.width;
		float prefabH = itemPrefab.GetComponent<RectTransform>().rect.height;

		float height = rowCount * prefabH;
		float width = columnCount * prefabW;

		if (FitToSize) {
			width = panelRect.rect.width;

			float scale = prefabH / prefabW;
			prefabW = width / columnCount;
			prefabH = prefabW * scale;

			height = prefabH * rowCount;

		} else {

			//adjust the height of the container so that it will just barely fit all its children
			panelRect.offsetMin = new Vector2(-width/2, -height/2);
			panelRect.offsetMax = new Vector2(width/2, height/2);
		}

		int j = -1;
		int i = 0;
		for (int x2 = 0; x2 < itemCount; x2++)
		{
			if (x2 % columnCount == 0){
				j++;
				i = 0;
			}

			GameObject newItem = null;
			//create a new item, name it, and set the parent
			if (ElementsToPut.Count > x2 ){
				newItem = ElementsToPut[x2];
			} else {
				newItem = Instantiate(itemPrefab) as GameObject;
			}

			newItem.name = gameObject.name + " item at (" + i + "," + j + ")";
			newItem.transform.parent = gameObject.transform;
			
			//move and size the new item
			RectTransform rectTransform = newItem.GetComponent<RectTransform>();
			
			float x = prefabW * (i) - width/2;
			float y = - prefabH * (j) + height/2 - prefabH;

			rectTransform.offsetMin = new Vector2(x, y);
			rectTransform.offsetMax = new Vector2(x + prefabW, y + prefabH);

			//this is used instead of a double for loop because itemCount may not fit perfectly into the rows/columns
			i++;
		}
	}

    void Start(){
		itemPrefab.SetActive(false);
    }

	public void Build(List<List<BuildingTemplate>> Buildings, List<Listener<ScoreType, float>> scoreTypeListeners) {

		bool addedChecker = true;

		foreach (GameObject go in ElementsToPut) {
			Destroy(go);
		}
		ElementsToPut.Clear();
		itemPrefab.SetActive(true);

		List<List<Building>> buildings = new List<List<Building>>();
		columnCount = Buildings[0].Count;

		foreach (List<BuildingTemplate> row in Buildings) {
			List<Building> buildingsRow = new List<Building>();
			int actualRow = 0;
			foreach (BuildingTemplate bt in row) {
				GameObject newItem = Instantiate(itemPrefab) as GameObject;
				if (!addedChecker) {
					addedChecker = true;
					newItem.AddComponent<UpdateChecker>();
				}

				Building b = newItem.GetComponent<Building>();
				float pan = (actualRow - ((columnCount-1)/2f))/((columnCount-1)/2f);
				b.CreateFromTemplate(bt, pan);
				b.Listeners = scoreTypeListeners;
				b.InformListeners();

				ElementsToPut.Add(newItem);
				buildingsRow.Add(b);
				actualRow++;
			}
			buildings.Add(buildingsRow);
		}

		Prepare();

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

		itemPrefab.SetActive(false);
	}

}
