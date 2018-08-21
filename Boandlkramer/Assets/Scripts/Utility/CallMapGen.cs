using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallMapGen : MonoBehaviour {

	void Start () {
		MapGenerator mapgen = GameObject.FindGameObjectWithTag ("MapGen").GetComponent<MapGenerator> ();
		mapgen.TestRandomGen ();
		GameObject.FindGameObjectWithTag ("PlayerContainer").transform.position = mapgen.SpawnPoint.transform.localPosition;
	}
}
