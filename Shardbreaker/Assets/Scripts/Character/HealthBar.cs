using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image bar;
    [SerializeField] private TextMeshProUGUI barText;
    [SerializeField] private PlayableDirector hitfeedback;
    [SerializeField] private string textFormat = "000";
    [SerializeField] public bool billboardHealthbar;
    private Camera playerCamera;

    private void Start()
    {
        playerCamera = Game.instance.player.GetComponent<Player>().playerCameraMain;
    }

    void LateUpdate()
    {
        if (playerCamera && billboardHealthbar)
        {
            transform.LookAt(transform.position + playerCamera.transform.forward);
        }  
    }

    public void UpdateHealthBar(float value, float maxhealth)
    {
        if (bar)
        {
            bar.fillAmount = value;
        }
        
        if (barText)
        {
            barText.text = (value * maxhealth).ToString(textFormat);
        }

        if (hitfeedback)
        {
            hitfeedback.Play();
        }
        

    }
}
