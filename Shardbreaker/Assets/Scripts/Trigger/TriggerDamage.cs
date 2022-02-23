using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            Debug.Log("<color=brown>PlayerOnTriggerEnter</color>");
            var player = collider.GetComponent<Player>();
            player.GetHit(10, DamageTypes.DamageType.Default);
        }

        //Destroy(gameObject, 1f);
    }
}
