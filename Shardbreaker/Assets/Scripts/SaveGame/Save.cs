using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Save
{
    public float playerHealth;
    public int playerAmmo;

    public float playerPositionX;
    public float playerPositionY;
    public float playerPositionZ;

    public bool isDead;
    public bool inputEnabled;

    //public List<int> livlingEnemyPositions = new List<int>();
}

