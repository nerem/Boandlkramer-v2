using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallMapGen : MonoBehaviour {

	void Start () {
		GameObject.FindGameObjectWithTag ("MapGen").GetComponent<MapGenerator> ().TestRandomGen ();
	}
}
