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

Buffer<float4> controlPoints : register(t0);


static const float3 lightPos = float3 (0.0f, 4.0f, -4.0f);
static const float3 ambientColor = float3(0.2f, 0.2f, 0.2f);
static const float3 lightColor = float3(1.0f, 1.0f, 1.0f);
static const float kd = 0.8, ks = 0.5f, m = 100.0f;

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


float4 getControlPoint(int i, int j, int k)
{
	int ind = i * 4 * 4 + j * 4 + k;
	return controlPoints[ind];
}

//--------------------------------------------------------------------------------------
// Vertex Shader
//--------------------------------------------------------------------------------------
VS_OUTPUT VS(VS_INPUT input)
{
	//deformation
	float u = input.Pos.x;
	float v = input.Pos.y;
	float w = input.Pos.z;

	float4 n00 = DeCasteljau(
		getControlPoint(0, 0, 0),
		getControlPoint(1, 0, 0),
		getControlPoint(2, 0, 0),
		getControlPoint(3, 0, 0), u);
	float4 n10 = DeCasteljau(
		getControlPoint(0, 1, 0),
		getControlPoint(1, 1, 0),
		getControlPoint(2, 1, 0),
		getControlPoint(3, 1, 0), u);
	float4 n20 = DeCasteljau(
		getControlPoint(0, 2, 0),
		getControlPoint(1, 2, 0),
		getControlPoint(2, 2, 0),
		getControlPoint(3, 2, 0), u);
	float4 n30 = DeCasteljau(
		getControlPoint(0, 3, 0),
		getControlPoint(1, 3, 0),
		getControlPoint(2, 3, 0),
		getControlPoint(3, 3, 0), u);

	float4 m0 = DeCasteljau(
		n00,
		n10,
		n20,
		n30, v);

	float4 n01 = DeCasteljau(
		getControlPoint(0, 0, 1),
		getControlPoint(1, 0, 1),
		getControlPoint(2, 0, 1),
		getControlPoint(3, 0, 1), u);
	float4 n11 = DeCasteljau(
		getControlPoint(0, 1, 1),
		getControlPoint(1, 1, 1),
		getControlPoint(2, 1, 1),
		getControlPoint(3, 1, 1), u);
	float4 n21 = DeCasteljau(
		getControlPoint(0, 2, 1),
		getControlPoint(1, 2, 1),
		getControlPoint(2, 2, 1),
		getControlPoint(3, 2, 1), u);
	float4 n31 = DeCasteljau(
		getControlPoint(0, 3, 1),
		getControlPoint(1, 3, 1),
		getControlPoint(2, 3, 1),
		getControlPoint(3, 3, 1), u);

	float4 m1 = DeCasteljau(
		n01,
		n11,
		n21,
		n31, v);

	float4 n02 = DeCasteljau(
		getControlPoint(0, 0, 2),
		getControlPoint(1, 0, 2),
		getControlPoint(2, 0, 2),
		getControlPoint(3, 0, 2), u);
	float4 n12 = DeCasteljau(
		getControlPoint(0, 1, 2),
		getControlPoint(1, 1, 2),
		getControlPoint(2, 1, 2),
		getControlPoint(3, 1, 2), u);
	float4 n22 = DeCasteljau( 
		getControlPoint(0, 2, 2),
		getControlPoint(1, 2, 2),
		getControlPoint(2, 2, 2),
		getControlPoint(3, 2, 2), u);
	float4 n32 = DeCasteljau( 
		getControlPoint(0, 3, 2),
		getControlPoint(1, 3, 2),
		getControlPoint(2, 3, 2),
		getControlPoint(3, 3, 2), u);

	float4 m2 = DeCasteljau(
		n02,
		n12,
		n22,
		n32, v);

	float4 n03 = DeCasteljau(
		getControlPoint(0, 0, 3),
		getControlPoint(1, 0, 3),
		getControlPoint(2, 0, 3),
		getControlPoint(3, 0, 3), u);
	float4 n13 = DeCasteljau( 
		getControlPoint(0, 1, 3),
		getControlPoint(1, 1, 3),
		getControlPoint(2, 1, 3),
		getControlPoint(3, 1, 3), u);
	float4 n23 = DeCasteljau( 
		getControlPoint(0, 2, 3),
		getControlPoint(1, 2, 3),
		getControlPoint(2, 2, 3),
		getControlPoint(3, 2, 3), u);
	float4 n33 = DeCasteljau( 
		getControlPoint(0, 3, 3),
		getControlPoint(1, 3, 3),
		getControlPoint(2, 3, 3),
		getControlPoint(3, 3, 3), u);

	float4 m3 = DeCasteljau(
		n03,
		n13,
		n23,
		n33, v);

	float4 p = DeCasteljau(
		m0,
		m1,
		m2,
		m3, w);

	float3 offsetUVW = input.Pos.xyz - 0.005 * normalize(input.Norm.xyz);//float4((p.xyz - float3(0.5f, 0.5f, 0.5f)) * 0.995f + float3(0.5f, 0.5f, 0.5f), 1.0f);

	float ou = offsetUVW.x;
	float ov = offsetUVW.y;
	float ow = offsetUVW.z;

	float4 on00 = DeCasteljau(
		getControlPoint(0, 0, 0),
		getControlPoint(1, 0, 0),
		getControlPoint(2, 0, 0),
		getControlPoint(3, 0, 0), ou);
	float4 on10 = DeCasteljau(
		getControlPoint(0, 1, 0),
		getControlPoint(1, 1, 0),
		getControlPoint(2, 1, 0),
		getControlPoint(3, 1, 0), ou);
	float4 on20 = DeCasteljau(
		getControlPoint(0, 2, 0),
		getControlPoint(1, 2, 0),
		getControlPoint(2, 2, 0),
		getControlPoint(3, 2, 0), ou);
	float4 on30 = DeCasteljau(
		getControlPoint(0, 3, 0),
		getControlPoint(1, 3, 0),
		getControlPoint(2, 3, 0),
		getControlPoint(3, 3, 0), ou);

	float4 om0 = DeCasteljau(
		on00,
		on10,
		on20,
		on30, ov);

	float4 on01 = DeCasteljau(
		getControlPoint(0, 0, 1),
		getControlPoint(1, 0, 1),
		getControlPoint(2, 0, 1),
		getControlPoint(3, 0, 1), ou);
	float4 on11 = DeCasteljau(
		getControlPoint(0, 1, 1),
		getControlPoint(1, 1, 1),
		getControlPoint(2, 1, 1),
		getControlPoint(3, 1, 1), ou);
	float4 on21 = DeCasteljau(
		getControlPoint(0, 2, 1),
		getControlPoint(1, 2, 1),
		getControlPoint(2, 2, 1),
		getControlPoint(3, 2, 1), ou);
	float4 on31 = DeCasteljau(
		getControlPoint(0, 3, 1),
		getControlPoint(1, 3, 1),
		getControlPoint(2, 3, 1),
		getControlPoint(3, 3, 1), ou);

	float4 om1 = DeCasteljau(
		on01,
		on11,
		on21,
		on31, ov);

	float4 on02 = DeCasteljau(
		getControlPoint(0, 0, 2),
		getControlPoint(1, 0, 2),
		getControlPoint(2, 0, 2),
		getControlPoint(3, 0, 2), ou);
	float4 on12 = DeCasteljau(
		getControlPoint(0, 1, 2),
		getControlPoint(1, 1, 2),
		getControlPoint(2, 1, 2),
		getControlPoint(3, 1, 2), ou);
	float4 on22 = DeCasteljau(
		getControlPoint(0, 2, 2),
		getControlPoint(1, 2, 2),
		getControlPoint(2, 2, 2),
		getControlPoint(3, 2, 2), ou);
	float4 on32 = DeCasteljau(
		getControlPoint(0, 3, 2),
		getControlPoint(1, 3, 2),
		getControlPoint(2, 3, 2),
		getControlPoint(3, 3, 2), ou);

	float4 om2 = DeCasteljau(
		on02,
		on12,
		on22,
		on32, ov);

	float4 on03 = DeCasteljau(
		getControlPoint(0, 0, 3),
		getControlPoint(1, 0, 3),
		getControlPoint(2, 0, 3),
		getControlPoint(3, 0, 3), ou);
	float4 on13 = DeCasteljau(
		getControlPoint(0, 1, 3),
		getControlPoint(1, 1, 3),
		getControlPoint(2, 1, 3),
		getControlPoint(3, 1, 3), ou);
	float4 on23 = DeCasteljau(
		getControlPoint(0, 2, 3),
		getControlPoint(1, 2, 3),
		getControlPoint(2, 2, 3),
		getControlPoint(3, 2, 3), ou);
	float4 on33 = DeCasteljau(
		getControlPoint(0, 3, 3),
		getControlPoint(1, 3, 3),
		getControlPoint(2, 3, 3),
		getControlPoint(3, 3, 3), ou);

	float4 om3 = DeCasteljau(
		on03,
		on13,
		on23,
		on33, ov);

	float4 op = DeCasteljau(
		om0,
		om1,
		om2,
		om3, ow);

	float4 n = p - op;

	VS_OUTPUT output = (VS_OUTPUT)0;
	output.WorldPos = p.xyz;
	output.Pos = mul(MVP, p);
	output.Color = Color;
	output.Norm = normalize(n.xyz);
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
PS_OUTPUT PS(VS_OUTPUT input) : SV_Target
{
	float4 dColor = input.Color;

	float3 viewVec = normalize(input.ViewVec);
	float3 normal = normalize(input.Norm);
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