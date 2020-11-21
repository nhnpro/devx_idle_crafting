Shader "FP/UnlitJellyFloorAO"
{
  Properties
  {
    _MainTex ("Albedo (RGB) Trans (A)", 2D) = "white" {}
    _Amplitude ("Amplitude", Range(0, 1)) = 0.5
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
          float tmpvar_5;
          tmpvar_5 = (frac((sin(dot(mul(unity_ObjectToWorld, in_v.vertex).xyz, float3(12.9898, 78.233, 45.5432))) * 43758.55)) * 3.14159);
          float4 tmpvar_6;
          tmpvar_6.zw = float2(0, 0);
          tmpvar_6.x = (((sin(((_Time.y * 3.11) + tmpvar_5)) * sin(((_Time.y * 4.11) + tmpvar_5))) * 0.2) * _Amplitude);
          tmpvar_6.y = ((sin(((_Time.y * 6.11) + tmpvar_5)) * 0.2) * _Amplitude);
          float4 tmpvar_7;
          tmpvar_7.w = 1;
          tmpvar_7.xyz = (in_v.vertex + tmpvar_6).xyz.xyz;
          out_v.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_7));
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
  }
  FallBack Off
}
