using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.GameInstance;
using Game.Player;
using Newtonsoft.Json;
using UnityEngine;
using VContainer.Unity;

namespace Game.Shared.SaveData
{
	public class RuntimePlayerDataService :
		IService,
		IDataAccessor<RuntimeSaveData>, IInitializeWaiter, IInitializable
	{
		public RuntimeSaveData Data { get; private set; } = new();
		private NewPlayerSetupDataAsset _newPlayerSetupData;
		public const string DATA_FILE_NAME = "RuntimeSaveData.json";

		private NewPlayerSetupDataAsset GetNewPlayerSetupData()
		{
			if (_newPlayerSetupData == null)
			{
				_newPlayerSetupData = Resources.Load<NewPlayerSetupDataAsset>("NewPlayerDefaultSetup");
			}

			return _newPlayerSetupData;
		}

		public async UniTask<RuntimeSaveData> LoadAsync(CancellationToken token = default)
		{
			var isExist = File.Exists(GetSavePath());
			var readDataStr = string.Empty;
			if (isExist)
			{
				readDataStr = await File.ReadAllTextAsync(GetSavePath(), Encoding.UTF8, cancellationToken: token)
					.AsUniTask();
			}

			if (string.IsNullOrEmpty(readDataStr))
			{
				AssignNewPlayerSetupData();
				return Data;
			}

			Debug.Log($"[RuntimeDataService] LoadAsync Finished: {readDataStr}");
			Data = JsonConvert.DeserializeObject<RuntimeSaveData>(readDataStr);
			Data.DefaultSetupLoaded = false;
			return Data;
		}

		private void AssignNewPlayerSetupData()
		{
			Data.PlacingItems = new List<PlayerItemDataModel>(GetNewPlayerSetupData().DefaultResources);
			Data.DefaultSetupLoaded = false;
		}

		public async UniTask SaveAsync(CancellationToken token = default)
		{
			var savePath = GetSavePath();
			var json = JsonConvert.SerializeObject(Data, new JsonSerializerSettings {
				Formatting = Formatting.Indented
			});
			await File.WriteAllTextAsync(savePath, json, Encoding.UTF8, token).AsUniTask();
			Debug.Log("[RuntimeDataService] SaveAsync Finished");
		}

		public string GetSavePath()
		{
			var path = FileHelpers.GetSaveDataPath();
			Directory.CreateDirectory(path);
			var filePath = Path.Combine(path, DATA_FILE_NAME);
			Debug.Log($"[RuntimeDataService] GetSavePath: {filePath}");
			return filePath;
		}

		public bool IsInitialized { get; private set; }
		public UniTask InitializeService()
		{
			throw new System.NotImplementedException();
		}

		public void Initialize()
		{
			UniTask.Create(async () => {
				await LoadAsync();
				IsInitialized = true;
			});
		}
	}
}