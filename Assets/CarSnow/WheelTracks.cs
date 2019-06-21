using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTracks : MonoBehaviour
{
    public Shader drawShader;
    public GameObject terrain;
    [Range(1, 500)] public float brushSize = 5;
    [Range(0, 1)] public float brushStrength = 0.5f;
    public LayerMask layerMask;
    public Transform[] carWheels;

    private Camera cam;
    private RenderTexture splatmap;
    private Material snowMat, drawMat;
    private RaycastHit hit;

    private void Awake()
    {
        drawMat = new Material(drawShader);
        snowMat = terrain.GetComponent<MeshRenderer>().material;
        splatmap = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
        snowMat.SetTexture("_SplatTex", splatmap);
    }

    private void Update()
    {
        foreach (var wheel in carWheels)
        {
            if (Physics.Raycast(wheel.position,Vector3.down,out hit, 1f, layerMask))
            {
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
}