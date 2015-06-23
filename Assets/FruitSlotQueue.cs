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

	public void removeAllFruits(){
		clearQueue ();
	}
	
	GameObject makeFruitUI(CollectableType fruitType, int fruitIndex) {
		GameObject slot = fruitSlots[fruitIndex];
		Vector3 spawnPosition = slot.transform.position;
		GameObject newFruitUI = (GameObject) Instantiate(fruitUiTemplates[fruitType], spawnPosition, Quaternion.identity);
		newFruitUI.transform.parent = this.transform;
		return newFruitUI;
	}
	
	void cashIn() {
		// score fruit combinations
		BonusType inARow = comboInARow(fruitTypes);
		BonusType alternate = comboAlternate(fruitTypes);
		BonusType rainbow = comboRainbow(fruitTypes);
		int total = Global.get().scoreBase
			* Global.getMultiplier(inARow)
			* Global.getMultiplier(alternate)
			+ Global.getAddition(rainbow);
		Global.controller.cashedIn(total, makeBonusSet(inARow, alternate, rainbow));
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
	
	private static List<BonusType> makeBonusSet(params BonusType[] bonuses) {
		List<BonusType> bonusSet = new List<BonusType>();
		for (int i=0; i<bonuses.Length; i++){
			BonusType bonus = bonuses[i];
			if (bonus != BonusType.NONE) {
				bonusSet.Add(bonus);
			}
		}
		return bonusSet;
	}
	
	/**
	 * Returns the largest number of the same type in a row.
	 */
	private static BonusType comboInARow(List<CollectableType> fruitTypes) {
		CollectableType current = CollectableType.NONE;
		int previousLargestRun = 0;
		int largestRun = 0;
		foreach (CollectableType c in fruitTypes) {
			// Rotten fruits are ignored.
			if (c >= CollectableType.FRUIT_BANANA_ROTTEN) {
				current = CollectableType.NONE;
				largestRun = 0;
				continue;
			}
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
		if (largestRun == 3) {
			return BonusType.THREE_IN_A_ROW;
		} else if (largestRun == 4) {
			return BonusType.FOUR_IN_A_ROW;
		} else if (largestRun == 5) {
			return BonusType.FIVE_IN_A_ROW;
		} else if (largestRun == 6) {
			return BonusType.SIX_IN_A_ROW;
		}
		return BonusType.NONE;
	}
	
	private static BonusType comboAlternate(List<CollectableType> fruitTypes) {
		CollectableType firstType = fruitTypes[0];
		bool isOdd = true;
		foreach(CollectableType c in fruitTypes) {
			// Rotten fruits are ignored.
			if (c >= CollectableType.FRUIT_BANANA_ROTTEN) {
				return BonusType.NONE;
			}
			if (isOdd && c != firstType) {
				return BonusType.NONE;
			} else if (!isOdd && c == firstType) {
				return BonusType.NONE;
			}
			isOdd = !isOdd;
		}
		return BonusType.ALTERNATE;
	}
	
	private static HashSet<CollectableType> rainbowSet = new HashSet<CollectableType>();
	private static BonusType comboRainbow(List<CollectableType> fruitTypes) {
		rainbowSet.Clear();
		foreach(CollectableType c in fruitTypes) {
			// Rotten fruits are ignored.
			if (c >= CollectableType.FRUIT_BANANA_ROTTEN) {
				return BonusType.NONE;
			}
			if (rainbowSet.Contains(c)) {
				return BonusType.NONE;
			}
			rainbowSet.Add(c);
		}
		return BonusType.RAINBOW;
	}
}
