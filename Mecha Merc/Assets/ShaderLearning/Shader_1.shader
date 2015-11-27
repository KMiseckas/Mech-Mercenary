Shader "Learning/Shader_ColorOnly"
{
	Properties
	{
		_Color("Color", Color) = (1.0,1.0,1.0,1.0)
	}

		SubShader
	{
		Pass
		{
			CGPROGRAM

			//Pragmas
			#pragma vertex vert
			#pragma fragment frag

			//Defined variables
			uniform float4 _Color;

			//Base input structs
			struct vertexInput
			{
				float4 vertex : POSITION;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
			};

			//Vertex function
			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				return o;
			}

			float4 frag(vertexOutput i) : COLOR
			{
				return _Color;
			}

			ENDCG
		}
	}
}
