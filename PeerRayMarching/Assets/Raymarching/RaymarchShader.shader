Shader "Michael/RaymarchShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"
            #include "DistanceFunctions.cginc"

            sampler2D _MainTex;
            uniform float4x4 _CamFrustum, _CamToWorldMatrix;
            uniform float _maxDistance;
            uniform float4 _sphere1, _box1;
            uniform float3 _lightDirection;
            uniform float _transparency;
            uniform sampler2D _CameraDepthTexture;
            uniform fixed4 _mainColour;
            uniform float3 _modInterval; 

            uniform StructuredBuffer<Shape> shapes;
            uniform int _noShapes;

            #define MAX_STEPS 100
            #define SURF_DIST 0.01

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 ray : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                half index = v.vertex.z;
                v.vertex.z = 0;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                o.ray = _CamFrustum[(int) index].xyz;
                o.ray /= abs(o.ray.z); // normalise in z direction
                
                o.ray = mul(_CamToWorldMatrix, o.ray);

                return o;
            }

            float getDistance(float3 position, Shape s){
                switch (s.shapeType){
                    case 0:
                        return sdEllipsoid(position - s.position, s.scale);
                    case 1:
                        return sdBox(position - s.position, s.scale);
                    case 2:
                        return sdTorus(position - s.position, s.scale);
                    default:
                        return sdEllipsoid(position - s.position, s.scale);
                }
                
            }

            DistanceInfo distanceField(float3 position) {
                DistanceInfo current;
                current.distance = getDistance(position, shapes[0]);
                current.colour = shapes[0].colour;
                // loop through all the shapes inorder to calculate correct distance function
                for(int i = 1; i < _noShapes; i++){
                    switch(shapes[i].operation){
                        case 0:
                            current = opU(current, getDistance(position, shapes[i]), shapes[i].colour);
                            break;
                        case 1:
                            current = opB(current.distance, getDistance(position, shapes[i]),
                             current.colour, shapes[i].colour, shapes[i].blendStrength);
                            break;
                        case 2: 
                            current = opS(current, getDistance(position, shapes[i]), shapes[i].colour);
                            break;
                        case 3:
                            current = opI(current, getDistance(position, shapes[i]), shapes[i].colour);
                            break;
                    }
                }

                return current;
            }

            float3 getNormal(float3 position){
                const float2 offset = float2 (0.001, 0);
                float3 normal = float3 (
                    distanceField(position + offset.xyy).distance - distanceField(position - offset.xyy).distance, // we just want the x position in the offcet so xyy will produce (0.001, 0, 0)
                    distanceField(position + offset.yxy).distance - distanceField(position - offset.yxy).distance, // we just want the x position in the offcet so yxy will produce (0, 0.001, 0)
                    distanceField(position + offset.yyx).distance - distanceField(position - offset.yyx).distance  // we just want the x position in the offcet so yyx will produce (0, 0, 0.001)
                );
                return normalize(normal);
            }

            fixed4 raymarching(float3 rayOrigin, float3 rayDirection, float depth) {
                float distanceTraveled = 0; // distance traveled allong the ray direction

                for(int i = 0; i < MAX_STEPS; i++){
                    if (distanceTraveled > _maxDistance || distanceTraveled >= depth){
                        // Environment
                        return fixed4(rayDirection,0);
                    }
                    float3 position = rayOrigin + rayDirection * distanceTraveled;
                    // check for hit in distance field
                    DistanceInfo distanceInfo = distanceField(position);
                    if (distanceInfo.distance < SURF_DIST){ // we have hit something
                        // shading
                        float3 n = getNormal(position);
                        float light = dot(-_lightDirection, n);
                        return float4(distanceInfo.colour.xyz * light, distanceInfo.colour.w); // set colour
                    }
                    distanceTraveled += distanceInfo.distance;
                }

                return fixed4(1,1,1,1); // we didn't hit anything
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float depth = LinearEyeDepth(tex2D(_CameraDepthTexture, i.uv).r);
                depth *= length(i.ray);

                fixed3 col = tex2D(_MainTex, i.uv);
                float3 rayDirection = normalize(i.ray.xyz);
                float3 rayOrigin = _WorldSpaceCameraPos;
                fixed4 result = raymarching(rayOrigin, rayDirection, depth);
                return fixed4(col * (1.0 - result.w) + result.xyz * result.w, 1.0); 
            }
            ENDCG
        }
    }
}
