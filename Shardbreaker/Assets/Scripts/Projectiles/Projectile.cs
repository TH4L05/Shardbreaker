using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(AudioSource))]
public class Projectile : MonoBehaviour
{
    [Header("Base")]
    public string projectileName = "";
    public Sprite uiSprite;
    [SerializeField] protected float lifeTime = 5f;
    [SerializeField] protected float speed = 20f;
    public DamageTypes.DamageType damageType;
    [Tooltip("A value graater then 0 set a manual damage value and ignore the damage value given by damagetype when hit = true")]
    [SerializeField] protected float manualDamageValue = 0f;
    [SerializeField] protected Rigidbody rbody;
    [SerializeField] protected Collider coll;
    [SerializeField] protected List<string> ignoredTags = new List<string>();

    [Header("VFX/SFX")]
    [SerializeField] protected GameObject hitParicleVFXGround;
    [SerializeField] protected GameObject hitParicleVFXObject;
    [SerializeField] protected GameObject specialVFX;
    public AkAmbient impactSound;
    bool test;

    private bool dectectHit;
    public float Speed { private get; set; }

    private void OnEnable()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider collider)
    {
        foreach (string tag in ignoredTags)
        {
            Debug.Log(tag);
            if (collider.CompareTag(tag))
            {

                test = true;
            }
        }

        if (test)
        {
            Debug.Log($"<color=yellow>{tag}</color>");
            test = false;
            return;
        }

        //Hittable hit = collider.GetComponent<Hittable>();

        //if (hit)
        //{
        Debug.Log($"<color=green>Projecile hit {collider.name}</color>");
        dectectHit = true;
        transform.GetChild(0).gameObject.SetActive(false);
        SetDamageAndVFX(collider);

        if (impactSound)
        {
            impactSound.HandleEvent(impactSound.gameObject);
            Destroy(gameObject, 1f);
        }
        else
        {
            Destroy(gameObject, 0.1f);
        }
        //}
    }

    private void SetDamageAndVFX(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
                Player character = collider.GetComponent<Player>();
                Debug.Log("<color=cyan>HIT Player</color>");
                character.GetHit(manualDamageValue, damageType);
                SpawnVFX1(collider);
              
        }
        else if (collider.CompareTag("Enemy"))
        {           
                Enemy character = collider.GetComponent<Enemy>();
                Debug.Log("<color=cyan>HIT Enemy</color>");
                character.GetHit(manualDamageValue, damageType);
                SpawnVFX1(collider);
                       
        }
        else if  (collider.gameObject.layer == 28)
        {
            SpawnVFX2();
        }
        else
        {
            SpawnVFX1(collider);
        }
    }

    private void SpawnVFX1(Collider collider)
    {
        Debug.Log("SPAWN OBJECT VFX");
        if (hitParicleVFXObject != null)
        {
            var particle = Instantiate(hitParicleVFXObject, transform.position, collider.transform.rotation.normalized);
        }
    }

    private void SpawnVFX2()
    {
        Debug.Log("SPAWN GROUND VFX");
        if (hitParicleVFXGround != null)
        {
            var particle = Instantiate(hitParicleVFXGround, transform.position, Quaternion.identity);
        }

        if (specialVFX != null)
        {
            var particle = Instantiate(specialVFX, transform.position, Quaternion.identity);
        }
    }

    void Update()
    {
       if (!dectectHit)
        {
            MoveProjectile();
        }
    }

    public virtual void MoveProjectile()
    {
        if (Speed != 0)
        {
            speed = Speed;
        }

        //rbody.AddForce(transform.forward * speed, ForceMode.VelocityChange);
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        if (rbody != null)
        {
            transform.forward = Vector3.Lerp(transform.forward, rbody.velocity, Time.deltaTime);
        }
    }

}
