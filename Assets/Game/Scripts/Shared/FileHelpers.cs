using UnityEngine;

namespace Game.Shared.SaveData
{
	public class FileHelpers
	{
		public static string GetBaseSaveDataPath()
		{
			return Application.persistentDataPath;
		}
		
		public static string GetSaveDataPath()
		{
			return $"{GetBaseSaveDataPath()}/Data/";
		}
	}
}