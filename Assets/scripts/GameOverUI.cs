using UnityEngine;
using System.Collections;

public class GameOverUI : MonoBehaviour {

	public GameController gameController;

	private Rect area;

	// Use this for initialization
	void Start () {
		area = new Rect (0, 0, Screen.width, Screen.height);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		GUILayout.BeginArea (area);
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		GUILayout.BeginVertical ();

		GUILayout.Space (Screen.height*0.6f);
		if (GUILayout.Button ("Again", GUILayout.MinWidth (100), GUILayout.MinHeight (50))) {
			gameController.SendMessage("gameReset");
		}
		GUILayout.FlexibleSpace ();

		GUILayout.EndVertical ();
		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();
		GUILayout.EndArea ();
	}
}
