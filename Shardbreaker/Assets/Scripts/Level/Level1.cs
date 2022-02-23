using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class Level1 : MonoBehaviour
{
    public List<GameObject> enemies;
    public List<GameObject> enemiesGroupSpecial;
    int enemycount;
    int enemydeathIndex;
    public GameObject wall;
    bool destroyed;
    bool abilityInfo;
    [SerializeField] private List<string> text = new List<string>();

    private void Awake()
    {
        FindObjectOfType<Game>().GetLevelReference();

        Time.timeScale = 1f;
        enemycount = enemies.Count;
        Enemy.EnemyIsDead += CountEnemy;
        TriggerWallDestroy.WallDestroyed += WallIsDestroyed;
    }

    void Start()
    {
        foreach (GameObject enemy in enemiesGroupSpecial)
        {
            var e = enemy.GetComponent<Enemy>();
            e.manualState = true;
            e.state = Enemy.State.Idle;
            e.target = GameObject.Find("Player").transform;
        }
    }

    void CountEnemy()
    {
        enemydeathIndex++;
        enemycount--;

        //Debug.Log($"<color=red>{ enemydeathIndex}</color>");
        if (enemydeathIndex == 1)
        {
            if (!abilityInfo)
            {
                abilityInfo = true;
                Game.instance.textbox.text = text[0];
                Game.instance.ShowText();
            }
        }
    }

    private void WallIsDestroyed()
    {
        destroyed = true;
        EnemyAttack();
    }


    void EnemyAttack()
    {
        foreach (GameObject enemy in enemiesGroupSpecial)
        {
            var e = enemy.GetComponent<Enemy>();
            e.manualState = false;
            e.state = Enemy.State.Alert;
        }
    }

}
