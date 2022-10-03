sampler uImage0 : register(s0);
sampler uImage1 : register(s1);

float2 uImageSize0;
float4 colorOne;

float4 White(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
	float pixW = 1.0 / uImageSize0.x;
	float pixH = 1.0 / uImageSize0.y;
	float4 color = tex2D(uImage0, coords);
    float2 squareWidth = float2(pixW, pixH);

    float4 clear = float4(0.0, 0.0, 0.0, 0.0);

    if (color.a != 0.0)
    {
        return colorOne * 0.5;
    }

    return float4(0.0, 0.0, 0.0, 0.0);
}

technique Technique1
{
    pass MarkedEffect
    {
        PixelShader = compile ps_2_0 White();
    }
}