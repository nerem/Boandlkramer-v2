using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    // reference to the player object that the camera focuses on
    public GameObject player;

    // Zoom Range
    public float minZoom = .8f;
    public float maxZoom = 2f;

    // Zoom Speed
    public float zoomSpeed = 0.33f;

	// rotation Speed and flag for rotating the camera
	public float rotationSpeed = 75f;
	bool bRotate = false;

    // initial distance from camera to player object
    private Vector3 offset;

	// reference to Dialogue Manager in order to disable zooming in chat mode
	DialogueManager dialogueManager;

    // Use this for initialization
    void Start ()
    {
        // save initial distance vector from player to camera 
        offset = transform.position - player.transform.position;

		dialogueManager = FindObjectOfType<DialogueManager>();
    }

    void LateUpdate()
    {
		if (player != null)
			transform.position = player.transform.position + offset;


		if (dialogueManager)
		{
			// while in dialogue, camera control is disabled
			if (dialogueManager.animator.GetBool("isOpen"))
			{
				return;
			}
		}
		else
		{
			// in case that the dialogue manager was not found yet, try that again
			dialogueManager = FindObjectOfType<DialogueManager>();
		}

		// Zoom with mouse wheel
		if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {

			if (transform.position.y >= minZoom)
				transform.localPosition += zoomSpeed * (transform.localRotation * Vector3.forward);
			/*
            if (Camera.main.fieldOfView >= minZoom)
                Camera.main.fieldOfView -= zoomSpeed;
			*/
		}
        else if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
			if (transform.position.y <= maxZoom)
				transform.localPosition -= zoomSpeed * (transform.localRotation * Vector3.forward);
			/*
			if (Camera.main.fieldOfView <= maxZoom)
                Camera.main.fieldOfView += zoomSpeed;
			*/
		}

		// rotate camera
		if (Input.GetMouseButtonDown(2))
		{
			bRotate = true;
		}

		if (Input.GetMouseButtonUp(2))
		{
			bRotate = false;
		}

		if (bRotate)
		{
			float val = Input.GetAxis("Mouse X");
			transform.RotateAround(player.transform.position, Vector3.up, val * rotationSpeed * Time.deltaTime);
		}


		AdjustCamera();

	}

	void AdjustCamera()
	{
		while (transform.position.y < minZoom)
		{
			transform.localPosition -= zoomSpeed * (transform.localRotation * Vector3.forward);
		}
		
		while (transform.position.y > maxZoom)
		{
			transform.localPosition += zoomSpeed * (transform.localRotation * Vector3.forward);
		}

		offset = transform.position - player.transform.position;
	}
	
}
