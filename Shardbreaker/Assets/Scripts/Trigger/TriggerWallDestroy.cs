using System;
using UnityEngine;

public class TriggerWallDestroy : MonoBehaviour
{
    public static Action WallDestroyed;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("ProjectileAbility"))
        {
            WallDestroyed();
            //Destroy(gameObject, 0.1f);
        }
    }
}
