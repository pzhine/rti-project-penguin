using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    public float RespawnTime;
    public Transform RespawnLocation;

    public void StartRespawnCoroutine(GameObject go) {
        ToggleObjectForRespawn(go, false);
        go.transform.position = new Vector3(999, -999, 999);

        StartCoroutine(RespawnCoroutine(go));
    }

    IEnumerator RespawnCoroutine(GameObject go) {
        yield return new WaitForSeconds(RespawnTime);

        ToggleObjectForRespawn(go, true);
        go.transform.position = RespawnLocation.position;
    }

    void ToggleObjectForRespawn(GameObject go, bool toggle) {
        go.GetComponent<MeshRenderer>().enabled = toggle;
        go.GetComponent<CharacterController>().enabled = toggle;
    }

}
