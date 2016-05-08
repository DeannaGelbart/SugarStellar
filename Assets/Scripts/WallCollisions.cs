using UnityEngine;
using System.Collections;

public class WallCollisions : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other) {
		Debug.Log ("In wall, trigger with " + other.gameObject.name + " tagged as " + other.gameObject.tag);
	}

	void OnCollisionEnter2D(Collision2D collision) {
		Debug.Log ("In wall, collision with " + collision.gameObject.name + " tagged as " + collision.gameObject.tag);
	}
}
