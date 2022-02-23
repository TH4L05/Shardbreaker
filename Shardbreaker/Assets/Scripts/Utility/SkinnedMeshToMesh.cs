using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SkinnedMeshToMesh : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMesh;
    public VisualEffect VfxGraph;
    public float refreshRate; 

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateVFXGraph());
    }

    private IEnumerator UpdateVFXGraph()
    {
        while (gameObject.activeSelf)
        {
            Mesh mesh = new Mesh();
            skinnedMesh.BakeMesh(mesh);
            Vector3[] vertices = mesh.vertices;
            Mesh mesh2 = new Mesh();
            mesh2.vertices = vertices;

            //VfxGraph.SetMesh("Mesh", mesh2);
            MeshCollider meshCollider = GetComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh2;

            yield return new WaitForSeconds(refreshRate);
        }
    }

}
