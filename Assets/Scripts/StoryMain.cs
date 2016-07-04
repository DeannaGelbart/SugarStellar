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
	public AudioClip[] audioClips;

	public string nameOfNextScene;
	public AudioSource audioSource;

	private int pageCount = 0;
	private int pageIndex = 0;

	void Start ()
	{
		for (int i = 0; i < dialogue.Length; i++) {
			if (dialogue [i] != null)
				pageCount++;
		}

		if (audioClips.Length > 0 && audioClips [0] != null) {
			audioSource.clip = audioClips [0];
			audioSource.Play ();
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

				if (pageIndex < audioClips.Length && audioClips [pageIndex] != null) {
					audioSource.clip = audioClips [pageIndex];
					audioSource.Play ();
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
