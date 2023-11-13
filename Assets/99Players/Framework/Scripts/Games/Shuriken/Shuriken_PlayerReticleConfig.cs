using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_PlayerReticleConfig : DecoratedMonoBehaviour
{
	[SerializeField]
	private float reticleSpeed = 10f;
	public float ReticleSpeed => reticleSpeed;
}
