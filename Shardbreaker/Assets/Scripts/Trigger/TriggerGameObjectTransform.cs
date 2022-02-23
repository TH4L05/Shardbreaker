using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGameObjectTransform : MonoBehaviour
{
    [SerializeField] private GameObject obj;
    [SerializeField] private bool setPosition;
    [SerializeField] private Vector3 position;
    [SerializeField] private bool setRotation;
    [SerializeField] private Vector3 rotation;

    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
        
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            Debug.Log("<color=orange>PlayerOnTriggerEnter</color>");

            if (setPosition)
            {
                obj.transform.position = position;
            }

            if (setRotation)
            {
                obj.transform.rotation = Quaternion.Euler(rotation);
            }

            Destroy(gameObject);
        }

        
    }
}
