cbuffer ConstantBuffer : register(b0)
{
	matrix MVP;
	float4 Color;
}

struct VS_OUTPUT
{
	float4 Pos : SV_POSITION;
	float4 Color : COLOR0;
};

//--------------------------------------------------------------------------------------
// Vertex Shader
//--------------------------------------------------------------------------------------
VS_OUTPUT VS(float4 Pos : POSITION)
{
	VS_OUTPUT output = (VS_OUTPUT)0;
	output.Pos = mul(MVP, Pos);
	output.Color = Color;
	return output;
}

struct PS_OUTPUT {
	float4 col1 : SV_Target0;
	float4 col2 : SV_Target1;
};

//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------
PS_OUTPUT PS(VS_OUTPUT input) : SV_Target
{
	PS_OUTPUT o = (PS_OUTPUT)0;
	o.col1 = input.Color;
	o.col2 = o.col1;
	return o;
}