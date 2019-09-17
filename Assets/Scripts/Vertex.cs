using UnityEngine.Rendering;
using Unity.Mathematics;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct Vertex
{
    public float3 pos;
    public float3 normal;

    public static VertexAttributeDescriptor[] GetVertexAttributes()
    {
        return new[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3),
        };
    }
}