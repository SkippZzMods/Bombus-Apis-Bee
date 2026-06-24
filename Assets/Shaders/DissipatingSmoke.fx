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

float2 uResolution;
float pixelRes;


float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = coords;
    if (pixelRes > 0)
        coords = floor(uv * pixelRes) / pixelRes;
    
    float4 tex = tex2D(uImage0, coords);
    float brightness = max(max(tex.r, tex.g), tex.b);

    tex.a = brightness;
    
    if (tex.a <= 0.01)
        return float4(0, 0, 0, 0);
    
    float2 noiseCoords = coords + float2(uTime * -0.5, uTime * 1.5);
    float4 noiseColor = tex2D(uImage1Sampler, noiseCoords);
    
    float2 distortedCoords = coords + (noiseColor.rg - 0.5) * 0.02;
    
    float dist = length(coords - 0.5) * 2.0;
    float shape = 1.0 - dist;
    
    float density = (noiseColor.r * 2.0) * shape - uProgress;
   
    float alpha = smoothstep(-2.0, 1.0, density);
    
    float colorMix = smoothstep(0.0, 1.0, density);
    float4 finalColor = tex * lerp(uSecondaryColor, uColor, colorMix);
    
    finalColor.a *= alpha;
    
    finalColor.rgb *= 1.25;
    
    return finalColor;
}

technique Technique1
{
    pass P0
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}