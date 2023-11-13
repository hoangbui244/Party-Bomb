using System.Collections.Generic;
using UnityEngine;
public class Hidden_CpuAi : MonoBehaviour
{
	private const float NEAR_DISTANCE = 2f;
	private const float FAR_DISTANCE = 3.5f;
	private const float NEAR_SQR_DISTANCE = 4f;
	private const float FAR_SQR_DISTANCE = 12.25f;
	private const float ESCAPE_DIR_DISTANCE = 3f;
	public const float AI_UPDATE_INTERVAL = 0.2f;
	private const float AI_ESCAPE_STANDBY_MAX_TIME = 1f;
	private const float AI_ESCAPE_WALK_MAX_TIME = 3f;
	private const float AI_ONI_FORCE_FIELD_CHANGE_TIME = 5f;
	private void Update()
	{
		if (SingletonCustom<Hidden_GameManager>.Instance.IsGameStart)
		{
			UpdateMethod();
		}
	}
	private void UpdateMethod()
	{
		for (int i = 0; i < 4; i++)
		{
			Hidden_CharacterScript chara = SingletonCustom<Hidden_CharacterManager>.Instance.GetChara(i);
			if (chara.IsPlayer)
			{
				continue;
			}
			chara.AiUpdateInterval -= Time.deltaTime;
			if (chara.AiUpdateInterval > 0f)
			{
				continue;
			}
			chara.AiUpdateInterval = 0.2f;
			Vector3 pos = chara.GetPos();
			pos.y = 0f;
			if (chara.CheckOniType(Hidden_CharacterScript.OniType.Oni))
			{
				if (chara.GetAiMoveType() == Hidden_CharacterScript.AiMoveType.Oni_Search)
				{
					bool flag = false;
					int aiTargetCharaNo = -1;
					if (0 < SingletonCustom<Hidden_CharacterManager>.Instance.GetInFieldEscaperCount(chara.NowFieldNo, out int[] _noArray))
					{
						float num = 100000f;
						for (int j = 0; j < _noArray.Length; j++)
						{
							Hidden_CharacterScript chara2 = SingletonCustom<Hidden_CharacterManager>.Instance.GetChara(_noArray[j]);
							if (!chara2.IsInvincible)
							{
								Vector3 vector = chara2.GetPos() - pos;
								vector.y = 0f;
								float sqrMagnitude = vector.sqrMagnitude;
								if ((!flag || num > sqrMagnitude) && Vector3.Dot(chara.GetDir(), vector.normalized) > 0.2f)
								{
									flag = true;
									aiTargetCharaNo = _noArray[j];
									num = sqrMagnitude;
								}
								else if (!flag || num > sqrMagnitude + 100f)
								{
									flag = true;
									aiTargetCharaNo = _noArray[j];
									num = sqrMagnitude + 100f;
								}
							}
						}
					}
					if (flag)
					{
						chara.AiTargetCharaNo = aiTargetCharaNo;
						chara.AiTargetTrans = SingletonCustom<Hidden_CharacterManager>.Instance.GetChara(chara.AiTargetCharaNo).transform;
						chara.AiTargetFieldNo = chara.NowFieldNo;
						chara.IsAiTargetChara = true;
						chara.SetAiMoveType(Hidden_CharacterScript.AiMoveType.Oni_Attack);
					}
					else if (chara.AiTargetFieldNo == -1 || !SingletonCustom<Hidden_FieldManager>.Instance.CheckIsConnectTwoFieldNo(chara.NowFieldNo, chara.AiTargetFieldNo))
					{
						chara.IsAiTargetChara = false;
						chara.SetAiTargetFieldNoAndTrans(SingletonCustom<Hidden_FieldManager>.Instance.SearchRandomConnectFieldNo(chara.NowFieldNo, chara.PrevFieldNo));
					}
					else if (Time.time - chara.NowFieldChangeTime > 5f)
					{
						chara.PrevFieldNo = chara.AiTargetFieldNo;
						chara.IsAiTargetChara = false;
						chara.SetAiTargetFieldNoAndTrans(SingletonCustom<Hidden_FieldManager>.Instance.SearchRandomConnectFieldNo(chara.NowFieldNo, chara.PrevFieldNo));
					}
					continue;
				}
				Hidden_CharacterScript chara3 = SingletonCustom<Hidden_CharacterManager>.Instance.GetChara(chara.AiTargetCharaNo);
				if (chara.IsAiTargetChara)
				{
					if (chara.NowFieldNo != chara3.NowFieldNo)
					{
						if (SingletonCustom<Hidden_FieldManager>.Instance.CheckIsConnectTwoFieldNo(chara.NowFieldNo, chara3.NowFieldNo))
						{
							chara.SetAiMoveType(Hidden_CharacterScript.AiMoveType.Oni_Search);
							chara.IsAiTargetChara = false;
							chara.SetAiTargetFieldNoAndTrans(SingletonCustom<Hidden_FieldManager>.Instance.SearchRandomConnectFieldNo(chara.NowFieldNo, chara.PrevFieldNo));
						}
						else
						{
							chara.IsAiTargetChara = false;
							chara.AiTargetFieldNo = chara3.NowFieldNo;
							chara.AiTargetTrans = SingletonCustom<Hidden_FieldManager>.Instance.SearchNearestConnectPoint(chara.NowFieldNo, chara.AiTargetFieldNo, pos);
						}
					}
				}
				else if (Time.time - chara.NowFieldChangeTime > 5f)
				{
					chara.PrevFieldNo = chara.AiTargetFieldNo;
					chara.SetAiMoveType(Hidden_CharacterScript.AiMoveType.Oni_Search);
					chara.IsAiTargetChara = false;
					chara.SetAiTargetFieldNoAndTrans(SingletonCustom<Hidden_FieldManager>.Instance.SearchRandomConnectFieldNo(chara.NowFieldNo, chara.PrevFieldNo));
				}
				continue;
			}
			Hidden_CharacterScript oniChara = SingletonCustom<Hidden_CharacterManager>.Instance.GetOniChara();
			Vector3 pos2 = oniChara.GetPos();
			pos2.y = 0f;
			if (chara.GetAiMoveType() == Hidden_CharacterScript.AiMoveType.Escaper_Standby)
			{
				if (SqrDistance(oniChara.GetPos(), pos) < 4f)
				{
					chara.IsAiTargetChara = false;
					chara.AiTargetFieldNo = SingletonCustom<Hidden_FieldManager>.Instance.SearchEscapeConnectFieldNo(chara.NowFieldNo, pos, pos - pos2);
					chara.AiTargetTrans = SingletonCustom<Hidden_FieldManager>.Instance.SearchNearestConnectPoint(chara.NowFieldNo, chara.AiTargetFieldNo, pos);
					chara.SetAiMoveType(Hidden_CharacterScript.AiMoveType.Escaper_Run);
				}
				else if (Time.time - chara.AiMoveTypeChangeTime > 1f)
				{
					chara.IsAiTargetChara = false;
					chara.SetAiTargetFieldNoAndTrans(SingletonCustom<Hidden_FieldManager>.Instance.SearchRandomConnectFieldNo(chara.NowFieldNo));
					chara.SetAiMoveType(Hidden_CharacterScript.AiMoveType.Escaper_Walk);
				}
			}
			else if (chara.GetAiMoveType() == Hidden_CharacterScript.AiMoveType.Escaper_Walk)
			{
				if (SqrDistance(oniChara.GetPos(), pos) < 4f)
				{
					chara.IsAiTargetChara = false;
					chara.AiTargetFieldNo = SingletonCustom<Hidden_FieldManager>.Instance.SearchEscapeConnectFieldNo(chara.NowFieldNo, pos, pos - pos2);
					chara.AiTargetTrans = SingletonCustom<Hidden_FieldManager>.Instance.SearchNearestConnectPoint(chara.NowFieldNo, chara.AiTargetFieldNo, pos);
					chara.SetAiMoveType(Hidden_CharacterScript.AiMoveType.Escaper_Run);
				}
				else if (chara.NowFieldNo == chara.AiTargetFieldNo)
				{
					chara.IsAiTargetChara = false;
					chara.SetAiTargetFieldNoAndTrans(SingletonCustom<Hidden_FieldManager>.Instance.SearchRandomConnectFieldNo(chara.NowFieldNo, chara.PrevFieldNo));
				}
				else if (Time.time - chara.AiMoveTypeChangeTime > 3f)
				{
					chara.SetAiMoveType(Hidden_CharacterScript.AiMoveType.Escaper_Standby);
				}
			}
			else if (chara.NowFieldNo == oniChara.NowFieldNo && SingletonCustom<Hidden_FieldManager>.Instance.CheckIsConnectTwoFieldNo(chara.NowFieldNo, oniChara.NowFieldNo))
			{
				if (chara.NowFieldNo == chara.AiTargetFieldNo)
				{
					chara.IsAiTargetChara = false;
					chara.SetAiTargetFieldNoAndTrans(SingletonCustom<Hidden_FieldManager>.Instance.SearchRandomConnectFieldNo(chara.NowFieldNo, chara.PrevFieldNo));
				}
			}
			else
			{
				chara.IsAiTargetChara = false;
				chara.SetAiTargetFieldNoAndTrans(SingletonCustom<Hidden_FieldManager>.Instance.SearchRandomConnectFieldNo(chara.NowFieldNo, chara.PrevFieldNo));
				chara.SetAiMoveType(Hidden_CharacterScript.AiMoveType.Escaper_Walk);
			}
		}
	}
	private Hidden_CharacterScript SearchNearestChara(int _charaNo, Hidden_CharacterScript.OniType _targetOniType)
	{
		Vector3 pos = SingletonCustom<Hidden_CharacterManager>.Instance.GetChara(_charaNo).GetPos();
		int num = -1;
		for (int i = 0; i < 4; i++)
		{
			if (i == _charaNo)
			{
				continue;
			}
			Hidden_CharacterScript chara = SingletonCustom<Hidden_CharacterManager>.Instance.GetChara(i);
			if (chara.CheckOniType(_targetOniType))
			{
				if (num == -1)
				{
					num = i;
				}
				else if (SqrDistance(SingletonCustom<Hidden_CharacterManager>.Instance.GetChara(num).GetPos(), pos, _isZeroY: true) > SqrDistance(chara.GetPos(), pos, _isZeroY: true))
				{
					num = i;
				}
			}
		}
		if (num == -1)
		{
			return null;
		}
		return SingletonCustom<Hidden_CharacterManager>.Instance.GetChara(num);
	}
	private Hidden_CharacterScript[] GetSortNearCharaList(int _charaNo, Hidden_CharacterScript.OniType _targetOniType)
	{
		List<Hidden_CharacterScript> list = new List<Hidden_CharacterScript>();
		for (int i = 0; i < 4; i++)
		{
			if (i != _charaNo)
			{
				Hidden_CharacterScript chara = SingletonCustom<Hidden_CharacterManager>.Instance.GetChara(i);
				if (chara.CheckOniType(_targetOniType))
				{
					list.Add(chara);
				}
			}
		}
		Vector3 charaPos = SingletonCustom<Hidden_CharacterManager>.Instance.GetChara(_charaNo).GetPos();
		list.Sort((Hidden_CharacterScript a, Hidden_CharacterScript b) => (!(SqrDistance(a.GetPos(), charaPos) < SqrDistance(b.GetPos(), charaPos))) ? 1 : (-1));
		return list.ToArray();
	}
	private Vector3 CalcEscapeDir(Vector3 _escaperPos, Vector3[] _oniPosList, float _checkRadius)
	{
		if (_oniPosList.Length == 1)
		{
			Vector3 vector = _escaperPos - _oniPosList[0];
			vector.y = 0f;
			vector = vector.normalized;
			if (!SingletonCustom<Hidden_FieldManager>.Instance.CheckInField(_escaperPos + vector * 3f))
			{
				Vector3 vector2 = Quaternion.Euler(0f, -90f, 0f) * vector;
				Vector3 vector3 = Quaternion.Euler(0f, 90f, 0f) * vector;
				if (SingletonCustom<Hidden_FieldManager>.Instance.JudgeTurnRight(_escaperPos, vector))
				{
					if (SingletonCustom<Hidden_FieldManager>.Instance.CheckInField(_escaperPos + vector2 * 3f))
					{
						return vector2;
					}
					if (SingletonCustom<Hidden_FieldManager>.Instance.CheckInField(_escaperPos + vector3 * 3f))
					{
						return vector3;
					}
					return vector2;
				}
				if (SingletonCustom<Hidden_FieldManager>.Instance.CheckInField(_escaperPos + vector3 * 3f))
				{
					return vector3;
				}
				if (SingletonCustom<Hidden_FieldManager>.Instance.CheckInField(_escaperPos + vector2 * 3f))
				{
					return vector2;
				}
				return vector3;
			}
			return vector;
		}
		List<Vector3> list = new List<Vector3>();
		float num = _checkRadius * _checkRadius;
		for (int i = 0; i < _oniPosList.Length; i++)
		{
			for (int j = i + 1; j < _oniPosList.Length; j++)
			{
				Vector3 a2 = (_oniPosList[i] + _oniPosList[j]) / 2f;
				a2.y = 0f;
				Vector3 vector4 = a2 - _oniPosList[i];
				vector4.y = 0f;
				float d = Mathf.Sqrt(num - vector4.sqrMagnitude);
				Vector3 point = vector4.normalized * d;
				Vector3 vector5 = a2 + Quaternion.Euler(0f, 90f, 0f) * point;
				Vector3 vector6 = a2 + Quaternion.Euler(0f, -90f, 0f) * point;
				vector5.y = 0f;
				vector6.y = 0f;
				bool flag = true;
				bool flag2 = true;
				for (int k = 0; k < _oniPosList.Length; k++)
				{
					if (k != i && k != j)
					{
						if (flag && SqrDistance(vector5, _oniPosList[k], _isZeroY: true) < num)
						{
							flag = false;
						}
						if (flag2 && SqrDistance(vector6, _oniPosList[k], _isZeroY: true) < num)
						{
							flag2 = false;
						}
						if (!flag && !flag2)
						{
							break;
						}
					}
				}
				if (flag)
				{
					list.Add(vector5);
				}
				if (flag2)
				{
					list.Add(vector6);
				}
			}
		}
		if (list.Count == 0)
		{
			return Vector3.zero;
		}
		list.Sort((Vector3 a, Vector3 b) => (!((a - _escaperPos).sqrMagnitude < (b - _escaperPos).sqrMagnitude)) ? 1 : (-1));
		_escaperPos.y = 0f;
		for (int l = 0; l < list.Count; l++)
		{
			Vector3 normalized = (list[l] - _escaperPos).normalized;
			if (SingletonCustom<Hidden_FieldManager>.Instance.CheckInField(_escaperPos + normalized * 3f))
			{
				return normalized;
			}
		}
		return (list[0] - _escaperPos).normalized;
	}
	private static float SqrDistance(Vector3 _posA, Vector3 _posB, bool _isZeroY = false)
	{
		if (_isZeroY)
		{
			_posA.y = _posB.y;
		}
		return (_posA - _posB).sqrMagnitude;
	}
}
