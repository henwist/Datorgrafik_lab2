#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix World;
matrix View;
matrix Projection;

//Lighting

// This sample uses a simple Lambert lighting model.
float3 LightDirection = normalize(float3(1, 1, 1));
float3 DiffuseLight = 1.25;
float3 AmbientLight = 0.35;
Texture Texture;

sampler TextureSampler = sampler_state
{
	texture = <Texture>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;

	AddressU = mirror;
	AddressV = mirror;
};

struct VertexShaderInput
{
	float4 Position : SV_POSITION;
	float4 Normal : NORMAL0;
	float2 TexCoord: TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Normal : NORMAL0;
	float2 TexCoord: TEXCOORD0;
	float4 Color: COLOR0;
};


//float rand_1_05(in float2 uv)
//{
//    float2 noise = (frac(sin(dot(uv ,float2(12.9898,78.233)*2.0)) * 43758.5453));
//        return abs(noise.x + noise.y) * 0.5;
//}


//VertexShaderOutput MainVS(in float4 position : SV_POSITION,
//	in VertexShaderInput input,
//	in float4x4 objWorld : TEXCOORD1,
//	in float4 perInstancePos : TEXCOORD5)
//{
//
//	VertexShaderOutput output = (VertexShaderOutput)0;
//
//	output.Position = mul(position, mul(objWorld, mul(World, mul(View, Projection))));
//	output.Position = output.Position + perInstancePos;
//	output.Color = output.Position;
//	output.TexCoord = input.TexCoord;
//
//	//    // Compute lighting, using a simple Lambert model.
//	//float3 worldNormal = mul(input.Normal, objWorld);
//	//float diffuseAmount = max(-dot(worldNormal, LightDirection), 0);
//	//float3 lightingResult = saturate(diffuseAmount * DiffuseLight + AmbientLight);
//	//output.Color = float4(lightingResult, 1);
//
//	return output;
//}

VertexShaderOutput MainVS(in float4 position : SV_POSITION,
	in VertexShaderInput input)
{

	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(position, mul(World, mul(View, Projection)));
	output.TexCoord = input.TexCoord;

	    // Compute lighting, using a simple Lambert model.
	//float3 worldNormal = mul(input.Normal, objWorld);
	float diffuseAmount = max(-dot(input.Normal, LightDirection), 0);
	float3 lightingResult = saturate(diffuseAmount * DiffuseLight + AmbientLight);
	output.Color = float4(lightingResult, 1);

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{

	float4 output = tex2D(TextureSampler, input.TexCoord) * input.Color;

	return output;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
