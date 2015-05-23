using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FruitSlotQueue : MonoBehaviour {

	[System.Serializable]
	public class FruitUiEntry
	{
		public CollectableType fruitType;
		public GameObject fruitUI;
	}
	
	public FruitUiEntry[] fruitUiList;
	
	private Dictionary<CollectableType, GameObject> fruitUiTemplates;
	private List<CollectableType> fruitTypes;
	private List<GameObject> fruitUiGobs;
	//private List<CollectableType> cashedInTypes;

	// Use this for initialization
	void Start () {
		fruitTypes = new List<CollectableType>();
		fruitUiGobs = new List<GameObject>();
		fruitUiTemplates = new Dictionary<CollectableType, GameObject>();
		foreach (FruitUiEntry entry in fruitUiList) {
			fruitUiTemplates.Add(entry.fruitType, entry.fruitUI);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void addFruit(FruitBehaviour fruit) {
		fruitTypes.Add(fruit.type);
		int fruitIndex = fruitTypes.Count;
		fruitUiGobs.Add(makeFruitUI(fruit.type, fruitIndex));
		if (fruitIndex == Global.get().gameNumSlots) {
			cashIn();
		}
	}
	
	GameObject makeFruitUI(CollectableType fruitType, int fruitIndex) {
		Debug.Log("make fruit UI");
		Vector3 spawnPosition = new Vector3();
		GameObject newFruitUI = (GameObject) Instantiate(fruitUiTemplates[fruitType], spawnPosition, Quaternion.identity);
		newFruitUI.transform.parent = this.transform;
		return newFruitUI;
	}
	
	void cashIn() {
		// score fruit combinations
		int inARowMultiplier = comboInARowMultiplier(fruitTypes);
		int total = Global.get().scoreBase * inARowMultiplier;
		Global.controller.cashedIn(total);
		Debug.Log("cashIn: " + total);
		
		// clear fruits
		fruitTypes.Clear();
		foreach (GameObject gob in fruitUiGobs) {
			Destroy(gob);
		}
		fruitUiGobs.Clear();
	}
	
	/**
	 * Returns the largest number of the same type in a row.
	 */
	private static int comboInARowMultiplier(List<CollectableType> fruitTypes) {
		CollectableType current = CollectableType.NONE;
		int previousLargestRun = 0;
		int largestRun = 0;
		foreach (CollectableType c in fruitTypes) {
			if (current == CollectableType.NONE) { // first element
				current = c;
				largestRun++;
			} else if (current == c) { // same as previous
				largestRun++;
			} else { // different from previous
				current = c;
				if (largestRun > previousLargestRun) {
					previousLargestRun = largestRun;
				}
				largestRun = 1;
			}
		}
		if (largestRun < previousLargestRun) {
			largestRun = previousLargestRun;
		}
		// singles and pairs do not count as in a row
		int multiplier = (largestRun < 3) ? 1 : largestRun;
		return multiplier;
	}
	
	// temporary ui
	void OnGUI() {
		GUI.Box(new Rect(0,0,300, 100), "test");
	}
}
