using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSettings : MonoBehaviour
{
    [SerializeField]
    private Toggle toggle;
    [SerializeField]
    private AudioSource myAudio;
    public void Awake()
    {
        
        if (!PlayerPrefs.HasKey("music"))
        {
            PlayerPrefs.SetInt("music", 1);
            toggle.isOn = true;
            //myAudio.enabled = true;
            PlayerPrefs.Save();
        }
        
        else
        {
            if (PlayerPrefs.GetInt("music") == 0)
            {
                //myAudio.enabled = false;
                toggle.isOn = false;
            }
            else
            {
                //myAudio.enabled = true;
                toggle.isOn = true;
            }
        }
    }
    public void ToggleMusic()
    {
        if (toggle.isOn)
        {
            PlayerPrefs.SetInt("music", 1);
            //myAudio.enabled = true;
        }
        else
        {
            PlayerPrefs.SetInt("music", 0);
            //myAudio.enabled = false;
        }
        PlayerPrefs.Save();
    }

}
