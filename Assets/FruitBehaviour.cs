using UnityEngine;
using System.Collections;

public class FruitBehaviour : MonoBehaviour, IPoolObject {

	public CollectableType type;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			Global.controller.gotFruit(this);
		} else if (other.gameObject.tag == "Respawn") {
			Global.controller.returnFruit(this);
		} else {
			Debug.Log ("fruit colliding with "+other.gameObject.tag + " named: " + other.gameObject.name);
		}
	}
	
	public void poolClear() {
		Destroy(this.gameObject);
	}
	
	public void poolUse() {
		// init object if necessary
		gameObject.SetActive(true);
	}
	
	public void poolReturn() {
		// reset object if necessary
		gameObject.SetActive(false);
	}
}
