using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

	[HideInInspector] public float force;
	private float angle;



	// Use this for initialization
	void Start () {

		Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), GameObject.FindGameObjectWithTag ("Player").GetComponent<BoxCollider2D> ());

		GameObject[] packages = GameObject.FindGameObjectsWithTag ("Package");
		foreach (GameObject package in packages) {
			Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), package.GetComponent<BoxCollider2D> ());
		}

		Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), GameObject.FindGameObjectWithTag ("Package").GetComponent<BoxCollider2D> ());

		if (GameObject.FindGameObjectWithTag ("Player").GetComponent<DriveVehicle> ().carryingPackage) {
			Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), GameObject.FindGameObjectWithTag ("Player").transform.GetChild(0).GetComponent<BoxCollider2D> ());
		}

		angle = transform.rotation.eulerAngles.z;

		GetComponent<Rigidbody2D> ().AddForce (new Vector2 (
			-force * Mathf.Sin (Mathf.Deg2Rad * angle), 
			force * Mathf.Cos (Mathf.Deg2Rad * angle)));

	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.collider.tag != "Player" && !other.transform.IsChildOf(GameObject.FindGameObjectWithTag("Player").transform) && other.collider.tag != "Projectile") {

			if (other.collider.tag == "Enemy") {
				other.gameObject.GetComponent<EnemyMovement> ().shootEnemy ();
			}

			Destroy (gameObject);
		}
	}
}
