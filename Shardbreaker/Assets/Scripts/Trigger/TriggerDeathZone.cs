using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDeathZone : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            Debug.Log("<color=brown>PlayerOnTriggerEnter</color>");
            var player = collider.GetComponent<Player>();
            player.GetHit(999, DamageTypes.DamageType.InstantDeath);
        }

        //Destroy(gameObject, 1f);
    }
    

}
