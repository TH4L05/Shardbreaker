using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevel : MonoBehaviour
{
    int enemycount;
    int enemydeathIndex;

    private void Awake()
    {
        enemycount = 0;
        Enemy.EnemyIsDead += CountEnemy;
    }

    void CountEnemy()
    {
        enemydeathIndex++;
        enemycount--;
    }
}
