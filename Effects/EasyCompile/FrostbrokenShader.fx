sampler uImage0 : register(s0);

texture uImage1;
sampler2D uImage1S = sampler_state { texture = <uImage1>; AddressU = wrap; AddressV = wrap; };

texture uImage2;
sampler2D uImage2S = sampler_state { texture = <uImage2>; AddressU = wrap; AddressV = wrap; };

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

float alpha;
float4 colorOne;
float4 colorTwo;
float4 colorThree;

float4 noiseColor;

float noiseScale;
float noiseScale2;

float2 offset;

float4 White(float2 coords : TEXCOORD0) : COLOR0
{
	float pixW = 1.0 / uImageSize0.x;
	float pixH = 1.0 / uImageSize0.y;
	float4 color = tex2D(uImage0, coords);
    float2 squareWidth = float2(pixW, pixH);
    float4 outline = colorThree * alpha;

    if (color.a != 0.0)
    {
        float2 uv = coords;
        uv.x = (uv + uTime) % 1;

        float lerper = tex2D(uImage1S, (coords * noiseScale) + offset).r;

        float4 retColor = lerp(colorOne, colorTwo, lerper);

        float4 noise = tex2D(uImage2S, (uv * noiseScale2 ) + offset);
        noise *= noiseColor;
            
        return retColor * noise;
    }

    float2 opposite = squareWidth * float2(1.0, -1.0);

    if (color.a > 0.0)
        outline *= 0.0;

    if (tex2D(uImage0, coords + squareWidth).a != 0.0)
        return outline;
    if (tex2D(uImage0, coords - squareWidth).a != 0.0)
        return outline;
    if (tex2D(uImage0, coords + opposite).a != 0.0)
        return outline;
    if (tex2D(uImage0, coords - opposite).a != 0.0)
        return outline;

    return float4(0.0, 0.0, 0.0, 0.0);
}

technique Technique1
{
    pass FrostbrokenShader
    {
        PixelShader = compile ps_2_0 White();
    }
}
