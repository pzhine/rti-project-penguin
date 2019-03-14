using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace RedRunner
{
    public class CollectableScreenTime
    {
        public string m_Name;
        public float m_LastBecameVisibleAt;
        public float m_CumulativeScreenTime;
        public bool m_IsVisible;
        public int m_Level;

        public CollectableScreenTime(string name, int level)
        {
            m_Name = name;
            m_CumulativeScreenTime = 0;
            m_IsVisible = false;
            m_Level = level;
        }
    }
    public class FirebaseLogger : MonoBehaviour
    {
        [SerializeField]
        public string m_ProjectId;

        string m_SessionUrl;
        int m_CurrentLevel;
        string m_LevelUrl;
        string m_CollectablesUrl;

        private Hashtable m_CollectableScreenTimes;

        public void StartSession(Action successCb, Action duplicateSessionCb) {
            Debug.Log("FirebaseLogger.StartSession");
            m_SessionUrl = String.Format(
                "https://{0}.firebaseio.com/sessions/{1}",
                m_ProjectId,
                GameManager.Singleton.m_SessionId);

            // we should only need to init this once because we don't have duplicate objects
            m_CollectableScreenTimes = new Hashtable();

            // first try a get to make sure session doesn't already exist
            string sessionUrl = String.Format("{0}.json", m_SessionUrl);
            StartCoroutine(RequestAsync(sessionUrl, null, "GET", (request) =>
            {
                // if null response, create the session
                if (request.downloadHandler.text == "null")
                {
                    string url = String.Format("{0}/start_timestamp.json", m_SessionUrl);
                    string body = "{ \".sv\": \"timestamp\" }";
                    StartCoroutine(RequestAsync(url, body));
                    successCb?.Invoke();
                }
                // otherwise, callback with error
                else
                {
                    duplicateSessionCb?.Invoke();
                }
            }));
        }

        public void StartLevel(int level)
        {
            Debug.Log("FirebaseLogger.StartLevel");
            m_CurrentLevel = level;
            m_LevelUrl = String.Format("{0}/levels/{1}", m_SessionUrl, level);
            m_CollectablesUrl = String.Format("{0}/collectables", m_LevelUrl);

            string url = String.Format("{0}.json", m_LevelUrl);
            string body = String.Format("{{ \"start_time\": {0} }}", Time.time);
            StartCoroutine(RequestAsync(url, body));
        }

        private void LogScreenTimes(int level)
        {
            foreach (string key in m_CollectableScreenTimes.Keys)
            {
                CollectableScreenTime cst = m_CollectableScreenTimes[key] as CollectableScreenTime;
                if (cst.m_Level == level)
                {
                    float time = cst.m_CumulativeScreenTime;
                    string url = String.Format("{0}/levels/{1}/collectables/{2}.json", m_SessionUrl, level, key);
                    string body = String.Format("{{ \"screen_time\": {0} }}", time);
                    StartCoroutine(RequestAsync(url, body, "PATCH"));
                }
            }
        }

        public void CollectableBecameVisible(string name)
        {
            if (!m_CollectableScreenTimes.ContainsKey(name)) {
                m_CollectableScreenTimes.Add(name, new CollectableScreenTime(name, m_CurrentLevel));
            }
            CollectableScreenTime cst = m_CollectableScreenTimes[name] as CollectableScreenTime;
            cst.m_IsVisible = true;
            cst.m_LastBecameVisibleAt = Time.timeSinceLevelLoad;
        }

        public void CollectableBecameInvisible(string name)
        {
            CollectableScreenTime cst = m_CollectableScreenTimes[name] as CollectableScreenTime;
            if (cst.m_IsVisible)
            {
                cst.m_CumulativeScreenTime += Time.timeSinceLevelLoad - cst.m_LastBecameVisibleAt;
                cst.m_IsVisible = false;
            }
        }

        public void EndLevel()
        {
            Debug.Log("FirebaseLogger.EndLevel");

            string url = String.Format("{0}.json", m_LevelUrl);
            string body = String.Format("{{ \"end_time\": {0} }}", Time.time);
            StartCoroutine(RequestAsync(url, body, "PATCH"));

            LogScreenTimes(m_CurrentLevel);
        }

        public void CollectItem(string name)
        {
            Debug.Log("FirebaseLogger.CollectItem");

            string url = String.Format("{0}/{1}.json", m_CollectablesUrl, name);
            string body = String.Format("{{ \"collect_time\": {0} }}", Time.time);
            StartCoroutine(RequestAsync(url, body));

            CollectableBecameInvisible(name);
        }

        IEnumerator RequestAsync(string url, string bodyJsonString, string verb="PUT", Action<UnityWebRequest> cb=null)
        {
            Debug.Log(String.Format("{0}: {1}", verb, url));
            var request = new UnityWebRequest(url, verb);
            if (!String.IsNullOrEmpty(bodyJsonString))
            {
                byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(bodyJsonString);
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            }
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            cb?.Invoke(request);

            //Debug.Log("Response: " + request.downloadHandler.text);
        }
    }
}
