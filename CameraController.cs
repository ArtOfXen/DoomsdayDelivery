using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	private GameObject player;

	[HideInInspector] public GameObject leftWall, rightWall, topWall, bottomWall;

	[HideInInspector] public float cameraExtentY, cameraExtentX;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		cameraExtentY = Camera.main.orthographicSize;
		cameraExtentX = cameraExtentY * Screen.width / Screen.height;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (!GameManager.levelComplete) {
			player = GameObject.FindGameObjectWithTag ("Player");
			if (transform.position.x >= leftWall.transform.position.x + cameraExtentX && player.transform.position.x < transform.position.x) {
				transform.position = new Vector3 (player.transform.position.x, transform.position.y, -10f);
			}

			if (transform.position.x <= rightWall.transform.position.x - cameraExtentX && player.transform.position.x > transform.position.x) {
				transform.position = new Vector3 (player.transform.position.x, transform.position.y, -10f);
			}

			if (transform.position.y >= bottomWall.transform.position.y + cameraExtentY && player.transform.position.y < transform.position.y) {
				transform.position = new Vector3 (transform.position.x, player.transform.position.y, -10f);
			}

			if (transform.position.y <= topWall.transform.position.y - cameraExtentY && player.transform.position.y > transform.position.y) {
				transform.position = new Vector3 (transform.position.x, player.transform.position.y, -10f);
			}
		}
	}
}
