using UnityEngine.Rendering;
using Unity.Mathematics;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct Vertex
{
    public half3 pos;
    public half3 normal;

    public static VertexAttributeDescriptor[] GetVertexAttributes()
    {
        return new[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float16, 3),
            new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float16, 3),
        };
    }
}