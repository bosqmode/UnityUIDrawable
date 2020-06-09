Shader "CustomRenderTexture/UIDrawableShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
	}

	SubShader
	{
	   Lighting Off
	   Blend One Zero

	   Pass
	   {
		   CGPROGRAM
		   #include "UnityCustomRenderTexture.cginc"
		   #pragma vertex CustomRenderTextureVertexShader
		   #pragma fragment frag
			#pragma target 3.0

		   float4      _Color;
		   sampler2D   _Tex;
		   fixed4 _startpos;
		   fixed4 _endpos;
		   float _thickness;
		   float _flow;
		   float _heightRatio;

		   // Distance from a point p to a line a-b, with smoothstepping acting as the brush's width
		   float DistanceToLine(float2 p, float2 a, float2 b) {
			   float thickness = _thickness / 100.0;

			   float2 pa = p - a;
			   float2 ba = b - a;

			   ba.y *= _heightRatio;
			   pa.y *= _heightRatio;

			   float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
			   float d = length(pa - ba * h);

			   return smoothstep(0.0, thickness, d);
		   }

		   float4 frag(v2f_customrendertexture IN) : COLOR
		   {
			   float4 col = tex2D(_Tex, IN.localTexcoord.xy);
			   float4 newcol = _Color;

			   // get the raw smoothstepped distance of the line at this pixel
			   float distance = 1 - DistanceToLine(IN.localTexcoord.xy, _startpos.xy, _endpos.xy);

			   // multiply by the flow/strength
			   distance = clamp(distance * _flow, 0.0, 1.0);
			   newcol *= distance;

			   // overlay with last frame's pixel
			   return (1.0f - distance) * col + newcol;
		   }
		   ENDCG
		}
	}
}