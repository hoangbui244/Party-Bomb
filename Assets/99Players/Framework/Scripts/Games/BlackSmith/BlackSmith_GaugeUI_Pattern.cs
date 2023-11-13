using UnityEngine;
public class BlackSmith_GaugeUI_Pattern : MonoBehaviour
{
	[SerializeField]
	[Header("ゲ\u30fcジのラインの種類")]
	private BlackSmith_GaugeUI.LineType lineType;
	[SerializeField]
	[Header("ゲ\u30fcジの場所の種類")]
	private BlackSmith_GaugeUI.PosType posType;
	[SerializeField]
	[Header("Perfect用エリア配列")]
	private BlackSmith_GaugeUI_Pattern_Area[] arrayPattern_PerfectArea;
	[SerializeField]
	[Header("Nice用エリア配列")]
	private BlackSmith_GaugeUI_Pattern_Area[] arrayPattern_NiceArea;
	[SerializeField]
	[Header("Good用エリア")]
	private BlackSmith_GaugeUI_Pattern_Area goodArea;
	public int GetPerfectCnt()
	{
		return arrayPattern_PerfectArea.Length;
	}
	public int GetBarIdx(Vector3 _pos, int _bar_MoveDir)
	{
		int result = (_bar_MoveDir != 1) ? (arrayPattern_PerfectArea.Length - 1) : 0;
		for (int i = 0; i < arrayPattern_PerfectArea.Length; i++)
		{
			if (i == arrayPattern_PerfectArea.Length - 1)
			{
				result = arrayPattern_PerfectArea.Length - 1;
				break;
			}
			float num = (arrayPattern_PerfectArea[i].GetMaxPosX() + arrayPattern_PerfectArea[i + 1].GetMinPosX()) / 2f;
			if (_pos.x <= num)
			{
				result = i;
				break;
			}
		}
		return result;
	}
	public bool IsCanPerfectInput(Vector3 _pos)
	{
		Vector3 pos = arrayPattern_PerfectArea[arrayPattern_PerfectArea.Length - 1].transform.InverseTransformPoint(_pos);
		return arrayPattern_PerfectArea[arrayPattern_PerfectArea.Length - 1].IsBeforeMax(pos);
	}
	public bool IsPerfectBetweenMinMax(Vector3 _pos)
	{
		bool flag = false;
		for (int i = 0; i < arrayPattern_PerfectArea.Length; i++)
		{
			Vector3 pos = arrayPattern_PerfectArea[i].transform.InverseTransformPoint(_pos);
			flag = arrayPattern_PerfectArea[i].IsBetweenMinMax(pos);
			if (flag)
			{
				break;
			}
		}
		return flag;
	}
	public bool IsPerfectBetweenMinMax(int _idx, Vector3 _pos)
	{
		Vector3 pos = arrayPattern_PerfectArea[_idx].transform.InverseTransformPoint(_pos);
		return arrayPattern_PerfectArea[_idx].IsBetweenMinMax(pos);
	}
	public bool IsNiceBetweenMinMax(Vector3 _pos)
	{
		bool flag = false;
		for (int i = 0; i < arrayPattern_NiceArea.Length; i++)
		{
			Vector3 pos = arrayPattern_NiceArea[i].transform.InverseTransformPoint(_pos);
			flag = arrayPattern_NiceArea[i].IsBetweenMinMax(pos);
			if (flag)
			{
				break;
			}
		}
		return flag;
	}
	public bool IsNiceBetweenMinMax(int _idx, Vector3 _pos)
	{
		Vector3 pos = arrayPattern_NiceArea[_idx].transform.InverseTransformPoint(_pos);
		return arrayPattern_NiceArea[_idx].IsBetweenMinMax(pos);
	}
	public bool IsCanGoodInput(Vector3 _pos)
	{
		Vector3 pos = goodArea.transform.InverseTransformPoint(_pos);
		return goodArea.IsBeforeMax(pos);
	}
	public bool IsGoodBetweenMinMax(Vector3 _pos)
	{
		Vector3 pos = goodArea.transform.InverseTransformPoint(_pos);
		return goodArea.IsBetweenMinMax(pos);
	}
	public bool IsInputTiming(int _idx, BlackSmith_PlayerManager.EvaluationType _evaluationType, float _inputTiming, Vector3 _pos)
	{
		switch (_evaluationType)
		{
		case BlackSmith_PlayerManager.EvaluationType.Good:
		{
			Vector3 vector = arrayPattern_PerfectArea[_idx].transform.InverseTransformPoint(_pos);
			if (IsGoodBetweenMinMax(_pos) && !IsNiceBetweenMinMax(_idx, _pos) && !IsPerfectBetweenMinMax(_idx, _pos))
			{
				return Mathf.Abs(_inputTiming - vector.x) <= 5f;
			}
			return false;
		}
		case BlackSmith_PlayerManager.EvaluationType.Nice:
		{
			Vector3 vector = arrayPattern_NiceArea[_idx].transform.InverseTransformPoint(_pos);
			if (IsNiceBetweenMinMax(_idx, _pos) && !IsPerfectBetweenMinMax(_idx, _pos))
			{
				return Mathf.Abs(_inputTiming - vector.x) <= 5f;
			}
			return false;
		}
		case BlackSmith_PlayerManager.EvaluationType.Perfect:
		{
			Vector3 vector = arrayPattern_PerfectArea[_idx].transform.InverseTransformPoint(_pos);
			if (IsPerfectBetweenMinMax(_idx, _pos))
			{
				return Mathf.Abs(_inputTiming - vector.x) <= 5f;
			}
			return false;
		}
		default:
		{
			Vector3 vector = goodArea.transform.InverseTransformPoint(_pos);
			if (!IsGoodBetweenMinMax(_pos))
			{
				return Mathf.Abs(_inputTiming - vector.x) <= 5f;
			}
			return false;
		}
		}
	}
	public float GetInputTiming(int _idx, BlackSmith_PlayerManager.EvaluationType _evaluationType, int _moveDir = 0)
	{
		switch (_evaluationType)
		{
		case BlackSmith_PlayerManager.EvaluationType.Good:
			return arrayPattern_NiceArea[_idx].GetInputTiming(_evaluationType);
		case BlackSmith_PlayerManager.EvaluationType.Nice:
			return arrayPattern_NiceArea[_idx].GetInputTiming(_evaluationType, 0, arrayPattern_PerfectArea[_idx]);
		case BlackSmith_PlayerManager.EvaluationType.Perfect:
			return arrayPattern_PerfectArea[_idx].GetInputTiming(_evaluationType);
		default:
			return goodArea.GetInputTiming(_evaluationType, _moveDir);
		}
	}
}
