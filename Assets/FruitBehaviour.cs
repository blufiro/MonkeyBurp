using UnityEngine;
using System.Collections;

public class FruitBehaviour : MonoBehaviour {

	public CollectableType type;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other) {
		Debug.Log ("fruit colliding with "+other.gameObject.tag);
		if (other.gameObject.tag == "Player") {
			Global.controller.gotFruit(this);
		}
	}
}
