using UnityEngine;
using System;
using System.Collections;

public class Shoot : MonoBehaviour
{
    [Header("General")]
    [SerializeField] protected Transform firingPoint;
    [SerializeField] protected GameObject[] bullets;
    [SerializeField] protected GameObject[] particles;
    public float timeBetweenShots = 1f;
    [SerializeField] protected bool isPlayer;

    private float lastShotTime = 0f;
    private float lastReloadTime = 0f;
    private int bulletIndex = 0;

    [Header("Player Specific")]   
    [SerializeField] protected UI_Ammo ui;
    [SerializeField] protected Camera playerCamera;

    [Header("Ammo")]
    [SerializeField] protected bool hasAmmo;
    [SerializeField] protected int ammoMax = 2;
    [SerializeField] protected float ammoReloadTime = 1f;
    private float defaultReloadTime = 0f;

    private bool needreload;
    public bool canReload;
    private bool onReload;

    public static Action<int> IncreaseAmmoValue;
    public static Action<int> DecreaseAmmoValue;
    public static Action<bool> OnReload;

    public int MaxAmmoAmount => ammoMax;
    public int Ammo { get; set; }


    private void Awake()
    {
        if (hasAmmo)
        {
            Ammo = ammoMax; 
        }       
    }

    private void Start()
    {
        StartSetUp();
    }

    public virtual void StartSetUp()
    {
        defaultReloadTime = ammoReloadTime;

        bulletIndex = 0;

        if (!isPlayer) return;

        //SetParticles(bulletIndex);
    }

    void Update()
    {
        if (isPlayer)
        {
            if (needreload && lastReloadTime + ammoReloadTime <= Time.time)
            {
                lastReloadTime = Time.time;
                Reload();
            }
        }
        
    }

    public void ReloadOnInput()
    {
        if (Ammo == ammoMax) return;
        if (onReload) return;
        if (!canReload) return;
        needreload = true;
        Ammo = 0;
    }

    public void Reload()
    {
        if (Ammo == -1)
        {
            Ammo = 0;
        }

        if (!onReload)
        {
            OnReload(true);
            onReload = true;
        }

        if (Ammo >= ammoMax)
        {
            needreload = false;
            onReload = false;
            ammoReloadTime = defaultReloadTime;
            OnReload(false);
        }
        else
        {
            Ammo++;
            AkSoundEngine.SetRTPCValue("Fireball_Ammo", Ammo);
            IncreaseAmmoValue(Ammo);
        }
    }


    public void CreateProjectileInstance()
    {
        needreload = false;
        Ammo--;
        AkSoundEngine.SetRTPCValue("Fireball_Ammo", Ammo);

        if (Ammo >=0)
        {
            Instantiate(bullets[bulletIndex], firingPoint.position, playerCamera.transform.rotation);
            DecreaseAmmoValue(Ammo);
        }       
        else
        {
            needreload = true;
        }       
    }

    public void CreateBulletInstanceEnemy(float distance)
    {
        GameObject projectile = Instantiate(bullets[bulletIndex], firingPoint.position, firingPoint.rotation);
        projectile.GetComponent<Projectile>().Speed = distance;
        var enemy = GetComponent<Enemy>();
        if (enemy.attackAudio)
        {
            enemy.attackAudio.HandleEvent(enemy.attackAudio.gameObject);
        }
    }

    /*public virtual void ChangeBullet()
    {
        bulletIndex++;
        
        if (bulletIndex > -1 + bullets.Length)
        {
            bulletIndex = 0;
        }

        SetParticles(bulletIndex);

        UpdateUI(bulletIndex);
    }*/

    /*public virtual void SetParticles(int index)
    {
        for (int i = 0; i <= -1 + bullets.Length; i++)
        {
            if (i != index)
            {
                particles[i].SetActive(false);
            }
            else
            {
                particles[i].SetActive(true);
            }
        }
    }*/
}
