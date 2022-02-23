using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDummyHand : MonoBehaviour
{
    [SerializeField] private float speed = 0.5f;
    [SerializeField] public int waypointStartIndex = 0;
    public int waypointIndex = 0;
    public Transform[] points;
    [SerializeField] private Animation anim;
  
    void Start()
    {
        waypointIndex = waypointStartIndex;
        transform.position = points[waypointIndex].transform.position;
    }

    void Update()
    {
        FollowThePath();
    }

    void FollowThePath()
    {
        if (waypointIndex >= points.Length)
        {
            waypointIndex = 0;
        }

        transform.position = Vector3.MoveTowards(transform.position, points[waypointIndex].transform.position, speed * Time.deltaTime);
        if (transform.position == points[waypointIndex].transform.position)
        {
            waypointIndex++;
        }
    }

    // TODO: try to fix
    //deactivated because of unkown error

    /*public void PlayAnimation(int index)
    {
        if (index == 0)
        {
            anim.Play("Revolver_Projectile_Spawn");
        }
        else
        {
            anim.Play("Revolver_Projectile_Use");
        }
    }*/
}
