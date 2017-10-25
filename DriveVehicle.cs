using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DriveVehicle : MonoBehaviour {

	[HideInInspector] public bool carryingPackage;

	public GameObject bullet;
	public GameObject explosion;

	[HideInInspector] public int ammo;
	[HideInInspector] public int boost;
	[HideInInspector] public int maxAmmoCount;
	[HideInInspector] public int maxBoost;
	[HideInInspector] public bool rampage;
	[HideInInspector] public int rampageKillCount;

	public float accelerationValue;

	private float spinBoostTime;
	private float spinTime;
	private float boostCooldown;
	private float boostTime;

	private bool spinning;



	// Use this for initialization
	void Start () {
		rampage = false;
		rampageKillCount = 0;
		maxBoost = 200;
		maxAmmoCount = 30;
		refillAmmo();
		refillBoost ();
		spinning = false;
		spinBoostTime = 0.5f;
		boostTime = 0f;
		boostCooldown = 0.25f;

	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (GameManager.gameInProgress) {
			float angle = transform.eulerAngles.z;

			float currentForceX = GetComponent<Rigidbody2D> ().velocity.x;
			float currentForceY = GetComponent<Rigidbody2D> ().velocity.y;

			if (boost < maxBoost) {
					boost++;
			}

			// boost
			if (Input.GetKey (KeyCode.Z) && spinning == false && (boost >= 40 || rampage)) {
				
				if (boostCooldown <= Time.time - boostTime) {
					boostTime = Time.time;

					GetComponent<AudioSource> ().Play ();

					Vector3 bulletPositionAdjustment = new Vector3 
						((GetComponent<BoxCollider2D> ().bounds.extents.x / 2) * Mathf.Cos(Mathf.Deg2Rad * angle), (GetComponent<BoxCollider2D> ().bounds.extents.x / 2) * Mathf.Sin(Mathf.Deg2Rad * angle));
					Vector3 backOfVehicle = new Vector3 (transform.position.x + (GetComponent<BoxCollider2D> ().bounds.extents.y * Mathf.Sin (Mathf.Deg2Rad * angle)), 
						transform.position.y - (GetComponent<BoxCollider2D> ().bounds.extents.y * Mathf.Cos (Mathf.Deg2Rad * angle)));

					GameObject leftExplosion = Instantiate (explosion, backOfVehicle + bulletPositionAdjustment, Quaternion.Euler (Vector3.zero));
					GameObject rightExplosion = Instantiate (explosion, backOfVehicle - bulletPositionAdjustment, Quaternion.Euler (Vector3.zero));

					if (!rampage) {
						boost -= 40;
					}
					float forceIncrease = accelerationValue;

					if (Time.time - spinTime <= spinBoostTime) {
						forceIncrease = forceIncrease * 2;
					}

					if (rampage) {
						forceIncrease = forceIncrease * 3;
					}

					GetComponent<Rigidbody2D> ().AddForce (new Vector2 (
						-forceIncrease * Mathf.Sin (Mathf.Deg2Rad * angle), 
						forceIncrease * Mathf.Cos (Mathf.Deg2Rad * angle)));

					if (ammo > 0 || rampage) {
						

						GameObject leftNewBullet = Instantiate (bullet, backOfVehicle + bulletPositionAdjustment, Quaternion.Euler (transform.rotation.eulerAngles + new Vector3 (0f, 0f, 180f)));
						GameObject rightNewBullet = Instantiate (bullet, backOfVehicle - bulletPositionAdjustment, Quaternion.Euler (transform.rotation.eulerAngles + new Vector3 (0f, 0f, 180f)));

						Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), leftNewBullet.GetComponent<BoxCollider2D> ());
						Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), rightNewBullet.GetComponent<BoxCollider2D> ());

						leftNewBullet.GetComponent<BulletScript> ().force = forceIncrease * 2 / 3;
						rightNewBullet.GetComponent<BulletScript> ().force = forceIncrease * 2 / 3;

						if (!rampage) {
							ammo--;
						}
					}
				}
			}

			if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)) {

				GetComponent<Rigidbody2D> ().AddForce (new Vector2 (-currentForceX / 20, -currentForceY / 20));

				if (Input.GetKey(KeyCode.LeftArrow)) {
					transform.Rotate (0f, 0f, 4f);
				} else if (Input.GetKey(KeyCode.RightArrow)) {
					transform.Rotate (0f, 0f, -4f);
				}
					
			}

			if (Input.GetKey (KeyCode.X) && !spinning) {

				GetComponent<Rigidbody2D> ().AddForce (new Vector2(-currentForceX, -currentForceY));
				if (Input.GetKey (KeyCode.RightArrow)) {
					StartCoroutine (spin (-1));
				} else {
					StartCoroutine (spin (1));
				}
				GetComponent<AudioSource> ().Play ();
			}
		}
	}

	IEnumerator spin (int directionMultiplier) {
		spinning = true;
		for (float i = 0; i < 180; i += 15) {
			transform.Rotate(new Vector3(0f, 0f, 15f * directionMultiplier));

			float angle = transform.rotation.eulerAngles.z;

			Vector3 bulletPositionAdjustment = new Vector3 
				((GetComponent<BoxCollider2D> ().bounds.extents.x / 2) * Mathf.Cos(Mathf.Deg2Rad * angle), (GetComponent<BoxCollider2D> ().bounds.extents.x / 2) * Mathf.Sin(Mathf.Deg2Rad * angle));
			Vector3 backOfVehicle = new Vector3 (transform.position.x + (GetComponent<BoxCollider2D> ().bounds.extents.y * Mathf.Sin (Mathf.Deg2Rad * angle)), 
				transform.position.y - (GetComponent<BoxCollider2D> ().bounds.extents.y * Mathf.Cos (Mathf.Deg2Rad * angle)));

			GameObject leftExplosion = Instantiate (explosion, backOfVehicle + bulletPositionAdjustment, Quaternion.Euler (Vector3.zero));
			GameObject rightExplosion = Instantiate (explosion, backOfVehicle - bulletPositionAdjustment, Quaternion.Euler (Vector3.zero));


			if (ammo > 0 || rampage) {
				
				GameObject leftNewBullet = Instantiate (bullet, backOfVehicle + bulletPositionAdjustment, Quaternion.Euler (transform.rotation.eulerAngles + new Vector3 (0f, 0f, 180f)));
				GameObject rightNewBullet = Instantiate (bullet, backOfVehicle - bulletPositionAdjustment, Quaternion.Euler (transform.rotation.eulerAngles + new Vector3 (0f, 0f, 180f)));

				Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), leftNewBullet.GetComponent<BoxCollider2D> ());
				Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), rightNewBullet.GetComponent<BoxCollider2D> ());

				leftNewBullet.GetComponent<BulletScript> ().force = accelerationValue;
				rightNewBullet.GetComponent<BulletScript> ().force = accelerationValue;

				if (!rampage) {
					ammo--;
				}
			}

			yield return null;
		}
		spinning = false;
		spinTime = Time.time;
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.collider.tag == "Enemy") 
		{
			float playerVelocity = GetComponent<Rigidbody2D>().velocity.magnitude;
			float enemyVelocity = other.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;

			if (playerVelocity >= other.gameObject.GetComponent<Rigidbody2D>().mass) {
				other.gameObject.GetComponent<EnemyMovement> ().runOverEnemy ();
			} else {
				if (carryingPackage) {
					other.gameObject.GetComponent<EnemyMovement> ().carryingPackage = true;
					float newPackageYPos = transform.GetChild(0).GetComponent<BoxCollider2D> ().bounds.extents.y / 2;

					transform.GetChild(0).GetComponent<PackageScript> ().pickup (other.gameObject, newPackageYPos);

					carryingPackage = false;
				}
			}
		}
	}

	public void refillAmmo() {
		ammo = maxAmmoCount;
	}

	public void refillBoost() {
		boost = maxBoost;
	}

	public void startRampage() {
		rampage = true;
		rampageKillCount = 0;

		refillAmmo ();
		refillBoost ();


	}

	public void endRampage() {
		rampage = false;

		refillAmmo ();
		refillBoost ();
	}

}
