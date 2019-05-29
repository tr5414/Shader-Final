Shader "Custom/RenderToTexture_CA"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
        _Alive ("Alive", Color) = (1,1,1,1)
        _Dead ("Dead", Color) = (0,0,0,1)
    }
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		
		Pass
		{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
            
            uniform float4 _MainTex_TexelSize;
            uniform float4 _Alive;
            uniform float4 _Dead;       
            
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
   
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
            
           
            sampler2D _MainTex;
            
            // Used by frag because the compiler thinks float4 != Color
            bool colorsAreEqual(float4 c1, float4 c2){
                return c1[0] == c2[0] && // red
                        c1[1] == c2[1] && // green
                        c1[2] == c2[2] && // blue
                        c1[3] == c2[3]; // alpha
            }

			fixed4 frag(v2f i) : SV_Target
			{
            
                float2 texel = float2(
                    _MainTex_TexelSize.x, 
                    _MainTex_TexelSize.y 
                );
                
                float cx = i.uv.x;
                float cy = i.uv.y;
                
                float4 C = tex2D( _MainTex, float2( cx, cy ));   
                
                float up = i.uv.y + texel.y * 1;
                float down = i.uv.y + texel.y * -1;
                float right = i.uv.x + texel.x * 1;
                float left = i.uv.x + texel.x * -1;
                
                float4 arr[8];
                
                arr[0] = tex2D(  _MainTex, float2( cx   , up ));   //N
                arr[1] = tex2D(  _MainTex, float2( right, up ));   //NE
                arr[2] = tex2D(  _MainTex, float2( right, cy ));   //E
                arr[3] = tex2D(  _MainTex, float2( right, down )); //SE
                arr[4] = tex2D(  _MainTex, float2( cx   , down )); //S
                arr[5] = tex2D(  _MainTex, float2( left , down )); //SW
                arr[6] = tex2D(  _MainTex, float2( left , cy ));   //W
                arr[7] = tex2D(  _MainTex, float2( left , up ));   //NW

                int cnt = 0;
                for(int i=0;i<8;i++){
                    if (colorsAreEqual(arr[i], _Alive)) {
                        cnt++;
                    }
                }

                if (colorsAreEqual(C, _Alive)) { // Originally we just checked the red channel
                    if (cnt == 2 || cnt == 3) {
                        //Any live cell with two or three live neighbours lives on to the next generation.
                        return _Alive;
                    } else {
                        //Any live cell with fewer than two live neighbours dies, as if caused by underpopulation.
                        //Any live cell with more than three live neighbours dies, as if by overpopulation.
                        return _Dead;
                    }
                } else { //cell is dead
                    if (cnt == 3) {
                        //Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
                        return _Alive;
                    } else {
                        return _Dead;
                    }
                }
                
            }

			ENDCG
		}

	}
	FallBack "Diffuse"
}