using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class SaveLoadManager : MonoBehaviour
{

    public GameObject player;

    public void SaveButton()
    {
        SaveBySerialization();
    }
    public void LoadButton()
    {
        Game.instance.deathMenu.SetActive(false);
        Game.instance.pauseMenu.SetActive(false);
        Game.instance.pauseMenu.GetComponentInParent<PauseMenu>().GamePaused = false;
        LoadBySerialization();
        Game.instance.player.GetComponent<Player>().InputEnabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    private Save CreateSaveGameObject()
    {
        Save save = new Save();

        save.playerPositionX = player.transform.position.x;
        save.playerPositionY = player.transform.position.y;
        save.playerPositionZ = player.transform.position.z;

        save.playerHealth = Game.instance.player.GetComponent<Health>().CurrentHealth;
        save.playerAmmo = Game.instance.player.GetComponent<Shoot>().Ammo;

        save.isDead = false;

        Debug.Log($"Save values are : Ammo={save.playerAmmo} / Health={save.playerHealth}");

        return save;
    }

    public void SaveBySerialization()
    {
        var save = CreateSaveGameObject();

        BinaryFormatter bf = new BinaryFormatter();

        FileStream fileStream = File.Create(Application.persistentDataPath + "/PlayerData.text");

        bf.Serialize(fileStream, save);

        fileStream.Close();

        Debug.Log(($"<color=yellow> Game File Saved </color>"));
    }
    public void LoadBySerialization()
    {
        if(File.Exists(Application.persistentDataPath + "/PlayerData.text"))
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream fileStream = File.Open(Application.persistentDataPath + "/PlayerData.text", FileMode.Open);

            Save save = bf.Deserialize(fileStream) as Save;
            fileStream.Close();

            player.transform.position = new Vector3(save.playerPositionX, save.playerPositionY, save.playerPositionZ);

            player.GetComponent<Health>().CurrentHealth = save.playerHealth;
            player.GetComponent<Health>().NoHealth = false;
            player.GetComponent<Health>().UpdateHealth(save.playerHealth);
            
            player.GetComponent<Shoot>().Ammo = save.playerAmmo;
            player.GetComponent<Player>().IsDead = save.isDead;

            player.GetComponent<Player>().BoulderHitpercentageValue = 0f;

            Debug.Log(($"<color=yellow> Game File Loaded </color>"));
        }
    }
}