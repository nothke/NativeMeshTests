using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Unity.Burst;
using UnityEngine.Profiling;

public class MeshTest : MonoBehaviour
{
    Mesh mesh;

    NativeArray<float3> vertices;
    NativeArray<float3> normals;

    const int SIZE = 32;
    const int TOTAL_SIZE = SIZE * SIZE;

    Vector3[] vertexArray;

    void Start()
    {
        vertices = new NativeArray<float3>(TOTAL_SIZE, Allocator.Persistent);
        normals = new NativeArray<float3>(TOTAL_SIZE, Allocator.Persistent);

        mesh = new Mesh();
        GetComponent<MeshFilter>().sharedMesh = mesh;

        mesh.SetVertices(vertices);
        mesh.triangles = GetTris(SIZE, TOTAL_SIZE);

        //mesh.SetVertexBufferData(vertices, 0, 0, TOTAL_SIZE, 0, UnityEngine.Rendering.MeshUpdateFlags.Default);

        vertexArray = GetVertexArray();

        mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
    }

    [BurstCompile]
    public struct VertexJob : IJobParallelFor
    {
        [WriteOnly] public NativeArray<float3> vertices;
        public float time;

        public void Execute(int i)
        {
            int x = i % SIZE;
            int z = i / SIZE;

            float y = noise.cnoise(float2(x * 0.213f + time, z * 0.213f));

            vertices[i] = float3(x, y, z);
        }
    }

    [BurstCompile]
    public struct RecalculateGridNormalsJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float3> vertices;
        [WriteOnly] public NativeArray<float3> normals;
        public int size;

        public void Execute(int i)
        {
            int x = i % SIZE;
            int y = i / SIZE;

            int iLeft = (x - 1) + y * size;
            int iRight = (x + 1) + y * size;
            int iDown = x + (y - 1) * size;
            int iUp = x + (y + 1) * size;

            if (x > 0 && x < size - 1 && y > 0 && y < size - 1)
            {
                float nX = vertices[iRight].y - vertices[iLeft].y;
                float nY = vertices[iUp].y - vertices[iDown].y;
                normals[i] = normalize(float3(-nX, 1, -nY));
            }
        }
    }

    private void Update()
    {
        var handle = new VertexJob()
        {
            vertices = vertices,
            time = Time.time
        }.Schedule(TOTAL_SIZE, 0);

        new RecalculateGridNormalsJob()
        {
            vertices = vertices,
            normals = normals,
            size = SIZE
        }.Schedule(TOTAL_SIZE, 0, handle).Complete();

        /*
        Profiler.BeginSample("SetVertices managed array");
        mesh.SetVertices(vertexArray);
        Profiler.EndSample();
        */

        float3[] managedF3s = new float3[TOTAL_SIZE];

        Profiler.BeginSample("CopyTo");
        vertices.CopyTo(managedF3s);
        Profiler.EndSample();

        Profiler.BeginSample("Copy vertices");
        for (int i = 0; i < TOTAL_SIZE; i++)
        {
            vertexArray[i] = vertices[i];
        }
        Profiler.EndSample();

        Profiler.BeginSample("SetVertices NativeArray");
        mesh.SetVertices(vertices);
        Profiler.EndSample();

        Profiler.BeginSample("SetVertices NativeArray");
        mesh.SetNormals(normals);
        Profiler.EndSample();

        /*
        Profiler.BeginSample("Recalculate normals");
        mesh.RecalculateNormals();
        Profiler.EndSample();*/

    }

    Vector3[] GetVertexArray()
    {
        Vector3[] vertexArray = new Vector3[TOTAL_SIZE];
        for (int i = 0; i < TOTAL_SIZE; i++)
        {
            int x = i % SIZE;
            int z = i / SIZE;

            float y = noise.cnoise(float2(x * 0.213f + Time.time, z * 0.213f));

            vertexArray[i] = float3(x, y, z);
        }
        return vertexArray;
    }

    private void OnDestroy()
    {
        if (!enabled) return;
        vertices.Dispose();
        normals.Dispose();
    }

    public static int[] GetTris(int width, int vertLength)
    {
        if (width < 2) width = 2;
        if (vertLength < 3) return null;

        List<int> indices = new List<int>();

        int startIndex = 0;

        for (int i = 0; i < vertLength - 1; i++)
        {
            //int y = i + width;

            if (i % width == width - 1) continue;

            if (startIndex + i + width + 1 >= vertLength)
                return indices.ToArray();

            // tri 1
            indices.Add(startIndex + i);
            indices.Add(startIndex + i + width);
            indices.Add(startIndex + i + 1);

            // tri 2
            indices.Add(startIndex + i + 1);
            indices.Add(startIndex + i + width);
            indices.Add(startIndex + i + width + 1);

        }

        return indices.ToArray();
    }
}
