using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shatter_Projectile : MonoBehaviour
{
    [Range(0.1f, 1f)]
    public float vertexColorPerHit = 1f;
    [Range(0.1f, 10f)]
    public float impactRadius = 1f;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Enemy")) return;

        MeshFilter meshFilter = collision.collider.GetComponent<MeshFilter>();
        Mesh mesh;
        if (meshFilter == null)
        {
            SkinnedMeshRenderer skinnedMeshFilter = collision.collider.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshFilter == null)
            {
                Debug.Log("Different MeshType foubd");
                return;
            }
            else
            {
                mesh = skinnedMeshFilter.sharedMesh;
            }
        }
        else
        {
            mesh = meshFilter.mesh;
        }

        Vector3[] vertices = mesh.vertices;

        //gets initialized in black
        //standard vertex color of mesh is white
        Color[] colors = mesh.colors;

        for (int i = 0; i < vertices.Length; i++)
        {
            if (CheckVertexWithinRadius(collision.collider.transform.TransformPoint(vertices[i]), transform.position, impactRadius))
            {
                if (colors[i].r < vertexColorPerHit)
                {
                    colors[i].r = 0;
                }
                else
                {
                    colors[i].r = colors[i].r - vertexColorPerHit;
                }
            }            
        }

        mesh.colors = colors;
    }

    bool CheckVertexWithinRadius(Vector3 vertex, Vector3 samplePosition, float radius)
    {
        bool isInRadius = false;

        float distance = Mathf.Sqrt( Mathf.Pow(samplePosition.x - vertex.x, 2) + Mathf.Pow(samplePosition.y - vertex.y, 2) + Mathf.Pow(samplePosition.z - vertex.z, 2));
        
        if (distance < radius)
        {
            isInRadius = true;
        }
        return isInRadius;
    }
}
