using System.IO;
using Game.Shared.SaveData;
using UnityEditor;
using UnityEngine;

namespace GameEditor.Scripts
{
	public static class EditorHelper
	{
		[MenuItem("Project Tools/Delete Save Data")]
		public static void DeleteSaveData()
		{
			var path = FileHelpers.GetSaveDataPath();
			var runtimeSaveDataPath = $"{path}/{RuntimePlayerDataService.DATA_FILE_NAME}";
			if (File.Exists(runtimeSaveDataPath))
			{
				File.Delete(runtimeSaveDataPath);
				Debug.Log("[EditorHelper] DeleteSaveData Finished");
			}
		}
	}
}