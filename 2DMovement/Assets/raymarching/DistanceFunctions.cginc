// go to https://iquilezles.org/www/articles/distfunctions/distfunctions.htm for distance functions

// shape data
struct Shape {
		float3 position;
		float3 scale;
		float4 colour;
    float3 rotation;
		int shapeType;
		int operation;
		float blendStrength;
		float roundness;
    // Texture2D text;
};

struct DistanceInfo {
	fixed4 colour;
	float distance;
};

// rotations

// rotation about x axis
float3x3 roll(float a){
  float s = sin(a);
  float c = cos(a);

  float3x3 r = {
    1.,0.,0.,
    0.,c,-s,
    0.,s,c,
  };
  return r;
}

// rotation about y axis
float3x3 pitch(float a){
  float s = sin(a);
  float c = cos(a);

  float3x3 r = {
    c,0.,s,
    0.,1.,0.,
    -s,0.,c,
  };
  return r;
}

// rotation about z axis
float3x3 yaw(float a){
  float s = sin(a);
  float c = cos(a);

  float3x3 r = {
    c,-s,0.,
    s,c,0.,
    0.,0.,1.,
  };
  return r;
}

float3 rotate(float3 a, float3 vec){
  return mul(mul(mul(yaw(a.z),pitch(a.y)),roll(a.x)),vec);
  
}

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

// Torus
float sdTorus( float3 p, float3 t)
{
  float2 q = float2(length(p.xz)-t.x,p.y);
  return length(q)-t.y;
}

// Capsule
float sdVerticalCapsule( float3 p, float h, float r )
{
  p.y -= clamp( p.y, 0.0, h );
  return length( p ) - r;
}

// Cone
float sdCone( float3 p, float2 c, float h )
{
  // c is the sin/cos of the angle, h is height
  // Alternatively pass q instead of (c,h),
  // which is the point at the base in 2D
  float2 q = h*float2(c.x/c.y,-1.0);
    
  float2 w = float2( length(p.xz), p.y );
  float2 a = w - q*clamp( dot(w,q)/dot(q,q), 0.0, 1.0 );
  float2 b = w - q*float2( clamp( w.x/q.x, 0.0, 1.0 ), 1.0 );
  float k = sign( q.y );
  float d = min(dot( a, a ),dot(b, b));
  float s = max( k*(w.x*q.y-w.y*q.x),k*(w.y-q.y)  );
  return sqrt(d)*sign(s);
}

// RoundBox
float sdRoundBox( float3 p, float3 b, float r )
{
  float3 q = abs(p) - b;
  return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0) - r;
}

// Hexagonal Prism
float sdHexPrism( float3 p, float2 h )
{
  const float3 k = float3(-0.8660254, 0.5, 0.57735);
  p = abs(p);
  p.xy -= 2.0*min(dot(k.xy, p.xy), 0.0)*k.xy;
  float2 d = float2(
       length(p.xy-float2(clamp(p.x,-k.z*h.x,k.z*h.x), h.x))*sign(p.y-h.x),
       p.z-h.y );
  return min(max(d.x,d.y),0.0) + length(max(d,0.0));
}

// Triangular Prism
float sdTriPrism( float3 p, float2 h )
{
  float3 q = abs(p);
  return max(q.z-h.y,max(q.x*0.866025+p.y*0.5,-p.y)-h.x*0.5);
}

// Cylinder
float sdCappedCylinder( float3 p, float h, float r )
{
  float2 d = abs(float2(length(p.xz),p.y)) - float2(h,r);
  return min(max(d.x,d.y),0.0) + length(max(d,0.0));
}

// Octahedron
float sdOctahedron( float3 p, float s)
{
  p = abs(p);
  float m = p.x+p.y+p.z-s;
  float3 q;
       if( 3.0*p.x < m ) q = p.xyz;
  else if( 3.0*p.y < m ) q = p.yzx;
  else if( 3.0*p.z < m ) q = p.zxy;
  else return m*0.57735027;
    
  float k = clamp(0.5*(q.z-q.y+s),0.0,s); 
  return length(float3(q.x,q.y-s+k,q.z-k)); 
}

// Pyramid
float sdPyramid( float3 p, float h)
{
  float m2 = h*h + 0.25;
    
  p.xz = abs(p.xz);
  p.xz = (p.z>p.x) ? p.zx : p.xz;
  p.xz -= 0.5;

  float3 q = float3( p.z, h*p.y - 0.5*p.x, h*p.x + 0.5*p.y);
   
  float s = max(-q.x,0.0);
  float t = clamp( (q.y-0.5*p.z)/(m2+0.25), 0.0, 1.0 );
    
  float a = m2*(q.x+s)*(q.x+s) + q.y*q.y;
  float b = m2*(q.x+0.5*t)*(q.x+0.5*t) + (q.y-m2*t)*(q.y-m2*t);
    
  float d2 = min(q.y,-q.x*m2-q.y*0.5) > 0.0 ? 0.0 : min(a,b);
    
  return sqrt( (d2+q.z*q.z)/m2 ) * sign(max(q.z,-p.y));
}

float pythag(float x, float y){
  return sqrt(x*x+y*y);
}

// Wiggle
float sdWiggle(float3 p, float3 b, float r)
{
	return sdBox(p,b) -sin(pythag(p.x,p.z)+_Time.y*2)*r;
}
// // float sdWiggle(float3 p, float3 b, float r)
// {
// 	return sdEllipsoid(p,b) -sin(pythag(p.x,p.z)+_Time.y*2)*r;
// }

// Water
float sdWater(float3 p, float3 b, float r)
{
	return sdBox(p,b) -sin(p.x+p.z+_Time.y*2)*r ;
}

float sdHollowBox(float3 p, float3 d, float thickness){
  float box = sdBox(p,d);
  return abs(box)-thickness;
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
		info.colour = d1.colour;
		return info;
	}
	d1.colour = colour;
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