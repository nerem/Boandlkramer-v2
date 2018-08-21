using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityInitializer : MonoBehaviour {

	public Transform spawn;

	// Use this for initialization
	void Start () {

		GameObject.FindGameObjectWithTag("PlayerContainer").transform.SetPositionAndRotation(spawn.position, Quaternion.identity);
		GameObject.FindGameObjectWithTag("Player").transform.localPosition = Vector3.zero;
	}
	
	
}
