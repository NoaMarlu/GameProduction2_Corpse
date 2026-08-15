Shader "Custom/Mask"
{
	SubShader
	{
		Tags{"Queue" = "Geometry-1"}
		ColorMask 0
		Stencil
		{
			Ref 1
			Comp Always
			Pass Replace
		}
		Pass{}
	}	
}