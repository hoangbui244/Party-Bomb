using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_ArrowObstacleObject : FlyingSquirrelRace_ObstacleObject
{
	private enum Type
	{
		Type_0,
		Type_1
	}
	[SerializeField]
	[Header("種類")]
	private Type type;
	[SerializeField]
	[DisplayName("移動速度")]
	private float moveSpeed;
	[SerializeField]
	[Header("動作可能判定にするクラス")]
	private FlyingSquirrelRace_ObstacleObject_CallAction callAcition;
	private bool isAction;
	[SerializeField]
	[DisplayName("オブジェクトル\u30fcト")]
	private GameObject root;
	[SerializeField]
	[DisplayName("レンダラ\u30fc")]
	private Renderer renderer;
	[SerializeField]
	[DisplayName("マテリアル")]
	private Material[] materials;
	private float stallWaitTime;
	[SerializeField]
	[Header("失速させるまでの時間")]
	private float STALL_WAIT_TIME;
	[SerializeField]
	[Header("失速させる速度")]
	private float stallSpped;
	private float stallDiff;
	protected override void OnInitialize()
	{
		callAcition.Initialize(SetIsAction);
	}
	protected override void OnFixedUpdateMethod(float speed)
	{
		if (!isAction)
		{
			return;
		}
		if (type == Type.Type_1)
		{
			Vector3 localEulerAngles = root.transform.localEulerAngles;
			localEulerAngles.x -= stallDiff;
			if (localEulerAngles.x < 180f && localEulerAngles.x > 45f)
			{
				localEulerAngles.x = 45f;
			}
			root.transform.localEulerAngles = localEulerAngles;
			UnityEngine.Debug.Log("rot x " + localEulerAngles.x.ToString());
		}
		base.transform.localPosition += root.transform.forward * moveSpeed * Time.fixedDeltaTime;
	}
	private void Update()
	{
		if (!isAction || type != Type.Type_1)
		{
			return;
		}
		if (stallWaitTime < STALL_WAIT_TIME)
		{
			stallWaitTime += Time.deltaTime;
			return;
		}
		stallDiff -= stallSpped * Time.deltaTime;
		if (stallDiff > -0.5f)
		{
			stallDiff = -0.5f;
		}
	}
	public void SetIsAction()
	{
		isAction = true;
		callAcition.gameObject.SetActive(value: false);
	}
}
