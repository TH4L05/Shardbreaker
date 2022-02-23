using UnityEngine;

[RequireComponent(typeof(Health))]
public class Character : MonoBehaviour
{ 
    [Header("Character Base")]
    [Tooltip("Maximum Character Health")]
    [SerializeField] protected float maxHealth;
    [Tooltip("Gravity value - only positve values ar valid \n (unused when character use a Rigidbody)")]
    [Range(1f, 30f)] [SerializeField] protected float gravity = 9.81f;
    [Tooltip("affects the Jump height of the Character \n (unused when navAgent is in use)")]
    [Range(1f, 50f)] [SerializeField] protected float jump_force = 7f;
    [Tooltip("normal speed of Character while normal movement")]
    [Range(1f, 20f)] [SerializeField] protected float move_speed = 5f;
    [Tooltip("speed of Character while sprinting")]
    [Range(1f, 20f)] [SerializeField] protected float sprint_speed = 7.5f;
    [Tooltip("speed of Character while crouching \n (unused when navAgent is in use)")]
    [Range(1f, 20f)] [SerializeField] protected float crouch_speed = 2.5f;
    [Tooltip("amount of Time when the gameobject get's deleted on death")]
    [SerializeField] protected float deleteTime = 1.5f;

    [Space(10)][Header("Component References")]
    [SerializeField] protected Animator animator;
    protected Health health;
    protected Timer timer;

    [Space(10)][Header("Damage Values")]
    [Range(0f, 100f)] [SerializeField] protected float damageDefault = 2f;
    [Range(0f, 100f)] [SerializeField] protected float damageFromFire = 2f;
    [Range(0f, 100f)] [SerializeField] protected float damageFromIce = 2f;
    [Range(0f, 100f)] [SerializeField] protected float damageFromBoulder = 2f;

   
    private void Start()
    {
        SetHealth();
        StartSetup();
    }

    private void SetHealth()
    {    
        health = GetComponent<Health>();
        if (health != null)
        {
            health.MaxHealth = maxHealth;
            health.UpdateHealth(maxHealth);
        }
    }

    public virtual void StartSetup()
    {     
    }

    public virtual void GetHit(float damage, DamageTypes.DamageType damageType)
    {
        if (damage > 0)
        {
            health.UpdateHealth(-damage);
        }

        switch (damageType)
        {
            case DamageTypes.DamageType.Default:
                health.UpdateHealth(-damageDefault);
                break;
            case DamageTypes.DamageType.Fire:
                health.UpdateHealth(-damageFromFire);
                break;
            case DamageTypes.DamageType.Ice:
                health.UpdateHealth(-damageFromIce);
                break;
            default:
                break;
        }
    }

    public virtual void Attack()
    {
    }

    public virtual void Death()
    {
        timer.StartTimer(deleteTime, true);
    }

    public virtual void Destroy()
    {
        Destroy(gameObject);     
    }
}
