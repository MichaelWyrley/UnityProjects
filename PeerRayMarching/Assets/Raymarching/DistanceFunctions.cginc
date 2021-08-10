// go to https://iquilezles.org/www/articles/distfunctions/distfunctions.htm for distance functions

// shape data
struct Shape {
		float3 position;
		float3 scale;
		float4 colour;
		int shapeType;
		int operation;
		float blendStrength;
};

struct DistanceInfo {
	fixed4 colour;
	float distance;
};

// Sphere
// s: radius
float sdEllipsoid( float3 p, float3 r )
{
  float k0 = length(p/r);
  float k1 = length(p/(r*r));
  return k0*(k0-1.0)/k1;
}

// Box
// b: size of box in x/y/z
float sdBox(float3 p, float3 b)
{
	float3 d = abs(p) - b;
	return min(max(d.x, max(d.y, d.z)), 0.0) +
		length(max(d, 0.0));
}

float sdTorus( float3 p, float3 t )
{
  float2 q = float2(length(p.xz)-t.x,p.y);
  return length(q)-t.y;
}


// BOOLEAN OPERATORS //

// Union
DistanceInfo opU(DistanceInfo d1, float d2, float4 colour)
{
	if(d1.distance > d2){
		DistanceInfo info;
		info.distance = d2;
		info.colour = colour;
		return info;
	}
	return d1;
}

// Subtraction
DistanceInfo opS(DistanceInfo d1, float d2, float4 colour)
{
	if(d1.distance < -d2){
		DistanceInfo info;
		info.distance = -d2;
		info.colour = colour;
		return info;
	}
	return d1;
}

// Intersection
DistanceInfo opI(DistanceInfo d1, float d2, float4 colour)
{
	if(d1.distance < d2){
		DistanceInfo info;
		info.distance = d2;
		info.colour = colour;
		return info;
	}
	return d1;
}

// Blend
DistanceInfo opB( float a, float b, float4 colA, float4 colB, float k )
{
    float h = clamp( 0.5+0.5*(b-a)/k, 0.0, 1.0 );
    float blendDst = lerp( b, a, h ) - k*h*(1.0-h);
    float4 blendCol = lerp(colB,colA,h);
		DistanceInfo info;
		info.distance = blendDst;
		info.colour = blendCol;
    return info;
}