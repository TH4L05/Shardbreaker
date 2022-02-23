using System;
using UnityEngine;

public class AbilityOrb : MonoBehaviour
{
    [Header("Ability")]
    [SerializeField] private int abilityNumber = 0;
    [SerializeField] private string abilityName ="";
    [Header("FX")]
    [SerializeField,Range(0,5)] private float destroyDelay = 0;
    [SerializeField] private Animator animator;
    [SerializeField] private string animatorSetTrigger = "";
    public AkAmbient normalAudio;
    public AkAmbient collectAudio;

    public static Action<int,string> OrbCollected;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            OrbCollected(abilityNumber, abilityName);

            GetComponent<SphereCollider>().enabled = false;

            if (animator != null)
            {
                GetComponent<Animator>().SetTrigger(animatorSetTrigger);
            }

            normalAudio.Stop(1);
            collectAudio.HandleEvent(collectAudio.gameObject);
            Destroy(gameObject, destroyDelay);
        }
    }
}
