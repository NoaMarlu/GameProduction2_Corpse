Shader "Custom/Masked"
{
	Properties{_Color ("Color",Color)=(0,0,0,0.85)}
	SubShader
	{
		Tags{"Queue" = "Geometry" "RenderType" = "Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha
		Stencil
		{
			Ref 1
			Comp NotEqual
		}
		Pass
		{
			Color [_Color]	
		}
	}
}