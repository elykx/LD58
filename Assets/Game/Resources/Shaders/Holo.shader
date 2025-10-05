Shader "Custom/Holo"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}

        _ColorA ("Color A", Color) = (1,0,0,1)
        _ColorB ("Color B", Color) = (0,1,1,1)

        _Speed ("Shift Speed", Float) = 1
        _Scale ("Pattern Scale", Float) = 5
        _Intensity ("Holo Intensity", Range(0,1)) = 0.5

        _Mode ("Pattern Mode (0=X,1=Y,2=Diag,3=Wave)", Int) = 0
        _Offset ("Phase Offset", Float) = 0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" 
               "IgnoreProjector"="True" "CanUseSpriteAtlas"="True" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _ColorA;
            float4 _ColorB;
            float _Speed;
            float _Scale;
            float _Intensity;
            int _Mode;
            float _Offset;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color    : COLOR;
            };

            struct v2f
            {
                float2 uv       : TEXCOORD0;
                fixed4 color    : COLOR;
                float4 vertex   : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv) * i.color;

                // выбираем направление перелива
                float uvShift = 0;
                if (_Mode == 0) uvShift = i.uv.x * _Scale;                 // горизонталь
                else if (_Mode == 1) uvShift = i.uv.y * _Scale;            // вертикаль
                else if (_Mode == 2) uvShift = (i.uv.x + i.uv.y) * _Scale; // диагональ
                else uvShift = sin(i.uv.x * _Scale + _Time.y * _Speed);    // волна

                float t = frac(_Time.y * _Speed + uvShift + _Offset);

                // интерполяция между двумя цветами
                float3 holo = lerp(_ColorA.rgb, _ColorB.rgb, t);

                // добавляем перелив поверх
                texColor.rgb += holo * _Intensity;

                return texColor;
            }
            ENDHLSL
        }
    }
}
