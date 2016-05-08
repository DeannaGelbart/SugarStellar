using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class InsertCoinMain : MonoBehaviour {

	public TextMesh difficultyDisplay;

	private PersistentValue difficultyHolder;

	void Start () {
		GameObject foundGO = GameObject.Find("Difficulty Persistence");
		if (foundGO) {			
			difficultyHolder = foundGO.GetComponent<PersistentValue> ();
		}	
	}
	
	void Update () {
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

	private void randomizeFreighterLocation() {
		GameObject foundGO = GameObject.Find("Freighter X Persistence");
		GameObject foundGO2 = GameObject.Find("Freighter Y Persistence");
		if (foundGO && foundGO2) {
			PersistentValue xHolder = foundGO.GetComponent<PersistentValue> ();
			PersistentValue yHolder = foundGO2.GetComponent<PersistentValue> ();

			float radius = 40;

			if (difficultyHolder != null) {
				radius += (difficultyHolder.value - 3) * 5;
				if (difficultyHolder.value == 5)
					radius += 5;
			}

			Vector2 freighterPos = getPointOnCircle (Random.Range (0, 360f), radius);
			xHolder.value = (int) Mathf.Round(freighterPos.x); 
			yHolder.value = (int) Mathf.Round(freighterPos.y);

			Debug.Log ("Placed freighter at (" + xHolder.value + ", " + yHolder.value + ") at radius " + radius); 
		}
	}


	private void increaseDifficulty() {
		if (difficultyHolder != null && difficultyHolder.value < 5) {
			difficultyHolder.value++;
			Debug.Log ("New difficulty: " + difficultyHolder.value);
		}
	}

	private void decreaseDifficulty() {
		if (difficultyHolder != null && difficultyHolder.value > 1) {
			difficultyHolder.value--;
			Debug.Log ("New difficulty: " + difficultyHolder.value);
		}
	}

	// TODO: put this in a library so it's not duplicated in 2 files.
	// From http://answers.unity3d.com/questions/33193/randomonunitcircle-.html
	private Vector2 getPointOnCircle(float angleDegrees, float radius) {
		float _x = 0;
		float _y = 0;
		float angleRadians = 0;
		Vector2 _returnVector;
		// convert degrees to radians
		angleRadians = angleDegrees * Mathf.PI / 180.0f;
		// get the 2D dimensional coordinates
		_x = radius * Mathf.Cos(angleRadians);
		_y = radius * Mathf.Sin(angleRadians);
		// derive the 2D vector
		_returnVector = new Vector2(_x, _y);
		// return the vector info
		return _returnVector;
	}
}
