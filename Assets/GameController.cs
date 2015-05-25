using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public GameObject world;
	public GameObject player;
	public GameObject[] fruitPrefabs;
	public GameObject snakePrefab;
	public GameObject eatMarkerPrefab;
	public GameObject[] trees;
	public GameObject pausePopup;
	public GameObject gameOverPopup;

	private bool isPaused;
	private GameState gameState;
	private PlayerBehaviour playerBehaviour;
	private long score = 0;
	private TextMesh scoreText;
	
	// Climbing Game
	private float scrollDistance;
	private float timeLeftSeconds;
	private int currLane;
	private Pool fruitsAndSnakesPool;
	private FruitSlotQueue slotQueue;
	private float[] spawnedDistances;
	private int spawnNextIndex;
	
	// Eating Game
	private float eatTimeRemainingSeconds;
	private Dictionary<GameObject, Vector2> eatMarkers;

	// Use this for initialization
	void Start() {
		Global.controller = this;
		scoreText = GameObject.FindGameObjectWithTag ("ScoreText").GetComponent<TextMesh>();
		slotQueue = GameObject.FindGameObjectWithTag ("SlotQueue").GetComponent<FruitSlotQueue>();
		pausePopup.SetActive (false);
		gameOverPopup.SetActive (false);
		fruitsAndSnakesPool = new Pool();
		spawnedDistances = new float[trees.Length-1];
		isPaused = false;
		gameState = GameState.INIT;
		playerBehaviour = player.GetComponent<PlayerBehaviour>();
		eatMarkers = new Dictionary<GameObject, Vector2>();
		// instantiate anim master singleton
		AnimMaster.get();
	}
	
	// Update is called once per frame
	void Update() {
		if (isPaused) return;
		gameUpdate();
	}

	private void gameStart() {
		gameState = GameState.PLAY_CLIMB;
		timeLeftSeconds = Global.get().roundDurationSeconds;
	}

	private void gameClimbEnd() {
		gameState = GameState.END_CLIMB;
		// transition to EAT
		AnimMaster.delay(this.gameObject, 0.25f).onComplete("gameInitEat");
	}

	private void gameInitEat() {
		// possible to get bonus for eating duration?
		eatTimeRemainingSeconds = Global.get().gameEatDurationSeconds;
		spawnEatMarkersIfNeeded();
		gameStartEat();
	}

	private void gameStartEat() {
		gameState = GameState.PLAY_EAT;
	}

	private void gameOver() {
		Debug.Log("gameOver");
		gameState = GameState.END_EAT;
		gameOverPopup.SetActive (true);
	}

	private void gameUpdate() {
		if (gameState == GameState.PLAY_CLIMB) {
			updateScroll();
			timeLeftSeconds -= Time.deltaTime;
			if(timeLeftSeconds <= 0){
				gameClimbEnd();
			}
		}
		
		if (gameState == GameState.PLAY_EAT) {
			eatTimeRemainingSeconds -= Time.deltaTime;
			if (eatTimeRemainingSeconds <= 0) {
				eatTimeRemainingSeconds = 0;
				gameOver();
			}
		}
		
		// update animations
		AnimMaster.get().update();
	}

	private void gameTogglePause() {
		isPaused = !isPaused;
		pausePopup.SetActive (isPaused);
	}

	private void gameReset() {
		setScroll (0);
		changeLane (0);
		addScore (-score);

		// clear all spawned objects
		fruitsAndSnakesPool.clear();
		
		// clear eat markers
		foreach (KeyValuePair<GameObject, Vector2> eatMarker in eatMarkers) {
			Destroy(eatMarker.Key);
		}
		eatMarkers.Clear();

		// reset trees
		foreach (GameObject tree in trees) {
			tree.SendMessage ("onReset");
		}

		// spawn fruits in between trees
		for (int i=Global.get().fruitAndSnakesPoolCount; i>=0; i--) {
			spawnRandomFruitOrSnake();
		}
		spawnUntilScroll(false);
		Debug.Log("spawned " + fruitsAndSnakesPool.getUsedCount() + ", free " + fruitsAndSnakesPool.getFreeCount());

		gameOverPopup.SetActive (false);
		gameStart();
	}

	private Vector3 getSpawnBasePosition(int index) {
		GameObject leftTree = trees[index];
		GameObject rightTree = trees[index + 1];
		return (leftTree.transform.position + rightTree.transform.position) * 0.5f;
	}
	
	private void spawnUntilScroll(bool shuffle) {
		while(spawnedDistances[spawnedDistances.Length-1] + Global.get().treeObjectDistance < scrollDistance + Global.get().getGameScreenHeight() * 2) {
			spawnNext(shuffle);
		}
	}
	
	private void spawnNext(bool shuffle) {
		int index = spawnNextIndex;
		spawnNextIndex = (spawnNextIndex + 1) % spawnedDistances.Length;
		if (shuffle) {
			fruitsAndSnakesPool.shuffleFree(5);
		}
		IPoolObject fruitOrSnake = fruitsAndSnakesPool.use();
		GameObject gob;
		if (fruitOrSnake.GetType() == typeof(FruitBehaviour)) {
			gob = ((FruitBehaviour)fruitOrSnake).gameObject;
		} else if (fruitOrSnake.GetType() == typeof(SnakeBehaviour)) {
			gob = ((SnakeBehaviour)fruitOrSnake).gameObject;
		} else {
			throw new UnityException("fruitOrSnake is not a known type: " + fruitOrSnake.GetType());
		}
		float objectDistance = spawnedDistances[index] + Global.get().treeObjectDistance;
		gob.transform.position = getSpawnBasePosition(index) + new Vector3 (0, objectDistance, 0);
		spawnedDistances[index] = objectDistance;
		//Debug.Log("spawned next object " + fruitOrSnake.GetType() + " at index: " + index + " with distance: " + objectDistance);
	}
	
	private GameObject spawnRandomFruitOrSnake() {
		if (Random.value < 0.9) {
			GameObject template = fruitPrefabs[(int)(Random.value * fruitPrefabs.Length)];
			GameObject spawnedObject =  (GameObject)Instantiate (template);
			FruitBehaviour fruit = spawnedObject.GetComponent<FruitBehaviour>();
			fruitsAndSnakesPool.add(fruit);
			spawnedObject.transform.parent = world.transform;
			return spawnedObject;
		} else {
			GameObject template = snakePrefab;
			GameObject spawnedObject =  (GameObject)Instantiate (template);
			SnakeBehaviour snake = spawnedObject.GetComponent<SnakeBehaviour>();
			fruitsAndSnakesPool.add(snake);
			spawnedObject.transform.parent = world.transform;
			return spawnedObject;
		}
	}

	private void updateScroll() {
		float distanceToScroll = Global.get().scrollSpeedPerSecond * Time.deltaTime;
		setScroll(scrollDistance + distanceToScroll);
	}
	
	private void setScroll(float distanceFromStart) {
		scrollDistance = distanceFromStart;
		world.transform.position = new Vector3 (world.transform.localPosition.x, -scrollDistance, world.transform.localPosition.z);
	}

	private void changeLane(int newLane) {
		if (newLane < 0 || newLane >= trees.Length) {
			return;
		}
		currLane = newLane;
		playerBehaviour.move(trees [currLane].transform.position.x);
	}
	
	private void spawnEatMarkersIfNeeded() {
		// depending on markers left we spawn differently
		int eatMarkersCount = eatMarkers.Count;
		switch (eatMarkersCount) {
			case 0: // no markers, spawn 3.
			{
				spawnEatMarker();
				spawnEatMarker();
				spawnEatMarker();
				break;
			}
			case 1: // 1 marker, spawn 1 immediately
			{
				spawnEatMarker();
				break;
			}
			case 2: // 2 markers, spawn 1 some time in future
			{
				AnimMaster.delay (this.gameObject, Random.Range(Global.get().nextEatMarkerSpawnSecondsMin, Global.get().nextEatMarkerSpawnSecondsMax))
					.onComplete("spawnEatMarker");
				break;
			}
			default: break;
		}
	}
	
	private void spawnEatMarker()  {
		float x, y;
		bool isCollided;
		
		do {
			isCollided = false;
			x = Random.Range (0, Global.get().markerGridDimX);
			y = Random.Range (0, Global.get().markerGridDimY);
			foreach (Vector2 eatMarkerGridCell in eatMarkers.Values) {
				if (x == eatMarkerGridCell.x || y == eatMarkerGridCell.y) {
					Debug.Log(x+","+y+" eatMarker:"+eatMarkerGridCell +" collided");
					isCollided = true;
				}
			}
		} while (isCollided);
		
		GameObject spawnedMarker = (GameObject) Instantiate(eatMarkerPrefab);
		spawnedMarker.transform.parent = world.transform;
		Vector3 spawnScreenPos = new Vector3(x * Screen.width/Global.get().markerGridDimX, y * Screen.height / Global.get().markerGridDimY);
		Vector3 spawnWorldPos = Camera.main.ScreenToWorldPoint(spawnScreenPos);
		spawnedMarker.transform.position = new Vector3(spawnWorldPos.x, spawnWorldPos.y, 0);
		eatMarkers.Add(spawnedMarker, new Vector2(x,y));
	}

	private void addScore(long newScore) {
		score += newScore;
		scoreText.text = score.ToString();
	}
	
	private Vector2 getGridCell(Vector2 screenPos) {
		return new Vector2(
			(int)(screenPos.x / Global.get().markerGridDimX),
			(int)(screenPos.y / Global.get().markerGridDimY));
	}

	// Input events
	// keyboard shortcut: LEFT ARROW
	void swipeLeft() {
		if (gameState == GameState.PLAY_CLIMB && playerBehaviour.state == PlayerState.CLIMB) {
			changeLane (currLane - 1);
		}
	}

	// keyboard shortcut: RIGHT ARROW
	void swipeRight() {
		if (gameState == GameState.PLAY_CLIMB && playerBehaviour.state == PlayerState.CLIMB) {
			changeLane (currLane + 1);
		}
	}

	// keyboard shortcut: SPACE
	void tap(Vector2 tapPosition) {
		Debug.Log ("tap: "+tapPosition+" in gamestate:"+gameState);
		switch (gameState) {
			case GameState.INIT:
			{
				gameReset();
				gameStart();
				break;
			}
			case GameState.PLAY_EAT:
			{
				Vector3 worldPos = Camera.main.ScreenToWorldPoint(tapPosition);
				Collider2D collider = Physics2D.OverlapPoint(worldPos);
				if (collider) {
					Debug.Log("hit eatMarker");
					// add score?
					Destroy(collider.gameObject);
					eatMarkers.Remove(collider.gameObject);
					// after some delay, respawn next markers
					AnimMaster
						.delay (this.gameObject, Random.Range (
							Global.get().respawnMarkerDelayMin,
							Global.get().respawnMarkerDelayMax))
							.onComplete("spawnEatMarkersIfNeeded");
				}
				break;
			}
			default: break;
		} 
	}

	// keyboard shortcut: P
	void debugTap() {
		Debug.Log ("debugTap");
		gameTogglePause();
	}

	// Game events
	public void gotFruit(FruitBehaviour fruit) {
		slotQueue.addFruit(fruit);
		fruitsAndSnakesPool.returnToPool(fruit);
	}
	
	public void hitSnake(SnakeBehaviour snake) {
		fruitsAndSnakesPool.returnToPool(snake);
	}
	
	public void returnFruit(FruitBehaviour fruit) {
		fruitsAndSnakesPool.returnToPool(fruit);
		spawnUntilScroll(true);
	}
	
	public void returnSnake(SnakeBehaviour snake) {
		fruitsAndSnakesPool.returnToPool(snake);
		spawnUntilScroll(true);
	}
	
	public void cashedIn(int total) {
		addScore(total);
	}
}
