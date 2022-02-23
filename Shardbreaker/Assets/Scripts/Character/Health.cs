using System;
using System.Collections;
using UnityEngine;

public class Health :MonoBehaviour
{
    [SerializeField]private HealthBar healthBar;
    [SerializeField] private bool isPlayer;
    [SerializeField] private float healthIncreasePerSecond = 1f;
    [SerializeField] private float timeUntilHealthRegenStarts = 6.5f;
    private bool isRegenerating;
    public static Action noHealthLeft;

    public Material vfxMaterial;

    public bool IsDead => CurrentHealth == 0;
    [HideInInspector] public float MaxHealth { get; set; }
    [HideInInspector] public float CurrentHealth { get; set; }
    [HideInInspector] public bool NoHealth { get; set; }

    private void Update()
    {
        if (isPlayer)
        {
            if (isRegenerating)
            {
                RegenerateHealth();
            }
        }
    }

    IEnumerator WaitforSec()
    {
        yield return new WaitForSeconds(timeUntilHealthRegenStarts);
        isRegenerating = true;
    }

    public void RegenerateHealth()
    {
        CurrentHealth += healthIncreasePerSecond * Time.deltaTime;

        if (CurrentHealth > MaxHealth)
        {
            isRegenerating = false;
            CurrentHealth = MaxHealth;
        }

        UpdateHealthBar();
    }


    public void UpdateHealth(float value)
    {
        if (isPlayer)
        {
            isRegenerating = false;
            StopCoroutine("WaitforSec");
            StartCoroutine("WaitforSec");
        }

        CurrentHealth += value;

        if (CurrentHealth > MaxHealth)
        {           
            CurrentHealth = MaxHealth;
        }
        else if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            
        }

        if (!NoHealth && CurrentHealth == 0 && isPlayer)
        {
            NoHealth = true;
            noHealthLeft();
        }

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (isPlayer)
        {
            if (vfxMaterial)
            {
                vfxMaterial.SetFloat("_Intensity", (((CurrentHealth * -1) + 100) / 100));
            }
            
            AkSoundEngine.SetRTPCValue("Player_Health",CurrentHealth);
        }

        float healthValue = CurrentHealth / MaxHealth;
        healthBar?.UpdateHealthBar(healthValue, MaxHealth);
    }

    public void ToggleHealthBarVisibility(bool isVisibile)
    {
        healthBar.gameObject.SetActive(isVisibile);
    }
}
