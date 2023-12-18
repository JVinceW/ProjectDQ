using Cysharp.Threading.Tasks;
using Game.PlacementActionBar;
using Game.Shared.GameResources;
using Game.Shared.SaveData;
using UniRx;
using UnityEngine;
using VContainer;

namespace Game.Placement
{
	public class PlacementLogicPresenter : MonoBehaviour
	{
		[SerializeField]
		private PlacementActionBarView _placementActionBarView;

		[SerializeField]
		private PlacementSystem _placementSystem;

		// Dependencies //
		// ============ //
		private RuntimePlayerDataService _playerDataService;

		[Inject]
		public void DependencyInjection(RuntimePlayerDataService playerDataService)
		{
			_playerDataService = playerDataService;
		}

		private void Start()
		{
			InitializeAsync().Forget();
		}

		private async UniTask InitializeAsync()
		{
			await UniTask.WaitUntil(() => GameInstance.GameInstance.Instance.IsAllServiceInitialized);
			InitializeObservable();
			InitializePlacementSystem();
		}

		private void InitializePlacementSystem()
		{
			_placementSystem.OnSuccessPlaceStructure += PlacementSystemOnOnSuccessPlaceStructure;
			_placementSystem.OnSuccessGatheringStructure += PlacementSystemOnOnSuccessGatheringStructure;
			_placementSystem.OnStopPlacement += OnStopPlacement;
			_placementSystem.ConstructGridFromSavedData().Forget();
		}

		private void PlacementSystemOnOnSuccessGatheringStructure(int itemId, Vector3Int position)
		{
			_playerDataService.Data.RemovePlacedItem(itemId, position);
			var remainingCount = _playerDataService.Data.GetRemainingQuantity(itemId);
			_placementActionBarView.UpdateRemainingCount(itemId, remainingCount);
		}

		private void OnStopPlacement()
		{
			// TODO: Stop placement
		}

		private void PlacementSystemOnOnSuccessPlaceStructure(int itemId, Vector3Int placementPosition)
		{
			_playerDataService.Data.AddNewPlacedItem(itemId, placementPosition);
			var remainingCount = _playerDataService.Data.GetRemainingQuantity(itemId);
			_placementActionBarView.UpdateRemainingCount(itemId, remainingCount);
		}

		private void InitializeObservable()
		{
			_placementActionBarView.ToggleGroupValueChanged
				.Subscribe(isOn => {
					if (!isOn)
					{
						_placementSystem.StopPlacement();
					}
				}).AddTo(this);
			_placementActionBarView.OnItemToggleOnOffStateChange.Subscribe(x => {
				OnActivatingForEdit(x.Item1, x.Item2);
			}).AddTo(this);
		}

		private void OnActivatingForEdit(bool isEditing, int itemId)
		{
			Debug.Log($"Start Editing Item ID: {itemId} - isEditing {isEditing}");
			if (isEditing)
			{
				_placementSystem.StartPlacement(itemId);
			}
		}
	}
}