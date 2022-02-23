using UnityEngine;
using UnityEngine.Playables;

public class PauseMenu : MonoBehaviour
{
    [Header("General")]
    public bool GamePaused;
    public bool pauseMenuActive;
    public GameObject pauseMenu;
    public GameObject optionMenu;
    [Header("wwise")]
    public AkAmbient pauseAudio;
    public AkAmbient resumeAudio;
    [Header("Playables")]
    public PlayableDirector showPauseMenu;
    public PlayableDirector hidePauseMenu;
    public PlayableDirector showOptionsMenu;
    public PlayableDirector hideOptionsMenu;

    public void Awake()
    {
        GamePaused = false;
    }

    public void ToggleMenu()
    {
        if (GamePaused && !pauseMenuActive)
        {
            Resume();
        }
        else
        {
            Pause();           
        }
    }

    public void ToggleOptionMenu(bool active)
    {
        pauseMenuActive = active;
        optionMenu.SetActive(active);
        pauseMenu.SetActive(!active);
    }

    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        Game.instance.player.GetComponent<Player>().InputEnabled = false;
        pauseAudio.HandleEvent(pauseAudio.gameObject);
        showPauseMenu.Play();
        Time.timeScale = 0f;
        GamePaused = true;
    }
    public void Resume()
    {
        GamePaused = false;
        Time.timeScale = 1f;
        hidePauseMenu.Play();
        resumeAudio.HandleEvent(resumeAudio.gameObject);
        Game.instance.player.GetComponent<Player>().InputEnabled = true;    
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void Quit()
    { 
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    }
#else
        Application.Quit();
    }
#endif
}
