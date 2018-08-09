using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

	public MeshRenderer Renderer;
	public float interactionRange = 2f;


	public virtual void Interact (Character other) {

		Debug.Log ("Interact.");
	}

	void OnMouseEnter () {
		// highlight object on mouse over
		//GetComponent<MeshRenderer> ().material.color = Color.green;
		Debug.Log ("Enter");

		Camera.current.GetComponent<HighlightsFX> ().objectRenderer = Renderer;
		Camera.current.GetComponent<HighlightsFX> ().enabled = true;
	}

	void OnMouseExit () {
		// remove highlighting
		//GetComponent<MeshRenderer> ().material.color = Color.red;

		Camera.current.GetComponent<HighlightsFX> ().enabled = false;
		//Camera.current.GetComponent<HighlightsFX> ().objectRenderer = null;
	}

	void OnDrawGizmosSelected () {

		Gizmos.DrawWireSphere (transform.position, interactionRange);
	}
}
