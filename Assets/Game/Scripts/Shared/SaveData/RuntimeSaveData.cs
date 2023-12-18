using System;
using System.Linq;
using System.Collections.Generic;
using GameData.DataAsset;
using UnityEngine;

namespace Game.Shared.SaveData
{
	/// <summary>
	/// Class use to storage the runtime data of player progress.
	/// </summary>
	[Serializable]
	public class RuntimeSaveData
	{
		public List<PlayerItemDataModel> PlacingItems = new();

		public List<PlacingItemGridDataModel> PlacingItemGrids = new();

		/// <summary>
		/// Flag to determine if the default setup has been loaded or the player progress has been loaded
		/// </summary>
		public bool DefaultSetupLoaded = true;

		#region Methods
		public int GetRemainingQuantity(int itemId)
		{
			var placedCount = GetPlacedQuantity(itemId);
			var item = PlacingItems.Find(x => x.Id == itemId);
			var remaining = item.Quantity - placedCount;
			return remaining;
		}

		public int GetPlacedQuantity(int itemId)
		{
			var placedCount = PlacingItemGrids.Count(x => x.Id == itemId);
			return placedCount;
		}

		public void AddNewPlacedItem(int itemId, Vector3Int gridPosition)
		{
			var placingItemGridData = new PlacingItemGridDataModel {
				Id = itemId,
				PlacedGridPosition = new Vector3IntSerializable(gridPosition.x, gridPosition.y, gridPosition.z)
			};
			PlacingItemGrids.Add(placingItemGridData);
		}

		public void RemovePlacedItem(int itemId, Vector3Int gridPosition)
		{
			var convertedVector = new Vector3IntSerializable(gridPosition.x, gridPosition.y, gridPosition.z);
			var idx = PlacingItemGrids.FindIndex(x => x.Id == itemId
			                                          && x.PlacedGridPosition.Equals(convertedVector));
			if (idx < 0)
			{
				return;
			}

			PlacingItemGrids.RemoveAt(idx);
		}

		public bool IsValidPlacing(int itemId)
		{
			return GetRemainingQuantity(itemId) > 0;
		}

		public bool IsValidGathering(int itemId)
		{
			return GetPlacedQuantity(itemId) > 0;
		}
		#endregion Methods
	}

	[Serializable]
	public class PlacingItemGridDataModel
	{
		public int Id;
		public Vector3IntSerializable PlacedGridPosition;
	}

	[Serializable]
	public class PlayerItemDataModel
	{
		public int Id;
		public string ItemName;
		public int Quantity;
	}
}