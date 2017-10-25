using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseScript : MonoBehaviour {

	public enum location {edge, notEdge};

	public location houseLocation;
	[HideInInspector] public int setNumber;
	[HideInInspector] public bool used = false;
	// Use this for initialization
	void Start () {
		//used = false;	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay2D(Collider2D other) {
		if (other.tag == "Player") 
		{
			if (other.gameObject.GetComponent<DriveVehicle> ().carryingPackage) {
				if (other.gameObject.GetComponent<DriveVehicle> ().transform.GetChild(0).tag == "Package"
					&& other.gameObject.GetComponent<DriveVehicle> ().transform.GetChild(0).GetComponent<PackageScript> ().setNumber == this.setNumber
					&& other.gameObject.GetComponent<DriveVehicle>().transform.GetChild(0).GetComponent<BoxCollider2D>().IsTouching(GetComponent<BoxCollider2D>())) {
					Destroy (other.gameObject.GetComponent<DriveVehicle> ().transform.GetChild(0).gameObject);
					other.gameObject.GetComponent<DriveVehicle> ().carryingPackage = false;

					transform.parent.gameObject.GetComponent<AudioSource> ().Play ();
					int moneyEarned = 10 + ((int)GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameManager> ().timeRemaining * 2);
					GameManager.money += moneyEarned;
					StartCoroutine (GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameManager> ().addDeliveryPoints (moneyEarned));
					GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameManager> ().numOfDeliveriesComplete++;

					GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameManager> ().startRampage();

				}
			}
		}
	}
}
