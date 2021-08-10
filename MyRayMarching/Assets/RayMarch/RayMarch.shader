Shader "Unlit/RayMarch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define MAX_STEPS 100
            #define MAX_DIST 1000
            #define SURF_DIST 0.001

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 ro : TEXCOORD1;
                float3 hitPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.ro = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos,1));
                o.hitPos = v.vertex;
                
                return o;
            }

            float GetDist(float3 p){
                float d = length(p) - .5; // distance to a sphere at the origin with a radius of 0.5
                d = length(float2 (length(p.xz) - .5, p.y)) - 0.1;
                return d;
            }

            // takes in ray origin and ray direction and returns the total distance
            float Raymarch(float3 ro, float3 rd) {
                float dO = 0;   // total distance from camera
                float dS;        // distance from serface
                for(int i = 0; i < MAX_STEPS; i++){
                    float3 p = ro + dO * rd;    // calculate the ray marching position
                    dS = GetDist(p);            // get the smallest distance to all the objects (for marching)
                    dO += dS;                   // increment total distance 
                    if(dS < SURF_DIST || dO > MAX_DIST)  // if we have collided with the object or we have reached a maximum distance
                        break;  
                }
                return dO;
            }

            float3 GetNormal(float3 p) {
                float2 e = float2(1e-2,0);
                float3 n = GetDist(p) - float3(
                    GetDist(p-e.xyy),
                    GetDist(p-e.yxy),
                    GetDist(p-e.yyx)
                );
                return normalize(n);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // set origin of texture to middle
                float2 uv = i.uv-.5;

                // ray origin (camera location)
                float3 ro = i.ro;

                // ray direction
                float3 rd = normalize(i.hitPos-ro);

                float d = Raymarch(ro,rd); // get the distance;
                
                // sample the texture
                fixed4 col = 0;

                if(d < MAX_DIST) { // hit surface
                    float3 p = ro + rd * d;
                    float3 n = GetNormal(p);
                    col.rgb = n;
                } else 
                    discard;
                
                return col;
            }
            ENDCG
        }
    }
}
