using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{

    public GameObject audioPanel;
    public GameObject videoPanel;
    public GameObject controlPanel;


    public AudioMixer volMixer;

    public Slider volSlider;

    public TMP_Dropdown qualityDropdown;

    public TMP_Dropdown resolutionDropdown;

    public Toggle fullscreenToggle;

    private int screenInt;

    Resolution[] resolutions;

    private bool isFullScreen = false;

    const string prefName = "optionvalue";
    const string resName = "resolutionoption";

    private void Awake()
    {
        screenInt = PlayerPrefs.GetInt("togglestate");

        if(screenInt == 1)
        {
            isFullScreen = true;
        }
        else
        {
            fullscreenToggle.isOn = false;
        }

        resolutionDropdown.onValueChanged.AddListener(new UnityAction<int>(index =>
        {
            PlayerPrefs.SetInt(resName, resolutionDropdown.value);
            PlayerPrefs.Save();
        })); // junge was ist diese syntax too bad
        qualityDropdown.onValueChanged.AddListener(new UnityAction<int>(index =>
        {
            PlayerPrefs.SetInt(prefName, qualityDropdown.value);
            PlayerPrefs.Save();
        }));
    }

    private void Start()
    {
        volSlider.value = PlayerPrefs.GetFloat("musicVolume", 1f);
        volMixer.SetFloat("volume", PlayerPrefs.GetFloat("musicVolume"));

        qualityDropdown.value = PlayerPrefs.GetInt(prefName, 3);

        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz";
            options.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height &&
                resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = PlayerPrefs.GetInt(resName, currentResolutionIndex);
        resolutionDropdown.RefreshShownValue();
    }

    public void OnVideoPanelPressed()
    {
        videoPanel.SetActive(true);
        audioPanel.SetActive(false);
        controlPanel.SetActive(false);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];

        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("");
    }

    public void ChangeVol(float volume)
    {
        PlayerPrefs.SetFloat("musicVolume", volume);
        volMixer.SetFloat("volume", PlayerPrefs.GetFloat("musicVolume"));
    }
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;

        if(isFullScreen == false)
        {
            PlayerPrefs.SetInt("togglestate", 0);
        }
        else
        {
            isFullScreen = true;
            PlayerPrefs.SetInt("togglestate", 1);
        }
    }
}
