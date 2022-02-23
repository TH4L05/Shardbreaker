using UnityEngine;

public class TriggerExplosion : MonoBehaviour
{
    public GameObject[] wallObjects;
    public GameObject EplosionPoint;
    public float radius = 1.0f ;
    public float force = 50f;
    public float upwardsModiefier = 0f;
    public ForceMode forceMode;
 
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            Debug.Log("<color=orange>PlayerOnTriggerEnter</color>");

            foreach (Collider coll in Physics.OverlapSphere(EplosionPoint.transform.position,radius))
            {
                if (coll.tag == "TriggerWall")
                {
                    coll.GetComponent<Rigidbody>().AddExplosionForce(force, EplosionPoint.transform.position, radius, upwardsModiefier, forceMode);
                }
            }

            Destroy(gameObject);
        }

       
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(EplosionPoint.transform.position, radius);
    }

}
