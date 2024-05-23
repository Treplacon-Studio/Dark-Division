Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _ScrollSpeed("Scroll Speed", Vector) = (1, 1, 0, 0)
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float2 _ScrollSpeed;

                struct appdata_t
                {
                    float4 vertex : POSITION;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = (mul(unity_ObjectToWorld, v.vertex)).xy;
                    o.uv += _ScrollSpeed * _Time.y;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    return col;
                }
                ENDCG
            }
        }

}
