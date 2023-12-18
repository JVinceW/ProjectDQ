using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Player;
using Game.Shared.GameResources;
using Game.Shared.SaveData;
using GameData.DataAsset;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace Game.GameInstance
{
	/// <summary>
	/// This is the entry point of the game.
	/// And it should be the only one instance of the game. This is the only one Singleton should be exist in the game.
	/// And we should not use singleton pattern in other places. Also, only use this instance to call the Injection method.
	/// Other complex logic should be put in the separate class.
	/// </summary>
	public class GameInstance : LifetimeScope
	{
		[Header("Basic config of the game")]
		[SerializeField]
		protected NewPlayerSetupDataAsset _newPlayerSetupData;

		[SerializeField]
		private PlacementItemDataTable _placementItemDataTable;

		[Header("Input Setting")]
		[Space(10)]
		[SerializeField]
		protected InputActionMap _simpleInputActionMap;

		private readonly List<IInitializeWaiter> _initializeWaiters = new();
		private static GameInstance instance;
		public static GameInstance Instance => instance;

		private readonly RuntimePlayerDataService _runtimePlayerDataService = new();
		private readonly GameResourceService _gameResourceService = new();
		private bool _isQuitApplication;

		public bool IsAllServiceInitialized {
			get { return _initializeWaiters.All(x => x.IsInitialized); }
		}

		protected override void Awake()
		{
			base.Awake();
			if (instance != null)
			{
				Destroy(gameObject);
			}

			instance = this;
			DontDestroyOnLoad(gameObject);
		}

		protected override void Configure(IContainerBuilder builder)
		{
			builder.RegisterInstance(_newPlayerSetupData);
			builder.RegisterInstance(_placementItemDataTable);

			// Register data service.
			_initializeWaiters.Add(_gameResourceService);
			_initializeWaiters.Add(_runtimePlayerDataService);
			Debug.LogWarning($"Get hash: {_runtimePlayerDataService.GetHashCode()}");
			builder.RegisterInstance(_runtimePlayerDataService)
				.AsImplementedInterfaces()
				.AsSelf();
			builder.RegisterInstance(_gameResourceService)
				.AsImplementedInterfaces()
				.AsSelf();
		}

		private void OnEnable()
		{
			_simpleInputActionMap.actions[0].performed += QuitApplication;
			_simpleInputActionMap.Enable();
		}

		private void QuitApplication(InputAction.CallbackContext obj)
		{
			Debug.Log("on quit application");
			if (_isQuitApplication)
			{
				return;
			}

			_isQuitApplication = true;
			UniTask.Create(async () => {
				await _runtimePlayerDataService.SaveAsync();
				Debug.Log("Quit application");
				Application.Quit();
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
#endif
				_isQuitApplication = false;
			});
		}

		private void OnDisable()
		{
			_simpleInputActionMap.actions[0].performed -= QuitApplication;
			_simpleInputActionMap.Disable();
		}
	}
}