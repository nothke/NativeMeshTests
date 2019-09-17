using UnityEngine.Rendering;
using Unity.Mathematics;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct Vertex
{
    public half4 pos;
    public half4 normal;

    public static VertexAttributeDescriptor[] GetVertexAttributes()
    {
        return new[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float16, 4),
            new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float16, 4),
        };
    }
}