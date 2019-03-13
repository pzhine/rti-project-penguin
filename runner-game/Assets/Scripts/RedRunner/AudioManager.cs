using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedRunner
{

	public class AudioManager : MonoBehaviour
	{

		#region Singleton

		private static AudioManager m_Singleton;

		public static AudioManager Singleton {
			get {
				return m_Singleton;
			}
		}

		#endregion

		#region Fields

		[Header ("Audio Sources")]
		[Space]
		[SerializeField]
		protected AudioSource m_CoinAudioSource;
        [SerializeField]
        protected AudioSource m_CharacterAudioSource;
        [SerializeField]
        protected AudioSource m_LevelAudioSource;

        [Header ("Sound Clips")]
		[Space]
		[SerializeField]
		protected AudioClip m_CoinSound;
		[SerializeField]
		protected AudioClip m_JumpSound;
        [SerializeField]
        protected AudioClip m_ClapSound;

        #endregion

        #region MonoBehaviour Messages

        void Awake ()
		{
			m_Singleton = this;
		}

		#endregion

		#region Methods


		public void PlaySoundAt (AudioClip clip, Vector3 position, float volume)
		{
			AudioSource.PlayClipAtPoint (clip, position, volume);
		}

		public void PlaySoundOn (AudioSource audio, AudioClip clip)
		{
			audio.clip = clip;
			audio.Play ();
		}

		public void PlayCoinSound (Vector3 position)
		{
			PlaySoundOn (m_CoinAudioSource, m_CoinSound);
		}

        public void PlayJumpSound()
        {
            PlaySoundOn(m_CharacterAudioSource, m_JumpSound);
        }

        public void PlayLevelCompleteSound()
        {
            PlaySoundOn(m_LevelAudioSource, m_ClapSound);
        }

        #endregion

    }

}