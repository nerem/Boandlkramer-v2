using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour {

	public string LevelName;

	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			Debug.Log ("Load Empty Scene.");
			SceneManager.LoadScene (LevelName);
		}
	}
}
