using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUI : MonoBehaviour {

	public void Kill () {

		Destroy (gameObject);
	}

	public void Deactivate () {

		gameObject.SetActive (false);
	}
}
