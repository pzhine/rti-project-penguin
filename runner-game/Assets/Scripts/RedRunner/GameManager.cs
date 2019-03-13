using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        public string m_CurrentSceneName;

        [SerializeField]
        public int OBJECTS_PER_LEVEL;

        [SerializeField]
        public int NUMBER_OF_LEVELS;

        [SerializeField]
        public FirebaseLogger m_Logger;

        [SerializeField]
        protected CanvasGroup m_CanvasGroup;

        [SerializeField]
        protected RectTransform m_PanelTransform;

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

            StartCoroutine(LoadSceneAsync("Scenes/Intro-Scene"));
        }



        public void StartGame(string sessionId=null, Action duplicateSessionCb=null)
        {
            m_SessionId = sessionId;
            if (String.IsNullOrEmpty(m_SessionId))
            {
                m_SessionId = ShortGuid();
            }

            if (duplicateSessionCb == null)
            {
                duplicateSessionCb = () =>
                {
                    // keep trying until we get a unique id
                    StartGame();
                };
            }

            Debug.Log(String.Format("StartGame! SessionId: {0}", m_SessionId));

            m_Logger.StartSession(
                () => { StartCoroutine(LoadNextLevelAsync()); },
                duplicateSessionCb);

        }

        IEnumerator FadeToBlack()
        {
            m_PanelTransform.localScale = new Vector3(1, 1, 1);
            while (m_CanvasGroup.alpha < 1)
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
            m_PanelTransform.localScale = new Vector3(0, 0, 0);

            yield return null;
        }

        IEnumerator LoadSceneAsync(string sceneName)
        {
            // Fadeout previous scene if there was one
            if (!String.IsNullOrEmpty(m_CurrentSceneName))
            {
                yield return FadeToBlack();
                AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(m_CurrentSceneName);
                while (!asyncUnload.isDone)
                {
                    yield return null;
                }
            }

            // Save SceneName to current
            m_CurrentSceneName = sceneName;

            // Fadein next scene
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            yield return FadeFromBlack();
        }

        IEnumerator LoadNextLevelAsync()
        {
            if (this.m_Level > (this.m_StartLevel - 1))
            {
                AudioManager.Singleton.PlayLevelCompleteSound();
                m_Logger.EndLevel();
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
            yield return LoadSceneAsync(nextLevel);
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
