using UnityEngine;
using System.Collections;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class StoryMain : MonoBehaviour {

	public GameObject[] dialogue;
	int dialogueIndex = 0; 
	int dialogueCount = 0;
	public GameObject[] art;

	public AudioClip[] audioClips;
	public AudioSource audioSource;

	public string nameOfNextScene;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < dialogue.Length; i++) {
			if (dialogue [i] != null)
				dialogueCount++;
		}

		if (audioClips.Length > 0 && audioClips [0] != null) {
			audioSource.clip = audioClips [0];
			audioSource.Play ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (dialogueIndex == (dialogueCount-1)) {				
				SceneManager.LoadScene(nameOfNextScene); 
			}
			else if (dialogueIndex < dialogueCount - 1) {
				// Hide the last panel behind the camera.
				hide(dialogue[dialogueIndex]);
				if (art[dialogueIndex] != null) {
					hide(art[dialogueIndex]);
				}
										
				// Bring the next panel in front of the camera.
				dialogueIndex++;
				show(dialogue[dialogueIndex]);
				if (art[dialogueIndex] != null) {
					show(art[dialogueIndex]);
				}

				if (dialogueIndex < audioClips.Length && audioClips [dialogueIndex] != null) {
					audioSource.clip = audioClips [dialogueIndex];
					audioSource.Play ();
				}
			}
		}
		
	}

	// Hide behind the camera 
	private void hide(GameObject o) {
		o.transform.position = new Vector3 (o.transform.position.x, o.transform.position.y, -10.0f);
	}

	// Bring in front of the camera
	private void show(GameObject o) {
		o.transform.position = new Vector3 (o.transform.position.x, o.transform.position.y, 0.0f);
	}
}
