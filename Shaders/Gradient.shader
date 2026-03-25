Shader "Curstom/Gradient"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _C1("Color start", Color) = (0, 0, 0, 1)
        _C2("Color end",   Color) = (0, 0, 0, 1)
        [MaterialToggle]IsVertical("Is vertical", float) = 0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest off
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 col: COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 col: COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.col = v.col;
                return o;
            }

            sampler2D _MainTex;
            bool IsVertical;
            float4 _C1;
            float4 _C2;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = lerp(_C1, _C2, IsVertical ? i.uv.y : i.uv.x);
                return col;
            }
            ENDCG
        }
    }
}
