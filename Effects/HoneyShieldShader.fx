float time;
float blowUpPower; //The power the blow up effect is set as. Ideally betwen 0 and 1
float blowUpSize; //The strenght of the expansion caused by the blow up effect
float3 shieldColor;
float shieldOpacity;
float3 shieldEdgeColor;
float shieldEdgeBlendStrenght;
float noiseScale;
float resolution;

float uTime;
float power;
float speed;
float2 offset;

texture sampleTexture;
sampler2D Texture1Sampler = sampler_state { texture = <sampleTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = wrap; AddressV = wrap; };

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float distanceFromCenter = length(uv - float2(0.5, 0.5)) * 2;
    
    //"Blow up" the noise map so it looks circular.
    float blownUpUVX = pow((abs(uv.x - 0.5)) * 2, blowUpPower);
    float blownUpUVY = pow((abs(uv.y - 0.5)) * 2, blowUpPower);
    float2 blownUpUV = float2(-blownUpUVY * blowUpSize * 0.5 + uv.x * (1 + blownUpUVY * blowUpSize), -blownUpUVX * blowUpSize * 0.5 + uv.y * (1 + blownUpUVX * blowUpSize));
    
    //Rescale
    blownUpUV *= noiseScale;
    //Scroll effect
    blownUpUV.x = (blownUpUV.x + time) % 1;
    
    //Get the noise color

    float2 off = float2(0, sin(uTime + (uv.x + offset.x) * speed) * sin(1.57 + (uv.x + offset.x) * 0.37 * speed - uTime) * sin((uv.x + offset.x) * 0.21 * speed - uTime) * power);

    float4 noiseColor = tex2D(Texture1Sampler, blownUpUV + off);

    //Apply a layers of fake fresnel
    noiseColor += pow(distanceFromCenter, 6);  // + pow(distanceFromCenter, 3) * 0.6; <- Brings us over into ps 3.0 territory if we have pixelation on.
    //Fade the edges
    if (distanceFromCenter > 0.95)
        noiseColor *= (1 - ((distanceFromCenter - 0.95) / 0.05));
    
    return noiseColor * float4(lerp(shieldColor, shieldEdgeColor, pow(distanceFromCenter, shieldEdgeBlendStrenght)), shieldOpacity);
}

technique Technique1
{
    pass ShieldPass
    {
        PixelShader = compile ps_3_0 main();
    }
}