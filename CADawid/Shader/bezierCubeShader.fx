#define INPUT_PATCH_SIZE 16
#define OUTPUT_PATCH_SIZE 16

cbuffer ConstantBuffer : register(b0)
{
	matrix MVP;
	float4 Color;
}

cbuffer ViewCB : register(b1)
{
	matrix invView;
}

struct HS_INPUT
{
	float4 LocalPos : POSITION;
};

struct HS_PatchOutput
{
	float edges[4] : SV_TessFactor;
	float inside[2] : SV_InsideTessFactor;
};

struct DS_ControlPoint
{
	float4 pos : POSITION;
};

struct PSInput
{
	float4 pos : SV_POSITION;
	float3 worldPos : POS;
	float4 Color : COLOR0;
	float3 norm : NORMAL0;
	float3 bnorm : NORMAL1;
	float3 tangent : NORMAL2;
};

//--------------------------------------------------------------------------------------
// Vertex Shader
//--------------------------------------------------------------------------------------
HS_INPUT VS(float4 Pos : POSITION)
{
	HS_INPUT output = (HS_INPUT)0;
	output.LocalPos = Pos;
	return output;
}

// --------------------------------------------------------------------------------------
// Hull Shader
//--------------------------------------------------------------------------------------
HS_PatchOutput HS_Patch(InputPatch<HS_INPUT, INPUT_PATCH_SIZE> ip, uint patchId : SV_PrimitiveID)
{
	HS_PatchOutput o;
	o.edges[0] = 10;
	o.edges[1] = 10;
	o.edges[2] = 10;
	o.edges[3] = 10;
	o.inside[0] = 10;
	o.inside[1] = 10;
	return o;
}

[domain("quad")]
[partitioning("integer")]
[outputtopology("triangle_cw")]
[outputcontrolpoints(OUTPUT_PATCH_SIZE)]
[patchconstantfunc("HS_Patch")]
DS_ControlPoint HS(InputPatch<HS_INPUT, INPUT_PATCH_SIZE> ip, uint i : SV_OutputControlPointID,
	uint patchID : SV_PrimitiveID)
{
	DS_ControlPoint o;
	o.pos = ip[i].LocalPos;
	return o;
}


// --------------------------------------------------------------------------------------
// DOMAIN Shader
//--------------------------------------------------------------------------------------
float4 Lerp(float4 p0, float4 p1, float t)
{
	return (1 - t) * p0 + t * p1;
}


float4 DeCasteljau(float4 p0, float4 p1, float4 p2, float4 p3, float t)
{
	float4 q0 = Lerp(p0, p1, t);
	float4 q1 = Lerp(p1, p2, t);
	float4 q2 = Lerp(p2, p3, t);

	float4 r0 = Lerp(q0, q1, t);
	float4 r1 = Lerp(q1, q2, t);

	float4 p = Lerp(r0, r1, t);
	return p;
}

float4 DeCasteljauDerivative(float4 p0, float4 p1, float4 p2, float4 p3, float t)
{
	float4 pp0 = (p1 - p0) * 3;
	float4 pp1 = (p2 - p1) * 3;
	float4 pp2 = (p3 - p2) * 3;

	float4 q0 = Lerp(pp0, pp1, t);
	float4 q1 = Lerp(pp1, pp2, t);

	float4 pp = Lerp(q0, q1, t);
	return pp;
}

[domain("quad")]
PSInput DS(HS_PatchOutput factors, float2 uv : SV_DomainLocation,
	const OutputPatch<DS_ControlPoint, OUTPUT_PATCH_SIZE> input)
{
	float4 n0 = DeCasteljau(
		input[0].pos,
		input[1].pos,
		input[2].pos,
		input[3].pos, uv.x);
	float4 n1 = DeCasteljau(
		input[4].pos,
		input[5].pos,
		input[6].pos,
		input[7].pos, uv.x);
	float4 n2 = DeCasteljau(
		input[8].pos,
		input[9].pos,
		input[10].pos,
		input[11].pos, uv.x);
	float4 n3 = DeCasteljau(
		input[12].pos,
		input[13].pos,
		input[14].pos,
		input[15].pos, uv.x);

	float4 nn0 = DeCasteljau(
		input[0].pos,
		input[4].pos,
		input[8].pos,
		input[12].pos, uv.y);
	float4 nn1 = DeCasteljau(
		input[1].pos,
		input[5].pos,
		input[9].pos,
		input[13].pos, uv.y);
	float4 nn2 = DeCasteljau(
		input[2].pos,
		input[6].pos,
		input[10].pos,
		input[14].pos, uv.y);
	float4 nn3 = DeCasteljau(
		input[3].pos,
		input[7].pos,
		input[11].pos,
		input[15].pos, uv.y);


	float4 p = DeCasteljau(n0, n1, n2, n3, uv.y);

	float4 tangent1 = DeCasteljauDerivative(n0, n1, n2, n3, uv.y);
	float4 tangent2 = DeCasteljauDerivative(nn0, nn1, nn2, nn3, uv.x);

	PSInput o;
	o.worldPos = p.xyz;
	o.pos = mul(MVP, p);
	o.norm = normalize(cross(tangent2.xyz, tangent1.xyz));
	o.tangent = normalize(tangent1.rgb);
	o.bnorm = normalize(tangent2.rgb);
	o.Color = Color;

	return o;
}


struct PS_OUTPUT {
	float4 col1 : SV_Target0;
	float4 col2 : SV_Target1;
};

//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------
static const float3 ambientColor = float3(0.2f, 0.2f, 0.2f);
static const float3 lightColor = float3(1.0f, 1.0f, 1.0f);
static const float3 lightPos = float3 (0.0f, 4.0f, -4.0f);
static const float kd = 1, ks = 0.5f, m = 100.0f;

PS_OUTPUT PS(PSInput i) : SV_TARGET
{
	float4 CameraPos = mul(invView, float4(0,0,0,1));
	float3 viewVec = normalize(CameraPos.xyz - i.worldPos);
	float3 normal = -normalize(i.norm);
	float4 dColor = i.Color;

	float3 color = dColor.rgb * ambientColor;
	
	float3 lightPosition = lightPos;
	float3 lightVec = normalize(lightPosition - i.worldPos);
	float3 halfVec = normalize(viewVec + lightVec);
	float tmp = saturate(dot(normal, lightVec));
	color += lightColor * dColor.xyz * kd * tmp; 
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