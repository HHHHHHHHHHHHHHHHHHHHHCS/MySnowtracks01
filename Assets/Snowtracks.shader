Shader "Custom/Snowtrack"
{
	Properties
	{
		_Tess ("Tessellation", Range(1, 32)) = 4
		_SnowColor ("Snow Color", Color) = (1, 1, 1, 1)
		_SnowTex ("Snow Texture", 2D) = "white" { }
		_GroundColor ("Ground Color", Color) = (1, 1, 1, 1)
		_GroundTex ("Ground Texture", 2D) = "white" { }
		_SplatTex ("SplatMap", 2D) = "black" { }
		_Displacement ("Displacement", Range(0, 1.0)) = 0.3
		_Glossiness ("Smoothness", Range(0, 1)) = 0.5
		_Metallic ("Metallic", Range(0, 1)) = 0.0
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		
		LOD 200
		
		CGPROGRAM
		
		#pragma surface surf Standard fullforwardshadows vertex:vert tessellate:tessDistance
		
		#pragma target 5.0
		
		#include "Tessellation.cginc"
		
		struct appdata
		{
			float4 vertex: POSITION;
			float4 tangent: TANGENT;
			float3 normal: NORMAL;
			float2 texcoord: TEXCOORD0;
		};
		
		float _Tess;
		
		float4 tessDistance(appdata v0, appdata v1, appdata v2)
		{
			float minDist = 5.0;
			float maxDist = 15.0;
			return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, _Tess);
		}
		
		sampler2D _SplatTex;
		float _Displacement;
		
		void vert(inout appdata v)
		{
			float d = tex2Dlod(_SplatTex, float4(v.texcoord.xy, 0, 0)).r * _Displacement;
			v.vertex.xyz -= v.normal * d;
			v.vertex.xyz += v.normal * _Displacement;
		}
		
		sampler2D _SnowTex;
		half4 _SnowColor;
		sampler2D _GroundTex;
		half4 _GroundColor;
		
		struct Input
		{
			float2 uv_GroundTex;
			float2 uv_SnowTex;
			float2 uv_SplatTex;
		};
		
		half _Glossiness;
		half _Metallic;
		
		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			half4 snowCol = tex2D(_SnowTex, IN.uv_SnowTex) * _SnowColor;
			half4 groundCol = tex2D(_GroundTex, IN.uv_GroundTex) * _GroundColor;
			half amount = tex2D(_SplatTex, IN.uv_SplatTex).r;
			half4 c = lerp(snowCol, groundCol, amount);
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		
		
		ENDCG
		
	}
}
