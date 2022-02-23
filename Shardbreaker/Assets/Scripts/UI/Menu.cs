using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject creditsMenu;
    private OptionsMenu options;

    private float mouseSensitivityDefault = 11f;
    private float musicVolumeDefault = 60f;
    private float sfxVolumeDefault = 80f;

    public void Awake()
    {

        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
        if (optionsMenu)
        {
            options = optionsMenu.GetComponent<OptionsMenu>();
        }

        GetPlayerPrefsData();

    }

    private void GetPlayerPrefsData()
    {
        PlayerPrefs.SetFloat("musicVolume", musicVolumeDefault);
        options.SetMusicValue(musicVolumeDefault);

        PlayerPrefs.SetFloat("sfxVolume", sfxVolumeDefault);
        options.SetSfxValue(sfxVolumeDefault);

        PlayerPrefs.SetFloat("mouseSensitivity", mouseSensitivityDefault);
        options.SetSensitivityValue(mouseSensitivityDefault);
        
        PlayerPrefs.Save();
    }

    public void ShowOptionMenu()
    {
        optionsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }
    public void HideOptionsMenu()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void ShowCreditsMenu()
    {
        creditsMenu.SetActive(true);
        //mainMenu.SetActive(false);
    }
    public void HideCreditsMenu()
    {
        mainMenu.SetActive(true);
        creditsMenu.SetActive(false);
    }

    public void Quit_Game()
    {
        Debug.Log("Quit Button pressed -> Quit Game");
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
