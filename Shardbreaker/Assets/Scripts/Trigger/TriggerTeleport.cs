using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTeleport : MonoBehaviour
{

    public Transform teleportPoint;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            Game.instance.player.transform.position = teleportPoint.transform.position;
        }
    }

}
