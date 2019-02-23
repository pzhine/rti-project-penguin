using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player") {
            //start death
            FindObjectOfType<Respawn>().StartRespawnCoroutine(collider.gameObject);
        }
    }

}
