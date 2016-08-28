using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/* This class has the main logic for the story scenes. */
public class StoryMain : MonoBehaviour
{


	/* Story scenes are defined by these arrays which each have one
	   entry for each page of the story:
		- dialogue[] has the dialogue for the current page of story
 		- art[]  has the art
		- audioClips[] has the music or any other audio
	*/
	public GameObject[] dialogue;
	public GameObject[] art;
	public AudioClip[] musicClips;
	public AudioClip[] soundFXClips;

	public string nameOfNextScene;
	public AudioSource musicAudioSource;
	public AudioSource soundFXAudioSource;

	private int pageCount = 0;
	private int pageIndex = 0;

	void Start ()
	{
		for (int i = 0; i < dialogue.Length; i++) {
			if (dialogue [i] != null)
				pageCount++;
		}

		if (musicClips.Length > 0 && musicClips [0] != null) {
			musicAudioSource.clip = musicClips [0];
			musicAudioSource.Play ();
		}

		if (soundFXClips.Length > 0 && soundFXClips [0] != null) {
			soundFXAudioSource.clip = soundFXClips [0];
			soundFXAudioSource.Play ();
		}
	}

	void Update ()
	{
		if (Input.GetKey("escape"))
			Application.Quit();
		
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (pageIndex == (pageCount - 1)) {				
				SceneManager.LoadScene (nameOfNextScene); 
			} else if (pageIndex < pageCount - 1) {
				// Hide the last panel behind the camera.
				hide (dialogue [pageIndex]);
				if (art [pageIndex] != null) {
					hide (art [pageIndex]);
				}
										
				// Bring the next panel in front of the camera.
				pageIndex++;
				show (dialogue [pageIndex]);
				if (art [pageIndex] != null) {
					show (art [pageIndex]);
				}

				if (pageIndex < musicClips.Length && musicClips [pageIndex] != null) {
					musicAudioSource.clip = musicClips [pageIndex];
					musicAudioSource.Play ();
				}

				if (pageIndex < soundFXClips.Length && soundFXClips [pageIndex] != null) {
					soundFXAudioSource.clip = soundFXClips [pageIndex];
					soundFXAudioSource.Play ();
				}

			}
		}
		
	}

	// Hide behind the camera
	private void hide (GameObject o)
	{
		o.transform.position = new Vector3 (o.transform.position.x, o.transform.position.y, -10.0f);
	}

	// Bring in front of the camera
	private void show (GameObject o)
	{
		o.transform.position = new Vector3 (o.transform.position.x, o.transform.position.y, 0.0f);
	}
}
