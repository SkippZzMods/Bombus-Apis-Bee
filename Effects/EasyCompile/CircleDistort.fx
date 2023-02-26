sampler uImage0 : register(s0);

texture uImage1;
sampler2D uImage1Sampler = sampler_state { texture = <uImage1>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = wrap; AddressV = wrap; };

texture uImage2;
sampler2D uImage2Sampler = sampler_state { texture = <uImage2>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = wrap; AddressV = wrap; };

float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float2 rotation(float2 coords, float delta)
{
    float2 ret;
    ret.x = (coords.x * cos(delta)) - (coords.y * sin(delta));
    ret.y = (coords.x * sin(delta)) + (coords.y * cos(delta));
    return ret;
}


float4 Main(float2 coords : TEXCOORD0) : COLOR0
{
    float PI = 3.1459;

    float aspectRatio = (uScreenResolution.x / uScreenResolution.y);

    float2 center = (0.5, 0.5);

    float2 diff = center - coords;
    diff *= float2(aspectRatio, 1);
    
    if (dot(diff, diff) > 0.05 * 0.05)  
      return tex2D(uImage0, coords);

    float2 offset = float2(uColor.y, uColor.z);
	float2 uv_n = coords;

	float repeats = uColor.x;
	
	for (int i = 0; i < 10; i++)
	{
		float intensity1 = tex2D(uImage1Sampler, uScreenPosition + (uv_n + float2(uProgress / 6.28f, uProgress / 6.28f) / repeats)).r;
		float intensity2 = tex2D(uImage1Sampler, uScreenPosition + (uv_n + float2(-uProgress / 6.28f, uProgress / 6.28f) / repeats)).r;
		float angle = (sqrt(intensity1 * intensity2) * 6.28f) * tex2D(uImage2Sampler, coords * 0.1f);
		uv_n += rotation(offset, angle);
	}

    float4 color = tex2D(uImage0, uv_n);

    if (dot(diff, diff) > 0.048 * 0.048)
        color *= (1 - (dot(diff, diff) - 0.048 * 0.048) / 0.002 * 0.002);
	
	return color;
}

technique Technique1
{
    pass MainPass
    {
        PixelShader = compile ps_3_0 Main();
    }
}