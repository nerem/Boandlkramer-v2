using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorInteraction : Interactable {

	public GameObject Door;
	public GameObject Left;
	public GameObject Right;

	public override void Interact (Character other) {

		Left.transform.localEulerAngles = new Vector3 (0, 100, 0);
		Right.transform.localEulerAngles = new Vector3 (0, -100, 0);
		Door.GetComponent<BoxCollider> ().enabled = false;
		Door.GetComponent<NavMeshObstacle> ().enabled = false;
		//Left.GetComponent<Animator> ().SetTrigger ("isOpen");

		Debug.Log ("Interact.");
	}
}
