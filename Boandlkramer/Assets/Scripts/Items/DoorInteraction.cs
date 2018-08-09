using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : Interactable {

	public GameObject Left;
	public GameObject Right;

	public override void Interact (Character other) {

		Left.transform.localEulerAngles = new Vector3 (0, 100, 0);
		Right.transform.localEulerAngles = new Vector3 (0, -100, 0);

		Debug.Log ("Interact.");
	}
}
