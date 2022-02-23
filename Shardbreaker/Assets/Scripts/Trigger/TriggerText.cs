using UnityEngine;
using TMPro;
using UnityEngine.Playables;

public class TriggerText : MonoBehaviour
{
    [SerializeField] private string text = "";
    //[SerializeField] private GameObject canvas;
    [SerializeField] private TextMeshProUGUI textbox;
    [SerializeField] private PlayableDirector playable;

    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (playable.state == PlayState.Playing)
            {
                playable.Stop();
            }
            textbox.text = text;
            Debug.Log("<color=orange>PlayerOnTriggerEnter</color>");
            //canvas.SetActive(true);
            playable.Play();

            Destroy(gameObject);
        }
    }

    /*private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            Debug.Log("<color=orange>PlayerOnTriggerExit</color>");
            //canvas.SetActive(false);
        }
    }*/


}
