using System;
using Game.Shared.SaveData;
using UnityEngine;
using VContainer;

namespace Game.Player
{
	public class PlayerState : MonoBehaviour
	{
		// Dependencies //
		// ============ //
		private RuntimePlayerDataService _playerDataService;

		[Inject]
		public void InjectDependencies(RuntimePlayerDataService playerDataService)
		{
			_playerDataService = playerDataService;
		}
	}
}