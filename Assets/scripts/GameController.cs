using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public GameObject world;
	public GameObject player;
	public GameObject[] fruitPrefabs;
	public GameObject[] rottenFruitPrefabs;
	public GameObject eatMarkerPrefab;
	public GameObject[] trees;
	public GameObject[] enemyPrefabs;
	public GameObject pausePopup;
	public GameObject gameOverPopup;
	public GameObject blindOverlay;

	private bool isPaused;
	private GameState gameState;
	private PlayerBehaviour playerBehaviour;
	private long score = 0;
	private TextMesh scoreText;
	
	// Climbing Game
	private float scrollDistance;
	private float timeLeftSeconds;
	private int currLane;
	private Pool spawnedGobPool;
	private FruitSlotQueue slotQueue;
	private float[] spawnedDistances;
	private int spawnNextIndex;
	private float nextEnemySpawnDistance;
	private long scoreAnimCurrent;
	private long scoreAnimTarget;

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
		blindOverlay.SetActive(false);
		blindOverlay.GetComponent<ParticleSystem> ().Stop ();
		spawnedGobPool = new Pool();
		spawnedDistances = new float[trees.Length-1];
		isPaused = false;
		gameState = GameState.INIT;
		playerBehaviour = player.GetComponent<PlayerBehaviour>();
		eatMarkers = new Dictionary<GameObject, Vector2>();
		// instantiate anim master singleton
		AnimMaster.get();
		changeLane (Global.get().startingLane, true);
	}
	
	// Update is called once per frame
	void Update() {
		if (isPaused) return;
		gameUpdate();
	}
	
	void OnGUI() {
		GUILayout.BeginArea (new Rect (100,100,Screen.width-100,Screen.height));
		GUILayout.Box ("spawnedGobPool: " + spawnedGobPool.getFreeCount() + " of " + spawnedGobPool.getTotalCount() + " used: "+ spawnedGobPool.getUsedCount());
		GUILayout.EndArea();
	}

	private void gameStart() {
		gameState = GameState.PLAY_CLIMB;
		timeLeftSeconds = Global.get().roundDurationSeconds;
		playerBehaviour.beginClimb();
	}

	private void gameClimbEnd() {
		gameState = GameState.END_CLIMB;
		playerBehaviour.endClimb();
		// transition to EAT
		AnimMaster.delay("", this.gameObject, 0.25f).onComplete("gameOver");
	}

	private void gameOver() {
		Debug.Log("gameOver");
		gameState = GameState.GAME_OVER;
		gameOverPopup.SetActive (true);
	}

	private void gameUpdate() {
		if (gameState == GameState.PLAY_CLIMB) {
			updateScroll();
			timeLeftSeconds -= Time.deltaTime;
			if(timeLeftSeconds <= 0){
				gameClimbEnd();
			}
			updateScoreRoll();
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
		changeLane (Global.get().startingLane, true);
		resetScore();

		// clear all spawned objects
		spawnedGobPool.clear();
		
		// clear eat markers
		foreach (KeyValuePair<GameObject, Vector2> eatMarker in eatMarkers) {
			Destroy(eatMarker.Key);
		}
		eatMarkers.Clear();

		// reset trees
		foreach (GameObject tree in trees) {
			tree.SendMessage ("onReset");
		}
		
		// reset spawn distances
		for (int i=spawnedDistances.Length-1; i>=0; i--) {
			spawnedDistances[i] = 0;
		}

		// spawn fruits in between trees
		for (int i=Global.get().fruitAndRottenFruitPoolCount; i>=0; i--) {
			spawnRandomFruitOrRottenFruit();
		}
		spawnUntilScroll();
		Debug.Log("spawned " + spawnedGobPool.getUsedCount() + ", free " + spawnedGobPool.getFreeCount());
		
		// reset spawning distance
		nextEnemySpawnDistance = Global.get().initialEnemySpawnDistance;

		gameOverPopup.SetActive (false);
		gameStart();
	}

	private Vector3 getSpawnBasePosition(int index) {
		GameObject leftTree = trees[index];
		GameObject rightTree = trees[index + 1];
		return (leftTree.transform.position + rightTree.transform.position) * 0.5f;
	}
	
	private void spawnUntilScroll() {
		while(spawnedDistances[spawnedDistances.Length-1] + Global.get().treeObjectDistance < scrollDistance + Global.get().getGameScreenHeight() * 2) {
			spawnNext();
		}
	}
	
	private void spawnNext() {
		int index = spawnNextIndex;
		spawnNextIndex = (spawnNextIndex + 1) % spawnedDistances.Length;
		IPoolObject spawnedGob = spawnedGobPool.use();
		GameObject gob;
		if (spawnedGob.GetType() == typeof(FruitBehaviour)) {
			gob = ((FruitBehaviour)spawnedGob).gameObject;
		} else {
			throw new UnityException("spawnedGob is not a known type: " + spawnedGob.GetType());
		}
		float objectDistance = spawnedDistances[index] + Global.get().treeObjectDistance + (UnityEngine.Random.value - 0.5f) * Global.get ().treeObjectDistanceRange;
		gob.transform.position = getSpawnBasePosition(index) + new Vector3 (0, objectDistance, 0);
		spawnedDistances[index] = objectDistance;
		//Debug.Log("spawned next object " + spawnedGob.GetType() + " at index: " + index + " with distance: " + objectDistance);
	}
	
	private GameObject spawnRandomFruitOrRottenFruit() {
		GameObject template;
		if (UnityEngine.Random.value < Global.get().fruitToRottenRatio) {
			template = fruitPrefabs[(int)(UnityEngine.Random.value * fruitPrefabs.Length)];
		} else {
			template = rottenFruitPrefabs[(int)(UnityEngine.Random.value * rottenFruitPrefabs.Length)];
		}
		GameObject spawnedObject =  (GameObject)Instantiate (template);
		FruitBehaviour fruitBehaviour = spawnedObject.GetComponent<FruitBehaviour>();
		spawnedGobPool.add(fruitBehaviour);
		spawnedObject.transform.parent = world.transform;
		return spawnedObject;
	}

	private GameObject spawnRandomEnemy() {
		GameObject template;
		template = enemyPrefabs[(int)(UnityEngine.Random.value * enemyPrefabs.Length)];
		GameObject spawnedObject = (GameObject)Instantiate (template);
		spawnedObject.transform.parent = world.transform;
		GameObject randomTree = trees[(int)(UnityEngine.Random.value * trees.Length)];
		Vector3 pos = spawnedObject.transform.position;
		pos.x = randomTree.transform.position.x;
		pos.y = Global.get().getGameScreenHalfHeight() + Global.get().enemySpawnYAboveScreen;
		spawnedObject.transform.position = pos;
		return spawnedObject;
	}

	private void updateScroll() { 
		float distanceToScroll = Global.get().scrollSpeedPerSecond * Time.deltaTime;
		setScroll(scrollDistance + distanceToScroll);
		if (scrollDistance > nextEnemySpawnDistance) {
			spawnRandomEnemy();
			nextEnemySpawnDistance += Global.get ().enemySpawnDistance;
		}
	}
	
	private void setScroll(float distanceFromStart) {
		scrollDistance = distanceFromStart;
		float offsetWorldY = -Global.get().getGameScreenHalfHeight();
		world.transform.position = new Vector3 (world.transform.localPosition.x, offsetWorldY - scrollDistance, world.transform.localPosition.z);
	}

	private void changeLane(int newLane, bool reset) {
		if (newLane < 0 || newLane >= trees.Length) {
			return;
		}
		currLane = newLane;
		if (reset) {
			playerBehaviour.reset(trees [currLane].transform.position.x);
		} else {
			playerBehaviour.jump(trees [currLane].transform.position.x);
		}
	}

	private void addScore(long newScore) {
		score += newScore;
	}
	
	private void resetScore() {
		score = 0;
		showScoreImmediate();
	}
	
	private void showScoreImmediate() {
		scoreAnimCurrent = score;
		scoreAnimTarget = score;
		scoreText.text = score.ToString();
	}

	private void showScoreAnimated() {
		scoreAnimTarget = score;
		// animation of score will take place in updateScoreRoll()
	}
	
	private void updateScoreRoll() {
		if (scoreAnimCurrent == scoreAnimTarget) {
			return;
		}
		scoreAnimCurrent += Global.get().scoreRollRate;
		if (scoreAnimCurrent > scoreAnimTarget) {
			scoreAnimCurrent = scoreAnimTarget;
		}
		scoreText.text = scoreAnimCurrent.ToString();
	}
	
	private void showBonus(BonusType bonus) {
		Debug.Log("showBonus" + bonus);
		scoreText.text = bonus.ToString();
	}

	// Input events
	// keyboard shortcut: LEFT ARROW
	void swipeLeft() {
		if (gameState == GameState.PLAY_CLIMB && playerBehaviour.state == PlayerState.CLIMB) {
			changeLane (currLane - 1, false);
		}
	}

	// keyboard shortcut: RIGHT ARROW
	void swipeRight() {
		if (gameState == GameState.PLAY_CLIMB && playerBehaviour.state == PlayerState.CLIMB) {
			changeLane (currLane + 1, false);
		}
	}

	// keyboard shortcut: SPACE
	void tap(Vector2 tapPosition) {
		Debug.Log ("tap: "+tapPosition+" in gamestate:"+gameState);
		switch (gameState) {
			case GameState.INIT:
			{
				gameReset();
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
		spawnedGobPool.returnToPool(fruit);
		addScore (Global.get ().collectFruitScore);
		showScoreAnimated();
	}
	
	public void returnFruit(FruitBehaviour fruit) {
		spawnedGobPool.returnToPoolRand(fruit);
		spawnUntilScroll();
	}
	
	public void cashedIn(int cashInScore, List<BonusType> bonuses) {
		Debug.Log("cashIn: " + cashInScore);
		addScore(cashInScore);
		float delay = 0f;
		AnimMaster.clearWithKey("score");
		if (bonuses.Count > 0) {
			foreach (BonusType bonus in bonuses) {
				AnimMaster.delay("score", gameObject, delay).onComplete("showBonus").onCompleteParams(bonus);
				delay += 1f;
			}
		}
		AnimMaster.delay("score", gameObject, delay).onComplete("showScoreImmediate");
		Debug.Log("Bonuses: " + string.Join(",", Array.ConvertAll(bonuses.ToArray(), i => i.ToString())));
	}

	public void playerBlink() {
		playerBehaviour.blinkEffect ();
		AnimMaster.delay ("monkeyBlink", playerBehaviour.gameObject, Global.get ().playerBlinkDuration).onComplete ("offBlinkEffect");
	}
	
	public void loseAllFruits() {
		slotQueue.removeAllFruits ();
		playerBlink();
	}
	
	public void blind() {
		blindOverlay.SetActive(true);
		blindOverlay.GetComponent<ParticleSystem> ().Play ();
		AnimMaster.clearWithKey("blind");
		AnimMaster.delay("blind", gameObject, Global.get().blindDurationSeconds).onComplete("blindOff");
		playerBlink();
	}
	
	public void blindOff() {
		blindOverlay.SetActive(false);
	}
	
	public void shuffleFruits() {
		slotQueue.shuffleFruits ();
		playerBlink();
	}
}
