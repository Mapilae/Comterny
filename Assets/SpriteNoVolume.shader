Shader "Unlit/SpriteNoVolume"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "Queue"="Overlay"
        }
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB
            SetTexture [_MainTex] { combine texture }
        }
    }
}