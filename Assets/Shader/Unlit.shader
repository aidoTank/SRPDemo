Shader "SRP/Unlit"
{
    Properties
    {
        _Color("Color",Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100


        Pass
        {
            HLSLPROGRAM
        // #pragma target 3.5
            #pragma preferred_hlslcc gles

            #pragma multi_compile_instancing

            #pragma vertex UnlitPassVertex
            #pragma fragment UnlitPassFragment

            #include "Unlit.hlsl"
            
            ENDHLSL
        }
    }
}
