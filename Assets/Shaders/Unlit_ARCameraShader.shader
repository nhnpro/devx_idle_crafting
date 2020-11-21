Shader "Unlit/ARCameraShader"
{
  Properties
  {
    _textureY ("TextureY", 2D) = "white" {}
    _textureCbCr ("TextureCbCr", 2D) = "black" {}
  }
  SubShader
  {
    Tags
    { 
      "RenderType" = "Opaque"
    }
    LOD 100
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "RenderType" = "Opaque"
      }
      LOD 100
      ZWrite Off
      Cull Off
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      #define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixVP;
      uniform float4x4 _DisplayTransform;
      uniform sampler2D _textureY;
      uniform sampler2D _textureCbCr;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float2 tmpvar_1;
          float4 tmpvar_2;
          tmpvar_2.w = 1;
          tmpvar_2.xyz = float3(in_v.vertex.xyz);
          tmpvar_1.x = (((conv_mxt4x4_0(_DisplayTransform).x * in_v.texcoord.x) + (conv_mxt4x4_0(_DisplayTransform).y * in_v.texcoord.y)) + conv_mxt4x4_0(_DisplayTransform).z).x;
          tmpvar_1.y = (((conv_mxt4x4_1(_DisplayTransform).x * in_v.texcoord.x) + (conv_mxt4x4_1(_DisplayTransform).y * in_v.texcoord.y)) + conv_mxt4x4_1(_DisplayTransform).z).x;
          out_v.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_2));
          out_v.xlv_TEXCOORD0 = tmpvar_1;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          float y_2;
          float tmpvar_3;
          tmpvar_3 = tex2D(_textureY, in_f.xlv_TEXCOORD0).x;
          y_2 = tmpvar_3;
          float4 tmpvar_4;
          tmpvar_4 = tex2D(_textureCbCr, in_f.xlv_TEXCOORD0);
          float4 tmpvar_5;
          tmpvar_5.w = 1;
          tmpvar_5.x = y_2;
          tmpvar_5.yz = tmpvar_4.xy;
          float4x4 tmpvar_6;
          conv_mxt4x4_0(tmpvar_6).x = 1;
          conv_mxt4x4_0(tmpvar_6).y = 1;
          conv_mxt4x4_0(tmpvar_6).z = 1;
          conv_mxt4x4_0(tmpvar_6).w = 0;
          conv_mxt4x4_1(tmpvar_6).x = 0;
          conv_mxt4x4_1(tmpvar_6).y = (-0.3441);
          conv_mxt4x4_1(tmpvar_6).z = 1.772;
          conv_mxt4x4_1(tmpvar_6).w = 0;
          conv_mxt4x4_2(tmpvar_6).x = 1.402;
          conv_mxt4x4_2(tmpvar_6).y = (-0.7141);
          conv_mxt4x4_2(tmpvar_6).z = 0;
          conv_mxt4x4_2(tmpvar_6).w = 0;
          conv_mxt4x4_3(tmpvar_6).x = (-0.701);
          conv_mxt4x4_3(tmpvar_6).y = 0.5291;
          conv_mxt4x4_3(tmpvar_6).z = (-0.886);
          conv_mxt4x4_3(tmpvar_6).w = 1;
          tmpvar_1 = mul(tmpvar_6, tmpvar_5);
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
