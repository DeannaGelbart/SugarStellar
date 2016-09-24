using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

/* Main logic for survival mode where the player tries to survive as long as possible. */
public class SurvivalModeMain : MonoBehaviour
{

	/* Visible objects in the scene. */
	public GameObject[] prototypeAsteroids;
	public GameObject prototypePirate;
	public int numberOfAsteroidsBeforeDifficultyModifier;
	public int numberOfPiratesPerFlockBeforeDifficultyModifier;
	public int numberOfPirateFlocks;
	public GameObject prototypeStar;
	public int numberOfStars;
	public GameObject aliceShip;
	public GameObject startingPoint;
	public TextMesh headsUpDisplay;

	public AudioSource music;

	/* The walls prevent the asteroids from drifting out too far. */
	public GameObject leftWall;
	public GameObject rightWall;
	public GameObject topWall;
	public GameObject bottomWall;
	private float maxX;
	private float minX;
	private float maxY;
	private float minY;

	private GameObject[][] pirateFlocks;
	private int difficulty = 5;
	private int timeToFinishThisLevel = 0;
	private PersistentValue timeTakenHolder;
	private bool reachedStoppingPoint = false; 
	private float maxPirateSpeed;
	private bool startChase = false;


	void Start ()
	{
		GameObject foundGO = GameObject.Find ("Difficulty Persistence");
		if (foundGO) {			
			difficulty = foundGO.GetComponent<PersistentValue> ().value;
		}	
		Debug.Log ("Difficulty: " + difficulty);

		foundGO = GameObject.Find ("Time Taken Persistence");
		if (foundGO) {
			timeTakenHolder = foundGO.GetComponent<PersistentValue> ();
			Debug.Log ("Time taken: " + getTimeTaken ());
		} 	
			
		maxX = rightWall.transform.position.x; 
		minX = leftWall.transform.position.x;
		maxY = topWall.transform.position.y;
		minY = bottomWall.transform.position.y; 

		maxPirateSpeed = 1.7f + (difficulty-1)/3; 
		if (difficulty == 3)
			maxPirateSpeed += 1f;
		if (difficulty == 4)
			maxPirateSpeed += 1f;
		if (difficulty == 5)
			maxPirateSpeed += 1f;

		initializeAsteroids ();
		initializeStars ();
		initializePirates ();

			}


	void FixedUpdate ()
	{
		// Once the player ship has moved, the pirates start chasing
		if (!startChase)
			if (aliceShip.transform.GetComponent<Rigidbody2D> ().velocity.magnitude > 0f)
				startChase = true;

		pirateMovements ();
	}

	void Update ()
	{
		if (Input.GetKey("escape"))
			Application.Quit();

		// Move to next scene once destination reached.
		if (reachedStoppingPoint && Input.GetKeyDown (KeyCode.Space)) {
			unfreeze ();
			SceneManager.LoadScene ("Insert Coin"); 
		}


		headsUpDisplay.text = "SURVIVED FOR " + Time.timeSinceLevelLoad.ToString("F1") + " SECONDS\nPIRATE SPEED BOOST:" + pirateSpeedBoost() .ToString ("F1");
	}

