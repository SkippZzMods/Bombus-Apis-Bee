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

float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float power;
float speed;
float opacity;

float uProgress;
float noiseIntensity;
float3 uColorArray[4];

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 noiseCoords = coords + float2(uTime * -0.5, uTime * 0.1);
    float4 noiseColor = tex2D(uImage1Sampler, noiseCoords);

    float2 distortionOffset = (noiseColor.rg - 0.5) * noiseIntensity;
    float2 uv = coords + distortionOffset;
    float4 shapeColor = tex2D(uImage0, uv);

    float brightness = max(max(shapeColor.r, shapeColor.g), shapeColor.b);

    shapeColor.a = brightness;
    
    if (shapeColor.a <= 0.01)
        return float4(0, 0, 0, 0);

    float colorPosition = brightness * 3.0; 

    int index1 = clamp(floor(colorPosition), 0, 3);
    int index2 = clamp(index1 + 1, 0, 3);

    float mixAmount = frac(colorPosition);

    float3 mappedColor = lerp(uColorArray[index1], uColorArray[index2], mixAmount);
    
    float4 finalColor = float4(mappedColor.r, mappedColor.g, mappedColor.b, brightness * uOpacity);

    float dissolveValue = noiseColor.r;
    float dist = length(coords - 0.5) * 2.0;

    float invDist = 1.0 - dist;

    float burnMap = dissolveValue - (invDist * 1.5);
    
    float threshold = lerp(-1.5, 1.2, uProgress);
    
    clip(burnMap - threshold);

    float edgeThickness = 0.15;
    float edgeCheck = smoothstep(0.0, edgeThickness, burnMap - threshold);
    
    if (edgeCheck < 1.0 && uProgress > 0.0)
    {
        float3 burnEdgeColor = uSecondaryColor * 2.5;
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