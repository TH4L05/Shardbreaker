using System.Collections.Generic;
using UnityEngine;

public class Ability2 : Ability
{
    [SerializeField] private Transform firingPoint;
    [SerializeField] private GameObject template;
    private List<Transform> targets = new List<Transform>();
    [SerializeField] private Transform rayspawnpoint;
    [SerializeField] private GameObject targetVisual;
    [SerializeField] private List<GameObject> targetVisuals = new List<GameObject>();
    [SerializeField] private Camera playerCamera;
    [SerializeField] protected LayerMask mask;
    private GameObject instance;

    public void CreateProjectileInstance(Transform target)
    {
        if (target != null)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hitInfo, 100f, mask))
            {
                Debug.Log($"<color=yellow>{hitInfo.collider.name}</color>");
                instance = Instantiate(template);
                instance.transform.position = firingPoint.transform.position;
                instance.transform.rotation = playerCamera.transform.rotation;
            }
        }
    }

    private void GetMultipleTargets()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            RaycastHit hit;
            if (Physics.Raycast(rayspawnpoint.position, rayspawnpoint.forward, out hit, 100f))
            {
                if (hit.transform.tag == "TEST" || hit.transform.tag == "Enemy")
                {
                    if (!targets.Contains(hit.transform))
                    {
                        targets.Add(hit.transform);
                        var tv = Instantiate(targetVisual, new Vector3(hit.transform.position.x , hit.transform.position.y + hit.transform.localScale.y + 1, hit.transform.position.z), hit.transform.rotation);
                        targetVisuals.Add(tv);

                    }
                    Debug.Log($"<color=green>{hit.transform}</color>");
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            foreach (var target in targets)
            {
                Debug.Log($"<color=cyan>{target}</color>");
                CreateProjectileInstance(target);
            }

            foreach (var targetVisual in targetVisuals)
            {
                Destroy(targetVisual);
            }
            targetVisuals.Clear();
            targets.Clear();
        }
    }

    public override void Use()
    {
        if (isEnabled && isActive)
        {
            GetMultipleTargets();
        }  
    }
}
