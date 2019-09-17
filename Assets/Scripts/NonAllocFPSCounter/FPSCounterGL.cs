using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCounterGL : MonoBehaviour
{
    public Texture2D fontTexture;
    public int pixelScale = 4;
    public float thresholdFPS = 60;

    public enum RenderSchedule { OnPostRender, WaitForEndOfFrameCoroutine }
    public RenderSchedule schedule;

    void Start()
    {
        for (int i = 0; i < texCoords.Length; i++)
        {
            texCoords[i].y = 16 - texCoords[i].y - h;
            texCoords[i] /= 16.0f;
        }

        c01 /= 16.0f;
        c11 /= 16.0f;
        c10 /= 16.0f;

        if (schedule == RenderSchedule.WaitForEndOfFrameCoroutine)
            StartCoroutine(EndOfFrameCo());
    }

    Material material;

    void CreateMaterial()
    {
        Shader shader = Shader.Find("Hidden/FPSCounter");
        material = new Material(shader);
        // Turn on alpha blending
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        // Turn backface culling off
        material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        // Turn off depth writes
        material.SetInt("_ZWrite", 0);

        // makes the material draw on top of everything
        material.SetInt("_ZTest", 0);

        material.SetTexture("_MainTex", fontTexture);
    }

    static readonly Vector2[] texCoords =
    {
        new Vector2(0,0),
        new Vector2(4,0),
        new Vector2(8,0),
        new Vector2(12,0),
        new Vector2(0,6),
        new Vector2(4,6),
        new Vector2(8,6),
        new Vector2(12,6),
        new Vector2(0,11),
        new Vector2(4,11), // .
        new Vector2(8,11)  // :
    };

    const int w = 3;
    const int h = 5;

    static Vector2 c01 = new Vector2(0, h);
    static Vector2 c11 = new Vector2(w, h);
    static Vector2 c10 = new Vector2(w, 0);

    float deltaTime = 0.0f;
    float fps;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        fps = 1.0f / deltaTime;
    }

    static readonly Color red = new Color(1, 0, 0, 1);
    static readonly Color white = new Color(1, 1, 1, 1);

    void OnPostRender()
    {
        if (schedule == RenderSchedule.OnPostRender)
            Render();
    }

    WaitForEndOfFrame wfeof;

    IEnumerator EndOfFrameCo()
    {
        wfeof = new WaitForEndOfFrame();

        while (true)
        {
            yield return wfeof;
            Render();
        }
    }

    void Render()
    {
        if (!material)
        {
            CreateMaterial();
        }

        GL.PushMatrix();
        material.SetPass(0);

        GL.LoadPixelMatrix();
        GL.Begin(GL.QUADS);

        if (fps < thresholdFPS)
            GL.Color(red);
        else
            GL.Color(white);

        int v = (int)fps;

        int i = Mathf.FloorToInt(Mathf.Log10(v));
        while (v > 0)
        {
            int n = v % 10;
            v = v / 10;

            float xo = 10 + i * pixelScale * (w + 1);
            float yo = 10;

            GL.TexCoord(texCoords[n]); // 00
            GL.Vertex3(xo, yo, 0);

            GL.TexCoord(texCoords[n] + c01); // 01
            GL.Vertex3(xo, yo + h * pixelScale, 0);

            GL.TexCoord(texCoords[n] + c11);// 11
            GL.Vertex3(xo + w * pixelScale, yo + h * pixelScale, 0);

            GL.TexCoord(texCoords[n] + c10); // 10
            GL.Vertex3(xo + w * pixelScale, yo, 0);

            i--;
        }

        GL.End();
        GL.PopMatrix();
    }
}
