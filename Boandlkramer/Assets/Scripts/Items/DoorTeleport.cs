using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTeleport : Interactable {

	public string LevelName;

	public override void Interact (Character other) {

		SceneManager.LoadScene (LevelName);
	}
}
