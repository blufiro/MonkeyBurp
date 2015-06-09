using UnityEngine;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y < -Camera.main.orthographicSize -300) {
			Destroy(this.gameObject);
		}
	}
}
