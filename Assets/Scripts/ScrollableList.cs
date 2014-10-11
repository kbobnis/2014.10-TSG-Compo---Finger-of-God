using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScrollableList : MonoBehaviour
{
    public GameObject itemPrefab;
    public int columnCount = 1;
	public List<GameObject> ElementsToPut = new List<GameObject>();
	public bool FitToSize;


	public void Prepare(){

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

		}

		//adjust the height of the container so that it will just barely fit all its children
		panelRect.offsetMin = new Vector2(-width/2, -height/2);
		panelRect.offsetMax = new Vector2(width/2, height/2);

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
			float y = prefabH * (j) - height/2;

			rectTransform.offsetMin = new Vector2(x, y);
			rectTransform.offsetMax = new Vector2(x + prefabW, y + prefabH);

			//this is used instead of a double for loop because itemCount may not fit perfectly into the rows/columns
			i++;
		}

	}

    void Start()
    {
		//Prepare ();
    }

}
