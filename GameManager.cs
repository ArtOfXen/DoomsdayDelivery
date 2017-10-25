using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	
	public GameObject vehicle;
	public GameObject zombie;

	public GameObject UI;
	public GameObject moneyUI;
	public GameObject levelCompleteUI;
	public GameObject fadeoutScreenUI;
	public GameObject bulletsUI;
	public GameObject bulletsUIBlocker;
	public GameObject boostUI;
	public GameObject boostUIBlocker;
	public GameObject timeUI;
	public GameObject timeUIBlocker;
	public GameObject gameOverUI;
	public GameObject readyUI;
	public GameObject goUI;
	public GameObject returnToMenuUI;
	public GameObject returnToMenuImageUI;
	public GameObject rampageUI;
	public GameObject rampageBar;
	public GameObject rampageKillTracker;
	public GameObject pointsIncreaseUI;

	public GameObject packagePrefab;
	public GameObject leftWall, rightWall, topWall, botWall;

	public GameObject background;
	public GameObject underBackground;

	public GameObject[] sets;
	public GameObject[] houses;

	public Sprite genericHouseSprite;


	[HideInInspector] public static int money;
	[HideInInspector] public static bool levelComplete;
	[HideInInspector] public static bool gameInProgress;
	private bool gameOver;

	[HideInInspector] public int numOfDeliveriesRequired;
	[HideInInspector] public int numOfDeliveriesComplete;

	private int maxNumOfEnemies;
	[HideInInspector] public static int currentNumOfEnemies;

	private float timePerLevel;
	private float startTime;
	[HideInInspector] public float timeRemaining;
	private float rampageStartTime;
	private int level;

	private float levelCompleteWaitTime;
	private float levelCompleteTime;

	private float maxBarSize;

	private bool countdownInProgress;
	private bool readyCountdownFinished, goCountdownFinished;



	private GameObject[] packages;
	private GameObject[] enemies;

	private float rampageBarInitialSize;
	private int rampageLossPerFrame;

	private GameObject activeVehicle;
	private GameObject activeUnderBackground;


	// Use this for initialization
	void Awake () {
		timePerLevel = 59f;
		timeRemaining = timePerLevel;

		gameOver = false;

		levelCompleteWaitTime = 2f;

		maxNumOfEnemies = 75;
		currentNumOfEnemies = 0;

		level = 0;

		maxBarSize = boostUI.GetComponent<RectTransform> ().sizeDelta.x;
		rampageBarInitialSize = rampageUI.GetComponent<RectTransform> ().sizeDelta.x;
		rampageLossPerFrame = 200;

		populateWorld ();
	}
	
	// Update is called once per frame
	void Update () {

		if (!gameInProgress && !countdownInProgress && !gameOver) {
			
			if (!readyCountdownFinished) {
				StartCoroutine (countdown (readyUI));
				readyCountdownFinished = true;
			} else if (!goCountdownFinished) {
				StartCoroutine (countdown (goUI));
				goCountdownFinished = true;
			}

			if (readyCountdownFinished && goCountdownFinished) {
				gameInProgress = true;
				startTime = Time.time;
			}

			
		} else if (gameInProgress) {
			GameObject player = GameObject.FindGameObjectWithTag ("Player");

			boostUI.GetComponent<RectTransform>().sizeDelta = new Vector2 (maxBarSize / player.GetComponent<DriveVehicle> ().maxBoost * player.GetComponent<DriveVehicle> ().boost, boostUI.GetComponent<RectTransform>().sizeDelta.y);
			bulletsUI.GetComponent<RectTransform>().sizeDelta = new Vector2 (maxBarSize / player.GetComponent<DriveVehicle> ().maxAmmoCount * player.GetComponent<DriveVehicle> ().ammo, bulletsUI.GetComponent<RectTransform>().sizeDelta.y);

			if (currentNumOfEnemies < maxNumOfEnemies) {
				int numOfNewEnemies = maxNumOfEnemies - currentNumOfEnemies;

				for (int i = 0; i < numOfNewEnemies; i++) {
					enemies[currentNumOfEnemies] = createObject (zombie, true);
					currentNumOfEnemies++;
				}
			}

			if (!levelComplete) {
				if (timeRemaining >= 0) {

					if (!player.GetComponent<DriveVehicle> ().rampage) {
			
						timeRemaining = timePerLevel - (Time.time - startTime);

						if (timeRemaining < 0) {
							timeUI.GetComponent<Text> ().text = "0.00";
						} else {
							timeUI.GetComponent<Text> ().text = timeRemaining.ToString ("F2");
						}
					}

					moneyUI.GetComponent<Text> ().text = money.ToString ();
					checkIfLevelComplete ();

				} else {
					gameInProgress = false;
					StartCoroutine (countdown (gameOverUI, false));
					gameOver = true;
				}

				if (GameObject.FindGameObjectWithTag ("Player").GetComponent<DriveVehicle> ().rampage) {
					rampageUI.GetComponent<RectTransform>().sizeDelta -= new Vector2 (rampageBarInitialSize / rampageLossPerFrame, 0f);
					if (rampageUI.GetComponent<RectTransform>().sizeDelta.x <= 0) {
						endRampage ();
					}
					rampageKillTracker.GetComponent<Text> ().text = GameObject.FindGameObjectWithTag ("Player").GetComponent<DriveVehicle> ().rampageKillCount.ToString ();
				}
			}
			else {
				levelCompleted ();
			}
		}

		if (gameOver) {
			returnToMenuUI.SetActive (true);
			returnToMenuImageUI.SetActive (true);
		}
			
	}

	void populateWorld() {

		levelComplete = false;
		gameInProgress = false;
		countdownInProgress = false;
		readyCountdownFinished = false;
		goCountdownFinished = false;

		currentNumOfEnemies = 0;

		timeUI.GetComponent<Text> ().text = timePerLevel.ToString ("F2");
		boostUI.GetComponent<RectTransform>().sizeDelta = new Vector2 (maxBarSize, boostUI.GetComponent<RectTransform>().sizeDelta.y);
		bulletsUI.GetComponent<RectTransform>().sizeDelta = new Vector2 (maxBarSize, bulletsUI.GetComponent<RectTransform>().sizeDelta.y);

		int numOfDeliveries = level + 3;
		packages = new GameObject[numOfDeliveries];
		enemies = new GameObject[maxNumOfEnemies];

		foreach (GameObject h in houses) {
			h.GetComponent<SpriteRenderer> ().sprite = genericHouseSprite;
		}

		activeVehicle = Instantiate (vehicle);

		SpriteRenderer backgroundSprite = background.GetComponent<SpriteRenderer> ();

		activeUnderBackground = Instantiate (underBackground);
		activeUnderBackground.transform.localScale = new Vector3 (underBackground.transform.localScale.x * backgroundSprite.size.x * 2, underBackground.transform.localScale.y * backgroundSprite.size.y * 2);

		currentNumOfEnemies = placeStartingZombies (enemies);

		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<CameraController> ().leftWall = leftWall;
		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<CameraController> ().rightWall = rightWall;
		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<CameraController> ().topWall = topWall;
		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<CameraController> ().bottomWall = botWall;

		int[] usedSets = new int[sets.Length];
		int numOfSetsUsed = 0;

		for (int i = 0; i < numOfDeliveries; i++) {
			bool newSet = false;
			int setNumber;
			do {
				newSet = true;
				setNumber = Random.Range (0, sets.Length);
				foreach (int s in usedSets) {
					if (s == setNumber) {
						newSet = false;
						break;
					}
				}
			} while (newSet == false);
			usedSets [numOfSetsUsed] = setNumber;

		
			int houseNo;
			do {
				 houseNo = Random.Range (0, houses.Length);
			} while (houses [houseNo].transform.GetChild(0).gameObject.GetComponent<HouseScript> ().used == true);

			houses [houseNo].transform.GetChild(0).gameObject.GetComponent<HouseScript> ().used = true;

			GameObject newPackage = createObject (packagePrefab, false, true);

			houses [houseNo].transform.GetChild(0).gameObject.GetComponent<HouseScript> ().setNumber = setNumber;
			newPackage.GetComponent<PackageScript> ().setNumber = setNumber;

			houses [houseNo].GetComponent<SpriteRenderer> ().sprite = sets [setNumber].transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite;
			newPackage.GetComponent<SpriteRenderer> ().sprite = sets [setNumber].transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite;

			packages [numOfSetsUsed] = newPackage;

			numOfSetsUsed++;

			if (numOfSetsUsed >= sets.Length) {
				break;
			}
		}

		numOfDeliveriesRequired = numOfSetsUsed;
		numOfDeliveriesComplete = 0;

		for (int i = currentNumOfEnemies; i < maxNumOfEnemies; i++) {
			GameObject newEnemy = createObject (zombie, false);
			enemies [i] = newEnemy;
			currentNumOfEnemies = i;
		}


		endRampage ();
	}

	Vector3 getRandomPositionInBounds(GameObject leftWall, GameObject rightWall, GameObject topWall, GameObject botWall, GameObject newObject) {
		float x, y;

		do {
			x = Random.Range (
				leftWall.transform.position.x + leftWall.GetComponent<BoxCollider2D> ().bounds.extents.x + newObject.GetComponent<SpriteRenderer> ().bounds.extents.x,
				rightWall.transform.position.x - rightWall.GetComponent<BoxCollider2D> ().bounds.extents.x - newObject.GetComponent<SpriteRenderer> ().bounds.extents.x);
		} while (x > activeVehicle.transform.position.x - activeVehicle.GetComponent<BoxCollider2D> ().bounds.extents.x &&
		         x < activeVehicle.transform.position.x + activeVehicle.GetComponent<BoxCollider2D> ().bounds.extents.x);

		do {
			y = Random.Range (
				botWall.transform.position.y + botWall.GetComponent<BoxCollider2D> ().bounds.extents.y + newObject.GetComponent<SpriteRenderer> ().bounds.extents.y,
				topWall.transform.position.y - topWall.GetComponent<BoxCollider2D> ().bounds.extents.y - newObject.GetComponent<SpriteRenderer> ().bounds.extents.y);
		} while (y > activeVehicle.transform.position.y - activeVehicle.GetComponent<BoxCollider2D> ().bounds.extents.y &&
		         y < activeVehicle.transform.position.y + activeVehicle.GetComponent<BoxCollider2D> ().bounds.extents.y);
		

		return new Vector3 (x, y, 0f);
	}
		

	void checkIfLevelComplete() {
		if (numOfDeliveriesComplete >= numOfDeliveriesRequired && !levelComplete) {
			levelComplete = true;
			levelCompleteUI.SetActive (true);
			levelCompleteTime = Time.time;
			StartCoroutine (fadeoutScreen ());
		}
	}

	void levelCompleted() {

		if (levelComplete) {
			gameInProgress = false;
			if (Time.time - levelCompleteTime >= levelCompleteWaitTime) {
				StopCoroutine (fadeoutScreen ());

				fadeoutScreenUI.GetComponent<Image> ().color = new Color(0f, 0f, 0f, 0f);
				levelCompleteUI.SetActive(false);

				foreach (GameObject p in packages) {
					Destroy (p);
				}

				GameObject[] projectiles = GameObject.FindGameObjectsWithTag ("Projectile");
				foreach (GameObject pr in projectiles) {
					Destroy (pr);					
				}

				GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
				foreach (GameObject e in enemies) {
					Destroy (e);
				}
					
				Destroy (activeVehicle);

				levelCompleteUI.SetActive (false);
				levelComplete = false;
				level++;
				populateWorld ();
			}
		}
	}

	IEnumerator fadeoutScreen() {
		for (int i = 0; i < 255; i += 17) {
			fadeoutScreenUI.GetComponent<Image> ().color = new Color(0, 0, 0, i / 255f);
			yield return null;
		}
	}

	int placeStartingZombies(GameObject[] zombieArray) {
		float radius = activeVehicle.GetComponent<BoxCollider2D> ().bounds.size.y * 3;
		int numOfZombiesInArray = 0;

		if (activeVehicle != null) {
			for (int i = 0; i < 360; i+= 36) {
				GameObject newZombie = Instantiate (zombie);
				newZombie.transform.position = new Vector3 (activeVehicle.transform.position.x + radius * Mathf.Sin ((float)i), activeVehicle.transform.position.y + radius * Mathf.Cos ((float)i));
				zombieArray [numOfZombiesInArray] = newZombie;
				numOfZombiesInArray++;
			}
		}

		return numOfZombiesInArray;
	}

	GameObject createObject(GameObject obj, bool spawnOutsideOfCameraBounds = false, bool spawnAwayFromHouses = false) {
		CameraController cam = Camera.main.GetComponent<CameraController> ();
		float camExtentX = cam.cameraExtentX;
		float camExtentY = cam.cameraExtentY;

		GameObject newObj = Instantiate (obj);



		bool validPlacement;

			do {
				validPlacement = true;
				newObj.transform.position = getRandomPositionInBounds (cam.leftWall, cam.rightWall, cam.topWall, cam.bottomWall, newObj);

				if ((newObj.transform.position.x <= activeVehicle.transform.position.x + activeVehicle.GetComponent<BoxCollider2D>().bounds.size.y * 3
						&& newObj.transform.position.x >= activeVehicle.transform.position.x - activeVehicle.GetComponent<BoxCollider2D>().bounds.size.y * 3)
					|| (newObj.transform.position.y <= activeVehicle.transform.position.y + activeVehicle.GetComponent<BoxCollider2D>().bounds.size.y * 3
						&& newObj.transform.position.y >= activeVehicle.transform.position.y - activeVehicle.GetComponent<BoxCollider2D>().bounds.size.y * 3)) 
				{
					validPlacement = false;
				}

				if (validPlacement && spawnOutsideOfCameraBounds) {
					if ((newObj.transform.position.x <= cam.gameObject.transform.position.x + camExtentX
						&& newObj.transform.position.x >= cam.gameObject.transform.position.x - camExtentX)
						|| (newObj.transform.position.y <= cam.gameObject.transform.position.y + camExtentY
							&& newObj.transform.position.y >= cam.gameObject.transform.position.y - camExtentY)) {
						validPlacement = false;
					}
				}

				if (validPlacement) {
					foreach (GameObject h in houses) {
				
					if (isColliding(newObj, h)) {
						validPlacement = false;
						break;
						}

					if (spawnAwayFromHouses && isColliding(newObj, h.transform.GetChild(0).gameObject)) {
						validPlacement = false;
						break;
					}
					}
				}


			} while (!validPlacement);

		return newObj;
	}

	public bool isColliding(GameObject firstObject, GameObject secondObject){

		BoxCollider2D firstCollider = firstObject.GetComponent<BoxCollider2D> ();
		BoxCollider2D secondCollider = secondObject.GetComponent<BoxCollider2D> ();

		if ((firstObject.transform.position.x + firstCollider.bounds.extents.x >= secondObject.transform.position.x - secondCollider.bounds.extents.x
			&& firstObject.transform.position.x - firstCollider.bounds.extents.x <= secondObject.transform.position.x + secondCollider.bounds.extents.x)
			&& (firstObject.transform.position.y + firstCollider.bounds.extents.y >= secondObject.transform.position.y - secondCollider.bounds.extents.y
			&& firstObject.transform.position.y - firstCollider.bounds.extents.y <= secondObject.transform.position.y + secondCollider.bounds.extents.y))
		{
			return true;
		} 

		if ((firstObject.transform.position.y + firstCollider.bounds.extents.y >= secondObject.transform.position.y - secondCollider.bounds.extents.y
			&& firstObject.transform.position.y - firstCollider.bounds.extents.y <= secondObject.transform.position.y + secondCollider.bounds.extents.y)
			&& (firstObject.transform.position.x + firstCollider.bounds.extents.x >= secondObject.transform.position.x - secondCollider.bounds.extents.x
			&& firstObject.transform.position.x - firstCollider.bounds.extents.x <= secondObject.transform.position.x + secondCollider.bounds.extents.x)) 
		{
			return true;
		}
			
		return false;
	}



	IEnumerator countdown(GameObject obj, bool deleteOnCompletion = true) {
		Vector3 initialScale = obj.transform.localScale;

		countdownInProgress = true;

		obj.SetActive (true);
		for (int i = 1; i <= 50; i++) {
			obj.transform.localScale = initialScale * (i * 2);
			yield return null;
		}

		yield return new WaitForSeconds (0.5f);

		if (deleteOnCompletion) {
			obj.SetActive (false);
			obj.transform.localScale = initialScale;
		}

		countdownInProgress = false;
	}

	public void returnToMenu() {
		Application.LoadLevel ("StartMenu");
	}

	public void startRampage() {
		checkIfLevelComplete ();
		if (!levelComplete) {
			if (GameObject.FindGameObjectWithTag ("Player").GetComponent<DriveVehicle> ().rampage) {
				rampageUI.GetComponent<RectTransform> ().sizeDelta = new Vector2 (rampageBarInitialSize, rampageUI.GetComponent<RectTransform> ().sizeDelta.y);
			} else {
				rampageStartTime = Time.time;
				boostUIBlocker.SetActive (true);
				bulletsUIBlocker.SetActive (true);
				rampageUI.SetActive (true);
				rampageUI.GetComponent<RectTransform> ().sizeDelta = new Vector2 (rampageBarInitialSize, rampageUI.GetComponent<RectTransform> ().sizeDelta.y);
				rampageBar.SetActive (true);
				rampageKillTracker.SetActive (true);
				timeUIBlocker.SetActive (true);
				GetComponent<AudioSource> ().pitch = 1.5f;

				GameObject.FindGameObjectWithTag ("Player").GetComponent<DriveVehicle> ().startRampage ();
			}

		}
	}

	public void endRampage() {
		float pauseDuration = Time.time - rampageStartTime;
		startTime += pauseDuration;

		boostUIBlocker.SetActive (false);
		bulletsUIBlocker.SetActive (false);
		rampageUI.SetActive (false);
		rampageUI.GetComponent<RectTransform>().sizeDelta = new Vector2 (rampageBarInitialSize, rampageUI.GetComponent<RectTransform>().sizeDelta.y);
		rampageBar.SetActive (false);
		timeUIBlocker.SetActive (false);
		StartCoroutine (addRampageKillsToScore ());
		GetComponent<AudioSource> ().pitch = 1f;

		GameObject.FindGameObjectWithTag ("Player").GetComponent<DriveVehicle> ().endRampage ();
	}

	public void extendRampage() {
		if (rampageUI.GetComponent<RectTransform>().sizeDelta.x < rampageBarInitialSize) {
			rampageUI.GetComponent<RectTransform>().sizeDelta += new Vector2 ((rampageBarInitialSize / rampageLossPerFrame) * 8, 0f);
		}

		if (rampageUI.GetComponent<RectTransform>().sizeDelta.x > rampageBarInitialSize) {
			rampageUI.GetComponent<RectTransform>().sizeDelta = new Vector2 (rampageBarInitialSize, rampageUI.GetComponent<RectTransform>().sizeDelta.y);
		}
	}

	IEnumerator addRampageKillsToScore(){
		Vector3 startLoc = rampageKillTracker.transform.position;
		rampageKillTracker.GetComponent<Text> ().color = new Color (236f / 255f, 237f / 255f, 30f / 255f, 1f);
		while (rampageKillTracker.transform.position.y < moneyUI.transform.position.y) {
			rampageKillTracker.transform.position += new Vector3 (0f, 10f);
			yield return null;
		}

		money += GameObject.FindGameObjectWithTag ("Player").GetComponent<DriveVehicle> ().rampageKillCount;
		rampageKillTracker.SetActive (false);
		rampageKillTracker.GetComponent<Text> ().color = new Color (1, 0, 0, 1);
		rampageKillTracker.transform.position = startLoc;
	}

	public IEnumerator addDeliveryPoints(int pointIncrease) {
		pointsIncreaseUI.SetActive (true);
		pointsIncreaseUI.GetComponent<Text> ().text = "+" + pointIncrease.ToString ();

		yield return new WaitForSeconds (2f);

		pointsIncreaseUI.SetActive (false);
	}
}
