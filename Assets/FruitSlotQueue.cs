using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FruitSlotQueue : MonoBehaviour {

	public List<CollectableType> fruitTypes;
	public List<GameObject> fruitGobs;
	public List<CollectableType> cashedInTypes;

	// Use this for initialization
	void Start () {
		fruitTypes = new List<CollectableType>();
		fruitGobs = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void addFruit(FruitBehaviour fruit) {
		fruitTypes.Add(fruit.type);
		if (fruitTypes.Count == Global.get().gameNumSlots) {
			cashIn();
		}
	}
	
	void cashIn() {
		// score fruit combinations
		int inARowMultiplier = comboInARowMultiplier(fruitTypes);
		int total = Global.get().scoreBase * inARowMultiplier;
		Global.controller.cashedIn(total);
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
