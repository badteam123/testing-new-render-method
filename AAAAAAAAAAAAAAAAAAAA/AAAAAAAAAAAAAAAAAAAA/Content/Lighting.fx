#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

extern matrix World;
extern matrix WorldViewProjection;
extern Texture2D Texture;

extern float4 AmbientColor;
extern float AmbientIntensity;
extern float3 playerPos;

struct PointLight
{
    float3 Position;
    float3 Color;
    float3 Attenuation;
};

// HARD CODED LIGHT CAP (as it has to be an array of a set size)
extern PointLight PointLights[4];
int NumLights;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float3 Normal : NORMAL0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float3 Normal : NORMAL0;
    float3 WorldPos : TEXCOORD0;
};

// VERTEX SHADER
VertexShaderOutput MainVS(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = mul(input.Position, WorldViewProjection);
    output.Color = input.Color;
    output.Normal = mul((float3x3) World, input.Normal); // Transform normal to world space
    output.WorldPos = mul(World, input.Position).xyz; // Calculate world position
    return output;
}

float4 MainPS(VertexShaderOutput input) : SV_TARGET
{
    float4 ambient = AmbientColor * AmbientIntensity;
    float4 finalColor = input.Color * ambient;
    
    finalColor.rgb = float3(0.1, 0.1, 0.1);
    
    float3 lightDir = playerPos - input.WorldPos;
    float distance = length(lightDir);
    finalColor.rgb -= min((distance*0.08) - 0.9, 0);

    return finalColor;
}



technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}
