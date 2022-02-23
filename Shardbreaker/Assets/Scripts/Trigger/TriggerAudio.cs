using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAudio : MonoBehaviour
{
    [SerializeField] private float deleteTime = 1f;
    [SerializeField] private bool destroyOnEnter;
    [SerializeField] private bool destroyOnExit;

    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (destroyOnEnter)
            {
                Destroy(gameObject, deleteTime);
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (destroyOnExit)
            {
                Destroy(gameObject, deleteTime);
            }
        }
    }
}
