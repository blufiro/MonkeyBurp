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
	
	public FruitUiEntry[] fruitUiTemplatesList;
	
	private Dictionary<CollectableType, GameObject> fruitUiTemplates;
	private List<CollectableType> fruitTypes;
	private List<GameObject> fruitUiGobs;
	private GameObject[] fruitSlots;
	private bool doClear;
	//private List<CollectableType> cashedInTypes;

	// Use this for initialization
	void Start () {
		fruitTypes = new List<CollectableType>();
		fruitUiGobs = new List<GameObject>();
		fruitUiTemplates = new Dictionary<CollectableType, GameObject>();
		foreach (FruitUiEntry entry in fruitUiTemplatesList) {
			fruitUiTemplates.Add(entry.fruitType, entry.fruitUI);
		}
		fruitSlots = new GameObject[Global.get().gameMaxSlots];
		for (int i=1; i<=Global.get().gameMaxSlots; i++) {
			GameObject slot = GameObject.Find("FruitSlot" + i);
			fruitSlots[i-1] = slot;
		}
		// Deactivate extra slots
		for (int i = Global.get().gameNumSlots; i < Global.get().gameMaxSlots; i++) {
			fruitSlots[i].SetActive(false);
		}
		doClear = false;
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void addFruit(FruitBehaviour fruit) {
		if (doClear) {
			// clear previous queue
			clearQueue();
		}
		fruitTypes.Add(fruit.type);
		int fruitIndex = fruitTypes.Count;
		fruitUiGobs.Add(makeFruitUI(fruit.type, fruitIndex-1));
		if (fruitIndex == Global.get().gameNumSlots) {
			cashIn();
			doClear = true;
		}
	}
	
	GameObject makeFruitUI(CollectableType fruitType, int fruitIndex) {
		Debug.Log("make fruit UI");
		GameObject slot = fruitSlots[fruitIndex];
		Vector3 spawnPosition = slot.transform.position;
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
	}
	
	void clearQueue() {
		// clear fruits
		fruitTypes.Clear();
		foreach (GameObject gob in fruitUiGobs) {
			Destroy(gob);
		}
		fruitUiGobs.Clear();
		doClear = false;
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
}
