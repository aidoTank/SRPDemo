#ifndef SPR_UNLIT_INCLUDED
#define SPR_UNLIT_INCLUDED


// /*
//VP矩阵可以放入每帧(per-frame)缓冲区
cbuffer UnityPerFrame {
    float4x4 unity_MatrixVP;
};

//M矩阵可以放入每次绘制(per-frame)缓冲区
cbuffer UnityPerDraw {
    float4x4 unity_ObjectToWorld;
}

#define UNITY_MATRIX_M unity_ObjectToWorld

cbuffer UnityPerMaterial {
    float4 _Color;
};
// */

// CBUFFER_START(UnityPerFrame)
//     float4x4 unity_MatrixVP;
// CBUFFER_END

// CBUFFER_START(UnityPerDraw)
//     float4x4 unity_ObjectToWorld;
// CBUFFER_END



struct VertexIntput {
    float4 pos : POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct VertexOutput {
    float4 clipPos : SV_POSITION;
};

VertexOutput UnlitPassVertex (VertexIntput input) {
    VertexOutput output;
    UNITY_VERTEX_INPUT_INSTANCE_ID(input);
    float4 worldPos = mul(UNITY_MATRIX_M, float4(input.pos.xyz, 1.0));
    output.clipPos = mul(unity_MatrixVP, worldPos);
    return output;
}

float4 UnlitPassFragment (VertexOutput output) : SV_TARGET {
    return _Color;
}


#endif //SPR_UNLIT_INCLUDED