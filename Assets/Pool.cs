using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Pool design pattern for resuing objects. O(1) to find a free object to use from pool, O(1) return the object to pool.
/// </summary>
public class Pool
{
	public enum State {
		FREE,
		USED,
	}
	private Dictionary<IPoolObject, State> pool;
	private List<IPoolObject> freeList;
	
	public Pool()
	{
		pool = new Dictionary<IPoolObject, State>();
		freeList = new List<IPoolObject>();
	}
	
	public void clear() {
		foreach (IPoolObject poolObject in pool.Keys) {
			poolObject.poolClear();
		}
		pool.Clear();
		freeList.Clear();
	}
	
	public void add(IPoolObject free) {
		// if free == null, probably the object does not implement IPoolObject interface.
		freeList.Add(free);
		pool.Add(free, State.FREE);
	}
	
	public IPoolObject use() {
		int lastIndex = freeList.Count - 1;
		if (lastIndex < 0) {
			throw new UnityException("not enough objects in the pool!");
		}
		IPoolObject toReturn = freeList[lastIndex];
		freeList.RemoveAt(lastIndex);
		pool[toReturn] = State.USED;
		toReturn.poolUse();
		return toReturn;
	}
	
	public void addAndUse(IPoolObject toUse) {
		// if toUse == null, probably the object does not implement IPoolObject interface.
		pool.Add(toUse, State.USED);
		toUse.poolUse();
	}
	
	public void returnToPool(IPoolObject toFree) {
		freeList.Add(toFree);
		pool[toFree] = State.FREE;
		toFree.poolReturn();
	}
	
	public int getFreeCount() {
		return freeList.Count;
	}
	
	public int getTotalCount() {
		return pool.Count;
	}
	
	public int getUsedCount() {
		return pool.Count - freeList.Count;
	}
	
	public float getPercentUsed() {
		return (float) getUsedCount() / (float) pool.Count;
	}	
}


