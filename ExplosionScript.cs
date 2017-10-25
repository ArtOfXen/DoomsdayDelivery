using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour {

	private GameObject smokeCloud;

	// Use this for initialization
	void Start () {
		smokeCloud = transform.GetChild (0).gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if (GetComponent<SpriteRenderer> ().color.a > 0) {
			GetComponent<SpriteRenderer> ().color -= new Color (0f, 0f, 0f, 0.1f);
		}
		if (smokeCloud.GetComponent<SpriteRenderer> ().color.a > 0) {
			smokeCloud.GetComponent<SpriteRenderer> ().color -= new Color (0f, 0f, 0f, 0.04f);
		}
		smokeCloud.transform.position += new Vector3 (0.02f, 0f, 0f);

		if (GetComponent<SpriteRenderer> ().color.a <= 0 && smokeCloud.GetComponent<SpriteRenderer> ().color.a <= 0) {
			Destroy (gameObject);
		}
	}
}
