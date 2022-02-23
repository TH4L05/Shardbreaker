using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDamageOverTime : MonoBehaviour
{
    [Header("Damage")]
    [Tooltip("a value >0 sets a manual damage value and ignors the DamageType")]
    [Range(0f, 10f)] public float damage = 2.5f;
    [Tooltip("Sets the DamageType (DamageType values are set in Characters)")]
    public DamageTypes.DamageType damageType;
    [Range(0.5f, 10f)] [SerializeField] private float damageRadius = 1f;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private float timeBetweenHits = 0.5f;
    private float lastHitTime = 0f;
    private Collider[] colliders;

    [Header("VFX")]
    [Tooltip("Material for VfxEffect")]
    public Material material;
    [Tooltip("")]
    [Range(0f,1f)] public float vfxIntensityIncrease = 0.166f;
    private float vfxIntensityValue = 0f;

    public AkAmbient effectAudio;

    private void Awake()
    {
        if (effectAudio != null)
        {
            effectAudio.HandleEvent(effectAudio.gameObject);
        }
    }

    private void Update()
    {
        TakeDamage();
    }

    private void TakeDamage()
    {
        if (lastHitTime + timeBetweenHits <= Time.time)
        {
            lastHitTime = Time.time;

            colliders = Physics.OverlapSphere(transform.position, damageRadius, targetMask);

            for (int i = 0; i < colliders.Length; i++)
            {
                /*if (colliders[i].gameObject.layer == targetMask)
                {
                    Debug.Log(colliders[i].gameObject.layer.ToString());
                }*/

                Enemy enemy = colliders[i].GetComponent<Enemy>();
                Player player = colliders[i].GetComponent<Player>();

                if (player)
                {
                    player.GetHit(damage, damageType);

                    if (!material) return;
                    vfxIntensityValue += vfxIntensityIncrease;
                    material.SetFloat("_Intensity", vfxIntensityValue);
                }

                if (enemy)
                {
                    enemy.GetHit(damage, damageType);
                }
            }          
        }
    }
    
    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {         
            vfxIntensityValue = 0f;
            material.SetFloat("_Intensity", vfxIntensityValue);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }

}
