using UnityEngine;
using System.Collections;

public class ShipController : MonoBehaviour {

	public AudioSource engineNoise;
	public Sprite shipWithoutThrust;
	public Sprite shipWithThrust;
	public Sprite damagedShip;
	public SpaceflightMain spaceflightMain;

	public double fuelUsed = 0.0;

	void Start () {
		engineNoise.mute = true;
	}

	void FixedUpdate () 
	{ 		
		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		SpriteRenderer sr = GetComponent<SpriteRenderer> ();

		float rotation = 0f;
		if (Input.GetKey (KeyCode.LeftArrow))
			rotation = 1f;
		else if (Input.GetKey (KeyCode.RightArrow))
			rotation = -1f;
		rb.AddTorque(rotation * 20f);

		if (Input.GetKey (KeyCode.Space) || Input.GetKey (KeyCode.UpArrow)) {
			engineNoise.mute = false;
			if (sr.sprite != damagedShip)
			  sr.sprite = shipWithThrust;
			rb.AddRelativeForce (new Vector2 (0f, 3.5f));
			fuelUsed += Time.deltaTime*0.8;
		} else {
			engineNoise.mute = true;
			if (sr.sprite != damagedShip)				
			  sr.sprite = shipWithoutThrust;
		}
	}		

	void OnTriggerEnter2D(Collider2D other) {
		Debug.Log ("In ship, trigger with " + other.gameObject.name + " tagged as " + other.gameObject.tag);
		if (other.gameObject.tag == "asteroid" || other.gameObject.tag == "pirate") {
			Debug.Log ("Calling got hit");
			SpriteRenderer sr = GetComponent<SpriteRenderer> ();
			sr.sprite = damagedShip;
			spaceflightMain.playerGotHit ();
		}
	}

	void OnCollisionEnter2D(Collision2D collision) {
		Debug.Log ("In ship, collision with " + collision.gameObject.name + " tagged as " + collision.gameObject.tag);
	}
}
