Shader "Custom/OutputTexture"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Alive ("Alive", Color) = (1,1,1,1)
        _Dead ("Dead", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
		Pass
		{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv: TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv: TEXCOORD0;
			};

			uniform sampler _MainTex;
			uniform float4 _Alive;
			uniform float4 _Dead;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float4 c = tex2D(_MainTex, i.uv);

                return float4(c.r, c.g, c.b, c.a);
			}
			
			ENDCG
		}
      
    }
    FallBack "Diffuse"
}
