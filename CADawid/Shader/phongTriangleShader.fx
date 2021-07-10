cbuffer ConstantBuffer : register(b0)
{
	matrix MVP;
	float4 Color;
}

cbuffer ConstantBufferView : register(b1)
{
	matrix invViewMatrix;
}

struct VS_INPUT
{
	float4 Pos : POSITION;
	float4 Norm : NORMAL0;
};

struct VS_OUTPUT
{
	float4 Pos : SV_POSITION;
	float3 WorldPos : POSITION0;
	float3 Norm : NORMAL0;
	float4 Color : COLOR0;
	float3 ViewVec : TEXCOORD1;
};


static const float3 ambientColor = float3(0.4f, 0.4f, 0.4f);
static const float3 lightColor = float3(1.0f, 1.0f, 1.0f);
static const float3 lightPos = float3 (0.0f, 4.0f, -4.0f);
static const float kd = 1, ks = 0.5f, m = 100.0f;

//--------------------------------------------------------------------------------------
// Vertex Shader
//--------------------------------------------------------------------------------------
VS_OUTPUT VS(VS_INPUT input)
{
	VS_OUTPUT output = (VS_OUTPUT)0;
	output.WorldPos = input.Pos.xyz;
	output.Pos = mul(MVP, input.Pos);
	output.Color = Color;
	output.Norm = normalize(input.Norm.xyz);
	float3 camPos = mul(invViewMatrix, float4(0.0f, 0.0f, 0.0f, 1.0f)).xyz;
	output.ViewVec = camPos - output.WorldPos;
	return output;
}

struct PS_OUTPUT {
	float4 col1 : SV_Target0;
	float4 col2 : SV_Target1;
};

//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------
PS_OUTPUT PS(VS_OUTPUT input)
{
	float4 dColor = input.Color;

	float3 viewVec = normalize(input.ViewVec);
	float3 normal = -normalize(input.Norm);
	float3 color = dColor.rgb * ambientColor;

	float3 lightPosition = lightPos.xyz;
	float3 lightVec = normalize(lightPosition - input.WorldPos);
	float3 halfVec = normalize(viewVec + lightVec);
	color += lightColor * dColor.xyz * kd * saturate(dot(normal, lightVec)); //diffuse color
	float nh = dot(normal, halfVec);
	nh = saturate(nh);
	nh = pow(nh, m);
	nh *= ks;
	color += lightColor * nh;

	PS_OUTPUT o = (PS_OUTPUT)0;
	o.col1 = float4(saturate(color), dColor.a);
	o.col2 = o.col1;
	return o;
}
