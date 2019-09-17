using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Unity.Collections;

[BurstCompile]
public struct VertexJob : IJobParallelFor
{
    [WriteOnly] public NativeArray<Vertex> vertices;
    public int size;
    public float time;

    public void Execute(int i)
    {
        int x = i % size;
        int z = i / size;

        float y = noise.cnoise(float2(x * 0.213f + time, z * 0.213f));

        Vertex vertex = vertices[i];
        vertex.pos = float3(x, y, z);
        vertices[i] = vertex;
    }
}

[BurstCompile]
public struct NormalsJob : IJobParallelFor
{
    public NativeArray<Vertex> vertices; // Read and Write
    public int size;

    public void Execute(int i)
    {
        int x = i % size;
        int y = i / size;

        int iLeft = (x - 1) + y * size;
        int iRight = (x + 1) + y * size;
        int iDown = x + (y - 1) * size;
        int iUp = x + (y + 1) * size;

        Vertex vertex = vertices[i];

        if (x > 0 && x < size - 1 && y > 0 && y < size - 1)
        {
            float nX = vertices[iRight].pos.y - vertices[iLeft].pos.y;
            float nY = vertices[iUp].pos.y - vertices[iDown].pos.y;
            vertex.normal = normalize(float3(-nX, 1, -nY));
        }
        else
            vertex.normal = up();

        vertices[i] = vertex;
    }
}

public static class VertexOps
{
    public static void SetVertexDataToMesh(Mesh mesh, NativeArray<Vertex> vertices)
    {
        mesh.SetVertexBufferData(vertices, 0, 0, vertices.Length);
    }
}