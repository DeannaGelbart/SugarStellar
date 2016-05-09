using UnityEngine;
using System.Collections;

// Controls the motion of a pirate ship.
public class PirateController : MonoBehaviour {
	public GameObject aliceShip;

	private float speed = 1;
	private bool chaseStarted = false;
	private int difficulty = 3;

	void Start () {
		// Set the speed of pirate ship based on the game's global difficulty setting.
		GameObject foundGO = GameObject.Find("Difficulty Persistence");
		if (foundGO) {			
			difficulty = foundGO.GetComponent<PersistentValue> ().value;
		}	
		speed *= difficulty / 3.0f;
	}
	
	void FixedUpdate () {		
		float range = Vector2.Distance(transform.position, aliceShip.transform.position);

		// If we are close enough to Alice's ship to be chasing her
		if ((chaseStarted && range < 10) || (!chaseStarted && range < 5)) {
			// Move toward Alice's ship
			transform.position = Vector2.MoveTowards (transform.position, aliceShip.transform.position, 
				speed * Time.deltaTime);
			chaseStarted = true;
		} else {
			// Hover up and down.
			// Sine motion taken from http://forum.unity3d.com/threads/sin-movement-on-y-axis.10357/
			float frequency = 0.3f;
			transform.position += 0.3f * transform.up * (Mathf.Sin (2 * Mathf.PI * frequency * Time.time) - 
				Mathf.Sin (2 * Mathf.PI * frequency * (Time.time - Time.deltaTime)));
		}
	}
}
