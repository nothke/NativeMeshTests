using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;

public class Example : MonoBehaviour
{
    // Vertex with FP32 position, FP16 2D normal and a 4-byte tangent.
    // In some cases StructLayout attribute needs
    // to be used, to get the data layout match exactly what it needs to be.
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    struct ExampleVertex
    {
        public Vector3 pos;
        public ushort normalX, normalY;
        public Color32 tangent;
    }

    void Start()
    {
        var mesh = new Mesh();
        // specify vertex count and layout
        var layout = new[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float16, 2),
            new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.UNorm8, 4),
        };
        var vertexCount = 10;
        mesh.SetVertexBufferParams(vertexCount, layout);

        // set vertex data
        var verts = new NativeArray<ExampleVertex>(vertexCount, Allocator.Temp);

        // ... fill in vertex array data here...

        mesh.SetVertexBufferData(verts, 0, 0, vertexCount);
    }
}