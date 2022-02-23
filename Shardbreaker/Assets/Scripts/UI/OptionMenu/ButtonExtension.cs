using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ButtonExtension : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image arrow;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private TMP_Text shadowText;
    [SerializeField] private float size;
    private float startSize;
    [SerializeField] private Color colorNormal;
    [SerializeField] private Color colorHover;
    [SerializeField] private Color colorClick;
    public AkAmbient buttonHover;
    public bool interactable = true;

    private void Start()
    {
        if (buttonText != null)
        {
            startSize = buttonText.fontSize;
        }
        
    }

    public void ToggleInteractability()
    {
        interactable = !interactable;
    }

    public void OnClick()
    {
        buttonText.color = colorClick;
        arrow.gameObject.SetActive(false);
        StartCoroutine(ResetColor());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!arrow) return;      
        arrow.gameObject.SetActive(true);

        if (!buttonText) return;
        buttonText.fontSize += size;
        buttonText.color = colorHover;

        if (!shadowText) return;
        shadowText.fontSize += size;

        if (buttonHover)
        {
            buttonHover.Stop(0);
            buttonHover.HandleEvent(buttonHover.gameObject);
        }       
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!arrow) return;
        arrow.gameObject.SetActive(false);

        if (!buttonText) return;
        buttonText.fontSize -= size;
        buttonText.color = colorNormal;

        if (!shadowText) return;
        shadowText.fontSize -= size;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!arrow) return;
        arrow.gameObject.SetActive(true);

        if (!buttonText) return;
        buttonText.fontSize += size;

        if (!shadowText) return;
        shadowText.fontSize += size;
    }
    public void OnDeselect(BaseEventData eventData)
    {
        if (!arrow) return;
        arrow.gameObject.SetActive(false);

        if (!buttonText) return;
        buttonText.fontSize -= size;

        if (!shadowText) return;
        shadowText.fontSize -= size;
    }

    IEnumerator ResetColor()
    {
        yield return new WaitForSeconds(1f);
        buttonText.color = colorNormal;
        buttonText.fontSize = startSize;
        shadowText.fontSize = startSize;
    }
}

/*// thanks krebsi
#if UNITY_EDITOR

[UnityEditor.CustomEditor(typeof(ButtonEvent)), UnityEditor.CanEditMultipleObjects]
public class MyButtonScriptEditor : UnityEditor.Editor { }

#endif*/
