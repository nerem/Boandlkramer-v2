using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityInitializer : MonoBehaviour {

	// Use this for initialization
	void Start () {

		GameObject.FindGameObjectWithTag("Player").transform.position = Vector3.zero;
	}
	
	
}
