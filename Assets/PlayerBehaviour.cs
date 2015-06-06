using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour {

	public PlayerState state;

	private float originalX;
	private float changeX;
	private float animTimeElapsed;

	// Use this for initialization
	void Start() {
	}
	
	// Update is called once per frame
	void Update() {
		switch (state) {
		case PlayerState.NONE: break;
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
	
	void setX(float x) {
		transform.position = new Vector3 (x, transform.position.y, transform.position.z);
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
	
	private void updateState(PlayerState newState) {
		state = newState;
		this.gameObject.GetComponent<Animator>().SetInteger("MonkeyState", (int) state);
	}
}
