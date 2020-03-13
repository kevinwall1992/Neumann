Shader "Unlit/UnitShader"
{
    Properties
    {
        MaterialTexture		("MaterialTexture", 2D)		= "white" {}
		MaterialColor		("Color", Color)			= (1, 1, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

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
            };

            sampler2D MaterialTexture;
            float4 MaterialTexture_ST;
			fixed4 MaterialColor;

			FragmentData vert (VertexData vertex_data)
            {
				FragmentData fragment_data;
				fragment_data.screen_position = UnityObjectToClipPos(vertex_data.position);
				fragment_data.texture_coordinates = TRANSFORM_TEX(vertex_data.texture_coordinates, MaterialTexture);

                return fragment_data;
            }

            fixed4 frag (FragmentData fragment_data) : SV_Target
            {
                fixed4 color = tex2D(MaterialTexture, fragment_data.texture_coordinates) * MaterialColor;

				return color;
            }
            ENDCG
        }
    }
}
