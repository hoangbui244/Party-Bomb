using System.Collections.Generic;
using UnityEngine;
public class MorphingRace_FieldManager : SingletonCustom<MorphingRace_FieldManager>
{
	public enum TargetPrefType
	{
		Human,
		Mouse,
		Eagle,
		Fish,
		Dog
	}
	public enum HumanTargetPrefType
	{
		Straight,
		Straight_Half,
		Straight_Half_Half
	}
	[SerializeField]
	[Header("デバッグ用：動物フィ\u30fcルドを固定するかどうかのフラグ")]
	private bool isOnlyAnimalType;
	[SerializeField]
	[Header("デバッグ用：動物フィ\u30fcルドを固定する動物の種類")]
	private TargetPrefType onlyAnimalType;
	[SerializeField]
	[Header("生成したフィ\u30fcルドを格納するル\u30fcト")]
	private Transform fiedlRoot;
	[SerializeField]
	[Header("フィ\u30fcルドを生成する開始アンカ\u30fc")]
	private Transform startGenerateAnchor;
	[SerializeField]
	[Header("生成するフィ\u30fcルドプレハブ配列（人キャラ）")]
	private MorphingRace_MorphingTarget[] arrayHumanMorphingTargetPref;
	[SerializeField]
	[Header("生成するフィ\u30fcルドプレハブ配列（動物）")]
	private MorphingRace_MorphingTarget[] arrayAnimalMorphingTargetPref;
	[SerializeField]
	[Header("生成するフィ\u30fcルドプレハブ（スタ\u30fcト）")]
	private MorphingRace_MorphingTarget startTargetPref;
	[SerializeField]
	[Header("生成するフィ\u30fcルドプレハブ（ゴ\u30fcル）")]
	private MorphingRace_MorphingTarget goalMorphingTargetPref;
	[SerializeField]
	[Header("生成する障害物プレハブ配列（障害物がない場合は None にする）")]
	private MorphingRace_GenerateObstacle[] arrayGenerateObstacle;
	private Vector3 beforeTargetEndPos;
	private Vector3 generateVec;
	private List<Vector3>[] rootPosList;
	private float[] arrayDistanceToGoal;
	private float[] arrayFowardDistance;
	public void Init()
	{
		isOnlyAnimalType = false;
		arrayDistanceToGoal = new float[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length];
		arrayFowardDistance = new float[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length];
		rootPosList = new List<Vector3>[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length];
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			rootPosList[i] = new List<Vector3>();
		}
		beforeTargetEndPos = startGenerateAnchor.position;
		generateVec = Vector3.zero;
		GenerateField(startTargetPref);
		GenerateField(arrayHumanMorphingTargetPref[1], _isFirstGenerate: true);
		for (int j = 0; j < arrayAnimalMorphingTargetPref.Length; j++)
		{
			if (isOnlyAnimalType)
			{
				GenerateField(arrayAnimalMorphingTargetPref[(int)(onlyAnimalType - 1)]);
			}
			else
			{
				GenerateField(arrayAnimalMorphingTargetPref[j]);
			}
		}
		GenerateField(goalMorphingTargetPref);
		for (int k = 0; k < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; k++)
		{
			SetDistanceToGoal(k);
		}
	}
	private void GenerateField(MorphingRace_MorphingTarget _generateTargetPref, bool _isFirstGenerate = false)
	{
		MorphingRace_MorphingTarget morphingRace_MorphingTarget = Object.Instantiate(_generateTargetPref, fiedlRoot);
		morphingRace_MorphingTarget.transform.position = beforeTargetEndPos;
		morphingRace_MorphingTarget.transform.localEulerAngles = Quaternion.Euler(morphingRace_MorphingTarget.transform.localEulerAngles) * generateVec * 90f;
		morphingRace_MorphingTarget.Init();
		MorphingRace_GenerateObstacle morphingRace_GenerateObstacle = arrayGenerateObstacle[(int)morphingRace_MorphingTarget.GetMorphingCharacterType()];
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			morphingRace_MorphingTarget.BezierPositionInit(i);
			morphingRace_MorphingTarget.InitMorphingTargetPoint(i);
			if (morphingRace_GenerateObstacle != null)
			{
				morphingRace_MorphingTarget.GenerateObstacle(i, morphingRace_GenerateObstacle);
			}
			if (morphingRace_MorphingTarget.GetIsRootPos())
			{
				if (!_isFirstGenerate)
				{
					morphingRace_MorphingTarget.GetBezierPosition(i).GetPosList().RemoveAt(0);
				}
				rootPosList[i].AddRange(morphingRace_MorphingTarget.GetBezierPosition(i).GetPosList());
			}
		}
		beforeTargetEndPos = morphingRace_MorphingTarget.GetEndAnchorPos();
		generateVec = (beforeTargetEndPos - morphingRace_MorphingTarget.GetStartAnchorPos()).normalized;
		generateVec.y = 0f;
		generateVec.z = 0f;
		Vector3 vector = beforeTargetEndPos;
		UnityEngine.Debug.Log("beforeTargetEndPos " + vector.ToString());
		vector = generateVec;
		UnityEngine.Debug.Log("generateVec " + vector.ToString());
	}
	public float GetDistanceToGoal(int _playerNo)
	{
		return arrayDistanceToGoal[_playerNo];
	}
	public void SetDistanceToGoal(int _playerNo)
	{
		Vector3 obj = rootPosList[_playerNo][0];
		Vector3 target = rootPosList[_playerNo][rootPosList[_playerNo].Count - 1];
		float num = CalcManager.Length(obj, target);
		arrayDistanceToGoal[_playerNo] = num;
	}
	public float GetFowardDistance(int _playerNo)
	{
		return arrayFowardDistance[_playerNo];
	}
	public float GetCurrentDistanceToGoal(int _playerNo)
	{
		return arrayDistanceToGoal[_playerNo] - arrayFowardDistance[_playerNo];
	}
	public void SetFowardDistance(int _playerNo, Vector3 _pos)
	{
		_pos.x = rootPosList[_playerNo][0].x;
		_pos.y = rootPosList[_playerNo][0].y;
		float value = CalcManager.Length(rootPosList[_playerNo][0], _pos);
		arrayFowardDistance[_playerNo] = Mathf.Clamp(value, 0f, arrayDistanceToGoal[_playerNo]);
	}
	public List<Vector3> GetRootPosList(int _playerNo)
	{
		return rootPosList[_playerNo];
	}
	public bool GetIsOnlyAnimalType()
	{
		return isOnlyAnimalType;
	}
	private void OnDrawGizmos()
	{
		Vector3 vector = startGenerateAnchor.position + new Vector3(0f, 0f, 25f);
		float num = CalcManager.Length(startTargetPref.GetStartAnchorPos(), startTargetPref.GetEndAnchorPos());
		Vector3 center = startGenerateAnchor.position + new Vector3(0f, 0f, num / 2f);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(center, new Vector3(50f, 0.1f, num));
		center.z += num / 2f;
		num = CalcManager.Length(arrayHumanMorphingTargetPref[1].GetStartAnchorPos(), arrayHumanMorphingTargetPref[1].GetEndAnchorPos());
		center.z += num / 2f;
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(center, new Vector3(50f, 0.1f, num));
		center.z += num / 2f;
		for (int i = 0; i < arrayAnimalMorphingTargetPref.Length; i++)
		{
			num = CalcManager.Length(arrayAnimalMorphingTargetPref[i].GetStartAnchorPos(), arrayAnimalMorphingTargetPref[i].GetEndAnchorPos());
			center.z += num / 2f;
			Gizmos.color = Color.white;
			Gizmos.DrawWireCube(center, new Vector3(50f, 0.1f, num));
			center.z += num / 2f;
		}
		num = CalcManager.Length(goalMorphingTargetPref.GetStartAnchorPos(), goalMorphingTargetPref.GetEndAnchorPos());
		center.z += num / 2f;
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(center, new Vector3(50f, 0.1f, num));
		if (rootPosList == null)
		{
			return;
		}
		for (int j = 0; j < rootPosList.Length; j++)
		{
			switch (j)
			{
			case 0:
				Gizmos.color = Color.green;
				break;
			case 1:
				Gizmos.color = Color.red;
				break;
			case 2:
				Gizmos.color = Color.blue;
				break;
			case 3:
				Gizmos.color = Color.yellow;
				break;
			}
			for (int k = 0; k < rootPosList[j].Count - 1; k++)
			{
				Gizmos.DrawLine(rootPosList[j][k], rootPosList[j][k + 1]);
			}
		}
	}
}
