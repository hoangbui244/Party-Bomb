using UnityEngine;
public class BlackSmith_GaugeUI_Pattern_Area : MonoBehaviour
{
	[SerializeField]
	[Header("Min")]
	private Transform min;
	[SerializeField]
	[Header("Max")]
	private Transform max;
	public bool IsBetweenMinMax(Vector3 _pos)
	{
		if (_pos.x >= min.localPosition.x)
		{
			return _pos.x <= max.localPosition.x;
		}
		return false;
	}
	public bool IsBeforeMax(Vector3 _pos)
	{
		return _pos.x <= max.localPosition.x;
	}
	public float GetMinPosX(bool _isLocal = false)
	{
		if (!_isLocal)
		{
			return min.position.x;
		}
		return min.localPosition.x;
	}
	public float GetMaxPosX(bool _isLocal = false)
	{
		if (!_isLocal)
		{
			return max.position.x;
		}
		return max.localPosition.x;
	}
	public float GetInputTiming(BlackSmith_PlayerManager.EvaluationType _evaluationType, int _moveDir = 0, BlackSmith_GaugeUI_Pattern_Area _checkArea = null)
	{
		switch (_evaluationType)
		{
		case BlackSmith_PlayerManager.EvaluationType.Good:
			if (UnityEngine.Random.Range(0, 2) != 0)
			{
				return UnityEngine.Random.Range(max.localPosition.x + 5f, max.localPosition.x + 10f);
			}
			return UnityEngine.Random.Range(min.localPosition.x - 10f, min.localPosition.x - 5f);
		case BlackSmith_PlayerManager.EvaluationType.Nice:
		{
			float num = (min.localPosition.x + _checkArea.GetMinPosX(_isLocal: true)) / 2f;
			float num2 = (max.localPosition.x + _checkArea.GetMaxPosX(_isLocal: true)) / 2f;
			if (UnityEngine.Random.Range(0, 2) != 0)
			{
				return UnityEngine.Random.Range(num2 - 5f, num2 + 5f);
			}
			return UnityEngine.Random.Range(num - 5f, num + 5f);
		}
		case BlackSmith_PlayerManager.EvaluationType.Perfect:
			return UnityEngine.Random.Range(min.localPosition.x + 5f, max.localPosition.x - 5f);
		default:
			if (_moveDir != -1)
			{
				return UnityEngine.Random.Range(max.localPosition.x + 5f, max.localPosition.x + 10f);
			}
			return UnityEngine.Random.Range(min.localPosition.x - 10f, min.localPosition.x - 5f);
		}
	}
}
