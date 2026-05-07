float2 screenSize;

float pixelSize;

matrix transformMatrix;

texture sampleTexture;
sampler2D samplerTex = sampler_state
{
    texture = <sampleTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

struct VertexInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct VertexOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

VertexOutput VSMain(VertexInput input)
{
    VertexOutput output;
    output.Position = mul(input.Position, transformMatrix);
    output.TexCoord = input.TexCoord;
    return output;
}

float4 PSMain(VertexOutput input) : COLOR
{
    float2 pixelCoord = input.TexCoord / pixelSize;
    pixelCoord = floor(pixelCoord + 0.5) * pixelSize;
    float4 color = tex2D(samplerTex, pixelCoord);
    return color;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VSMain();
        PixelShader = compile ps_3_0 PSMain();
    }
}

