using UnityEngine;
public class RockClimbing_Castle : MonoBehaviour
{
	[SerializeField]
	[Header("窓から投げる用の障害物クラス")]
	private RockClimbing_Obstacle_Throw obstacleThrow;
	[SerializeField]
	[Header("スタ\u30fcト用の土台クラス")]
	private RockClimbing_ClimbOnFoundation startClimbOnFoundation;
	[SerializeField]
	[Header("ゴ\u30fcル用の土台クラス")]
	private RockClimbing_ClimbOnFoundation goalClimbOnFoundation;
	private RockClimbing_ClimbOnFoundation[] arrayClimbOnFoundation;
	[SerializeField]
	[Header("プレイヤ\u30fcが乗ることができる土台配列（屋根）")]
	private RockClimbing_ClimbOnFoundation[] arrayClimbOnFoundationRoof;
	private RockClimbing_ClimbOnFoundationObject_Group climbOnFoundationObjectGroup;
	private bool isObjectGroupCreate;
	[SerializeField]
	[Header("忍者キャラ")]
	private RockClimbing_CastleNiinja[] arrayCharaNiinja;
	[SerializeField]
	[Header("各順位用の位置アンカ\u30fc")]
	private Transform[] arrayGoalMoveAnchor;
	public void Init(bool _isFirstPlayer, bool _isReverse)
	{
		for (int i = 0; i < arrayCharaNiinja.Length; i++)
		{
			arrayCharaNiinja[i].Init();
			obstacleThrow.Init();
			obstacleThrow.SetCharaNinja(i, arrayCharaNiinja[i]);
			arrayCharaNiinja[i].gameObject.SetActive(value: false);
		}
		CreateClimbOnFoundationObjectGroup(_isReverse);
		int num = 0;
		RockClimbing_ClimbOnFoundationObject_Group.ClimbOnFoundationAnchor[] arrayClimbOnFoundationAnchor = climbOnFoundationObjectGroup.GetArrayClimbOnFoundationAnchor();
		for (int j = 0; j < arrayClimbOnFoundationAnchor.Length; j++)
		{
			num += arrayClimbOnFoundationAnchor[j].GetArrayClimbOnFoundation().Length;
		}
		num += arrayClimbOnFoundationRoof.Length;
		arrayClimbOnFoundation = new RockClimbing_ClimbOnFoundation[num];
		num = 0;
		for (int k = 0; k < arrayClimbOnFoundationAnchor.Length; k++)
		{
			for (int l = 0; l < arrayClimbOnFoundationAnchor[k].GetArrayClimbOnFoundation().Length; l++)
			{
				arrayClimbOnFoundation[num] = arrayClimbOnFoundationAnchor[k].GetArrayClimbOnFoundation()[l];
				num++;
			}
		}
		for (int m = 0; m < arrayClimbOnFoundationRoof.Length; m++)
		{
			arrayClimbOnFoundation[num] = arrayClimbOnFoundationRoof[m];
			num++;
		}
		for (int n = 0; n < arrayClimbOnFoundation.Length; n++)
		{
			arrayClimbOnFoundation[n].Init();
		}
	}
	public void CreateClimbOnFoundationObjectGroup(bool _isReverse)
	{
		isObjectGroupCreate = true;
		RockClimbing_ClimbOnFoundationObject_Group[] arrayClimbOnFoundationObjectGroupPref = SingletonCustom<RockClimbing_ClimbingWallManager>.Instance.GetArrayClimbOnFoundationObjectGroupPref();
		RockClimbing_ClimbOnFoundationObject_Group original = arrayClimbOnFoundationObjectGroupPref[Random.Range(0, arrayClimbOnFoundationObjectGroupPref.Length)];
		climbOnFoundationObjectGroup = UnityEngine.Object.Instantiate(original);
		climbOnFoundationObjectGroup.transform.parent = base.transform;
		climbOnFoundationObjectGroup.transform.localPosition = Vector3.zero;
		climbOnFoundationObjectGroup.transform.SetLocalScaleX(_isReverse ? (-1f) : 1f);
		climbOnFoundationObjectGroup.Init();
	}
	public void CreateClimbOnFoundationObjectGroup(RockClimbing_ClimbOnFoundationObject_Group _objectGroup, bool _isReverse)
	{
		climbOnFoundationObjectGroup = UnityEngine.Object.Instantiate(_objectGroup);
		climbOnFoundationObjectGroup.transform.parent = base.transform;
		climbOnFoundationObjectGroup.transform.localPosition = _objectGroup.transform.localPosition;
		climbOnFoundationObjectGroup.transform.SetLocalScaleX(_isReverse ? (-1f) : 1f);
		climbOnFoundationObjectGroup.Init();
	}
	public void UpdateMethod()
	{
		obstacleThrow.UpdateMethod();
	}
	public RockClimbing_ClimbOnFoundationObject_Group GetClimbOnFoundationObjectGroup()
	{
		return climbOnFoundationObjectGroup;
	}
	public bool GetIsObjectGroupCreate()
	{
		return isObjectGroupCreate;
	}
	public RockClimbing_ClimbOnFoundation GetStartClimbOnFoundation()
	{
		return startClimbOnFoundation;
	}
	public RockClimbing_ClimbOnFoundation GetGoalClimbOnFoundation()
	{
		return goalClimbOnFoundation;
	}
	public RockClimbing_ClimbOnFoundation[] GetArrayClimbOnFoundation()
	{
		return arrayClimbOnFoundation;
	}
	public RockClimbing_ClimbOnFoundation[] GetArrayClimbOnFoundationRoof()
	{
		return arrayClimbOnFoundationRoof;
	}
	public RockClimbing_Obstacle_Throw_Group GetThrowObstacleGroup(int _playerNo)
	{
		return obstacleThrow.GetThrowObstacleGroup(_playerNo);
	}
	public void StopThrowObstacle(int _playerNo)
	{
		obstacleThrow.StopThrowObstacle(_playerNo);
	}
	public Transform GetGoalMoveAnchor(int _idx)
	{
		return arrayGoalMoveAnchor[_idx];
	}
}
