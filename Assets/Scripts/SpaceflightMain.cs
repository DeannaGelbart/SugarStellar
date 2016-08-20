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
	private bool piratesStartChase = false;
	private bool piratesStopChase = false;

	private int difficulty = 2;
	private PersistentValue livesHolder;
	private int timeToFinishThisLevel = 0;
	private PersistentValue timeTakenHolder;

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

		foundGO = GameObject.Find ("Time Taken Persistence");
		if (foundGO) {
			timeTakenHolder = foundGO.GetComponent<PersistentValue> ();
			Debug.Log ("Time taken: " + getTimeTaken ());
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
			Vector2 offset = MathUtilities.getPointOnCircle (angle, Random.Range(6.5f, 10f));
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
		pirateMovements ();
	}

	void Update ()
	{
		if (Input.GetKey("escape"))
			Application.Quit();
		
		distanceLogic ();
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

	private void setTimeTaken (int timeTaken)
	{
		if (timeTakenHolder != null)
			timeTakenHolder.value = timeTaken;
	}

	private int getTimeTaken ()
	{
		if (timeTakenHolder != null)
			return timeTakenHolder.value;
		else
			return 0;
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

	private void distanceLogic () 
	{
		float distanceFromStart = Vector3.Distance (aliceShip.transform.position, startingPoint.transform.position);
		float distanceToDestination = Vector3.Distance (aliceShip.transform.position, destination.transform.position);

		// The pirates won't start chasing until alice is a little ways away from the freighter.
		if (distanceFromStart > 2f)
			piratesStartChase = true;

		if (!reachedMusicTransitionPoint) {
			// If we're almost there, the music & the pirates calm down.
			if (distanceToDestination <= 15f && endingMusic != null) {
				reachedMusicTransitionPoint = true;
				endingMusic.Play ();
				endingMixerSnapshot.TransitionTo (0.2f);
				piratesStopChase = true;
			}
		}
			
		if (!reachedStoppingPoint) {
			if (distanceToDestination < 4) {
				reachedStoppingPoint = true;
				freeze ();
				timeToFinishThisLevel = (int) Mathf.Round(Time.timeSinceLevelLoad);
				headsUpDisplay.text = "You made it!\n" + "In " + timeToFinishThisLevel + " seconds";

			} else {
				headsUpDisplay.text = "Lives:" + getLives () + "  Distance:" + distanceToDestination.ToString ("F1"); 
				if (piratesStartChase && piratesStopChase)
					headsUpDisplay.text += "\nThe pirates gave up!";
			}
		}

		// Move to next scene once destination reached.
		if (reachedStoppingPoint && Input.GetKeyDown (KeyCode.Space)) {
			unfreeze ();
			if (nameOfNextScene == "End") {
				setLives (3); // Reset # of lives for next time they play			
				setTimeTaken (getTimeTaken () + timeToFinishThisLevel);
				Debug.Log("Time to reach freighter and then return to bakery: " + getTimeTaken());
			} else {
				setTimeTaken (timeToFinishThisLevel);
			}
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

	// The pirates chase Alice. Their movement code, below, is closely based on the 
	// Boids flocking pseudocode at http://www.kfish.org/boids/pseudocode.html
	// and the Unity Boids implementation at http://wiki.unity3d.com/index.php?title=Flocking
	// Basically, any bugs and hackiness are my fault but all good ideas came from those two links. 
	private void pirateMovements() 
	{
		if (!piratesStartChase)
			return;

		float maxSpeed = 1.7f + (difficulty-1)/3; 
		if (difficulty == 3)
			maxSpeed += 1f;
		if (difficulty == 4)
			maxSpeed += 2f;
		if (difficulty == 5)
			maxSpeed += 3f;
	
		// No need to adjust velocity every frame. 
		if(Time.frameCount % 10 != 0) return;

		foreach (GameObject p in pirates)
		{
			Rigidbody2D r = p.transform.GetComponent<Rigidbody2D> ();

			Vector2 v = new Vector2 ();

			if (piratesStopChase) {
				r.velocity = v;
			} else {				
				v += 0.3f*boidsAttractionToFlockCenter (p);
				v += 5.4f * boidsDistancingFromOtherBoids(p);
				v += 0.4f * boidsVelocityMatchingWithOtherBoids (p);

				Vector2 towardsAlice = (Vector2)aliceShip.transform.position - (Vector2)p.transform.position;

				// This is the part that makes the pirates chase her.
				float chaseFactor = ((difficulty / 5.5f) + 2.4f);
				if (difficulty >= 3) {
					chaseFactor += difficulty / 3.5f;
				}
				float d = Vector3.Distance (aliceShip.transform.position, p.transform.position);
				if (d > 10) {				
					Debug.Log ("Adding catchup boost since distance from pirate to alice = " + d);
					chaseFactor += 1.5f;
					if (difficulty >= 3)
						chaseFactor += difficulty/2f;
				}
				v += chaseFactor * towardsAlice;				

				Vector2 randomization = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
				v += 0.5f * randomization;

				r.velocity = r.velocity + v * Time.deltaTime;
				float speed = r.velocity.magnitude;
				if (speed > maxSpeed) {
					Debug.Log ("Speedlimiting " + speed + " to " + maxSpeed); 
					r.velocity = maxSpeed * r.velocity.normalized;
				}
			}
		}		
	}

	private Vector2 boidsAttractionToFlockCenter(GameObject thisPirate)
	{
		Vector2 flockCenter = new Vector2();
		foreach (GameObject otherPirate in pirates) {
			if (thisPirate != otherPirate) {
				flockCenter += (Vector2) otherPirate.transform.position;
			}
		}
		flockCenter /= (pirates.Length-1);

		return flockCenter - (Vector2) thisPirate.transform.position;
	}

	private Vector2 boidsDistancingFromOtherBoids(GameObject thisPirate)
	{
		Vector2 c = new Vector2();

		foreach (GameObject otherPirate in pirates) {
			if (thisPirate != otherPirate) {
				Vector2 otherPiratePos = (Vector2) otherPirate.transform.position;
				Vector2 thisPiratePos = (Vector2)thisPirate.transform.position;
				if (Vector2.Distance(thisPiratePos, otherPiratePos) < 3) {
					Vector2 moveAway = thisPiratePos - otherPiratePos;
					c = c + moveAway;
				}					
			}
		}

		return c;
	}

	private Vector2 boidsVelocityMatchingWithOtherBoids(GameObject thisPirate)
	{
		Vector2 flockVelocity = new Vector2();

		foreach (GameObject otherPirate in pirates) {
			if (thisPirate != otherPirate) {
				flockVelocity += otherPirate.GetComponent<Rigidbody2D>().velocity; 
			}
		}

		flockVelocity /= (pirates.Length-1);

		return flockVelocity - thisPirate.GetComponent<Rigidbody2D>().velocity;
	}

}
