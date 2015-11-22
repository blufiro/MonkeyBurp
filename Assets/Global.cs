using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Global
{
	// climbing control variables
	public float scrollSpeedPerSecond = 200;
	public float swipeDeltaSqMagnitude= 10000*10000;
	public float swipeMinDistance = 10000;
	public float playerMoveAnimSeconds = 0.2f;
	public float playerBlinkDuration = 1.0f;
	
	// init variables
	public float treeObjectDistance = 170;
	public int fruitAndRottenFruitPoolCount = 100;
	public float fruitToRottenRatio = 0.95f;
	public float roundDurationSeconds = 2 * 60;
	public int startingLane = 2; // 0 to 4, start in middle
	public int gameNumSlots = 5;
	public int gameMaxSlots = 6;
	public int scoreBase = 500;

	// enemy variables
	public float enemySpawnDistance = 200;
	public float chickenFallSpeedPerSecond = 100;
	public float blindDurationSeconds = 5;
	

	public static GameController controller;

	private static Global m_instance = new Global();
	public static Global get() {
		return m_instance;
	}

	private Global(){
	}
	
	public int getGameScreenHeight() {
		return (int) (Camera.main.orthographicSize * 2);
	}
	
	public static int getMultiplier(BonusType bonusType) {
		switch (bonusType) {
			case BonusType.NONE: return 1;
			case BonusType.THREE_IN_A_ROW: return 2;
			case BonusType.FOUR_IN_A_ROW: return 7;
			case BonusType.FIVE_IN_A_ROW: return 10;
			case BonusType.SIX_IN_A_ROW: return 15;
			case BonusType.ALTERNATE: return 5;
		}
		throw new UnityException("BonusType multiplier not implemented: " + bonusType);
	}
	
	public static int getAddition(BonusType bonusType) {
		switch (bonusType) {
			case BonusType.NONE: return 0;
			case BonusType.RAINBOW: return 1000;
		}
		throw new UnityException("BonusType addition not implemented: " + bonusType);
	}
}

public enum SwipeDirection {
	LEFT,
	RIGHT
}

public enum GameState {
	INIT,
	MENU,
	START, // preparing to play
	PLAY_CLIMB,
	END_CLIMB,
	GAME_OVER,
	LEADERBOARD,
}

public enum CollectableType {
	NONE,
	FRUIT_BANANA,
	FRUIT_CHERRY,
	FRUIT_GRAPES,
	FRUIT_ORANGE,
	FRUIT_PEACH,
	FRUIT_PUMPKIN,
	FRUIT_BANANA_ROTTEN,
	FRUIT_CHERRY_ROTTEN,
	FRUIT_GRAPES_ROTTEN,
	FRUIT_ORANGE_ROTTEN,
	FRUIT_PEACH_ROTTEN,
}

public enum PlayerState {
	NONE = 0,
	CLIMB = 1,
	JUMP = 2,
}

public enum BonusType {
	NONE,
	THREE_IN_A_ROW,
	FOUR_IN_A_ROW,
	FIVE_IN_A_ROW,
	SIX_IN_A_ROW,
	ALTERNATE,
	RAINBOW,
}