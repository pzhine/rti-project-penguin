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
        const int OBJECTS_PER_LEVEL = 2;
        const int NUMBER_OF_LEVELS = 4;

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

        public Property<int> m_Coin = new Property<int>(OBJECTS_PER_LEVEL);
        public Property<int> m_Level = new Property<int>(0);

        [SerializeField]
        protected CanvasGroup m_CanvasGroup;


        void Awake()
        {
            Debug.Log("GameManager Awake!");
            if (m_Singleton != null)
            {
                Destroy(gameObject);
                return;
            }
            m_Singleton = this;

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
            if (this.m_Level.Value > 0)
            {
                yield return FadeToBlack();
                AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync("Scenes/Level-" + this.m_Level.Value + "-Scene");
                while (!asyncUnload.isDone)
                {
                    yield return null;
                }
            }
            this.m_Level.Value += 1;

            if (this.m_Level.Value <= NUMBER_OF_LEVELS)
            {
                Debug.Log("Advance to level " + this.m_Level.Value);
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Scenes/Level-" + this.m_Level.Value + "-Scene", LoadSceneMode.Additive);
                //SceneManager.LoadScene("Scenes/Level-X-Scene", LoadSceneMode.Additive);
                while (!asyncLoad.isDone)
                {
                    yield return null;
                }
                yield return FadeFromBlack();
            }
            else {
                Debug.Log("Game over, advance to questionnaire");
                //TODO: Load recognition questionnaire
            }

            yield return null;

        }

        void Update()
        {
            if (m_Coin.Value == 0)
            {
                m_Coin.Value = OBJECTS_PER_LEVEL;
                StartCoroutine(LoadNextLevelAsync());
            }
        }

        [System.Serializable]
        public class LoadEvent : UnityEvent
        {

        }

    }

}
