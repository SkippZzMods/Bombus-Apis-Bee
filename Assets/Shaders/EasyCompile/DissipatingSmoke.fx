sampler uImage0 : register(s0);

texture uImage1;
sampler2D uImage1Sampler = sampler_state
{
    texture = <uImage1>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float4 uColor;
float4 uSecondaryColor;
float uOpacity;
float uSaturation;
float uTime;
float uProgress;


float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 noiseCoords = coords + float2(uTime * -0.5, uTime * 0.1);
    float4 noiseColor = tex2D(uImage1Sampler, noiseCoords);
    
    float dissolveValue = noiseColor.r * 1.5;
    float dist = length(coords - 0.5) * 2.0;

    float dissipate = dissolveValue - (dist * 1.5);
    
    float threshold = lerp(-1.0, 2.0, uProgress);
    
    clip(dissipate - threshold);

    float4 tex = tex2D(uImage0, coords);
    
    float4 finalColor = tex * uColor;
    
    float edgeThickness = 0.15;
    float edgeCheck = smoothstep(0.0, edgeThickness, dissipate - threshold);
    
    if (edgeCheck < 1.0 && uProgress > 0.0)
    {
        float3 burnEdgeColor = uSecondaryColor;
        finalColor.rgb = lerp(burnEdgeColor, finalColor.rgb, edgeCheck);
    }
    
    return finalColor;
}

technique Technique1
{
    pass P0
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}