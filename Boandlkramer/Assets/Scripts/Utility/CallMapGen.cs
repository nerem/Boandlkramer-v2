using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallMapGen : MonoBehaviour {

	void Start () {
		MapGenerator mapgen = GameObject.FindGameObjectWithTag ("MapGen").GetComponent<MapGenerator> ();
		mapgen.TestRandomGen ();
		mapgen.GetComponent<EnemySpawner>().SpawnEnemies(mapgen.Map);
		GameObject.FindGameObjectWithTag ("PlayerContainer").transform.position = mapgen.SpawnPoint.transform.localPosition;
		GameObject.FindGameObjectWithTag("Player").transform.localPosition = Vector3.zero;
	}
}
