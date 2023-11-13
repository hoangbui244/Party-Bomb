using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_Distance : DecoratedMonoBehaviour
{
	[SerializeField]
	[DisplayName("距離の表示")]
	private SpriteNumbers distance;
	public void Initialize()
	{
	}
	public void UpdateDistance(int remaining)
	{
		distance.Set(remaining);
	}
	public void HideDistance()
	{
		base.gameObject.SetActive(value: false);
	}
}
