﻿using UnityEngine;
using System.Collections;

// This moves the camera along with the ship and moves the player's heads up display 
// text (showing distance to the destination, etc.) along with the camera. We put the 
// HUD under the camera so we can use the camera bloom effect with it.

public class CameraTracking : MonoBehaviour {

	public Transform playerShip;
	public Vector3 cameraOffset;
	public Transform hud; 
	public Vector3 hudOffset;

	void Start () {
		transform.position = cameraOffset;
	}

	void FixedUpdate () {
		transform.position = new Vector3 (playerShip.position.x + cameraOffset.x, playerShip.position.y + cameraOffset.y, cameraOffset.z);
		hud.position = new Vector3 (transform.position.x + hudOffset.x, transform.position.y + hudOffset.y, hudOffset.z);
	}
}