namespace Game.GameInstance
{
	/// <summary>
	/// Interface that should be implemented by any class that needs to wait for initialization.
	/// </summary>
	public interface IInitializeWaiter
	{
		bool IsInitialized { get; }
	}
}