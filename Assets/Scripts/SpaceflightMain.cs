using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

/* Main logic for the gameplay scenes where the player is flying around. */
public class SpaceflightMain : MonoBehaviour
{

	public string nameOfNextScene;

	/* Visible objects in the scene. */
	public GameObject[] prototypeAsteroids;
	public int numberOfAsteroidsBeforeDifficultyModifier;
	public GameObject prototypeStar;
	public int numberOfStars;
	public GameObject bakery;
	public GameObject aliceShip;
	public GameObject freighter;
	public GameObject startingPoint;
	public GameObject destination;
	public GameObject[] pirates;
	public TextMesh headsUpDisplay;

	/* When there are pirates in the scene, the music will
	 change once we escape them so we need to crossfade it. */
	public AudioSource endingMusic;
	public AudioMixerSnapshot startingMixerSnapshot;
	public AudioMixerSnapshot endingMixerSnapshot;

	/* The walls prevent the asteroids from drifting out too far. */
	public GameObject leftWall;
	public GameObject rightWall;
	public GameObject topWall;
	public GameObject bottomWall;
	private float maxX;
	private float minX;
	private float maxY;
	private float minY;

	private bool reachedMusicTransitionPoint = false;
	private bool reachedStoppingPoint = false;

	private int difficulty = 3;
	private PersistentValue livesHolder;

	void Start ()
	{
		GameObject foundGO = GameObject.Find ("Difficulty Persistence");
		if (foundGO) {			
			difficulty = foundGO.GetComponent<PersistentValue> ().value;
		}	
		Debug.Log ("Difficulty: " + difficulty);

		foundGO = GameObject.Find ("Lives Persistence");
		if (foundGO) {
			livesHolder = foundGO.GetComponent<PersistentValue> ();
			Debug.Log ("Loaded lives: " + getLives ());
		} 	
			
		/* Bonnie's freighter's location is randomized before gameplay starts so that
		 it will be the same in both spaceflight scenes.  */
		foundGO = GameObject.Find ("Freighter X Persistence");
		GameObject foundGO2 = GameObject.Find ("Freighter Y Persistence");
		if (foundGO && foundGO2) {
			int x = foundGO.GetComponent<PersistentValue> ().value;
			int y = foundGO2.GetComponent<PersistentValue> ().value;
			freighter.transform.position = new Vector3 (x, y, freighter.transform.position.z);
		}

		maxX = rightWall.transform.position.x; 
		minX = leftWall.transform.position.x;
		maxY = topWall.transform.position.y;
		minY = bottomWall.transform.position.y; 

		if (destination == bakery) {
			aliceShip.transform.position = freighter.transform.position;
		} else {
			aliceShip.transform.position = bakery.transform.position;
		}

		// Pirates surround the freighter in the second spaceflight scene.
		for (int i = 0; pirates != null && i < pirates.Length && pirates [i] != null; i++) {
			Transform t = pirates [i].transform;
			t.position = freighter.transform.position;			
			float angle = (360f / pirates.Length) * i;
			Vector2 offset = MathUtilities.getPointOnCircle (angle, 6.9f);
			t.position = new Vector3 (t.position.x + offset.x, 
				t.position.y + offset.y, t.position.z);
		}

		initializeAsteroids ();
		initializeStars ();

		// If they restart the level due to crashing their ship, make sure
		// we start with the initial music.
		if (startingMixerSnapshot != null) {
			startingMixerSnapshot.TransitionTo (0.1f);
		}			
	}

	void FixedUpdate ()
	{
		rotateBakery ();
	}

	void Update ()
	{
		distanceToDestinationLogic (); 
	}

	public void playerGotHit ()
	{
		freeze ();
		reachedStoppingPoint = true;

		setLives (getLives () - 1);
		if (getLives () <= 0) {
			nameOfNextScene = "Beginning";
			headsUpDisplay.text = "That's the way the\ncookie crumbles!\nGame over";
			setLives (5);
		} else {
			headsUpDisplay.text = "No use crying over\nspilled milk 'n' cookies!\nTime to try again!";
			nameOfNextScene = SceneManager.GetActiveScene ().name;
		}
	}
		
	private void setLives (int lives)
	{
		if (livesHolder != null)
			livesHolder.value = lives;
	}

