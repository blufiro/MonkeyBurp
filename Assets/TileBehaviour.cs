using UnityEngine;
using System.Collections;

public class TileBehaviour : MonoBehaviour {

	class TilePiece {
		public GameObject tile;
		public Vector3 initialLocalPosition;
		
		public TilePiece(GameObject tile) {
			this.tile = tile;
			this.initialLocalPosition = tile.transform.localPosition;
		}
		
		public void reset() {
			this.tile.transform.localPosition = this.initialLocalPosition;
		}
	}

	// assumes is bottom aligned
	private GameObject tile;
	private int targetTileHeight;
	private int tileHeight;
	private TilePiece[] tiles;
	private Vector3 cachedTranslate;

	// Use this for initialization
	void Start () {
		tile = this.transform.FindChild("tree_01").gameObject;
		tileHeight = (int) ((SpriteRenderer)tile.renderer).sprite.bounds.size.y;
		cachedTranslate = new Vector3 (0, tileHeight, 0);
		targetTileHeight  = (int) (Camera.main.orthographicSize * 2);
		
		int repeat = Mathf.CeilToInt((float) targetTileHeight / tileHeight) + 1;
		Debug.Log("repeat: " + repeat + " screen height: " + Screen.height + " tile height: " + tileHeight);
		tiles = new TilePiece[repeat];
		for (int y=0; y<repeat; y++) {
			GameObject newTile = (GameObject)Instantiate (tile);
			newTile.transform.parent = this.transform;
			newTile.transform.localPosition = tile.transform.localPosition + (cachedTranslate * y);
			tiles[y] = new TilePiece(newTile);
		}
		tile.SetActive(false);
		onReset ();
	}

	public void onReset() {
		Debug.Log ("tree reset! " + " h:" + tileHeight);

		foreach (TilePiece tile in tiles) {
			tile.reset();
		}
	}
	
	// Update is called once per frame
	void Update () {
		// The world position is moving, so we do not move individual tiles here.
		// We only shift them when they go out of the camera, keeping all tiles in order.
		if (tiles[0].tile.transform.position.y < -Camera.main.orthographicSize) {
			foreach (TilePiece tilePiece in tiles) {
				tilePiece.tile.transform.Translate(cachedTranslate);
			}
		}
	}
}
