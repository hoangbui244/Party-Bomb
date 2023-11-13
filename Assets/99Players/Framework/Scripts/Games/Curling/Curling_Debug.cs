using TMPro;
using UnityEngine;
public class Curling_Debug : SingletonCustom<Curling_Debug>
{
	public TextMeshPro throw_CurveDirText;
	public TextMeshPro throw_VelDirText;
	public TextMeshPro throw_PowerText;
	public TextMeshPro inputStickMagText;
	public void Set(Vector3 _throw_CurveDir, Vector3 _throw_VelDir, float _throw_Power, float _inputStickMag)
	{
		throw_CurveDirText.text = "Throw_CurveDir : " + _throw_CurveDir.ToString();
		throw_VelDirText.text = "ThrowDir : " + _throw_VelDir.ToString();
		throw_PowerText.text = "ThrowPower : " + _throw_Power.ToString();
		inputStickMagText.text = "InputStickMag : " + _inputStickMag.ToString();
	}
}
