using UnityEngine;
using System.Collections;

public class SnakeBehaviour : MonoBehaviour, IPoolObject {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void poolClear() {
		Destroy(this.gameObject);
	}
	
	public void poolUse() {
		// init object if necessary
	}
	
	public void poolReturn() {
		// reset object if necessary
	}
}
