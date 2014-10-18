using UnityEngine;
using System.Collections;

public class TileBehaviour : MonoBehaviour {

	// assumes is bottom aligned
	public GameObject tile;
	// assumes is top aligned
	public GameObject top;

	private float tileHeight;
	private GameObject tile2;
	private Vector3 originalPosition;
	private Vector3 translate;
	private GameObject currAboveTile;
	private GameObject currBelowTile;
	private int numTilesToWrap;
	private float offsetToTileToTop;

	// Use this for initialization
	void Start () {
		tileHeight = ((SpriteRenderer)tile.renderer).sprite.bounds.size.y;
		originalPosition = tile.transform.localPosition;
		tile2 = (GameObject)Instantiate (tile);
		tile2.transform.parent = transform;
		translate = new Vector3 (0, tileHeight, 0);
		reset ();
	}

	public void reset() {
		// top
		top.transform.localPosition = originalPosition + new Vector3 (0, Global.get ().gameDistance, 0);

		numTilesToWrap = Mathf.CeilToInt(Global.get().gameDistance/tileHeight);
		offsetToTileToTop = numTilesToWrap * tileHeight - Global.get ().gameDistance;
		Debug.Log ("tree reset! "+numTilesToWrap + " o:" + offsetToTileToTop + " h:" + tileHeight + " d:" + Global.get ().gameDistance);

		// tiles
		tile.transform.localPosition = originalPosition - new Vector3(0, offsetToTileToTop, 0);
		tile2.transform.localPosition = tile.transform.localPosition + new Vector3 (0, tileHeight, 0);
		currAboveTile = tile2;
		currBelowTile = tile;
	}
	
	// Update is called once per frame
	void Update () {
		if (currAboveTile.transform.position.y < -Camera.main.orthographicSize) {
			if(numTilesToWrap > 0){
				currBelowTile.transform.Translate(translate);
				//swap
				GameObject temp = currAboveTile;
				currAboveTile = currBelowTile;
				currBelowTile = temp;
				numTilesToWrap--;
			}
		}
	}
}
