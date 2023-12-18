using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Shared.SaveData;
using GameData.DataAsset;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.PlacementActionBar
{
	public class PlacementActionBarView : MonoBehaviour
	{
		// Configurations //
		// =============== //
		[SerializeField]
		private GameObject _actionButtonPrefab;

		[SerializeField]
		private ToggleGroup _toggleGroup;

		[SerializeField]
		private Transform _actionButtonsContainer;

		// States //
		// ====== //
		private readonly Dictionary<int, PlacementActionButton> _actionButtonComponents = new();
		private PlacementActionButton _selectedButton;

		// Dependencies //
		// ============ //
		private RuntimePlayerDataService _playerDataService;
		private PlacementItemDataTable _placementItemDataTable;

		// Events //
		// ====== //
		public IObservable<bool> ToggleGroupValueChanged =>
			_toggleGroup.ObserveEveryValueChanged(x => x.AnyTogglesOn());

		public readonly Subject<(bool, int)> OnItemToggleOnOffStateChange = new();

		[Inject]
		public void DependencyInjection(RuntimePlayerDataService dataService,
			PlacementItemDataTable placementItemDataTable)
		{
			_playerDataService = dataService;
			_placementItemDataTable = placementItemDataTable;
		}

		private void Start()
		{
			GameInstance.GameInstance.Instance.Container.Inject(this);
			InitializeAsync().Forget();
		}

		private async UniTask InitializeAsync()
		{
			await UniTask.WaitUntil(() => GameInstance.GameInstance.Instance.IsAllServiceInitialized);
			CreatePlacementButton();
		}

		private void CreatePlacementButton()
		{
			foreach (var item in _playerDataService.Data.PlacingItems)
			{
				var placementItemDataAsset = _placementItemDataTable
					.DataRows.FirstOrDefault(x => x.Id == item.Id);
				var remainQuantity = _playerDataService.Data.GetRemainingQuantity(item.Id);
				var data = new PlacementActionButtonData {
					ItemId = item.Id,
					Quantity = remainQuantity,
				};
				if (placementItemDataAsset != null)
				{
					data.Icon = placementItemDataAsset.IconSprite;
				}

				RegisterButton(data);
			}
		}

		public void RegisterButton(in PlacementActionButtonData data)
		{
			PlacementActionButton component;
			if (_actionButtonComponents.TryGetValue(data.ItemId, out var buttonComponent))
			{
				// Already have this button
				component = buttonComponent;
			} else
			{
				var buttonGo = Instantiate(_actionButtonPrefab, _actionButtonsContainer);
				component = buttonGo.GetComponent<PlacementActionButton>();
				_actionButtonComponents[data.ItemId] = component;
			}

			component.SetupData(_toggleGroup, data);
			component.Toggle.ObserveEveryValueChanged(x => x.isOn)
				.Subscribe(isOn => OnItemToggleOnOffStateChange.OnNext((isOn, component.ItemId)))
				.AddTo(component);
		}

		public void UpdateRemainingCount(int itemId, int remainingCount)
		{
			if (!_actionButtonComponents.ContainsKey(itemId))
			{
				return;
			}

			var component = _actionButtonComponents[itemId];
			component.UpdateRemainingCount(remainingCount);
		}
	}
}