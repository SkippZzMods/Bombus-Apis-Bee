sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
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

float alpha;
float whiteness;
float4 colorOne;
float4 colorTwo;

float4 White(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
	float pixW = 1.0 / uImageSize0.x;
	float pixH = 1.0 / uImageSize0.y;
	float4 color = tex2D(uImage0, coords);
    float2 squareWidth = float2(pixW, pixH);
    float4 outline = colorTwo * alpha;
    float4 white = float4(1.0, 1.0, 1.0, 1.0) * whiteness;
    outline += white;

    float4 clear = float4(0.0, 0.0, 0.0, 0.0);

    if (color.a != 0.0)
    {
        return (lerp(clear, colorOne, 0.33 * alpha) + white) * 0.5;
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
    pass MarkedEffect
    {
        PixelShader = compile ps_2_0 White();
    }
}