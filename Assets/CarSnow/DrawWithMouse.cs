using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawWithMouse : MonoBehaviour
{
    public Shader drawShader;

    [Range(1, 500)] public float brushSize = 5;
    [Range(0, 1)] public float brushStrength = 0.5f;

    private Camera cam;
    private RenderTexture splatmap;
    private Material snowMat, drawMat;
    private RaycastHit hit;


    private void Awake()
    {
        cam = Camera.main;
        drawMat = new Material(drawShader);
        drawMat.SetColor("_Color", Color.red);

        snowMat = GetComponent<MeshRenderer>().material;
        splatmap = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
        snowMat.SetTexture("_SplatTex", splatmap);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                //必须是Mesh Collider 才有 textureCoord
                drawMat.SetVector("_Coordinate", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0));
                drawMat.SetFloat("_Size", brushSize);
                drawMat.SetFloat("_Strength", brushStrength);
                RenderTexture temp =
                    RenderTexture.GetTemporary(splatmap.descriptor);
                Graphics.Blit(splatmap, temp);
                Graphics.Blit(temp, splatmap, drawMat);
                RenderTexture.ReleaseTemporary(temp);
            }
        }
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, 256, 256), splatmap, ScaleMode.ScaleToFit, false);
    }
}