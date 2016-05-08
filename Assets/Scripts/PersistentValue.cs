using UnityEngine;
using System.Collections;

// Store a value in a way that persists across scenes.
public class PersistentValue : MonoBehaviour {

	public int value;

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}
}
