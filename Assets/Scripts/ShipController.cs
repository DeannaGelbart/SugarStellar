﻿using UnityEngine;
using System.Collections;

// Controls the motion of Alice's ship.
public class ShipController : MonoBehaviour
{

	public AudioSource engineNoise;
	public AudioSource explosionNoise;

	public Sprite shipWithoutThrust;
	public Sprite shipWithThrust;
	public Sprite damagedShip;
	public SpaceflightMain spaceflightMain;

	void Start ()
	{
		engineNoise.mute = true;
	}

	void FixedUpdate ()
	{ 		
		Rigidbody2D rb = GetComponent<Rigidbody2D> ();
		SpriteRenderer sr = GetComponent<SpriteRenderer> ();

		float rotation = 0f;
		if (Input.GetKey (KeyCode.LeftArrow))
			rotation = 1f;
		else if (Input.GetKey (KeyCode.RightArrow))
			rotation = -1f;
		rb.AddTorque (rotation * 20f);

		if (Input.GetKey (KeyCode.Space) || Input.GetKey (KeyCode.UpArrow)) {
			engineNoise.mute = false;
			if (sr.sprite != damagedShip)
				sr.sprite = shipWithThrust;
			rb.AddRelativeForce (new Vector2 (0f, 3.1f));
		} else {
			engineNoise.mute = true;
			if (sr.sprite != damagedShip)
				sr.sprite = shipWithoutThrust;
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		Debug.Log ("In ship, trigger with " + other.gameObject.name + " tagged as " + other.gameObject.tag);
		if (other.gameObject.tag == "asteroid" || other.gameObject.tag == "pirate") {
			SpriteRenderer sr = GetComponent<SpriteRenderer> ();
			sr.sprite = damagedShip;
			explosionNoise.Play ();
			spaceflightMain.playerGotHit ();
		}
			
		// In survival mode, can't let the player cheat by going outside the walls, so bounce them
		// back in. 
		if (other.gameObject.tag == "wallSurvivalMode") {
			Debug.Log ("Wall bounce");
			Rigidbody2D rb = GetComponent<Rigidbody2D> ();
			rb.velocity = -1 * rb.velocity;
		}
			
	}
}
