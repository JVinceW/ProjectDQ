using UnityEngine;

namespace Game.Sound
{
	public class BGMController : MonoBehaviour
	{
		[SerializeField]
		private AudioSource _bgmAudioSource;

		public void StopBGM(bool isPause)
		{
			if (isPause)
			{
				_bgmAudioSource.Pause();
			} else
			{
				_bgmAudioSource.Stop();
			}
		}
		
		public void PlayBGM(bool isReset)
		{
			if (isReset)
			{
				_bgmAudioSource.Stop();
			}
			_bgmAudioSource.Play();
		}
	}
}