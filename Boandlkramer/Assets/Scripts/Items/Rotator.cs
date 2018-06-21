﻿
using UnityEngine;

public class Rotator : MonoBehaviour {

	public float rotationSpeed = 100f;
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
	}
}
