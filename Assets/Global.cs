using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Global
{
	// climbing variables
	public float scrollSpeedPerSecond = 2;
	public float gameDistance = 20 * 2;
	public float swipeDeltaSqMagnitude= 100*100;
	public float swipeMinDistance = 100;
	public float playerMoveAnimSeconds = 0.15f;
	public float treeObjectDistance = 2;
	public Dictionary<CollectableType, long> fruitPoints;
	
	// eating variables
	public float gameEatDurationSeconds = 30;
	public float nextEatMarkerSpawnSecondsMin = 0.2f;
	public float nextEatMarkerSpawnSecondsMax = 0.8f;
	public float respawnMarkerDelayMin = 0.25f;
	public float respawnMarkerDelayMax = 0.75f;
	public int markerGridDimX = 15;
	public int markerGridDimY = 8;

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