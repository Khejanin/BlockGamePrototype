#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 WorldViewProjection;
Texture2D SpriteTexture;
Texture2D Displacement;
Texture2D FlowMap;
float time;
float4 coffeeColor;

SamplerState Sampler;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    /*float3 Normal : NORMAL0;
    float3 Binormal : BINORMAL0;
    float3 Tangent : TANGENT0;*/
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    //float3 View : TEXCOORD1;
    //float3x3 WorldToTangentSpace : TEXCOORD2;
};


VertexShaderOutput MainVS(VertexShaderInput input)
{
    VertexShaderOutput output;
    
    output.Position = mul(input.Position, WorldViewProjection);
    output.TexCoord = input.TexCoord;
    
    return output;
}

float3 FlowUVW (float2 uv, float2 flowVector, float time, bool flowB) {
	float phaseOffset = flowB ? 0.5 : 0;
	float progress = frac(time + phaseOffset);
	float3 uvw;
	uvw.xy = uv - flowVector * progress;
	uvw.z = 1 - abs(1 - 2 * progress);
	return uvw;
}


float4 MainPS(VertexShaderOutput input) : COLOR
{
    //Sample Textures
    //Basic
    float4 withCoffeeColor = SpriteTexture.Sample(Sampler, input.TexCoord); 
    
    float4 flow = FlowMap.Sample(Sampler, input.TexCoord);
    
    //Calculate UVS
    float noise = flow.w;
    float noiseTime = time/10 + noise;
    float3 uva = FlowUVW(input.TexCoord,flow.xy,noiseTime,false);
    float3 uvb = FlowUVW(input.TexCoord,flow.xy,noiseTime,true);
    
    
    //Other
    float4 tex1 = Displacement.Sample(Sampler,uva.xy) * uva.z;
    float4 tex2 = Displacement.Sample(Sampler,uvb.xy) * uvb.z;
    
    
    //float2 test = float2(slideUV.x,slideUV.y);
	
	float4 finalTex = tex1+tex2;
	
	float4 cheat = withCoffeeColor * 0.000001f + /* displacement * 0.0000001f +*/ flow *0.0000001f + finalTex * 0.000001f;
	
	return cheat + finalTex * float4(111/255.0,78/255.0,5/255.0,0.8);
	//return  withCoffeeBool * withoutCoffeeColorDistorted + withoutCofeeBool * withoutCoffeeColor + cheat;
}

technique SpriteDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};