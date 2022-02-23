using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    void Awake()
    {
        transform.rotation = Random.rotation;
    }
}

