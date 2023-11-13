using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_ReticleColorConfigs : DecoratedScriptableObject
{
	[SerializeField]
	private Color[] colors;
	public Color GetColor(int playerNo)
	{
		return colors[playerNo];
	}
}
