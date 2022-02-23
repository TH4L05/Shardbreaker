using System.Collections;
using UnityEngine;

public class Player : Character
{
    [Header("Spezific")]
    [SerializeField] private CharacterController playerController;
    [SerializeField] private Shoot shootScript;
    public Camera playerCameraMain;
    public Transform rayspawnpoint;
    private int abilityIndex = 0;
    private float lastShotTime = 0f;
    [SerializeField] protected LayerMask mask;
    private float originalHeight;
    [SerializeField] private float reducedHeight = 1f;
    public Transform headCollisonPoint;
    public Transform rayCastFoot;
    public Transform playerPivot;

    [Header("Audio")]
    public float accumulated_Distance = 1f;
    public float step_Distance = 0f;
    public float walk_step_Distance = 1f;
    public float sprint_step_Distance = 0.5f;
    public float crouch_step_Distance = 1.5f;

    public AkAmbient playFootStepsAudio;
    public AkSwitch footStepsSwitch;
    public AkAmbient primaryFireAudio;
    public AkAmbient reloadPrimaryAudio;
    public AkAmbient jumpStartAudio;
    public AkAmbient jumpLandAudio;

    [Header("Other")]
    public bool GodMode;
    public float boulderHitpercentageIncrease = 0.3f;
    public float BoulderHitpercentageValue { get; set; }

    bool shootInputPressed;

    public bool InputEnabled { get; set; }
    public bool IsDead { get; set; }
    public bool IsJumping { get; set; }
    public bool IsCrouching { get; set; }
    public bool IsSprinting { get; set; }
    public bool canShoot { get; set; }

    public float OriginalHeight => originalHeight;
    public float ReducedHeight => reducedHeight;
    public CharacterController Controller => playerController;
    public float MoveSpeed => move_speed;
    public float SprintSpeed => sprint_speed;
    public float CrouchSpeed => crouch_speed;
    public float Gravity => gravity;
    public float JumpForce => jump_force;

    private void Update()
    {
        if (shootInputPressed)
        {
            ShootProjectile();
        }
    }

    public override void StartSetup()
    {
        base.StartSetup();
        InputEnabled = true;
        canShoot = true;
        abilityIndex = -1;
        step_Distance = walk_step_Distance;
        originalHeight = playerController.height;

        Cursor.lockState = CursorLockMode.Locked;
        Shoot.OnReload += CanShoot;
        Health.noHealthLeft += Death;
    }

    public void PlayAnimations(float speed)
    {
        if (!InputEnabled) return;

        if (IsJumping)          
        {
            animator.SetTrigger("Jump");
        }

        animator.SetFloat("speed", speed);
    }

    public void ActivateIdleAnimation()
    {
        animator.SetFloat("speed", 0);
    }

    public void SwitchAbility()
    {
        if (!InputEnabled) return;

        abilityIndex++;

        if (abilityIndex > Game.instance.abilities.Count -1)
        {
            abilityIndex = 0;
        }

        Game.instance.UpdateUIAbility(abilityIndex);
    }


    public void ReciveShootInput()
    {
        shootInputPressed = !shootInputPressed;
    }

    public void ShootProjectile()
    {
        if (!InputEnabled) return;

        if (IsCrouching) return;

        if (shootScript.Ammo < 0) return;

        if (canShoot && lastShotTime + shootScript.timeBetweenShots <= Time.time)
        {
            lastShotTime = Time.time;

            if (Physics.Raycast(playerCameraMain.transform.position, playerCameraMain.transform.forward, out RaycastHit hitInfo, 100f))
            {
                Debug.Log($"<color=yellow>{hitInfo.collider.name}</color>");               
            }

            animator.SetTrigger("Shoot");
            primaryFireAudio.HandleEvent(primaryFireAudio.gameObject);
            StartCoroutine("ShotDelay");
            StartCoroutine("BlockReload");
        } 
    }

    IEnumerator ShotDelay()
    {
        yield return new WaitForSeconds(0.2f);
        shootScript.CreateProjectileInstance();
    }

    IEnumerator BlockReload()
    {
        GetComponent<Shoot>().canReload = false;
        yield return new WaitForSeconds(0.6f);
        GetComponent<Shoot>().canReload = true;
    }

    public void CanShoot(bool value)
    {
        if (!InputEnabled) return;

        if (value)
        {
            reloadPrimaryAudio.HandleEvent(reloadPrimaryAudio.gameObject);
            animator.SetTrigger("Reload");
        }

        canShoot = !value;
    }

    public void UseAbility()
    {
        if (!InputEnabled) return;

        if (IsCrouching) return;

        if (abilityIndex < 0 || Game.instance.abilities[abilityIndex].OnCoolDown) return;

        if (Game.instance.abilities[abilityIndex].isEnabled)
        {
            Game.instance.abilities[abilityIndex].Use();

            animator.SetTrigger("UseAbility");
        }
    }

    public override void GetHit(float damage, DamageTypes.DamageType damageType)
    {
        animator.SetTrigger("GetHit");

        if (GodMode) return;

        if (damage > 0)
        {
            health.UpdateHealth(-damage);
        }
        else
        {
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
                case DamageTypes.DamageType.Boulder:
                    CalculateBoulderDamage();
                    break;
                default:
                    break;
            }
        }

        AkSoundEngine.SetRTPCValue("Player_Health", health.CurrentHealth);

    }

    void CalculateBoulderDamage()
    {
        float damage = health.CurrentHealth * (((maxHealth - health.CurrentHealth) * BoulderHitpercentageValue + damageFromBoulder) / 100);
        health.UpdateHealth(-damage);
        BoulderHitpercentageValue += boulderHitpercentageIncrease;
    }

    public override void Death()
    {
        IsDead = true;
        InputEnabled = false;
        Game.instance.deathMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("Player has no Health left");
    }
}
