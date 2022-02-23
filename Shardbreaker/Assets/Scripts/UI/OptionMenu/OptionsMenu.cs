using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private RenderPipelineAsset[] qualityLevels;
    [SerializeField] private TMP_Text resolutionText;
    [SerializeField] private TMP_Text sensitivityText;
    [SerializeField] private TMP_Text musicText;
    [SerializeField] private TMP_Text sfxText;
    [SerializeField] private string sensitivityTextFormat = "#.0";

    private float mouseSensitivity;
    private float musicVolume;
    private float sfxVolume;
    public Slider sensitivitySlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    private  int selectedResolution;
    Resolution[] resolutions;
    private bool fullScreen;
 
    private void Awake()
    {
        resolutions = Screen.resolutions;
    }

    public void SetMusicValue(float value)
    {
        musicVolume = value;
        musicVolumeSlider.value = musicVolume;
        musicText.text = musicVolume.ToString("000");
        AkSoundEngine.SetRTPCValue("Master_Music_Fader", musicVolume);
    }

    public void SetSfxValue(float value)
    {
        sfxVolume = value;
        sfxVolumeSlider.value = sfxVolume;
        sfxText.text = sfxVolume.ToString("000");
        AkSoundEngine.SetRTPCValue("Master_SFX_Fader", sfxVolume);
    }

    public void SetSensitivityValue(float value)
    {
        mouseSensitivity = value;
        sensitivitySlider.value = mouseSensitivity;
        sensitivityText.text = mouseSensitivity.ToString(sensitivityTextFormat);
    }

    public void ChangeQualityLevel(int level)
    {
        QualitySettings.SetQualityLevel(level);
        QualitySettings.renderPipeline = qualityLevels[level];
    }

    public void ResolutionButtonLeft()
    {
        if(selectedResolution > 0)
        {
            selectedResolution--;
        }
        SetResolution();
    }
    public void ResolutionButtonRight()
    {
        if (selectedResolution < resolutions.Length - 1)
        {
            selectedResolution++;
        }
        SetResolution();
    }
    public void SetResolution()
    {
        resolutionText.text = resolutions[selectedResolution].width + " x " + resolutions[selectedResolution].height;
        Screen.SetResolution(resolutions[selectedResolution].width, resolutions[selectedResolution].height, fullScreen);
    }

    public void SetFullScreen(bool isFullscreen)
    {
        fullScreen = isFullscreen;
        Screen.fullScreen = fullScreen;
    }


    public void SetVsync(bool vsync)
    {
        if (vsync)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
    }

    public void SetMouseSensitivity(float value)
    {
        mouseSensitivity = value;
        sensitivityText.text = mouseSensitivity.ToString(sensitivityTextFormat);
        PlayerPrefs.SetFloat("mouseSensitivity", mouseSensitivity);
        PlayerPrefs.Save();
    }

    public void ChangeMusicVolume(float value)
    {
        musicVolume = value;
        AkSoundEngine.SetRTPCValue("Master_Music_Fader", musicVolume);
        musicText.text = musicVolume.ToString("000");
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.Save();
    }

    public void ChangeSFXVolume(float value)
    {
        sfxVolume = value;
        AkSoundEngine.SetRTPCValue("Master_SFX_Fader", sfxVolume);
        sfxText.text = sfxVolume.ToString("000");
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        PlayerPrefs.Save();
    }
}
