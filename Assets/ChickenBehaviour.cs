using UnityEngine;
using System.Collections;

public class ChickenBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// keep falling
		float fallDistance = Global.get().chickenFallSpeedPerSecond * Time.deltaTime;
		transform.position -= new Vector3(0, fallDistance, 0);
	}

	void OnTriggerEnter2D(Collider2D other) {
		Debug.Log ("chicken colliding with "+other.gameObject.tag + " named: " + other.gameObject.name);
		if (other.gameObject.tag == "Player") {
			Global.controller.blind();
			gameObject.SetActive(false);
		}
	}
}
