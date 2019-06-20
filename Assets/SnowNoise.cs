using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowNoise : MonoBehaviour
{
    public Shader snowFallShader;
    [Range(0.001f, 0.1f)] public float flakeAmount = 0.01f;
    [Range(0f, 1f)] public float flakeOpacity = 0.1f;

    private Material snowFallMat;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        snowFallMat=new Material(snowFallShader);
    }

    private void Update()
    {
        snowFallMat.SetFloat("_FlakeAmount",flakeAmount);
        snowFallMat.SetFloat("_FlakeOpacity", flakeOpacity);

        RenderTexture snow = meshRenderer.material.GetTexture("_SplatTex") as RenderTexture;
        var temp = RenderTexture.GetTemporary(snow.descriptor);
        Graphics.Blit(snow,temp,snowFallMat);
        Graphics.Blit(temp,snow);
        meshRenderer.material.SetTexture("_SplatTex",snow);
        RenderTexture.ReleaseTemporary(temp);
    }
}
