using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Playables;

public class UI_Ammo : MonoBehaviour
{
    //[SerializeField] private Image bulletSprite;
    //[SerializeField] private Image reloadSprite;
    //[SerializeField] private TextMeshProUGUI bulletName;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Image ammoBar;
    [SerializeField] private PlayableDirector ammoFeedback;

    public void Start()
    {
        Shoot.DecreaseAmmoValue += UpdateAmmo;
        Shoot.IncreaseAmmoValue += UpdateAmmo;

        
        UpdateAmmo(Game.instance.player.GetComponent<Shoot>().MaxAmmoAmount);
    }

    public void UpdateAmmo(int ammo)
    {
        if (ammoText != null)
        {
            ammoText.text = ammo.ToString();
        }

        if (ammoFeedback != null)
        {
            ammoFeedback.Play();
        }
    }

    /*public void UpdateAmmoPlus(int ammo)
    {
        if (ammoText != null)
        {
            ammoText.text = ammo.ToString();
        }
    }*/

}
