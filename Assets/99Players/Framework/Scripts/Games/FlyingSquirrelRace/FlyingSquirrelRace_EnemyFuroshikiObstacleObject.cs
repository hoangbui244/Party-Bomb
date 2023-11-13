using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_EnemyFuroshikiObstacleObject : FlyingSquirrelRace_ObstacleObject
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
	[Header("ル\u30fcト")]
	private GameObject root;
	[SerializeField]
	[DisplayName("忍者キャラパ\u30fcツ")]
	private FlyingSquirrelRace_EnemyNinjaCharacterParts enemyNinjaCharacterParts;
	[SerializeField]
	[DisplayName("忍者キャラのマテリアル")]
	private Material[] materials;
	[SerializeField]
	[Header("風呂敷")]
	private SkinnedMeshRenderer furoshiki;
	[SerializeField]
	[DisplayName("風呂敷のマテリアル")]
	private Material[] mat_Furoshiki;
	private const int CapeBlendShapeIndex_2 = 1;
	[SerializeField]
	[Header("最小値 : blendShape_2")]
	private float MinBlendShape_2;
	[SerializeField]
	[Header("最大値 : blendShape_2")]
	private float MaxBlendShape_2 = 50f;
	[SerializeField]
	[Header("収縮時間 : blendShape_2")]
	private float shrinkageTime_2 = 0.15f;
	private float blendShape_2;
	private float blendShapeRepeat_2;
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
		blendShape_2 = GetCapeSize(1);
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
			if (localEulerAngles.x > 180f)
			{
				localEulerAngles.x = 0f;
			}
			root.transform.localEulerAngles = localEulerAngles;
			UnityEngine.Debug.Log("rot x " + localEulerAngles.x.ToString());
		}
		base.transform.localPosition += root.transform.forward * moveSpeed * Time.fixedDeltaTime;
	}
	private void Update()
	{
		blendShapeRepeat_2 += Time.fixedDeltaTime * MaxBlendShape_2 / shrinkageTime_2;
		blendShape_2 = Mathf.PingPong(blendShapeRepeat_2, MaxBlendShape_2);
		blendShape_2 = Mathf.Clamp(blendShape_2, MinBlendShape_2, MaxBlendShape_2);
		SetCapeSize(1, blendShape_2);
		if (!isAction || type != Type.Type_1)
		{
			return;
		}
		if (stallWaitTime < STALL_WAIT_TIME)
		{
			stallWaitTime += Time.deltaTime;
			return;
		}
		stallDiff += stallSpped * Time.deltaTime;
		if (stallDiff > 0.5f)
		{
			stallDiff = 0.5f;
		}
	}
	public void SetIsAction()
	{
		isAction = true;
		callAcition.gameObject.SetActive(value: false);
	}
	public float GetCapeSize(int _capeBlendShapeIndex)
	{
		return furoshiki.GetBlendShapeWeight(_capeBlendShapeIndex);
	}
	public void SetCapeSize(int _capeBlendShapeIndex, float _blendShapeSize)
	{
		furoshiki.SetBlendShapeWeight(_capeBlendShapeIndex, _blendShapeSize);
	}
}
