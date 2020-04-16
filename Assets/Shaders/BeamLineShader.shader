﻿Shader "Unlit/BeamLineShader"
{
    Properties
    {
        MaterialTexture			("MaterialTexture", 2D)				= "white" {}
		MaterialColor			("Color", Color)					= (1, 1, 0, 1)
		StretchFactor           ("StretchFactor", Float)            = 1 
		AnimationMoment         ("AnimationMoment", Float)          = 0
    }
    SubShader
    {
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct VertexData
            {
                float4 position : POSITION;
                float2 texture_coordinates : TEXCOORD0;
            };

            struct FragmentData
            {
                float2 texture_coordinates : TEXCOORD0;
                float4 screen_position : SV_POSITION;
				float4 world_position : TEXCOORD1;
            };

            sampler2D MaterialTexture;
            float4 MaterialTexture_ST;
			fixed4 MaterialColor;
			float StretchFactor;
			float AnimationMoment;

			FragmentData vert (VertexData vertex_data)
            {
				FragmentData fragment_data;
				fragment_data.screen_position = UnityObjectToClipPos(vertex_data.position);
				fragment_data.texture_coordinates = TRANSFORM_TEX(vertex_data.texture_coordinates, MaterialTexture);

				fragment_data.world_position = vertex_data.position;

                return fragment_data;
            }

            fixed4 frag (FragmentData fragment_data) : SV_Target
            {
				fixed4 color = MaterialColor;
					
				float2 texture_coordinates = fragment_data.texture_coordinates;
				texture_coordinates.x -= AnimationMoment;
				texture_coordinates.x /= StretchFactor;

				color.a *= tex2D(MaterialTexture, texture_coordinates).a;

				return color;
            }
            ENDCG
        }
    }
}
