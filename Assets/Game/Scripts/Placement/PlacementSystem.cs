using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.DataModel;
using Game.Shared.SaveData;
using GameData.DataAsset;
using UnityEngine;
using VContainer;

namespace Game.Placement
{
	public class PlacementSystem : MonoBehaviour
	{
		[SerializeField]
		private GameObject _mouseIndicator, _cellIndicator;

		[SerializeField]
		private PlacementSystemInput _placementSystemInput;

		[SerializeField]
		private Grid _grid;

		[SerializeField]
		private PlacementItemDataTable _dataTable;

		[SerializeField]
		private GameObject _gridVisualization;

		[SerializeField]
		private Color _validColor, _invalidColor;

		[SerializeField]
		private Renderer _previewRenderer;

		[Space(10)]
		[Header("SFX")]
		[SerializeField]
		private AudioSource _audioSource;

		[SerializeField]
		private AudioClip _placeStructureSfx;

		[SerializeField]
		private AudioClip _invalidPlacementPositionSfx;

		[SerializeField]
		private AudioClip _gatheredResourceSfx;

		public event Action<int, Vector3Int> OnSuccessPlaceStructure, OnSuccessGatheringStructure;

		public event Action OnStopPlacement;

		private readonly Dictionary<GameObject, Vector3Int> _placedGo = new();
		private GridDataModel _gridData;

		private int _selectedIdx = -1;
		private RuntimePlayerDataService _playerDataService;

		#region Lifecycle
		[Inject]
		public void DependencyInjection(RuntimePlayerDataService playerDataServices)
		{
			_playerDataService = playerDataServices;
		}

		private void Start()
		{
			StopPlacement();
			_gridData = new GridDataModel();
		}

		private void Update()
		{
			if (_selectedIdx < 0)
			{
				return;
			}

			var mousePosition = _placementSystemInput.GetPlaceableSurfaceMapPosition();
			var gridPosition = _grid.WorldToCell(mousePosition);
			var gridWorldPosition = _grid.CellToWorld(gridPosition);
			var placementValid = CheckPlacementValidity(gridPosition);

			// prevent Z fighting
			mousePosition.y += 0.01f;
			gridWorldPosition.y += 0.01f;

			_previewRenderer.material.color = placementValid ? _validColor : _invalidColor;
			_mouseIndicator.transform.position = mousePosition;
			_cellIndicator.transform.position = gridWorldPosition;
		}
		#endregion

		#region Methods
		public void StartPlacement(int itemId)
		{
			StopPlacement();
			_selectedIdx = _dataTable.DataRows.FindIndex(x => x.Id == itemId);
			if (_selectedIdx < 0)
			{
				Debug.Log($"Item not found : {itemId}");
				return;
			}

			Debug.Log("_selectedIdx : " + _selectedIdx);
			_gridVisualization.SetActive(true);
			_cellIndicator.SetActive(true);
			_placementSystemInput.OnLeftClicked += PlaceStructure;
			_placementSystemInput.OnRightClicked += GatheringStructure;
			_placementSystemInput.OnExitPlacementModeClicked += StopPlacement;
		}

		public void StopPlacement()
		{
			Debug.Log("OnClick StopPlacement");
			_selectedIdx = -1;
			_placementSystemInput.OnLeftClicked -= PlaceStructure;
			_placementSystemInput.OnRightClicked -= StopPlacement;
			_placementSystemInput.OnExitPlacementModeClicked -= StopPlacement;
			_cellIndicator.SetActive(false);
			_gridVisualization.SetActive(false);
			OnStopPlacement?.Invoke();
		}

		public async UniTask ConstructGridFromSavedData()
		{
			foreach (var placed in _playerDataService.Data.PlacingItemGrids)
			{
				var dataRowIdx = _dataTable.DataRows.FindIndex(x => x.Id == placed.Id);
				var dataRow = _dataTable.DataRows[dataRowIdx];
				var gridPosition = new Vector3Int(placed.PlacedGridPosition.X, placed.PlacedGridPosition.Y,
					placed.PlacedGridPosition.Z);
				var go = await dataRow.ObjectPrefab.InstantiateAsync().ToUniTask();
				go.transform.SetPositionAndRotation(_grid.CellToWorld(gridPosition), Quaternion.identity);
				_placedGo.Add(go, gridPosition);
				_gridData.AddObjectAt(gridPosition, dataRow.ObjectCellSize, dataRow.Id);
			}
		}
		#endregion Methods

		#region Subroutine
		private void PlaceStructure()
		{
			if (_placementSystemInput.IsPointerUI())
			{
				return;
			}

			ExecutePlacement().Forget();
		}

		private async UniTask ExecutePlacement()
		{
			var mousePosition = _placementSystemInput.GetPlaceableSurfaceMapPosition();
			var gridPosition = _grid.WorldToCell(mousePosition);

			var placementValid = CheckPlacementValidity(gridPosition);
			if (!placementValid)
			{
				_audioSource.PlayOneShot(_invalidPlacementPositionSfx);
				return;
			}

			var dataRow = _dataTable.DataRows[_selectedIdx];
			var structureGo = await dataRow.ObjectPrefab.InstantiateAsync();
			structureGo.transform.position = _grid.CellToWorld(gridPosition);
			_audioSource.PlayOneShot(_placeStructureSfx);
			_placedGo.Add(structureGo, gridPosition);
			_gridData.AddObjectAt(gridPosition, dataRow.ObjectCellSize, dataRow.Id);
			OnSuccessPlaceStructure?.Invoke(dataRow.Id, gridPosition);
		}

		private bool CheckPlacementValidity(Vector3Int gridPosition)
		{
			// check conflict with other object
			var dataRow = _dataTable.DataRows[_selectedIdx];
			var canPlace = _gridData.CanPlaceObjectAt(gridPosition, dataRow.ObjectCellSize);

			// check if the player has enough resource
			var hasEnoughResource = _playerDataService.Data.IsValidPlacing(dataRow.Id);
			return canPlace && hasEnoughResource;
		}

		private void GatheringStructure()
		{
			if (_placementSystemInput.IsPointerUI())
			{
				return;
			}

			_placementSystemInput.GetHitObjectInfoOnMousePosition(out var hits, out var hitCount);
			if (hitCount <= 0)
			{
				return;
			}

			var isNearestHitPlacedObject =
				_placementSystemInput.IsNearestHitOtherPlacementObject(hits, out var nearestHit);
			if (!isNearestHitPlacedObject)
			{
				return;
			}

			var objectToGathering = nearestHit.transform.root.gameObject;
			if (!_placedGo.ContainsKey(objectToGathering))
			{
				Debug.LogWarning(
					"Something wrong with the data, object not registered to reference data map when placing");
				return;
			}

			var gridPositionOfObject = _placedGo[objectToGathering];
			var placementData = _gridData.GetPlacementDataAt(gridPositionOfObject);
			if (placementData == null)
			{
				Debug.LogWarning("Something wrong with the data, object not registered to grid data map when placing");
				return;
			}

			var idx = _dataTable.DataRows.FindIndex(x => x.Id == placementData.Id);
			if (idx < 0)
			{
				return;
			}

			var dataRow = _dataTable.DataRows[idx];
			_placedGo.Remove(objectToGathering);
			_gridData.RemoveObjectAt(gridPositionOfObject, dataRow.ObjectCellSize, dataRow.Id);
			_audioSource.PlayOneShot(_gatheredResourceSfx);
			OnSuccessGatheringStructure?.Invoke(dataRow.Id, gridPositionOfObject);
			Destroy(objectToGathering);
		}
		#endregion Subroutine
	}
}