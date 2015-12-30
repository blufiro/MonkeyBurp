using UnityEngine;
using System.Collections;

public class TreeBehaviour : MonoBehaviour {

	public Sprite[] sprites;

	// Use this for initialization
	void Start () {
		int randSpriteIndex = (int) (Random.value * (float) sprites.Length);
		this.gameObject.GetComponent<SpriteRenderer>().sprite = sprites[randSpriteIndex];
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
