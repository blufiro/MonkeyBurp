using UnityEngine;
using System.Collections;

public class InputControlsBehaviour : MonoBehaviour {

	private bool isSwipe = false;
	private Vector2 swipeStartPosition;

	private Vector2 prevMousePos;

	// Use this for initialization
	void Start() {
		Input.simulateMouseWithTouches = true;
	}

	void OnGUI()  {
		GUILayout.BeginArea (new Rect (0,0,Screen.width,Screen.height));
		GUILayout.Box ("isSwipe: " + isSwipe.ToString());
		GUILayout.Box ("swipeStartPosition: " + swipeStartPosition.ToString());
		GUILayout.Box ("prevMousePos: " + prevMousePos.ToString());
		GUILayout.EndArea();
	}
	
	// Update is called once per frame
	void Update() {
		//keyboard
		if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			this.gameObject.SendMessage("swipeLeft");
		} else if(Input.GetKeyDown(KeyCode.RightArrow)){
			this.gameObject.SendMessage("swipeRight");
		} else if (Input.GetKeyDown (KeyCode.Space)) {
			this.gameObject.SendMessage("tap", randomScreenPos(), SendMessageOptions.RequireReceiver );
		} else if (Input.GetKeyDown (KeyCode.P)) {
			this.gameObject.SendMessage("debugTap");
		}

		// mouse tap
		if (Input.GetMouseButtonDown (0)) {
			Vector2 mousePos = Input.mousePosition;
			Debug.Log ("mouse down"+mousePos);
			this.gameObject.SendMessage((mousePos.x < Screen.width/2) ? "swipeLeft" : "swipeRight");
			this.gameObject.SendMessage("tap", mousePos, SendMessageOptions.RequireReceiver);
		}

		/*
		Vector2 mousePos = Input.mousePosition;
		if (Input.GetMouseButtonDown (0)) {
			isSwipe = false;
			swipeStartPosition = mousePos;
			prevMousePos = Input.mousePosition;
		}else if(Input.GetMouseButtonUp(0)){
			if (isSwipe) {
				Vector2 displacement = mousePos - swipeStartPosition;
				Debug.Log ("mouse displacement:"+displacement.x+" swipeMinDistance"+Global.get().swipeMinDistance);
				if(Mathf.Abs(displacement.x) > Global.get().swipeMinDistance) {
					this.gameObject.SendMessage((displacement.x < 0)? "swipeLeft" : "swipeRight");
				}
			} else {
				this.gameObject.SendMessage("tap");
			}
		}else if (Input.GetMouseButton (0)) {
			if((mousePos-prevMousePos).sqrMagnitude > Global.get().swipeDeltaSqMagnitude) {
				isSwipe = true;
				Debug.Log ("swipe= true");
			}
			prevMousePos = Input.mousePosition;
		}
		*/

		// no touch
		if(Input.touches.Length == 0) {
			return;
		}
		// three tap
		int beganCount = 0;
		foreach (Touch touch in Input.touches){
			if(touch.phase == TouchPhase.Began){
				beganCount++;
			}
			if (beganCount == 3) {
				this.gameObject.SendMessage("debugTap");
				break;
			}
		}
		

		/*
		// touch
		Touch t = Input.GetTouch(0);
		switch (t.phase) {
			case TouchPhase.Began:
			{
				isSwipe = false;
				swipeStartPosition = t.position;
			}break;
			case TouchPhase.Moved:
			{
				if(t.deltaPosition.sqrMagnitude > Global.get().swipeDeltaSqMagnitude) {
					isSwipe = true;
				}
			}break;
			case TouchPhase.Ended:
			{
				if (isSwipe) {
					Vector2 displacement = t.position - swipeStartPosition;
					if(Mathf.Abs(displacement.x) < Global.get().swipeMinDistance) {
						this.gameObject.SendMessage((displacement.x < 0)? "swipeLeft" : "swipeRight");
					}
				} else {
					this.gameObject.SendMessage("tap");
				}
			}break;
			case TouchPhase.Stationary:
			{
				// do nothing
			}break;
			case TouchPhase.Canceled:
			{
				isSwipe = false;
			}break;
		}
		*/
	}
	
	private Vector2 randomScreenPos() {
		return new Vector2(Random.Range(0, Screen.width), Random.Range (0, Screen.height));
	}
}
