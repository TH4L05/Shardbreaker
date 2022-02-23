using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class UI_Ability : MonoBehaviour
{
    [SerializeField] private List<Image> abilityActiveImage = new List<Image>();
    [SerializeField] private Image[] abilityImage;
    [SerializeField] private Image[] abilityImageFill;
    [SerializeField] private Image[] coolDownImage;
    [SerializeField] private PlayableDirector[] abilityImageDirector;

    private void Start()
    {
        StartSetup();
    }

    private void StartSetup()
    {
        Ability.AbitlityEnabled += UpdateAbilityImage;
        Ability.CoolDownStart += ActivateCoolDownImage;
        Ability.CoolDownEnd += DeactivateCoolDownImage;
        Ability.CoolDownTimeLeft += UpdateCoolDown;
        AbilityOrb.OrbCollected += UpdateAbilityDeathCount;
    }

    public void UpdateAbilityImage(string name)
    {
        int index = 0;

        foreach (var ability in Game.instance.abilities)
        {
            
            if (ability.AbilityName == name)
            {
                abilityImage[index].sprite = ability.UI_Sprite;
            }

            index++;
        }
    }

    public void SetAbilityActive(int index)
    {
        foreach (var image in abilityActiveImage)
        {
            image.gameObject.SetActive(false);
        }

        foreach (var ability in Game.instance.abilities)
        {
            ability.SetActivity(false);
        }

        Game.instance.abilities[index].SetActivity(true);
        abilityActiveImage[index].gameObject.SetActive(true);
    }

    private void UpdateAbilityDeathCount(int value, string name)
    {
        Game.instance.abilities[value - 1].UpdateKillAmount();
        float killvalue = Game.instance.abilities[value - 1].KillAmount / Game.instance.abilities[value - 1].KillAmountToEnable;
        if (killvalue <= 1)
        {
            abilityImageFill[value - 1].fillAmount = killvalue;
            abilityImageDirector[value - 1].Play();
        }
        
    }


    private int GetIndex(string name)
    {
        int index = 0;
        foreach (var item in Game.instance.abilities)
        {
            if (item.AbilityName != name)
            {
                index++;
            }
        }
        return index -2;
    }


    public void ActivateCoolDownImage(string name)
    {

        coolDownImage[GetIndex(name)].gameObject.SetActive(true);
    }

    public void DeactivateCoolDownImage(string name)
    {
        coolDownImage[GetIndex(name)].gameObject.SetActive(false);
    }

    public void UpdateCoolDown(string name, float coolDown)
    {
        coolDownImage[GetIndex(name)].fillAmount = coolDown;
    }

}
