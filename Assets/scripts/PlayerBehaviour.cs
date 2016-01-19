using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour {

	private static int BLINK_OFF = 0;
	private static int BLINK_ON = 1;

	public PlayerState state;

	private float originalX;
	private float changeX;
	private float animTimeElapsed;

	// Use this for initialization
	void Start() {
		updateState(PlayerState.IDLE);
	}
	
	// Update is called once per frame
	void Update() {
		switch (state) {
		case PlayerState.IDLE: break;
		case PlayerState.CLIMB: break;
		case PlayerState.JUMP:
			{
			animTimeElapsed += Time.deltaTime;
			if (animTimeElapsed < Global.get().playerMoveAnimSeconds) {
				setX(Easing.EaseInOutQuad(animTimeElapsed, originalX, changeX, Global.get().playerMoveAnimSeconds));
			} else {
				setX(originalX + changeX);
				updateState(PlayerState.CLIMB);
			}

			}break;
		default:
			Debug.LogError("PlayerState not implemented!"+state);
			break;
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		Debug.Log ("player colliding with "+other.gameObject.tag + " named: " + other.gameObject.name);
	}

	public void reset(float x) {
		setX(x);
	}

	public void beginClimb() {
		updateState(PlayerState.CLIMB);
	}

	public void endClimb() {
		updateState(PlayerState.IDLE);
	}
	
	public void jump (float x)
	{
		originalX = transform.position.x;
		changeX = x - originalX;
		animTimeElapsed = 0;
		if ((changeX < 0 && transform.localScale.x > 0)
			|| (changeX > 0 && transform.localScale.x < 0)) {
			Vector3 flipXScale = transform.localScale;
			flipXScale.x *= -1;
			transform.localScale = flipXScale;
		}
		updateState(PlayerState.JUMP);
	}
	
	private void setX(float x) {
		transform.position = new Vector3 (x, transform.position.y, transform.position.z);
	}

	private void updateState(PlayerState newState) {
		state = newState;
		this.gameObject.GetComponent<Animator>().SetInteger("MonkeyState", (int) state);
	}

	public void blinkEffect(){
		this.gameObject.GetComponent<Animator> ().SetInteger ("MonkeyBlink", BLINK_ON);
	}

	public void offBlinkEffect() {
		this.gameObject.GetComponent<Animator> ().SetInteger ("MonkeyBlink", BLINK_OFF);
	}
}
