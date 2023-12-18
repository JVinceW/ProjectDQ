using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.DataModel
{
	[Serializable]
	public class GridDataModel
	{
		private Dictionary<Vector3Int, PlacementData> _placementData = new();

		public void AddObjectAt(Vector3Int position, Vector3Int objSize, int objId)
		{
			List<Vector3Int> occupiedCells = CalculatePosition(position, objSize);
			var data = new PlacementData(occupiedCells, objId);
			foreach (var cell in occupiedCells)
			{
				var addResult = _placementData.TryAdd(cell, data);
				if (addResult)
				{
					continue;
				}
				Debug.Log("can not add, cell is occupied");
				return;
			}
		}

		public void RemoveObjectAt(Vector3Int position, Vector3Int objSize, int objId)
		{
			var occupiedCells = CalculatePosition(position, objSize);
			foreach (var cell in occupiedCells)
			{
				_placementData.Remove(cell);
			}
		}

		private List<Vector3Int> CalculatePosition(Vector3Int position, Vector3Int objSize)
		{
			var occupiedCells = new List<Vector3Int>();
			for (var x = 0; x < objSize.x; x++)
			{
				for (var y = 0; y < objSize.y; y++)
				{
					for (var z = 0; z < objSize.z; z++)
					{
						var occupiedPosition = position + new Vector3Int(x, y, z);
						occupiedCells.Add(occupiedPosition);
					}
				}
			}

			return occupiedCells;
		}

		public bool CanPlaceObjectAt(Vector3Int position, Vector3Int objSize)
		{
			var occupiedCells = CalculatePosition(position, objSize);
			return occupiedCells.All(cell => !_placementData.ContainsKey(cell));
		}
		
		public PlacementData GetPlacementDataAt(Vector3Int position)
		{
			return _placementData.GetValueOrDefault(position);
		}
	}

	[Serializable]
	public class PlacementData
	{
		public List<Vector3Int> OccupiedCells;
		public int Id;

		public PlacementData(List<Vector3Int> occupiedCells, int id)
		{
			OccupiedCells = occupiedCells;
			Id = id;
		}
	}
}