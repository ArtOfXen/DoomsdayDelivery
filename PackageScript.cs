using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageScript : MonoBehaviour {

	[HideInInspector] public int setNumber;
	private bool pickupable;

	// Use this for initialization
	void Start () {
		pickupable = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameManager> ().timeRemaining < 59f) {
			pickupable = true;
		}
		// check if out of bounds

		//GameManager gm = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameManager> ();

		/*if (transform.position.y > gm.background.GetComponent<SpriteRenderer> ().bounds.extents.y ||
		    transform.position.y < -gm.background.GetComponent<SpriteRenderer> ().bounds.extents.y ||
		    transform.position.x > gm.background.GetComponent<SpriteRenderer> ().bounds.extents.x ||
		    transform.position.x < -gm.background.GetComponent<SpriteRenderer> ().bounds.extents.x) {

			if (transform.parent != null) {
				if (transform.parent.tag == "Enemy") {
					dropPackage ();
				}
			}

			if (transform.position.y > gm.background.GetComponent<SpriteRenderer> ().bounds.extents.y) {
				transform.position = new Vector3 (0f, 14.6f);
			}

			if (transform.position.y < -gm.background.GetComponent<SpriteRenderer> ().bounds.extents.y) {
				transform.position = new Vector3 (0f, -14.6f);
			}

			if (transform.position.x > gm.background.GetComponent<SpriteRenderer> ().bounds.extents.x) {
				transform.position = new Vector3 (14.6f, 0f);
			}
			if (transform.position.x < -gm.background.GetComponent<SpriteRenderer> ().bounds.extents.x) {
				transform.position = new Vector3 (-14.6f, 0f);
			}
		}*/


	}

	void OnCollisionEnter2D(Collision2D other) {

		if (pickupable) {
			if (other.collider.tag == "Player" || other.collider.tag == "Enemy") {
				float yPosition;
				if (other.collider.tag == "Player") {
					if (other.gameObject.GetComponent<DriveVehicle> ().carryingPackage == false) {
						yPosition = -(GetComponent<BoxCollider2D> ().bounds.extents.y / 2);
						GetComponent<AudioSource> ().Play ();
						pickup (other.gameObject, yPosition);
					}
				}

				if (other.collider.tag == "Enemy") {
					if (other.gameObject.GetComponent<EnemyMovement> ().carryingPackage == false) {
						if (transform.parent != null) {
							if (transform.parent.tag != "Enemy") {
								yPosition = GetComponent<BoxCollider2D> ().bounds.extents.y / 2;

								pickup (other.gameObject, yPosition);
							}
						} else {
							yPosition = GetComponent<BoxCollider2D> ().bounds.extents.y / 2;

							pickup (other.gameObject, yPosition);
						}


					}
				}
			}
		}
	}
		
	public void pickup(GameObject pickupper, float yPos) {
		if (transform.parent != null) {
			dropPackage ();
		}

		if (pickupper.tag == "Player") {
			pickupper.GetComponent<DriveVehicle> ().carryingPackage = true;
		} else if (pickupper.tag == "Enemy") {
			pickupper.GetComponent<EnemyMovement> ().carryingPackage = true;
		}
	
		GetComponent<Rigidbody2D> ().velocity = Vector3.zero;
		GetComponent<Rigidbody2D> ().angularVelocity = 0f;
		transform.SetParent (pickupper.transform);
		transform.localPosition = new Vector3 (0f, yPos, 0f);
		transform.localRotation = Quaternion.Euler (Vector3.zero);
		Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), pickupper.GetComponent<BoxCollider2D> ());
		GetComponent<Rigidbody2D> ().bodyType = RigidbodyType2D.Kinematic;
	}

	public void dropPackage() {
		if (transform.parent.tag == "Player") {
			transform.parent.gameObject.GetComponent<DriveVehicle> ().carryingPackage = false;
		} 
		else if (transform.parent.tag == "Enemy") {
			transform.parent.gameObject.GetComponent<EnemyMovement> ().carryingPackage = false;
		}

		Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), transform.parent.GetComponent<BoxCollider2D> (), false);
		transform.SetParent (null);
		GetComponent<BoxCollider2D> ().enabled = false;
		GetComponent<BoxCollider2D> ().enabled = true;
		GetComponent<Rigidbody2D> ().bodyType = RigidbodyType2D.Dynamic;
	}

}
