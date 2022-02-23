using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TriggerBoulder : MonoBehaviour
{
    public List<GameObject> objectList = new List<GameObject>();
    public PlayableDirector playable;
    public bool on_off;
    public bool timeline;

    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (timeline)
            {
                //collider.transform.rotation = Quaternion.Euler(0, 0, 0);
                playable.Play();
            }
            else
            {
                foreach (GameObject obj in objectList)
                {
                    obj.GetComponent<BoulderSpawn>().isEnabled = on_off;
                }
            }
            Destroy(gameObject, 0.1f);
        }

       
    }
}
