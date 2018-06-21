using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    // reference to the player object that the camera focuses on
    public GameObject player;

    // Zoom Range
    public float minZoom = 30f;
    public float maxZoom = 60f;

    // Zoom Speed
    public float zoomSpeed = 4f;

    // initial distance from camera to player object
    private Vector3 offset;

    // Use this for initialization
    void Start ()
    {
        // save initial distance vector from player to camera 
        offset = transform.position - player.transform.position;
    }

    void Update()
    {
		// Zoom with mouse wheel
		if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
			if (transform.position.y >= minZoom)
				transform.localPosition += Time.deltaTime * zoomSpeed * (transform.localRotation * Vector3.forward);
			/*
            if (Camera.main.fieldOfView >= minZoom)
                Camera.main.fieldOfView -= zoomSpeed;
			*/
			offset = transform.position - player.transform.position;
		}
        else if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
			if (transform.position.y <= maxZoom)
				transform.localPosition -= Time.deltaTime * zoomSpeed * (transform.localRotation * Vector3.forward);
			/*
			if (Camera.main.fieldOfView <= maxZoom)
                Camera.main.fieldOfView += zoomSpeed;
			*/
			offset = transform.position - player.transform.position;
		}

		if (player != null)
			transform.position = player.transform.position + offset;

	}
	
}
