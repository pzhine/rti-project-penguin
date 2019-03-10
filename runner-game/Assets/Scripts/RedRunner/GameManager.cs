using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using RedRunner.Characters;
using RedRunner.Collectables;
// using RedRunner.TerrainGeneration;

namespace RedRunner
{
    public sealed class GameManager : MonoBehaviour
    {
        public delegate void ScoreHandler(float newScore, float highScore, float lastScore);

        public static event ScoreHandler OnScoreChanged;

        private static GameManager m_Singleton;
        public static GameManager Singleton
        {
            get
            {
                return m_Singleton;
            }
        }

        public int m_Coin;
        public int m_Level;
        public string m_SessionId;

        [SerializeField]
        public int OBJECTS_PER_LEVEL;

        [SerializeField]
        public int NUMBER_OF_LEVELS;

        [SerializeField]
        public FirebaseLogger m_Logger;

        [SerializeField]
        protected CanvasGroup m_CanvasGroup;

        [SerializeField]
        protected int m_StartLevel;

        void Awake()
        {
            Debug.Log("GameManager Awake!");
            if (m_Singleton != null)
            {
                Destroy(gameObject);
                return;
            }
            m_Singleton = this;
            m_Level = m_StartLevel - 1;
            m_Coin = OBJECTS_PER_LEVEL;
            m_SessionId = ShortGuid();

            m_Logger.StartSession();

            StartCoroutine(LoadNextLevelAsync());
        }

        IEnumerator FadeToBlack()
        {

            while(m_CanvasGroup.alpha < 1)
            {
                m_CanvasGroup.alpha += Time.deltaTime;
                yield return null;

            }
            yield return null;
        }

        IEnumerator FadeFromBlack()
        {
            while (m_CanvasGroup.alpha > 0)
            {
                m_CanvasGroup.alpha -= Time.deltaTime;
                yield return null;

            }
            yield return null;
        }

        IEnumerator LoadNextLevelAsync()
        {
            if (this.m_Level > (this.m_StartLevel - 1))
            {
                m_Logger.EndLevel();

                yield return FadeToBlack();
                AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync("Scenes/Level-" + this.m_Level + "-Scene");
                while (!asyncUnload.isDone)
                {
                    yield return null;
                }
            }
            this.m_Level += 1;
            string nextLevel = "Scenes/Level-" + this.m_Level + "-Scene";

            if (this.m_Level > NUMBER_OF_LEVELS)
            {
                nextLevel = "Scenes/GameOver-Scene";
            }
            else
            {
                m_Logger.StartLevel(this.m_Level);
            }

            Debug.Log("Advance to level " + nextLevel);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextLevel, LoadSceneMode.Additive);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            yield return FadeFromBlack();
        }

        void Update()
        {
            if (m_Coin == 0)
            {
                m_Coin = OBJECTS_PER_LEVEL;
                StartCoroutine(LoadNextLevelAsync());
            }
        }

        private string ShortGuid()
        {
            string encoded = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            return encoded.Substring(0, 22).Replace("/", "_").Replace("+", "__");
        }

        [System.Serializable]
        public class LoadEvent : UnityEvent
        {

        }

    }

}
