using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace RedRunner
{
    public class FirebaseLogger : MonoBehaviour
    {
        [SerializeField]
        public string m_ProjectId;

        string m_SessionUrl;
        int m_CurrentLevel;
        string m_LevelUrl;
        string m_CollectablesUrl;

        public void StartSession(Action successCb, Action duplicateSessionCb) {
            Debug.Log("FirebaseLogger.StartSession");
            m_SessionUrl = String.Format(
                "https://{0}.firebaseio.com/sessions/{1}",
                m_ProjectId,
                GameManager.Singleton.m_SessionId);

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

        public void EndLevel()
        {
            Debug.Log("FirebaseLogger.EndLevel");

            string url = String.Format("{0}.json", m_LevelUrl);
            string body = String.Format("{{ \"end_time\": {0} }}", Time.time);
            StartCoroutine(RequestAsync(url, body, "PATCH"));
        }

        public void CollectItem(string name)
        {
            Debug.Log("FirebaseLogger.CollectItem");

            string url = String.Format("{0}/{1}.json", m_CollectablesUrl, name);
            string body = String.Format("{{ \"collect_time\": {0} }}", Time.time);
            StartCoroutine(RequestAsync(url, body));
        }

        IEnumerator RequestAsync(string url, string bodyJsonString, string verb="PUT", Action<UnityWebRequest> cb=null)
        {
            Debug.Log(url);
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
