#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix World;
matrix View;
matrix Projection;

float4 AmbientColor;
float AmbientIntensity;

float3 Light;
float4 DiffuseColor;
float DiffuseIntensity;


texture2D ColorMap;
sampler2D ColorMapSampler = sampler_state
{
    Texture = <ColorMap>;
    MinFilter = linear;
    MagFilter = linear;
    MipFilter = linear;
};
 
texture2D NormalMap;
sampler2D NormalMapSampler = sampler_state
{
    Texture = <NormalMap>;
    MinFilter = linear;
    MagFilter = linear;
    MipFilter = linear;
};

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
    float4 color = tex2D(ColorMapSampler, input.TexCoord);
    float3 normalMap = 2.0 *(tex2D(NormalMapSampler, input.TexCoord)) - 1.0;
        
    normalMap = normalize(mul(normalMap, input.WorldToTangentSpace));
    
    float4 normal = float4(normalMap,1.0);
     
    float4 diffuse = saturate(dot(-Light,normal));
    
    return  color + color * diffuse * DiffuseColor * DiffuseIntensity + color * AmbientIntensity * AmbientColor;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
};