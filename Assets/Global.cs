using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Global
{
	// climbing variables
	public float scrollSpeedPerSecond = 200;
	public float swipeDeltaSqMagnitude= 10000*10000;
	public float swipeMinDistance = 10000;
	public float playerMoveAnimSeconds = 0.15f;
	public float treeObjectDistance = 170;
	public int fruitAndSnakesPoolCount = 100;
	public float roundDurationSeconds = 2 * 60;
	public Dictionary<CollectableType, long> fruitPoints;
	public int gameNumSlots = 5;
	public int gameMaxSlots = 6;
	public int scoreBase = 500;

	public static GameController controller;

	private static Global m_instance = new Global();
	public static Global get() {
		return m_instance;
	}

	private Global(){
		// TODO: make points based on combination
		fruitPoints = new Dictionary<CollectableType, long>()
		{
			{ CollectableType.FRUIT_BANANA, 1 },
			{ CollectableType.FRUIT_CHERRY, 1 },
			{ CollectableType.FRUIT_GRAPES, 1 },
			{ CollectableType.FRUIT_ORANGE, 1 },
			{ CollectableType.FRUIT_PEACH, 1 },
			{ CollectableType.FRUIT_PUMPKIN, 1000 },
		};
	}
	
	public int getGameScreenHeight() {
		return (int) (Camera.main.orthographicSize * 2);
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
	
}

public enum PlayerState {
	NONE,
	CLIMB,
	MOVE,
}