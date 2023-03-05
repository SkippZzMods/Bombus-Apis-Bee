float4 codedColor = float4(0.0, 1.0, 0.0, 1.0);
float4 uColorOne;
float4 uColorTwo;
float4 redColor;

float2 noiseScale;
float2 offset;

float uTime;

float time;
float power;
float speed;
float2 uOffset;

sampler2D SpriteTextureSampler;

texture distort;
sampler2D distortS = sampler_state { texture = <distort>; AddressU = wrap; AddressV = wrap; };

texture red;
sampler2D redS = sampler_state { texture = <red>; AddressU = wrap; AddressV = wrap; };

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 color = tex2D(SpriteTextureSampler, input.TextureCoordinates);

    float2 coords = input.TextureCoordinates;

    coords.x = (coords.x + uTime) % 1;

    float2 off = float2(0, sin(time + (coords.x + uOffset.x) * speed) * sin(1.57 + (coords.x + uOffset.x) * 0.37 * speed - time) * sin((coords.x + uOffset.x) * 0.21 * speed - time) * power);

    coords += off;

    float lerper = tex2D(distortS, (coords * noiseScale) + offset).r;
    
    if (color.g == codedColor.g)
    {
        float4 retColor = lerp(uColorOne, uColorTwo, lerper);

        return retColor;
    }
    
    return color;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile ps_2_0 MainPS();
    }
};
