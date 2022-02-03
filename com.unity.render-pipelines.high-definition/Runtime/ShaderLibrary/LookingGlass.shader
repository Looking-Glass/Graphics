Shader "Hidden/HDRP/LookingGlass"
{
    HLSLINCLUDE

        #pragma target 4.5
        #pragma only_renderers d3d11 ps4 xboxone vulkan metal switch
		#pragma enable_d3d11_debug_symbols
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

        TEXTURE2D_ARRAY(_MainTex);
        //SamplerState sampler_LinearClamp;
        SamplerState sampler_PointClamp;

		// LookingGlass
		uniform float pitch;
		uniform float slope;
		uniform float center;
		uniform float subpixelSize;
		uniform float4 tile;
        uniform float viewRangeMin;
        uniform float viewRangeMax;

		struct Attributes
		{
			uint vertexID : SV_VertexID;
		};

		struct Varyings
		{
			float4 positionCS : SV_POSITION;
			float2 texcoord   : TEXCOORD0;
		};

		float4 LookingGlass(Varyings input)
		{
			float2 viewUV = input.texcoord.xy;

			// then sample quilt
			float4 col = float4(0, 0, 0, 1);
			[unroll]
			for (int subpixel = 0; subpixel < 3; subpixel++) {
				// determine view for this subpixel based on pitch, slope, center
				float viewLerp = input.texcoord.x + subpixel * subpixelSize;
				viewLerp += input.texcoord.y * slope;
				viewLerp *= pitch;
				viewLerp -= center;
				// make sure it's positive and between 0-1
				viewLerp = 1.0 - fmod(viewLerp + ceil(abs(viewLerp)), 1.0);
				// translate to quilt coordinates
                float view = floor(viewLerp * tile.z); // multiply by total views
                //float view = floor(viewLerp * 8); // multiply by total views
				//col[subpixel] = UNITY_SAMPLE_TEX2DARRAY(_MainTex, float3(i.uv.xy, view))[subpixel];
				//col[subpixel] = textureName.Sample(_MainTex, float3(viewUV, subpixel));

                if (view >= viewRangeMin && view <= viewRangeMax)
				    col[subpixel] = SAMPLE_TEXTURE2D_ARRAY(_MainTex, sampler_PointClamp, input.texcoord.xy, view - viewRangeMin)[subpixel];
			}
			return col;
		}

        Varyings VertQuad(Attributes input)
        {
            Varyings output;
            output.positionCS = GetQuadVertexPosition(input.vertexID);
            output.positionCS.xy = output.positionCS.xy * float2(2.0f, 2.0f) + float2(-1.0f, -1.0f); //convert to -1..1 (HACK: forced y-flip)
            output.texcoord = GetQuadTexCoord(input.vertexID);
            return output;
        }

		float4 FragBilinear(Varyings input) : SV_Target
		{
			return LookingGlass(input);
        }

    ENDHLSL

    SubShader
    {
        Tags{ "RenderPipeline" = "HDRenderPipeline" }

        Pass
        {
            ZWrite Off ZTest Always Cull Off
            Blend One One

            HLSLPROGRAM
                #pragma vertex VertQuad
                #pragma fragment FragBilinear
            ENDHLSL
        }

    }

    Fallback Off
}
