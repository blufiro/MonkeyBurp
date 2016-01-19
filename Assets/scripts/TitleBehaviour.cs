using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleBehaviour : MonoBehaviour {

	public Text versionText;
	
	// Use this for initialization
	void Start () {
		versionText.text = "Version: " + Application.version;
		Input.simulateMouseWithTouches = true;
	}
	
	// Update is called once per frame
	void Update () {
		// mouse tap
		if (Input.GetMouseButtonDown (0)) {
			Vector2 mousePos = Input.mousePosition;
			Debug.Log ("mouse down"+mousePos);
			this.gameObject.SendMessage("onTap", mousePos, SendMessageOptions.RequireReceiver);
		}
	}
	
	void onTap() {
		Application.LoadLevel("gameScene");
	}
}
