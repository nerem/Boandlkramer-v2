using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

	public float interactionRange = 2f;


	public virtual void Interact (Character other) {

		Debug.Log ("Interact.");
	}

	void OnMouseEnter () {
		// highlight object on mouse over
		//GetComponent<MeshRenderer> ().material.color = Color.green;
	}

	void OnMouseExit () {
		// remove highlighting
		//GetComponent<MeshRenderer> ().material.color = Color.red;
	}

	void OnDrawGizmosSelected () {

		Gizmos.DrawWireSphere (transform.position, interactionRange);
	}
}
