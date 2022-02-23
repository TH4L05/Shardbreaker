using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletArmRevolver : MonoBehaviour
{
    [SerializeField] public Transform[] points;
    [SerializeField] private GameObject[] bulletDummies;
    private int ammo = 1;
    private int ammo_max = 1;

    public Shoot shootRef;

    private void OnEnable()
    {
        ammo_max = shootRef.MaxAmmoAmount;
        ammo = ammo_max;

        for (int i = 0; i < ammo_max; i++)
        {
            var projectileDummy = bulletDummies[i].GetComponent<ProjectileDummyHand>();
            projectileDummy.points = points;
            projectileDummy.waypointStartIndex = i;
            projectileDummy.GetComponent<Animation>().Play("Revolver_Projectile_Spawn");
        }

        //Shoot.IncreaseAmmoValue += UpdateAmmoPlus;
        Shoot.DecreaseAmmoValue += UpdateAmmoMinus;
    }

    public void UpdateAmmoPlus()
    {
        //Debug.Log(ammo-1);
        foreach (var projectileDummy in bulletDummies)
        {
            projectileDummy.GetComponent<Animation>().Play("Revolver_Projectile_Spawn");
        }
        //bulletDummies[ammo-1].GetComponent<ProjectileDummyHand>().PlayAnimation(0);
        //bulletDummies[ammo-1].transform.GetChild(0).GetComponent<ParticleSystem>().Play();
    }

    void UpdateAmmoMinus(int ammo)
    {
        if (bulletDummies[ammo] != null)
        {
            bulletDummies[ammo].GetComponent<Animation>().Play("Revolver_Projectile_Use");
        }
        
        //bulletDummies[ammo].transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
    }
}
