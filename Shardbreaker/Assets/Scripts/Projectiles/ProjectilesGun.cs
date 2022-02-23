using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProjectilesGun : MonoBehaviour
{
    // projectiles
    public GameObject projectiles;

    // projectile force
    public float shootForce, upwardForce;

    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, projectilesPerTap;
    public bool allowButtonHold;

    int projectilesLeft, projectilesShot;

    // bools
    bool shooting, readyToShoot, reloading;

    // reference
    public Camera fpsCam;
    public Transform attackPoint;

    // graphics
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay; 



    public bool allowInvoke = true;

    private void Awake()
    {
        // make sure ammo is full
        projectilesLeft = magazineSize;
        readyToShoot = true;
    }
    private void Update()
    {
        MyInput();

        //set ammo display
        if (ammunitionDisplay != null)
            ammunitionDisplay.SetText(projectilesLeft / projectilesPerTap + " / " + magazineSize / projectilesPerTap);
    }

    public virtual void MyInput()
    {
        // check if allowed to hold down button and take corresponding input
        if (allowButtonHold) 
            shooting = Input.GetKey(KeyCode.Mouse0);

        else 
            shooting = Input.GetKey(KeyCode.Mouse0);

        // reloading
        if (Input.GetKeyDown(KeyCode.R) && projectilesLeft < magazineSize && !reloading) 
            Reload();
        // reload auto. when trying to shoot without ammo
        if (readyToShoot && !reloading && projectilesLeft <= 0) 
            Reload();

        // shooting
        if(readyToShoot && shooting && !reloading && projectilesLeft > 0)
        {
            // set projectiles shot to 0
            projectilesShot = 0;

            Shoot();
        }
    }
    public virtual void Shoot()
    {
        readyToShoot = false;

        // find the hit position using raycast
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // check if ray hits something
        Vector3 targetPoint;
        if(Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }

        // calculate direction from attackPoint to targetPoint
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        // spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        // new direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        // instantiate projectile
        GameObject currentProjectile = Instantiate(projectiles, attackPoint.position, Quaternion.identity);

        // rotate projectile to shoot direction
        currentProjectile.transform.forward = directionWithoutSpread.normalized;

        // add force to projectile
        currentProjectile.GetComponent<Rigidbody>().AddForce(directionWithoutSpread.normalized * shootForce, ForceMode.Impulse);
        currentProjectile.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        // instantiate muzzle flash
        if(muzzleFlash != null)
        {
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        }

        projectilesLeft--;
        projectilesShot++;

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }

        // if more than one bulletsPerTap make sure to repeat shoot function
        if(projectilesShot < projectilesPerTap && projectilesLeft > 0)
        {
            Invoke("Shoot", timeBetweenShooting);
        }

    }
    private void ResetShot()
    {
        // allow shooting and invoking again
        readyToShoot = true;
        allowInvoke = true;
    }

    public virtual void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }
    public virtual void ReloadFinished()
    {
        projectilesLeft = magazineSize;
        reloading = false;
    }


}
