using Cysharp.Threading.Tasks;
using Game.GameInstance;
using Game.Shared.SaveData;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer.Unity;

namespace Game.Shared.GameResources
{
	public class GameResourceService : IService, IInitializable, IInitializeWaiter
	{
		public bool IsInitialized { get; private set; }

		public void Initialize()
		{
			InitializeService().Forget();
		}

		private async UniTask InitializeService()
		{
			await Addressables.InitializeAsync();
			Debug.Log("[GameResourceService]Finished initialized GameResourceService");
			IsInitialized = true;
		}
	}
}