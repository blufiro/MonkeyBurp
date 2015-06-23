using UnityEngine;
using System.Collections;

public class SnakeBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other) {
		Debug.Log ("snake colliding with "+other.gameObject.tag + " named: " + other.gameObject.name);
		if (other.gameObject.tag == "Player") {
			Global.controller.loseAllFruits ();
			gameObject.SetActive(false);
		}
	}
}
