using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

	public float moveSpeed;
	public Sprite deathSprite;

	public AudioClip[] runOverSounds;

	private CameraController mainCam;
	[HideInInspector] public bool carryingPackage;
	private bool dead;

	private GameObject[] houses;
	private bool movingAwayFromHouse;

	// Use this for initialization
	void Start () {
		mainCam = Camera.main.GetComponent<CameraController> ();
		carryingPackage = false;
		dead = false;
		movingAwayFromHouse = false;
		houses = GameObject.FindGameObjectsWithTag ("House");
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.gameInProgress) {
			if (!dead) {
				GameObject player = GameObject.FindGameObjectWithTag ("Player");


				if (transform.position.x > mainCam.gameObject.transform.position.x + mainCam.cameraExtentX
				   || transform.position.x < mainCam.gameObject.transform.position.x - mainCam.cameraExtentX
				   || transform.position.y > mainCam.gameObject.transform.position.y + mainCam.cameraExtentY
				   || transform.position.y < mainCam.gameObject.transform.position.y - mainCam.cameraExtentY) {
					GetComponent<BoxCollider2D> ().enabled = false;
					GetComponent<SpriteRenderer> ().enabled = false;
				} else if (GetComponent<BoxCollider2D> ().enabled == false || GetComponent<SpriteRenderer> ().enabled == false) {
					GetComponent<BoxCollider2D> ().enabled = true;
					GetComponent<SpriteRenderer> ().enabled = true;
				}

				if (transform.childCount == 0 && carryingPackage) {
					carryingPackage = false;
				}

				if (carryingPackage) {
					GetComponent<BoxCollider2D> ().enabled = true;
				}

				float angle = transform.rotation.eulerAngles.z;
				if (carryingPackage) {
					if (movingAwayFromHouse) {
						transform.up = Vector3.zero;
					} else {
						transform.up = transform.position - player.transform.position;
					}

					foreach (GameObject h in houses) {
						if (h.transform.GetChild(0).gameObject.GetComponent<HouseScript>().houseLocation == HouseScript.location.edge && 
							GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameManager> ().isColliding (gameObject, h.transform.GetChild (0).gameObject))
						{
							movingAwayFromHouse = true;
							break;
						}
					}
				} else {
					transform.up = player.transform.position - transform.position;

				}

				transform.position += new Vector3 (-Mathf.Sin (Mathf.Deg2Rad * angle) * moveSpeed, Mathf.Cos (Mathf.Deg2Rad * angle) * moveSpeed);
			}

			if (dead && (transform.position.x > mainCam.gameObject.transform.position.x + mainCam.cameraExtentX
			   || transform.position.x < mainCam.gameObject.transform.position.x - mainCam.cameraExtentX
			   || transform.position.y > mainCam.gameObject.transform.position.y + mainCam.cameraExtentY
			   || transform.position.y < mainCam.gameObject.transform.position.y - mainCam.cameraExtentY)) {
				Destroy (gameObject);
			}
		}
	}

	public void shootEnemy() {
		if (carryingPackage) {
			GameObject carriedObject = transform.GetChild (0).gameObject;
			carriedObject.GetComponent<PackageScript> ().dropPackage ();
		}

		GameObject.FindGameObjectWithTag ("Player").GetComponent<DriveVehicle> ().refillBoost ();

		killEnemy();
	}

	public void runOverEnemy() {
		GameObject player = GameObject.FindGameObjectWithTag ("Player");

		if (carryingPackage) {
			GameObject carriedObject = transform.GetChild (0).gameObject;
			float newYPos = -(carriedObject.GetComponent<BoxCollider2D> ().bounds.extents.y / 2);

			if (player.GetComponent<DriveVehicle> ().carryingPackage == false) {
				player.GetComponent<DriveVehicle> ().carryingPackage = true;
				carriedObject.GetComponent<PackageScript> ().pickup (player, newYPos);
			} else {
				carriedObject.GetComponent<PackageScript> ().dropPackage ();}
		}

		player.GetComponent<DriveVehicle> ().refillAmmo();

		killEnemy ();
	}

	void killEnemy() {
		
		int randomSound = Random.Range (0, runOverSounds.Length);
		GetComponent<AudioSource> ().clip = runOverSounds [randomSound];
		GetComponent<AudioSource> ().Play ();

		if (GameObject.FindGameObjectWithTag ("Player").GetComponent<DriveVehicle> ().rampage) {
			GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameManager> ().extendRampage();
			GameObject.FindGameObjectWithTag ("Player").GetComponent<DriveVehicle> ().rampageKillCount++;
		}

		dead = true;			
		GetComponent<Animator> ().enabled = false;
		GetComponent<SpriteRenderer> ().sprite = deathSprite;
		GetComponent<SpriteRenderer> ().sortingOrder = 0;
		GetComponent<BoxCollider2D> ().enabled = false;
		GameManager.currentNumOfEnemies--;

	}
}
