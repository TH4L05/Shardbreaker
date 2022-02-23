using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSpawnGameObject : MonoBehaviour
{
    [SerializeField] private List<GameObject> objects = new List<GameObject>();
    [SerializeField] private Transform spawnPoint;
    public bool spawn;
    public bool setActive;

    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
        spawnPoint.GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            Debug.Log("<color=orange>PlayerOnTriggerEnter</color>");
            if (spawn)
            {
                foreach (GameObject obj in objects)
                {
                    Instantiate(obj, spawnPoint.position, Quaternion.identity);
                }
                
            }
            else if (setActive)
            {
                foreach (GameObject obj in objects)
                {
                    obj.SetActive(true);
                }
            }

            Destroy(gameObject);
        }

        
    }
}