	public void playerGotHit ()
	{
		freeze ();
		reachedStoppingPoint = true;

		timeToFinishThisLevel = (int) Mathf.Round(Time.timeSinceLevelLoad);
		headsUpDisplay.text = "You survived !\n" + "In " + timeToFinishThisLevel + " seconds";
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


	private void initializeStars ()
	{
		GameObject[] stars = new GameObject[numberOfStars];

		for (int i = 0; i < stars.Length; i++) {
			stars [i] = Instantiate (prototypeStar);
			Transform t = stars [i].transform;

			t.position = new Vector3 (Random.Range (minX, maxX), Random.Range (minY, maxY), 0);
			t.position *= 1.5f; // spread the stars wider than the asteroid belt
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
			modifier = 1.1f;
		else if (difficulty == 5)
			modifier = 1.2f;

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

	private void initializePirates ()
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

	
		int numberOfPiratesPerFlock = (int)(numberOfPiratesPerFlockBeforeDifficultyModifier * modifier);
		Debug.Log (numberOfPiratesPerFlock + " pirates per flock over " + numberOfPirateFlocks + " flocks");

		pirateFlocks = new GameObject[numberOfPirateFlocks][];
		for (int f = 0; f < numberOfPirateFlocks; f++) {			
			GameObject[] flock = new GameObject[numberOfPiratesPerFlock];
			pirateFlocks [f] = flock;
					
			for (int i = 0; i < flock.Length; i++) {
				flock [i] = Instantiate (prototypePirate);

				Transform t = flock [i].transform;
				t.position = aliceShip.transform.position;			
				float angle = (360f / flock.Length) * i;
				Vector2 offset = MathUtilities.getPointOnCircle (angle, Random.Range(12.5f, 20f));
				t.position = new Vector3 (t.position.x + offset.x, 
					t.position.y + offset.y, t.position.z);
			}
			 
		}
	}

	private float pirateSpeedBoost() 
	{



		float boost = Time.timeSinceLevelLoad / 4f;
		if (boost > 5f)
			boost = 5f;

		//return boost;
		return 0f; 
	}

	// The pirates chase Alice. Their movement code, below, is closely based on the 
	// Boids flocking pseudocode at http://www.kfish.org/boids/pseudocode.html
	// and the Unity Boids implementation at http://wiki.unity3d.com/index.php?title=Flocking
	// Basically, any bugs and hackiness are my fault but all good ideas came from those two links. 
	private void pirateMovements() 
	{

		if (!startChase)
			return;

		// No need to adjust velocity every frame. 
		if(Time.frameCount % 10 != 0) return;

		float maxSpeed = maxPirateSpeed + pirateSpeedBoost (); 

		for (int f = 0; f < numberOfPirateFlocks; f++) {			
			GameObject[] pirateFlock = pirateFlocks [f];
			foreach (GameObject p in pirateFlock)
			{
				Rigidbody2D r = p.transform.GetComponent<Rigidbody2D> ();

				Vector2 v = new Vector2 ();

				v += 0.3f*boidsAttractionToFlockCenter (p, pirateFlock);
				v += 5.9f * boidsDistancingFromOtherBoids(p, pirateFlock);
				v += 0.3f * boidsVelocityMatchingWithOtherBoids (p, pirateFlock);

				Vector2 towardsAlice = (Vector2)aliceShip.transform.position - (Vector2)p.transform.position;
				towardsAlice.Normalize (); // CHANGE: Pirates in survival start further away from Alice than in story mode, so normalize this vector.

				// This is the part that makes the pirates chase her.
				float chaseFactor = ((difficulty / 5.5f) + 11.4f);
				if (difficulty >= 3) {
					chaseFactor += difficulty / 6.5f;
				}
				float d = Vector3.Distance (aliceShip.transform.position, p.transform.position);
				if (d > 10 && Time.timeSinceLevelLoad > 5f) {				
					Debug.Log ("Adding catchup boost since distance from pirate to alice = " + d);
					chaseFactor += 1.5f;
					if (difficulty >= 3)
						chaseFactor += difficulty/5f;
				}
				v += chaseFactor * towardsAlice;				

				Vector2 randomization = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
				v += 0.2f * randomization;

				// CHANGE: this was
				//r.velocity = r.velocity + v * Time.deltaTime;
				float smoothingAlpha = 0.1f;
				r.velocity = (1f - smoothingAlpha) * r.velocity + smoothingAlpha * v;

				float speed = r.velocity.magnitude;
				if (speed > maxSpeed) {
					Debug.Log ("Speedlimiting " + speed + " to " + maxSpeed); 
					r.velocity = maxSpeed * r.velocity.normalized;
				}
			}
		}		
	}

	private Vector2 boidsAttractionToFlockCenter(GameObject thisPirate, GameObject[] pirateFlock)
	{
		Vector2 flockCenter = new Vector2();
		foreach (GameObject otherPirate in pirateFlock) {
			if (thisPirate != otherPirate) {
				flockCenter += (Vector2) otherPirate.transform.position;
			}
		}
		flockCenter /= (pirateFlock.Length-1);

		return flockCenter - (Vector2) thisPirate.transform.position;
	}

	private Vector2 boidsDistancingFromOtherBoids(GameObject thisPirate, GameObject[] pirateFlock)
	{
		Vector2 c = new Vector2();

		foreach (GameObject otherPirate in pirateFlock) {
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

	private Vector2 boidsVelocityMatchingWithOtherBoids(GameObject thisPirate, GameObject[] pirateFlock)
	{
		Vector2 flockVelocity = new Vector2();

		foreach (GameObject otherPirate in pirateFlock) {
			if (thisPirate != otherPirate) {
				flockVelocity += otherPirate.GetComponent<Rigidbody2D>().velocity; 
			}
		}

		flockVelocity /= (pirateFlock.Length-1);

		return flockVelocity - thisPirate.GetComponent<Rigidbody2D>().velocity;
	}

}
