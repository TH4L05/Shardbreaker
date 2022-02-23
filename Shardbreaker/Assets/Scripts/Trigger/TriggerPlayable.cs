using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TriggerPlayable : MonoBehaviour
{
    public PlayableDirector director;
    public bool hasExtra;
    public List<GameObject> stones = new List<GameObject>();
    public float deleteTime;
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("ProjectileAbility"))
        {
            director.Play();
            if (hasExtra)
            {
                StartCoroutine(PlayAnimation());
            }
            Destroy(gameObject, deleteTime);
        }
    }

    IEnumerator PlayAnimation()
    {
        yield return new WaitForSeconds(1.3f);
        foreach (GameObject stone in stones)
        {
            Animation anim = stone.GetComponent<Animation>();
            anim.Play();
        }       
    }
}
