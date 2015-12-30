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
		tileHeight = (int) ((SpriteRenderer)tile.GetComponent<Renderer>()).sprite.bounds.size.y;
		targetTileHeight  = Global.get().getGameScreenHeight();
		
		int repeat = Mathf.CeilToInt((float) targetTileHeight / tileHeight) + 1;
		cachedTranslate = new Vector3 (0, tileHeight * repeat, 0);
		Debug.Log("repeat: " + repeat + " screen height: " + Screen.height + " tile height: " + tileHeight);
		tiles = new TilePiece[repeat];
		for (int y=0; y<repeat; y++) {
			GameObject newTile = (GameObject)Instantiate (tile);
			newTile.name = "tree" + y;
			newTile.transform.parent = this.transform;
			newTile.transform.localPosition = tile.transform.localPosition + new Vector3(0, tileHeight * y, 0);
			tiles[y] = new TilePiece(newTile);
		}
		tile.SetActive(false);
		onReset ();
	}

	public void onReset() {
		foreach (TilePiece tile in tiles) {
			tile.reset();
		}
	}
	
	// Update is called once per frame
	void Update () {
		// The world position is moving, so we do not move individual tiles here.
		// We only shift the bottom piece to the top when they go out of the screen, keeping all tiles in order.
		if (tiles[0].tile.transform.position.y < -Global.get().getGameScreenHalfHeight() -tileHeight) {
			TilePiece firstTile = tiles[0];
			for (int i = 1; i < tiles.Length; i++) {
				tiles[i-1] = tiles[i];
			}
			tiles[tiles.Length - 1] = firstTile;
			firstTile.tile.transform.Translate(cachedTranslate);
		}
	}
}
