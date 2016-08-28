using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/* This class has the main logic for the insert coin screen. */
public class InsertCoinMain : MonoBehaviour
{

	public TextMesh difficultyDisplay;

	private PersistentValue difficultyHolder;
	private PersistentValue timeTakenHolder;

	void Start ()
	{
		GameObject foundGO = GameObject.Find ("Difficulty Persistence");
		if (foundGO) {			
			difficultyHolder = foundGO.GetComponent<PersistentValue> ();
		}	

		foundGO = GameObject.Find ("Time Taken Persistence");
		if (foundGO) {
			timeTakenHolder = foundGO.GetComponent<PersistentValue> ();
			timeTakenHolder.value = 0;
		} 	
	}

	void Update ()
	{
		if (Input.GetKey("escape"))
			Application.Quit();
		
		if (Input.GetKeyDown (KeyCode.Space)) {
			randomizeFreighterLocation ();
			SceneManager.LoadScene ("Beginning Story"); 
		} else if (Input.GetKeyDown (KeyCode.RightArrow))
			increaseDifficulty ();
		else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			decreaseDifficulty ();
		}

		if (difficultyHolder != null)
			difficultyDisplay.text = "DIFFICULTY: <- " + difficultyHolder.value + " ->";
	}

	private void randomizeFreighterLocation ()
	{
		GameObject foundGO = GameObject.Find ("Freighter X Persistence");
		GameObject foundGO2 = GameObject.Find ("Freighter Y Persistence");
		if (foundGO && foundGO2) {
			PersistentValue xHolder = foundGO.GetComponent<PersistentValue> ();
			PersistentValue yHolder = foundGO2.GetComponent<PersistentValue> ();

			float radius = 50;

			Debug.Log ("Radius before adjustment = " + radius);
			if (difficultyHolder != null) {
				radius += (difficultyHolder.value - 3) * 5;
			}
			Debug.Log ("Radius after adjustment = " + radius);

			Vector2 freighterPos = MathUtilities.getPointOnCircle (Random.Range (0, 360f), radius);
			xHolder.value = (int)Mathf.Round (freighterPos.x); 
			yHolder.value = (int)Mathf.Round (freighterPos.y);

			Debug.Log ("Placed freighter at (" + xHolder.value + ", " + yHolder.value + ") at radius " + radius); 
		}
	}


	private void increaseDifficulty ()
	{
		if (difficultyHolder != null && difficultyHolder.value < 5) {
			difficultyHolder.value++;
			Debug.Log ("New difficulty: " + difficultyHolder.value);
		}
	}

	private void decreaseDifficulty ()
	{
		if (difficultyHolder != null && difficultyHolder.value > 1) {
			difficultyHolder.value--;
			Debug.Log ("New difficulty: " + difficultyHolder.value);
		}
	}

}
