using System.Collections.Generic;
using UnityEngine;
public class BlackSmith_Weapon : MonoBehaviour
{
	private int playerNo;
	[SerializeField]
	[Header("作成に必要なポイント")]
	private int createNeedPoint;
	private int addPoint;
	[SerializeField]
	[Header("残りポイントによって表示するメッシュ（叩くほど変わっていく）")]
	private SkinnedMeshRenderer mesh;
	private const int BLEND_SHAPE_WEIGHT_CNT = 3;
	private List<int> highWeightIdxList = new List<int>();
	private float[] arrayPrevPercentDecimal = new float[3];
	private int prevPercentInt;
	public void Init(int _playerNo)
	{
		playerNo = _playerNo;
		for (int i = 0; i < 3; i++)
		{
			arrayPrevPercentDecimal[i] = 100f;
			mesh.SetBlendShapeWeight(i, arrayPrevPercentDecimal[i]);
		}
	}
	public void AddCreateNeedPoint(BlackSmith_PlayerManager.EvaluationType _evaluationType, int _highWeightIdx)
	{
		int evaluationPoint = SingletonCustom<BlackSmith_WeaponManager>.Instance.GetEvaluationPoint(_evaluationType);
		addPoint += evaluationPoint;
		if (addPoint > createNeedPoint)
		{
			addPoint = createNeedPoint;
		}
		LeanTween.cancel(base.gameObject);
		LeanTween.value(base.gameObject, 0f, 1f, 0.02f).setOnUpdate(delegate(float _value)
		{
			base.transform.SetLocalPositionX((LeanTween.shake.Evaluate(_value) - 0.5f) * 0.02f);
		}).setLoopPingPong(2);
		float num = (float)addPoint / (float)createNeedPoint * 100f;
		if (IsCreateWeapon())
		{
			LeanTween.value(base.gameObject, 1f, 0f, 0.25f).setOnUpdate(delegate(float _value)
			{
				for (int i = 0; i < 3; i++)
				{
					float blendShapeWeight = mesh.GetBlendShapeWeight(i);
					if (blendShapeWeight > 0f)
					{
						mesh.SetBlendShapeWeight(i, blendShapeWeight * _value);
					}
				}
			});
		}
		else
		{
			float num2 = arrayPrevPercentDecimal[_highWeightIdx] - (float)(evaluationPoint * 3);
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			LeanTween.value(base.gameObject, mesh.GetBlendShapeWeight(_highWeightIdx), num2, 0.25f).setOnUpdate(delegate(float _value)
			{
				mesh.SetBlendShapeWeight(_highWeightIdx, _value);
			});
			arrayPrevPercentDecimal[_highWeightIdx] = num2;
		}
		UnityEngine.Debug.Log("percentDecimal " + num.ToString());
		int num3 = (int)num;
		UnityEngine.Debug.Log("percentInt " + num3.ToString());
		LeanTween.value(base.gameObject, prevPercentInt, num3, 0.25f).setOnUpdate(delegate(float _value)
		{
			SingletonCustom<BlackSmith_UIManager>.Instance.SetCreatePercent(playerNo, (int)_value);
		});
		prevPercentInt = num3;
	}
	public bool IsCreateWeapon()
	{
		return addPoint == createNeedPoint;
	}
	public int GetHighWeightIdx()
	{
		highWeightIdxList.Clear();
		float num = -1f;
		for (int i = 0; i < 3; i++)
		{
			if (num < mesh.GetBlendShapeWeight(i))
			{
				num = mesh.GetBlendShapeWeight(i);
				highWeightIdxList.Clear();
				highWeightIdxList.Add(i);
			}
			else if (num == mesh.GetBlendShapeWeight(i))
			{
				highWeightIdxList.Add(i);
			}
		}
		return highWeightIdxList[Random.Range(0, highWeightIdxList.Count)];
	}
}
