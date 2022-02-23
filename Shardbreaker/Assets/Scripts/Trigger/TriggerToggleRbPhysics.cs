using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerToggleRbPhysics : MonoBehaviour
{
    [SerializeField] private bool triggerEnabled = true;
    [SerializeField] private bool toggleVisibility;
    [SerializeField] private GameObject objectwithRb;
    private Rigidbody rb;

    [Header("OnTriggerEnter")]
    [SerializeField] private bool useGravity;
    [SerializeField] private bool isKinematic;

    
 
    private void Awake()
    {
        //GetComponent<MeshRenderer>().enabled = false;
        rb = objectwithRb.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!triggerEnabled) return;

        if (collider.CompareTag("Player"))
        {
            objectwithRb.SetActive(toggleVisibility);
            rb.useGravity = useGravity;
            rb.isKinematic = isKinematic;
        }

        Destroy(gameObject, 0.1f);
    }

}
