using UnityEngine;
using System.Collections;


public class CameraTracking : MonoBehaviour {

	public Transform playerShip;
	public Vector3 cameraOffset;
	public Transform uiText; // We put the UI text under the camera so we can use a bloom effect.
	public Vector3 uiTextOffset;

	void Start () {
		transform.position = cameraOffset;
	}
		
	void FixedUpdate () {
		transform.position = new Vector3 (playerShip.position.x + cameraOffset.x, playerShip.position.y + cameraOffset.y, cameraOffset.z);
		uiText.position = new Vector3 (transform.position.x + uiTextOffset.x, transform.position.y + uiTextOffset.y, uiTextOffset.z);
	}
}
