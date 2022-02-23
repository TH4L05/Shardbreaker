using UnityEngine;

public class TriggerSavePoint : MonoBehaviour
{
    public SaveLoadManager Manager;

    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            Manager.SaveButton();
            Debug.Log("<color=yellow>SAVE GAME</color>");

            Destroy(gameObject, 1f);
        }

        
    }
}
