using UnityEngine;
using System.Collections;

// Controls the motion of a pirate ship.
public class PirateController : MonoBehaviour {
	public GameObject aliceShip;
	public GameObject bakery;

	private float speedMultiplier = 0.9f;
	private bool chaseStarted = false;
	private int difficulty = 5;

	void Start () {
		// Set the speed of pirate ship based on the game's global difficulty setting.
		GameObject foundGO = GameObject.Find("Difficulty Persistence");
		if (foundGO) {			
			difficulty = foundGO.GetComponent<PersistentValue> ().value;
		}	
		speedMultiplier *= difficulty / 3f;
	}
	
	void FixedUpdate () {		
		// If we are close enough to Alice's ship to be chasing her
		if (aliceIsChaseable()) {
			// Move toward Alice's ship
			float distFromPirateToAlice = Vector2.Distance(transform.position, aliceShip.transform.position);
			float speed = speedMultiplier + distFromPirateToAlice/6f;
			transform.position = Vector2.MoveTowards (transform.position, aliceShip.transform.position, 
				speed * Time.deltaTime); 
			chaseStarted = true;
		} else {
			// Hover up and down.
			// Sine motion formula from http://forum.unity3d.com/threads/sin-movement-on-y-axis.10357/
			float frequency = 0.3f;
			transform.position += 0.3f * transform.up * (Mathf.Sin (2 * Mathf.PI * frequency * Time.time) - 
				Mathf.Sin (2 * Mathf.PI * frequency * (Time.time - Time.deltaTime)));
		}
	}

	bool aliceIsChaseable() {
		float distFromPirateToAlice = Vector2.Distance(transform.position, aliceShip.transform.position);
		float distFromAliceToHome = Vector2.Distance(bakery.transform.position, aliceShip.transform.position);

		// Pirates stop chasing Alice when she is near her home, because for the stake
		// of the storyline she has to escape them by the time she gets home;
		if (distFromAliceToHome <= 15) {
			chaseStarted = false;
			return false;
		}			
		// Else if pirate is close enough to Alice, chase her. 
		else if ((!chaseStarted && distFromPirateToAlice < 5) || (chaseStarted && distFromPirateToAlice < 15))  {
			chaseStarted = true;	
			return true;
		}
		else {
			chaseStarted = false;
			return false;
		}
	}
}
