using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Shared.SaveData
{
	public interface IDataAccessor<T>
	{
		T Data { get; }
		public UniTask<T> LoadAsync(CancellationToken token = default); 
		public UniTask SaveAsync(CancellationToken token = default);
		public string GetSavePath();
	}
}