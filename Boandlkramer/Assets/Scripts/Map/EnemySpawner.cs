using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LoG.Map;

public class EnemySpawner : MonoBehaviour {

	public List<EnemyType> EnemyTypes;
	public int NumberOfEnemies;

	public void SpawnEnemies (Map map) {

		List<MapTile> tiles = map.Grid.Elements.Values.OrderBy (x => Random.value).Take (NumberOfEnemies).ToList ();
		foreach (var tile in tiles) {
			EnemyType type = EnemyTypes.OrderBy (x => Random.value).First ();
			GameObject instance = Instantiate (type.Object, (Vector3)tile.Coordinates, Quaternion.identity);
			instance.GetComponent<Enemy> ().enemyType = type;
			instance.GetComponent<Enemy> ().LoadCharacterData ();
		}

	}
}
