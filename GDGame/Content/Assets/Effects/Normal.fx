#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

#define FLAGS_DX11 (D3DCOMPILE_ENABLE_BACKWARDS_COMPATIBILITY)

matrix World;
matrix View;
matrix Projection;

float3 Light;
float4 DiffuseColor;
float DiffuseIntensity;
float DisplacementIntensity;

SamplerState Sampler;

texture2D ColorMap;
 
texture2D NormalMap;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float3 Normal : NORMAL0;
    float3 Binormal : BINORMAL0;
    float3 Tangent : TANGENT0;
};
 
struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float3x3 WorldToTangentSpace : TEXCOORD2;
};
 
//Used the XNA Shader 4.0 Tutorials on digitalerr0r.net
VertexShaderOutput VertexShaderFunction(VertexShaderInput input,float3 Normal : NORMAL)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
 
    float4 WorldPosition = mul(input.Position, World);
    float4 ViewPosition = mul(WorldPosition,View);
    
    output.Position = mul(ViewPosition, Projection);
    
    output.TexCoord = input.TexCoord;
    
    output.WorldToTangentSpace[0] = mul(normalize(input.Tangent), World);
    output.WorldToTangentSpace[1] = mul(normalize(input.Binormal), World);
    output.WorldToTangentSpace[2] = mul(normalize(input.Normal), World);
   
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{   
    float4 color = ColorMap.Sample(Sampler, input.TexCoord);
    float3 normalMap = 2.0 *(NormalMap.Sample(Sampler, input.TexCoord)) - 1.0;
        
    normalMap = normalize(mul(normalMap, input.WorldToTangentSpace));
    
    float4 normal = float4(normalMap,1.0);
     
    float4 diffuse = saturate(dot(-Light,normal));
    
    return  color * diffuse * DiffuseIntensity + color * float4(0.5,0.5,0.5,1);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
};