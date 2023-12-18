using System.Collections.Generic;
using Game.Shared.SaveData;
using UnityEngine;

namespace Game.Player
{
	[CreateAssetMenu(fileName = "NewPlayerDefaultSetup", menuName = "Project/NewPlayerSetupDataAsset", order = 0)]
	public class NewPlayerSetupDataAsset : ScriptableObject
	{
		[SerializeField]
		private List<PlayerItemDataModel> _defaultResources;

		public List<PlayerItemDataModel> DefaultResources => _defaultResources;
	}
}