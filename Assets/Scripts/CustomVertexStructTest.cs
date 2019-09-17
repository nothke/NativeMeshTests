using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Unity.Burst;
using UnityEngine.Profiling;

public class CustomVertexStructTest : MonoBehaviour
{
    Mesh mesh;

    NativeArray<Vertex> vertices;

    const int SIZE = 256;
    const int TOTAL_SIZE = SIZE * SIZE;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().sharedMesh = mesh;

        VertexOps.SetMeshVertexBufferParams(mesh, TOTAL_SIZE);

        vertices = new NativeArray<Vertex>(TOTAL_SIZE, Allocator.Persistent);

        mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);


    }

    void Update()
    {
        UpdateMesh();
    }

    private void OnDestroy()
    {
        if (!enabled) return;
        handle.Complete();
        vertices.Dispose();
    }

    bool inited;

    JobHandle handle;

    void UpdateMesh()
    {
        if (inited)
            handle.Complete();

        Profiler.BeginSample("Set Vertex Data");
        VertexOps.SetVertexDataToMesh(mesh, vertices);
        Profiler.EndSample();

        handle = new VertexJob()
        {
            vertices = vertices,
            size = SIZE,
            time = Time.time
        }.Schedule(TOTAL_SIZE, 0);

        handle = new NormalsJob()
        {
            vertices = vertices,
            size = SIZE,
        }.Schedule(TOTAL_SIZE, 0, handle);

        if (!inited)
        {
            int[] tris = MeshTest.GetTris(SIZE, TOTAL_SIZE);
            mesh.triangles = tris;
            inited = true;
        }
    }

}
