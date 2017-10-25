using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuVehicle : MonoBehaviour {

	public GameObject explosion;

	private int spinExplosionCounter;
	private int maxSpinExplosionCounter;
	private bool spinning;

	// Use this for initialization
	void Start () {
		spinExplosionCounter = 0;
		maxSpinExplosionCounter = 15;
		spinning = false;
	}
	
	// Update is called once per frame
	void Update () {

		float angle = transform.eulerAngles.z;

		Vector3 bulletPositionAdjustment = new Vector3 
			((GetComponent<BoxCollider2D> ().bounds.extents.x / 2) * Mathf.Cos(Mathf.Deg2Rad * angle), (GetComponent<BoxCollider2D> ().bounds.extents.x / 2) * Mathf.Sin(Mathf.Deg2Rad * angle));
		Vector3 backOfVehicle = new Vector3 (transform.position.x + (GetComponent<BoxCollider2D> ().bounds.extents.y * Mathf.Sin (Mathf.Deg2Rad * angle)), 
			transform.position.y - (GetComponent<BoxCollider2D> ().bounds.extents.y * Mathf.Cos (Mathf.Deg2Rad * angle)));

		if (Input.GetKeyDown (KeyCode.Z)) {

			GameObject leftExplosion = Instantiate (explosion, backOfVehicle + bulletPositionAdjustment, Quaternion.Euler (Vector3.zero));
			GameObject rightExplosion = Instantiate (explosion, backOfVehicle - bulletPositionAdjustment, Quaternion.Euler (Vector3.zero));
		}

		if (Input.GetKey (KeyCode.X) && !spinning) {
			if (Input.GetKey (KeyCode.RightArrow)) {
				StartCoroutine (spin (-1));
			} else {
				StartCoroutine (spin (1));
			}
		}

		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)) {

			if (Input.GetKey(KeyCode.LeftArrow)) {
				transform.Rotate (0f, 0f, 5f);
			} else if (Input.GetKey(KeyCode.RightArrow)) {
				transform.Rotate (0f, 0f, -5f);
			}
		}
	}

	IEnumerator spin (int directionMultiplier) {
		spinning = true;
		for (float i = 0; i < 180; i += 10) {
			transform.Rotate (new Vector3 (0f, 0f, 10f * directionMultiplier));
	
			yield return null;
		}
		spinning = false;
	}
}
