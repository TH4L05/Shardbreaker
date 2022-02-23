using System.Collections;
using UnityEngine;

public class TriggerPlatformFall : MonoBehaviour
{
    public GameObject platform;
    public float waitTime = 1f;


    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            Debug.Log("<color=orange>PlayerOnTriggerEnter</color>");
            StartCoroutine(Wait());
        }

        //Destroy(gameObject,waitTime);
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);
        platform.GetComponent<Rigidbody>().isKinematic = false;
    }

}