	private int getLives ()
	{
		if (livesHolder != null)
			return livesHolder.value;
		else
			return 3;
	}

	private void freeze ()
	{
		aliceShip.GetComponent<ShipController> ().engineNoise.mute = true;
		Time.timeScale = 0; // Pause all motion and stop FixedUpdates.
	}

	private void unfreeze ()
	{
		Time.timeScale = 1; // Restore FixedUpdates.
	}

	private void distanceToDestinationLogic ()
	{
		float distance = Vector3.Distance (aliceShip.transform.position, destination.transform.position);

		if (!reachedMusicTransitionPoint) {
			if (distance <= 15f && endingMusic != null) {
				reachedMusicTransitionPoint = true;
				endingMusic.Play ();
				endingMixerSnapshot.TransitionTo (0.2f);
			}
		}
			
		if (!reachedStoppingPoint) {
			if (distance < 4) {
				reachedStoppingPoint = true;
				freeze ();
				headsUpDisplay.text = "You made it!\n" + "In " + Time.timeSinceLevelLoad.ToString ("F0") + " seconds and\n" +
				aliceShip.GetComponent<ShipController> ().fuelUsed.ToString ("F0") + " cups of sugar.";
			} else {
				headsUpDisplay.text = "Lives:" + getLives () + "  Distance:" + distance.ToString ("F1"); 
			}
		}

		// Move to next scene once destination reached.
		if (reachedStoppingPoint && Input.GetKeyDown (KeyCode.Space)) {
			unfreeze ();
			if (nameOfNextScene == "End")
				setLives (3); // Reset # of lives for next time they play			
			SceneManager.LoadScene (nameOfNextScene); 
		}
	}
		

	private void rotateBakery ()
	{
		// Bakery spins slowly
		if (bakery != null)
			bakery.transform.Rotate (0, 0, -4f * Time.deltaTime);		
	}


	private void initializeStars ()
	{
		GameObject[] stars = new GameObject[numberOfStars];

		for (int i = 0; i < stars.Length; i++) {
			stars [i] = Instantiate (prototypeStar);
			Transform t = stars [i].transform;

			// this game has no occlusion so it looks weird if a star is on a landmark
			float minDistanceFromLandmarks = 2; 

			bool good = false;
			do {
				t.position = new Vector3 (Random.Range (minX, maxX), Random.Range (minY, maxY), 0);
				t.position *= 1.5f; // spread the stars wider than the asteroid belt

				float d1 = Vector3.Distance (bakery.transform.position, t.position);
				float d2 = Vector3.Distance (freighter.transform.position, t.position);
				if (d1 > minDistanceFromLandmarks && d2 > minDistanceFromLandmarks)
					good = true;
			} while (!good);
		}
	}

	private void initializeAsteroids ()
	{		

		float modifier = 1.0f;

		if (difficulty == 1)
			modifier = 0.5f;
		else if (difficulty == 2)
			modifier = 0.75f;
		else if (difficulty == 4)
			modifier = 1.25f;
		else if (difficulty == 5)
			modifier = 1.4f;

		int numberOfAsteroids = (int)(numberOfAsteroidsBeforeDifficultyModifier * modifier);
		GameObject[] asteroids = new GameObject[numberOfAsteroids];

		float minDistanceFromShip = 4.5f;

		for (int i = 0; i < asteroids.Length; i++) {
			GameObject prototypeAsteroid = prototypeAsteroids [Random.Range (0, prototypeAsteroids.Length)];
			asteroids [i] = Instantiate (prototypeAsteroid);
			Transform t = asteroids [i].transform;
			Rigidbody2D r = t.GetComponent<Rigidbody2D> ();

			t.localEulerAngles = new Vector3 (0, 0, Random.Range (0, 360f));

			float speed = Random.Range (0.2f, 1f) * difficulty / 2.7f;
			Vector2 movementDirection = Quaternion.Euler (0, 0, Random.Range (0, 360f)) * new Vector2 (0, 1f);
			r.velocity = movementDirection * speed;		

			bool good = false;
			do {
				t.position = new Vector3 (Random.Range (minX, maxX), Random.Range (minY, maxY), 0);
				float d = Vector3.Distance (aliceShip.transform.position, t.position);
				if (d > minDistanceFromShip)
					good = true;
			} while (!good);
		}
	}
}
