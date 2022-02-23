using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceShield_Boss : MonoBehaviour
{
    public EnemySlimeBoss boss;
    
    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Semi-Erfolg");
        if (collider.CompareTag("ProjectileAbility"))
        {
            Debug.Log("Erfolg");
            boss.UpdateCrystalCount();
        }

    }

}
