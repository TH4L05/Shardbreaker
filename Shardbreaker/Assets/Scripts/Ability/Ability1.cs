using UnityEngine;

public class Ability1 : Ability
{
    [SerializeField] private Transform firingPoint;
    [SerializeField] private GameObject template;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform playerPivot;
    public AkAmbient shootAudio;

    public override void Use()
    {
        if (isEnabled && isActive)
        {
            timer.StartTimer(coolDownTime, true);
            onCoolDown = true;
            CoolDownStart(abilityName);
            CreateProjectileInstance();
        }
    }

    public void CreateProjectileInstance()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hitInfo, 100f))
        {
            Debug.Log($"<color=yellow>{hitInfo.collider.name}</color>");  
        }
        shootAudio.HandleEvent(shootAudio.gameObject);
        Instantiate(template, firingPoint.position, playerPivot.rotation);
    }
}
