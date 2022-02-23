using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.InputSystem;

public class Game : MonoBehaviour
{
    public static Game instance;

    public GameObject player;
    public GameObject deathMenu;
    public GameObject pauseMenu;
    public GameObject optionsMenu;
    private OptionsMenu options;

    [SerializeField] private UI_Ability ui_ability;

    [Header("Ability Section")]
    public List<Ability> abilities = new List<Ability>();

    public TextMeshProUGUI textbox;
    public PlayableDirector playable;
    public GameObject crosshair;

    [Header("GlobalVariables")]
    public float mouseSensitivity;
    public float musicVolume;
    public float sfxVolume;
    public bool startBattle;
    public int enemyInBattleIndex;
    public InputHandler input;
    private bool isBoss;

    private void Awake()
    {
        instance = this;
        GetLevelReference();

    }

    public void GetLevelReference()
    {

        player = GameObject.Find("Player");
        ui_ability = FindObjectOfType<UI_Ability>();

        abilities.Clear();
        abilities.Add(FindObjectOfType<Ability1>());
        abilities.Add(FindObjectOfType<Ability2>());
        abilities.Add(FindObjectOfType<Ability3>());

        //textbox = GameObject.Find("Text").GetComponent<TextMeshProUGUI>();
        //playable = GameObject.Find("ShowText").GetComponent<PlayableDirector>();
        // = GameObject.Find("Crosshair");

        optionsMenu = GameObject.Find("OptionsMenu");
        pauseMenu = GameObject.Find("PauseMenu");
        //deathMenu = GameObject.Find("Canvas_Death");

        if (optionsMenu)
        {
            options = optionsMenu.GetComponent<OptionsMenu>();
        }

        input = FindObjectOfType<InputHandler>();
        input.GetComponentReferences();

        GetPlayerPrefsData();

        ToggleCanvas(false);
    }

    private void ToggleCanvas(bool value)
    {
        optionsMenu.SetActive(value);
        pauseMenu.SetActive(value);
        deathMenu.SetActive(value);
        //var loadingScreen = GameObject.Find("LoadingScreen");
        //loadingScreen.SetActive(value);
        var triggerText = GameObject.Find("CanvasTriggerText");
        triggerText.SetActive(value);
        //var cutScene = GameObject.Find("Canvas_CutScene");
        //cutScene.SetActive(value);
        //var loadingScreen = GameObject.Find("LoadingScreen");
        //loadingScreen.SetActive(false);
    }
    private void Start()
    {
        Enemy.EnemyStartAttack += ChangeMusicState;
        Enemy.EnemyIsDead += CheckMusicState;
        EnemySlimeBoss.theBoss += SetBoss;
    }

    public void SetBoss()
    {
        isBoss = true;
    }

    public void ChangeMusicState()
    {
        enemyInBattleIndex++;
        if (!startBattle)
        {
            startBattle = true;
        }

        if (isBoss)
        {
            AkSoundEngine.SetState("Music_States", "Battle_Boss");
        }
        else
        {
            AkSoundEngine.SetState("Music_States", "Battle");
        }
        
    }

    public void CheckMusicState()
    {
        enemyInBattleIndex--;
        startBattle = false;
        StartCoroutine(WaitTime());
    }

    IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(3f);
        if (enemyInBattleIndex <= 0)
        {
            AkSoundEngine.SetState("Music_States", "Exploration");
        }
    }


    private void GetPlayerPrefsData()
    {
        musicVolume = PlayerPrefs.GetFloat("musicVolume");
        options.SetMusicValue(musicVolume);

        sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
        options.SetSfxValue(sfxVolume);

        mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity");
        options.SetSensitivityValue(mouseSensitivity);
        player.GetComponent<MouseLook>().SetSensitivity(mouseSensitivity);
    }

    public void UpdateMouseSensitivity()
    {
        mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity");
        player.GetComponent<MouseLook>().SetSensitivity(mouseSensitivity);
    }

    public void UpdateUIAbility(int index)
    {
        ui_ability.SetAbilityActive(index);
    }

    public void ShowText()
    {
        playable.Play();
    }

    public void PlayCrosshairEffect()
    {
        crosshair.GetComponent<PlayableDirector>().Play();
    }


    public void SceneChange()
    {
        ToggleCanvas(true);

        input.SetNUll();
        player = null;
        ui_ability = null;
        abilities.Clear();
        textbox = null;
        playable = null;
        crosshair = null;

        optionsMenu = null;
        pauseMenu = null;
        deathMenu = null;
    }

}