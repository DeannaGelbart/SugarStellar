using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/* This class has the main logic for the insert coin screen. */
public class HighScoresMain : MonoBehaviour
{

	public TextMesh highScoresDisplay;

	private int difficulty = 0;
	private int timeTaken = 0;

	void Start ()
	{
		GameObject foundGO = GameObject.Find ("Difficulty Persistence");
		if (foundGO) {			
			difficulty = foundGO.GetComponent<PersistentValue> ().value;
		}	
		foundGO = GameObject.Find ("Time Taken Persistence");
		if (foundGO) {
			timeTaken = foundGO.GetComponent<PersistentValue> ().value;
		} 	

		highScoresDisplay.text = "YOUR COMBINED TIME: " + timeTaken + " SECONDS\n\n";
		highScoresDisplay.text += "RECORD-SETTING TIMES:\n\n";

		for (int d = 1; d <= 5; d++) {
			highScoresDisplay.text += "DIFFICULTY " + d + ": ";

			string prefsKey = "BestTimeForDifficulty" + d;

			int t = PlayerPrefs.GetInt(prefsKey, 0);    

			if (d == difficulty && (t == 0 || timeTaken < t)) {
				PlayerPrefs.SetInt (prefsKey, timeTaken);
				PlayerPrefs.Save();
				highScoresDisplay.text += timeTaken + " SECONDS (NEW BEST!)\n"; 
			} else if (t > 0) {
				highScoresDisplay.text += t + " SECONDS\n"; 
			} else {
				highScoresDisplay.text += "NONE\n";
			}
		}



	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			SceneManager.LoadScene ("Beginning"); 
		} 
	}



}
