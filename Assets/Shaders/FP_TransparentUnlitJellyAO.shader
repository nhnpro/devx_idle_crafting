Shader "FP/TransparentUnlitJellyAO"
{
  Properties
  {
    _MainTex ("Albedo (RGB) Trans (A)", 2D) = "white" {}
    _Amplitude ("Amplitude", Range(0, 1)) = 0
  }
  SubShader
  {
    Tags
    { 
      "IGNOREPROJECTOR" = "true"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
    }
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
        "SHADOWSUPPORT" = "true"
      }
      ZWrite Off
      Offset -1, -1
      Blend SrcAlpha OneMinusSrcAlpha
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
      //uniform float4x4 unity_MatrixVP;
      uniform float4 _MainTex_ST;
      uniform float _Amplitude;
      uniform sampler2D _MainTex;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 color :COLOR;
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
          float3 darkside_2;
          float3x3 tmpvar_3;
          tmpvar_3[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
          tmpvar_3[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
          tmpvar_3[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
          float3 tmpvar_4;
          tmpvar_4 = float3(max(0, sign((-dot(normalize(mul(in_v.normal, tmpvar_3)), _WorldSpaceLightPos0.xyz)))));
          darkside_2 = tmpvar_4;
          tmpvar_1.xyz = float3(((in_v.color.xyz * 0.25) + 0.75));
          float amp_5;
          float4 tmpvar_6;
          tmpvar_6 = mul(unity_ObjectToWorld, in_v.vertex);
          amp_5 = (_Amplitude * clamp((tmpvar_6.y * 5), 0, 1));
          float tmpvar_7;
          tmpvar_7 = (frac((sin(dot(tmpvar_6.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.55)) * 3.14159);
          float4 tmpvar_8;
          tmpvar_8.zw = float2(0, 0);
          tmpvar_8.x = (((sin(((_Time.y * 3.11) + tmpvar_7)) * sin(((_Time.y * 4.11) + tmpvar_7))) * 0.2) * amp_5);
          tmpvar_8.y = ((sin(((_Time.y * 6.11) + tmpvar_7)) * 0.2) * amp_5);
          float4 tmpvar_9;
          tmpvar_9.w = 1;
          tmpvar_9.xyz = (in_v.vertex + tmpvar_8).xyz.xyz;
          out_v.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_9));
          out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          out_v.xlv_COLOR0 = tmpvar_1;
          out_v.xlv_COLOR1 = darkside_2;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 col_1;
          float4 tmpvar_2;
          tmpvar_2 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
          col_1.w = tmpvar_2.w;
          col_1.xyz = float3((tmpvar_2.xyz * min(float4(max(1, in_f.xlv_COLOR1.x)), in_f.xlv_COLOR0).x));
          col_1.w = col_1.w;
          float x_3;
          x_3 = (col_1.w - 0.1);
          if((x_3<0))
          {
              discard;
          }
          out_f.color = col_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 2, name: 
    {
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "LIGHTMODE" = "SHADOWCASTER"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
        "SHADOWSUPPORT" = "true"
      }
      ZWrite Off
      Offset -1, -1
      Blend SrcAlpha OneMinusSrcAlpha
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
          float2 xlv_TEXCOORD1 :TEXCOORD1;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 xlv_TEXCOORD1 :TEXCOORD1;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float amp_1;
          float4 tmpvar_2;
          tmpvar_2 = mul(unity_ObjectToWorld, in_v.vertex);
          amp_1 = (_Amplitude * clamp((tmpvar_2.y * 5), 0, 1));
          float tmpvar_3;
          tmpvar_3 = (frac((sin(dot(tmpvar_2.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.55)) * 3.14159);
          float4 tmpvar_4;
          tmpvar_4.zw = float2(0, 0);
          tmpvar_4.x = (((sin(((_Time.y * 3.11) + tmpvar_3)) * sin(((_Time.y * 4.11) + tmpvar_3))) * 0.2) * amp_1);
          tmpvar_4.y = ((sin(((_Time.y * 6.11) + tmpvar_3)) * 0.2) * amp_1);
          float4 tmpvar_5;
          float4 wPos_6;
          float4 tmpvar_7;
          tmpvar_7 = mul(unity_ObjectToWorld, (in_v.vertex + tmpvar_4));
          wPos_6 = tmpvar_7;
          if((unity_LightShadowBias.z!=0))
          {
              float3x3 tmpvar_8;
              tmpvar_8[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
              tmpvar_8[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
              tmpvar_8[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
              float3 tmpvar_9;
              tmpvar_9 = normalize(mul(in_v.normal, tmpvar_8));
              float tmpvar_10;
              tmpvar_10 = dot(tmpvar_9, normalize((_WorldSpaceLightPos0.xyz - (tmpvar_7.xyz * _WorldSpaceLightPos0.w))));
              wPos_6.xyz = float3((tmpvar_7.xyz - (tmpvar_9 * (unity_LightShadowBias.z * sqrt((1 - (tmpvar_10 * tmpvar_10)))))));
          }
          tmpvar_5 = mul(unity_MatrixVP, wPos_6);
          float4 clipPos_11;
          clipPos_11.xyw = tmpvar_5.xyw;
          clipPos_11.z = (tmpvar_5.z + clamp((unity_LightShadowBias.x / tmpvar_5.w), 0, 1));
          clipPos_11.z = lerp(clipPos_11.z, max(clipPos_11.z, (-tmpvar_5.w)), unity_LightShadowBias.y);
          out_v.vertex = clipPos_11;
          out_v.xlv_TEXCOORD1 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float x_1;
          x_1 = (tex2D(_MainTex, in_f.xlv_TEXCOORD1).w - 0.1).x;
          if((x_1<0))
          {
              discard;
          }
          out_f.color = float4(0, 0, 0, 0);
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
