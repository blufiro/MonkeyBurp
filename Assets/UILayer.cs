using UnityEngine;
using System.Collections;

public class UILayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.renderer.sortingLayerID = this.transform.parent.renderer.sortingLayerID;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
