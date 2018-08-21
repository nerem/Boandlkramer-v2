using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class DoorTeleport : Interactable {

	public string LevelName;

	public override void Interact (Character other) {

		PlayerController playerController = other.GetComponent<PlayerController>();
		playerController.StartCoroutine(playerController.Teleport());
		SceneManager.LoadScene (LevelName);
	}
}
