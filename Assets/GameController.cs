using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public GameObject world;
	public GameObject player;
	public GameObject fruitPrefab;
	public GameObject snakePrefab;
	public GameObject[] trees;
	public GameObject pausePopup;
	public GameObject gameOverPopup;

	private bool isPaused;
	private GameState gameState;
	private PlayerBehaviour playerBehaviour;
	private float scrollDistance;
	private int currLane;
	private long score = 0;
	private TextMesh distanceLeftText;
	private TextMesh scoreText;
	private List<GameObject> spawned;

	// Use this for initialization
	void Start () {
		Global.controller = this;
		distanceLeftText = GameObject.FindGameObjectWithTag ("DistanceText").GetComponent<TextMesh>();
		scoreText = GameObject.FindGameObjectWithTag ("ScoreText").GetComponent<TextMesh>();
		pausePopup.SetActive (false);
		gameOverPopup.SetActive (false);
		spawned = new List<GameObject> ();
		isPaused = false;
		gameState = GameState.INIT;
		playerBehaviour = player.GetComponent<PlayerBehaviour> ();
	}
	
	// Update is called once per frame
	void Update () {
		gameUpdate ();	
	}

	private void gameStart() {
		gameState = GameState.PLAY_CLIMB;
	}

	private void gameWin() {
		gameState = GameState.END_CLIMB;
		gameOverPopup.SetActive (true);
	}

	private void gameLose() {
		gameState = GameState.END_CLIMB;
		gameOverPopup.SetActive (true);
	}

	private void gameUpdate() {
		if (isPaused) return;

		if (gameState == GameState.PLAY_CLIMB) {
			float distanceToScroll = Global.get ().scrollSpeedPerSecond * Time.deltaTime;
			if (scrollDistance + distanceToScroll > Global.get ().gameDistance) {
				distanceToScroll = Global.get ().gameDistance - scrollDistance;
			}
			updateScroll(scrollDistance + distanceToScroll);
			float distanceLeft = Global.get ().gameDistance-scrollDistance;
			distanceLeftText.text = "Distance Left: "+Mathf.FloorToInt(distanceLeft);
			if(distanceLeft <= 0){
				gameWin ();
			}
		}
	}

	private void gameTogglePause() {
		isPaused = !isPaused;
		pausePopup.SetActive (isPaused);
	}

	private void gameReset() {
		updateScroll (0);
		changeLane (0);
		addScore (-score);

		// clear fruits
		foreach (GameObject gob in spawned) {
			Destroy (gob);
		}

		// reset trees
		foreach (GameObject tree in trees) {
			tree.SendMessage ("reset");
		}

		// spawn fruits in between trees
		for (int i = 1; i < trees.Length; i++) {
			GameObject leftTree = trees[i-1];
			GameObject rightTree = trees[i];
			Vector3 inBetweenTreePos = (leftTree.transform.position + rightTree.transform.position) * 0.5f;
			spawnFruitsAndSnakes(inBetweenTreePos);
		}

		gameOverPopup.SetActive (false);
		gameStart ();
	}

	private void spawnFruitsAndSnakes(Vector3 spawnBasePosition) {
		float coveredDistance = 0;
		while(coveredDistance+Global.get ().minTreeObjectDistance < Global.get ().gameDistance) {
			float maxDistance = Global.get ().maxTreeObjectDistance;
			if(coveredDistance+maxDistance >= Global.get ().gameDistance) {
				maxDistance = Global.get ().gameDistance - coveredDistance;
			}
			float objectDistance = coveredDistance + Random.Range (Global.get ().minTreeObjectDistance, maxDistance);
			GameObject template = (Random.value < 0.9) ? fruitPrefab : snakePrefab;
			GameObject spawnedObject =  (GameObject)Instantiate (template);
			spawnedObject.transform.parent = world.transform;
			spawnedObject.transform.position = spawnBasePosition + new Vector3 (0, objectDistance, 0);
			spawned.Add(spawnedObject);
			coveredDistance = objectDistance;
		}
	}

	private void updateScroll(float scrollDist) {
		scrollDistance = scrollDist;
		world.transform.position = new Vector3 (world.transform.localPosition.x, -scrollDistance, world.transform.localPosition.z);
	}

	private void changeLane(int newLane) {
		if (newLane < 0 || newLane >= trees.Length) {
			return;
		}
		currLane = newLane;
		playerBehaviour.move(trees [currLane].transform.position.x);
	}

	private void addScore(long newScore) {
		score += newScore;
		scoreText.text = score.ToString ();
	}

	// Input events
	void swipeLeft() {
		if (gameState == GameState.PLAY_CLIMB && playerBehaviour.state == PlayerState.CLIMB) {
			changeLane (currLane - 1);
		}
	}
	
	void swipeRight() {
		if (gameState == GameState.PLAY_CLIMB && playerBehaviour.state == PlayerState.CLIMB) {
			changeLane (currLane + 1);
		}
	}
	
	void tap() {
		if (gameState== GameState.INIT){
			gameReset ();
			gameStart ();
		}else {
			gameTogglePause();
		}
	}

	// keyboard P
	void debugTap() {
		gameTogglePause ();
	}

	// Game events
	public void gotFruit(FruitBehaviour fruit) {
		addScore (Global.get ().fruitPoints [fruit.type]);
		Destroy (fruit.gameObject);
	}
}
