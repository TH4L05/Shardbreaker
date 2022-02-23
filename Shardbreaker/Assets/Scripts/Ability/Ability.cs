using UnityEngine;
using UnityEngine.UI;
using System;


public class Ability : MonoBehaviour
{
    [SerializeField] protected string abilityName;
    [SerializeField] protected Sprite ui_sprite;
    [SerializeField] protected int amountKillsToEnable = 1;
    [SerializeField] protected float coolDownTime = 1f;
    [SerializeField] protected Timer timer;

    private int killamount = 0;
    /*[HideInInspector]*/ public bool isEnabled;
    protected bool isActive;
    protected bool onCoolDown;
    private float coolDownLeft;
    public string enableText = "";

    public static Action<string> AbitlityEnabled;
    public static Action<string> CoolDownStart;
    public static Action<string> CoolDownEnd;
    public static Action<string, float> CoolDownTimeLeft;


    public bool OnCoolDown => onCoolDown;
    public Sprite UI_Sprite => ui_sprite;
    public string AbilityName => abilityName;
    public float KillAmountToEnable => amountKillsToEnable;
    public float KillAmount => killamount;

    void Update()
    {
        if (onCoolDown)
        {
            if (!timer.Timer_isRunning)
            {
                CoolDownEnd(abilityName);
                onCoolDown = false;
            }
            else
            {
                coolDownLeft = timer.Time_Left / coolDownTime;
                CoolDownTimeLeft(abilityName,coolDownLeft);
                //Debug.Log($"<color=orange>{coolDownLeft}</color>");
            }
        }
    }

    

    public virtual void Use()
    {
    }

    public void SetActivity(bool value)
    {
        isActive = value;
    }

    public void UpdateKillAmount()
    {
        killamount++;

        if (killamount == amountKillsToEnable)
        {
            isEnabled = true;
            isActive = true;
            AbitlityEnabled(abilityName);
            Game.instance.textbox.text = enableText;
            Game.instance.ShowText();
        }
    }
}
