using UnityEngine;
using System.Collections;

public class SnakeBehaviour : MonoBehaviour, IPoolObject {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			Global.controller.hitSnake(this);
		} else if (other.gameObject.tag == "Respawn") {
			Global.controller.returnSnake(this);
		} else {
			Debug.Log ("snake colliding with "+other.gameObject.tag + " named: " + other.gameObject.name);
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
