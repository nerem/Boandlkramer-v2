using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementIndicator : MonoBehaviour {

	[SerializeField]
	PlayerController player;

	public void Deactivate () {

		player.indicator.SetActive (false);
	}
}
