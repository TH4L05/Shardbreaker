using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public Color start;
    public Color change;
    public Light thelight;

 
    void Start()
    {
        thelight.color = start;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            thelight.color = change;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            thelight.color = start;
        }
    }

}
