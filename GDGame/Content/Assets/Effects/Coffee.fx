#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;

Texture2D Displacement : t0;
Texture2D FlowMap : t1;
Texture2D test : t2;

float4 coffeeColor;

float time;

SamplerState Sampler : register(s0);

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};


VertexShaderOutput MainVS(VertexShaderInput input)
{
    VertexShaderOutput output;
    
    float4 WorldPosition = mul(input.Position, World);
    float4 ViewPosition = mul(WorldPosition,View);
    
    output.Position = mul(ViewPosition,Projection);
    
    
    
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
    float4 flow = FlowMap.Sample(Sampler, input.TexCoord);
    
    //Calculate UVS
    float noise = flow.w;
    float noiseTime = time/10 + noise;
    float3 uva = FlowUVW(input.TexCoord,flow.xy,noiseTime,false);
    float3 uvb = FlowUVW(input.TexCoord,flow.xy,noiseTime,true);
    
    
    //Other
    float4 tex1 = Displacement.Sample(Sampler,uva.xy) * uva.z;
    float4 tex2 = Displacement.Sample(Sampler,uvb.xy) * uvb.z;
	
	float4 finalTex = tex1+tex2;
	
	return finalTex * coffeeColor ;
}


technique SpriteDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};