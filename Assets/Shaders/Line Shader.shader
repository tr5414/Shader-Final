Shader "Custom/Line Shader"
{
    Properties
    {
        //_MainTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        /*
        Tags { "RenderType"="Opaque" }
        LOD 100
        */

        Pass
        {
            //Tags { "LightMode" = "ForwardAdd" } //Important! In Unity, point lights are calculated in the the ForwardAdd pass
            //Blend One One //Turn on additive blending if you have more than one point light

            //Cull off // ?

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                //float2 uv : TEXCOORD0;
            };

            struct v2f
            {             
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                //float2 uv : TEXCOORD0;
                float3 vertexInWorldCoords : TEXCOORD1;
                //float heightVal : TEXCOORD2;
            };

            uniform sampler _MainTex;
            uniform float _NumPartitions; // Will be 256 for now but we could theoretically change that
            uniform float _Magnitudes[256]; // Will be set by LineScript. Is it possible for us to not have to hard-code this?

            v2f vert (appdata v)
            {
                v2f o;

                o.vertexInWorldCoords = mul(unity_ObjectToWorld, v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                float4 xyz = float4(
                        v.vertex.x,
                        v.vertex.y + _Magnitudes[floor((v.vertex.x+2)/(4*_NumPartitions))],
                        v.vertex.z,
                        1.0
                );
                
                // set 0.uv and o.heightVal

                o.vertex = UnityObjectToClipPos(xyz); 
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                // Just make it green for now
                return float4(0.0, 1.0, 0.0, 1.0);
            }
            ENDCG
        }
    }
}
