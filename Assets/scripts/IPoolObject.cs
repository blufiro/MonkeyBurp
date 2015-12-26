/// <summary>
/// Interface for Pool class.
/// </summary>
public interface IPoolObject
{
	/// <summary>
	/// Called when the pool is cleared. Use this method to destroy the object if necessary.
	/// </summary>
	void poolClear();
	
	/// <summary>
	/// Called when the the object is taken out of the pool for use.
	/// </summary>
	void poolUse();
	
	/// <summary>
	/// Called when the object is returned to the pool for reuse. Do not destroy the object.
	/// </summary>
	void poolReturn();
}