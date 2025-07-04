Shader "Custom/RadialGradientWithMask"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        _Color1 ("Color 1", Color) = (1,0,0,1)
        _Color2 ("Color 2", Color) = (0,0,1,1)
        _RotationSpeed ("Rotation Speed", Float) = 1
        _FadeAmount ("Fade Amount", Range(0,1)) = 1
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _Color1;
            fixed4 _Color2;
            float _RotationSpeed;
            float _FadeAmount; // THIS WAS MISSING - CAUSED THE ERROR

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Get original texture alpha (for capsule shape)
                fixed4 tex = tex2D(_MainTex, i.texcoord);
                float alpha = tex.a * _FadeAmount; // Apply fade to alpha
                
                if (alpha <= 0) return fixed4(0,0,0,0);
                
                // Calculate rotating gradient
                float2 center = i.texcoord - 0.5;
                float angle = atan2(center.y, center.x) + (_Time.y * _RotationSpeed);
                float gradient = (sin(angle * 2) + 1) * 0.5; // More distinct color bands
                
                // Blend colors
                fixed4 color = lerp(_Color1, _Color2, gradient);
                color.a = alpha; // Maintain original transparency
                
                return color;
            }
            ENDCG
        }
    }
}