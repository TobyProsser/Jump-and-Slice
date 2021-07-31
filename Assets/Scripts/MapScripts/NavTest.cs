using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavTest : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) navMeshSurface.BuildNavMesh();
    }
}
