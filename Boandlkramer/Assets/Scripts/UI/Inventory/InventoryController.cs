using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour {

	[SerializeField]
	GameObject panel;
	[SerializeField]
	GameObject slot;

	[SerializeField]
	Character player;

	GameObject[,] grid;

	void Start () {

		GenerateInventoryGrid (6, 7);
	}

	void GenerateInventoryGrid (int columns, int rows) {

		grid = new GameObject[columns, rows];

		for (int i = 0; i < columns; i++) {
			for (int j = 0; j < rows; j++) {

				grid[i, j] = Instantiate (slot, panel.transform) as GameObject;
				grid[i, j].GetComponent<RectTransform> ().anchoredPosition = new Vector2 (i * 60 + 2, -j * 60 - 2);
			}
		}
	}

	void RefreshInventory () {

		
	}
}
