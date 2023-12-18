using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameData.DataAsset
{
	[CreateAssetMenu(fileName = "PlacementItemDataTable", menuName = "Project/DataTable/PlacementItemDataTable", order = 0)]
	public class PlacementItemDataTable : ScriptableObject
	{
		[SerializeField]
		private List<PlacementItemDataTableRow> _dataRows = new();

		public List<PlacementItemDataTableRow> DataRows => _dataRows;
	}

	[Serializable]
	public class PlacementItemDataTableRow
	{
		[SerializeField]
		private int _id;

		[SerializeField]
		private string _itemName;

		[SerializeField]
		private AssetReferenceGameObject _objectPrefab;

		[SerializeField]
		private AssetReferenceSprite _iconSprite;

		/// <summary>
		/// Size of the object when place on grid.
		/// </summary>
		[SerializeField]
		private Vector3Int _objectCellSize;

		/// <summary>
		/// Limit the quantity of the object can be placed.
		/// </summary>
		[SerializeField]
		private int _maxQuantity = int.MaxValue;
		
		// Accessors //
		// ========= //
		public int Id => _id;
		public string ItemName => _itemName;
		public AssetReferenceGameObject ObjectPrefab => _objectPrefab;
		public Vector3Int ObjectCellSize => _objectCellSize;
		public AssetReferenceSprite IconSprite => _iconSprite;
		public int MaxQuantity => _maxQuantity;
	}
}