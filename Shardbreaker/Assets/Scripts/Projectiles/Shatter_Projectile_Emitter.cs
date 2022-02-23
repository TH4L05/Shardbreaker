using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shatter_Projectile_Emitter : MonoBehaviour
{
    public GameObject bulletPrefab;
    Rigidbody rb;
    public float explosionForce = 1f;
    public float explosionRadius = 2f;
        
    private void Update()
    {
        if (Keyboard.current[Key.Space].wasPressedThisFrame)
        {
            EmitBullet();
        }
    }

    void EmitBullet()
    {
        GameObject go = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        rb = go.GetComponent<Rigidbody>();
        rb.AddExplosionForce(Random.Range(-explosionForce, explosionForce), go.transform.position, explosionRadius);
        
        Destroy(go, 10);
    }
}
