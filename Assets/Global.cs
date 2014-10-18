using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Global
{
	public float scrollSpeedPerSecond = 5;
	public float gameDistance = 20 * 2;
	public float swipeDeltaSqMagnitude= 100*100;
	public float swipeMinDistance = 100;
	public float playerMoveAnimSeconds = 0.15f;
	public float minTreeObjectDistance = 5;
	public float maxTreeObjectDistance = 10;
	public Dictionary<CollectableType, long> fruitPoints;

	public static GameController controller;

	private static Global m_instance = new Global();
	public static Global get() {
		return m_instance;
	}

	private Global(){
		fruitPoints = new Dictionary<CollectableType, long>()
		{
			{ CollectableType.FRUIT, 10 },
		};
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
	PLAY_EAT,
	END_EAT,
	LEADERBOARD,
}

public enum CollectableType {
	FRUIT,
}

public enum PlayerState {
	NONE,
	CLIMB,
	MOVE,
}