// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "VertexPaint/Blend4Tex" {
    Properties {
        _Diffuse ("Diffuse", Color) = (0.5,0.5,0.5,1)
        _Texturered ("Texture(red)", 2D) = "white" {}
        _Texturegreen ("Texture(green)", 2D) = "white" {}
        _Textureblue ("Texture(blue)", 2D) = "white" {}
        _Textureblack ("Texture(black)", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Textureblack; uniform float4 _Textureblack_ST;
            uniform sampler2D _Texturered; uniform float4 _Texturered_ST;
            uniform float4 _Diffuse;
            uniform sampler2D _Texturegreen; uniform float4 _Texturegreen_ST;
            uniform sampler2D _Textureblue; uniform float4 _Textureblue_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection =  i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float attenuation = LIGHT_ATTENUATION(i)*2;
                float3 attenColor = attenuation * _LightColor0.xyz;
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor + UNITY_LIGHTMODEL_AMBIENT.rgb*2;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float2 node_361 = i.uv0;
                float4 node_4 = i.vertexColor;
                finalColor += diffuseLight * (lerp(lerp(lerp(tex2D(_Textureblack,TRANSFORM_TEX(node_361.rg, _Textureblack)).rgb,tex2D(_Texturered,TRANSFORM_TEX(node_361.rg, _Texturered)).rgb,node_4.r),tex2D(_Texturegreen,TRANSFORM_TEX(node_361.rg, _Texturegreen)).rgb,node_4.g),tex2D(_Textureblue,TRANSFORM_TEX(node_361.rg, _Textureblue)).rgb,node_4.b)*_Diffuse.rgb);
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Textureblack; uniform float4 _Textureblack_ST;
            uniform sampler2D _Texturered; uniform float4 _Texturered_ST;
            uniform float4 _Diffuse;
            uniform sampler2D _Texturegreen; uniform float4 _Texturegreen_ST;
            uniform sampler2D _Textureblue; uniform float4 _Textureblue_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection =  i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float attenuation = LIGHT_ATTENUATION(i)*2;
                float3 attenColor = attenuation * _LightColor0.xyz;
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float2 node_362 = i.uv0;
                float4 node_4 = i.vertexColor;
                finalColor += diffuseLight * (lerp(lerp(lerp(tex2D(_Textureblack,TRANSFORM_TEX(node_362.rg, _Textureblack)).rgb,tex2D(_Texturered,TRANSFORM_TEX(node_362.rg, _Texturered)).rgb,node_4.r),tex2D(_Texturegreen,TRANSFORM_TEX(node_362.rg, _Texturegreen)).rgb,node_4.g),tex2D(_Textureblue,TRANSFORM_TEX(node_362.rg, _Textureblue)).rgb,node_4.b)*_Diffuse.rgb);
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
