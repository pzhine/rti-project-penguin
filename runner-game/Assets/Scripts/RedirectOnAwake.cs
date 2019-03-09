using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections;
using System;

public class RedirectOnAwake : MonoBehaviour
{
    [SerializeField]
    protected string m_Url;

    [SerializeField]
    protected string m_SessionIdToken;

    [SerializeField]
    protected int m_WaitSeconds;

    void Awake()
    {
        StartCoroutine(RedirectAsync());
    }

    IEnumerator RedirectAsync()
    {
        yield return new WaitForSeconds(m_WaitSeconds);
        string url = m_Url.Replace(m_SessionIdToken, ShortGuid());
#if !UNITY_WEBGL
        Application.OpenURL(url);
#else
        RedirectToURL(url);
#endif
        yield return null;
    }

    private string ShortGuid()
    {
        string encoded = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        encoded = encoded
            .Replace("/", "_")
            .Replace("+", "-");
        return encoded.Substring(0, 22);
    }

    [DllImport("__Internal")]
    private static extern void RedirectToURL(string str);
}