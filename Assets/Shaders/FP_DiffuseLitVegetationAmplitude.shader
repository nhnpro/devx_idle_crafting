Shader "FP/DiffuseLitVegetationAmplitude"
{
  Properties
  {
    _MainTex ("Albedo (RGB)", 2D) = "white" {}
    _Amplitude ("Amplitude", Range(0, 1)) = 0
  }
  SubShader
  {
    Tags
    { 
    }
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "LIGHTMODE" = "FORWARDBASE"
        "SHADOWSUPPORT" = "true"
      }
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile DIRECTIONAL
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4 _Time;
      //uniform float4 _WorldSpaceLightPos0;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4 glstate_lightmodel_ambient;
      //uniform float4x4 unity_MatrixVP;
      uniform float4 _LightColor0;
      uniform float4 _MainTex_ST;
      uniform float _Amplitude;
      uniform sampler2D _MainTex;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_COLOR0 :COLOR0;
          float3 xlv_COLOR1 :COLOR1;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_COLOR0 :COLOR0;
          float3 xlv_COLOR1 :COLOR1;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float4 tmpvar_1;
          tmpvar_1 = mul(unity_ObjectToWorld, in_v.vertex);
          float4 tmpvar_2;
          tmpvar_2.yw = float2(0, 0);
          tmpvar_2.x = (((sin(((_Time.y * 13.7) + tmpvar_1.z)) * 0.1) * _Amplitude) * tmpvar_1.y);
          tmpvar_2.z = (((((sin(((_Time.y * 9.7) + tmpvar_1.z)) * 0.1) * _Amplitude) + (sin(((_Time.y * 3.1) + tmpvar_1.x)) * 0.04)) + (sin(((_Time.y * 1.9) + tmpvar_1.z)) * (0.1 + (sin((_Time.y * 0.5)) * 0.05)))) * tmpvar_1.y);
          float4 diffuse_3;
          float3 darkside_4;
          float3x3 tmpvar_5;
          tmpvar_5[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
          tmpvar_5[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
          tmpvar_5[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
          float tmpvar_6;
          tmpvar_6 = dot(normalize(mul(in_v.normal, tmpvar_5)), _WorldSpaceLightPos0.xyz);
          float4 tmpvar_7;
          float4 x_8;
          x_8 = (glstate_lightmodel_ambient * 2);
          tmpvar_7 = lerp(x_8, _LightColor0, float4(clamp(((tmpvar_6 * 2) + 0.5), 0, 1)));
          diffuse_3 = tmpvar_7;
          float3 tmpvar_9;
          tmpvar_9 = float3(max(0, sign((-tmpvar_6))));
          darkside_4 = tmpvar_9;
          float4 tmpvar_10;
          tmpvar_10.w = 1;
          tmpvar_10.xyz = (in_v.vertex + tmpvar_2).xyz.xyz;
          out_v.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_10));
          out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          out_v.xlv_COLOR0 = diffuse_3;
          out_v.xlv_COLOR1 = darkside_4;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 col_1;
          col_1.xyz = tex2D(_MainTex, in_f.xlv_TEXCOORD0).xyz.xyz;
          col_1.w = 1;
          float4 tmpvar_2;
          tmpvar_2 = ((col_1 * in_f.xlv_COLOR0) * max(1, in_f.xlv_COLOR1.x));
          out_f.color = tmpvar_2;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 2, name: 
    {
      Tags
      { 
        "LIGHTMODE" = "SHADOWCASTER"
        "SHADOWSUPPORT" = "true"
      }
      // m_ProgramMask = 6
      CGPROGRAM
      #pragma multi_compile SHADOWS_DEPTH
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4 _Time;
      //uniform float4 _WorldSpaceLightPos0;
      //uniform float4 unity_LightShadowBias;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4x4 unity_MatrixVP;
      uniform float _Amplitude;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float3 normal :NORMAL;
      };
      
      struct OUT_Data_Vert
      {
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 vertex :Position;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float4 tmpvar_1;
          tmpvar_1 = mul(unity_ObjectToWorld, in_v.vertex);
          float4 tmpvar_2;
          tmpvar_2.yw = float2(0, 0);
          tmpvar_2.x = (((sin(((_Time.y * 13.7) + tmpvar_1.z)) * 0.1) * _Amplitude) * tmpvar_1.y);
          tmpvar_2.z = (((((sin(((_Time.y * 9.7) + tmpvar_1.z)) * 0.1) * _Amplitude) + (sin(((_Time.y * 3.1) + tmpvar_1.x)) * 0.04)) + (sin(((_Time.y * 1.9) + tmpvar_1.z)) * (0.1 + (sin((_Time.y * 0.5)) * 0.05)))) * tmpvar_1.y);
          float4 tmpvar_3;
          float4 wPos_4;
          float4 tmpvar_5;
          tmpvar_5 = mul(unity_ObjectToWorld, (in_v.vertex + tmpvar_2));
          wPos_4 = tmpvar_5;
          if((unity_LightShadowBias.z!=0))
          {
              float3x3 tmpvar_6;
              tmpvar_6[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
              tmpvar_6[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
              tmpvar_6[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
              float3 tmpvar_7;
              tmpvar_7 = normalize(mul(in_v.normal, tmpvar_6));
              float tmpvar_8;
              tmpvar_8 = dot(tmpvar_7, normalize((_WorldSpaceLightPos0.xyz - (tmpvar_5.xyz * _WorldSpaceLightPos0.w))));
              wPos_4.xyz = float3((tmpvar_5.xyz - (tmpvar_7 * (unity_LightShadowBias.z * sqrt((1 - (tmpvar_8 * tmpvar_8)))))));
          }
          tmpvar_3 = mul(unity_MatrixVP, wPos_4);
          float4 clipPos_9;
          clipPos_9.xyw = tmpvar_3.xyw;
          clipPos_9.z = (tmpvar_3.z + clamp((unity_LightShadowBias.x / tmpvar_3.w), 0, 1));
          clipPos_9.z = lerp(clipPos_9.z, max(clipPos_9.z, (-tmpvar_3.w)), unity_LightShadowBias.y);
          out_v.vertex = clipPos_9;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          out_f.color = float4(0, 0, 0, 0);
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
