#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
Texture2D WithoutCoffee;
float4 coffeeColor;

SamplerState Sampler;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 withCoffeeColor = SpriteTexture.Sample(Sampler, input.TextureCoordinates);
    float withCoffeeBool = withCoffeeColor == float4(250.0/255,0,250.0/255,1) ? 1 : 0;
    
    coffeeColor = float4(0,0,0,.5);
    
    float4 withoutCoffeeColor = WithoutCoffee.Sample(Sampler, input.TextureCoordinates);
	
	float4 color = withoutCoffeeColor + coffeeColor;
	
	color += withCoffeeColor * 0.000001;
	return  color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};