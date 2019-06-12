Shader "Custom/Line Shader"
{
    Properties
    {

    }
    SubShader
    {
        // These things don't seem to affect anything
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Tags { "LightMode" = "ForwardAdd" } //Important! In Unity, point lights are calculated in the the ForwardAdd pass
            //Blend One One //Turn on additive blending if you have more than one point light

            //Cull off // What does this do? It doesn't seem to change anything either

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {             
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float3 vertexInWorldCoords : TEXCOORD1;
            };

            uniform sampler _MainTex;
            uniform float _NumPartitions; // Will be 256 for now but we could theoretically change that
            uniform float _Magnitudes[256]; // Will be set by LineScript. Is it possible for us to not have to hard-code this?

            v2f vert (appdata v)
            {
                v2f o;

                o.vertexInWorldCoords = mul(unity_ObjectToWorld, v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                // The line is assumed to go from -1 to 1 on the world x-axis
                float4 xyz = float4(
                        v.vertex.x,
                        v.vertex.y + 100*_Magnitudes[floor((o.vertexInWorldCoords.x+1.0)*(_NumPartitions/2 - 1))], // Idk why /2 - 1 is necessary
                        v.vertex.z,
                        1.0
                );

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
