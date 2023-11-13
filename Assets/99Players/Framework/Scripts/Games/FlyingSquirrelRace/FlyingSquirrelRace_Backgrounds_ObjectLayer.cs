using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_Backgrounds_ObjectLayer : DecoratedMonoBehaviour
{
	[SerializeField]
	[DisplayName("遅延")]
	private float delay;
	[SerializeField]
	[DisplayName("速度")]
	private float speed = 1f;
	[SerializeField]
	[DisplayName("操作開始後から更新")]
	private bool updateWhenDuringGame;
	private Vector3 origin;
	private float x;
	protected float Speed => speed;
	public virtual void Initialize()
	{
		origin = base.transform.localPosition;
		x = (0f - delay) * speed;
		base.transform.localPosition = new Vector3(origin.x - x, origin.y, origin.z);
	}
	public void UpdateMethod(float speed)
	{
		if (!updateWhenDuringGame || SingletonMonoBehaviour<FlyingSquirrelRace_GameMain>.Instance.GameState == FlyingSquirrelRace_Definition.GameState.DuringGame)
		{
			x += speed * this.speed;
			base.transform.localPosition = new Vector3(origin.x - x, origin.y, origin.z);
		}
	}
}
