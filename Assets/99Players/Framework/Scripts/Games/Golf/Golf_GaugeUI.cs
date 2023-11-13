using System;
using UnityEngine;
public class Golf_GaugeUI : MonoBehaviour
{
	[Serializable]
	private struct GaugeAnchor
	{
		public Transform max;
		public Transform min;
	}
	private Vector3 originPos;
	[SerializeField]
	[Header("画面外へ移動させる座標")]
	private Vector3 movePos;
	[SerializeField]
	[Header("旗アイコン")]
	private GameObject flagIcon;
	[SerializeField]
	[Header("バ\u30fc")]
	private GameObject bar;
	[SerializeField]
	[Header("パワ\u30fcゲ\u30fcジ")]
	private SpriteRenderer powerGauge;
	private float originPowerGaugeWidth;
	private Vector3 originBarPos;
	[SerializeField]
	[Header("バ\u30fcの移動制限のアンカ\u30fcル\u30fcト")]
	private Transform moveLimitRoot;
	[SerializeField]
	[Header("バ\u30fcの移動制限のアンカ\u30fc")]
	private GaugeAnchor moveLimit;
	[SerializeField]
	[Header("インパクト時のバ\u30fcのアンカ\u30fcル\u30fcト")]
	private Transform impactRoot;
	[SerializeField]
	[Header("インパクト時のバ\u30fcのGood以上の有効範囲アンカ\u30fc")]
	private GaugeAnchor impactGoodCoverage;
	[SerializeField]
	[Header("インパクト時のバ\u30fcのPefectアンカ\u30fc")]
	private GaugeAnchor impactPerfect;
	[SerializeField]
	[Header("バ\u30fcの移動速度")]
	private float bar_MoveSpeed;
	private int moveDir;
	private bool isReturnOriginPos;
	[SerializeField]
	[Header("パワ\u30fc決定時のエフェクト")]
	private ParticleSystem powerDecideEffect;
	[SerializeField]
	[Header("インパクト決定時のPerfectエフェクト")]
	private ParticleSystem impactPerfectEffect;
	[SerializeField]
	[Header("インパクト決定時のGoodエフェクト")]
	private ParticleSystem impactGoodEffect;
	[SerializeField]
	[Header("インパクト決定時のBadエフェクト")]
	private ParticleSystem impactBadEffect;
	public void Init()
	{
		originPos = base.transform.localPosition;
		originBarPos = bar.transform.localPosition;
		originPowerGaugeWidth = powerGauge.size.x;
		flagIcon.transform.SetLocalPositionX(powerGauge.transform.localPosition.x + powerGauge.size.x * SingletonCustom<Golf_UIManager>.Instance.GetCupPowerLerp());
	}
	public void InitPlay()
	{
		isReturnOriginPos = false;
		base.transform.localPosition = movePos;
		bar.transform.localPosition = originBarPos;
		Vector2 size = powerGauge.size;
		size.x = 0f;
		powerGauge.size = size;
		moveDir = 1;
	}
	public void Move(bool _inside, float _time)
	{
		if (_inside)
		{
			LeanTween.moveLocal(base.gameObject, originPos, _time);
		}
		else
		{
			LeanTween.moveLocal(base.gameObject, movePos, _time);
		}
	}
	public void UpdateMethod()
	{
		Vector3 localPosition = bar.transform.localPosition;
		switch (SingletonCustom<Golf_GameManager>.Instance.GetState())
		{
		case Golf_GameManager.State.SHOT_POWER:
			if (!isReturnOriginPos)
			{
				localPosition.x += Time.deltaTime * bar_MoveSpeed * (float)moveDir;
				localPosition.x = Mathf.Clamp(localPosition.x, originBarPos.x, moveLimit.max.localPosition.x);
				bar.transform.localPosition = localPosition;
				Vector2 size = powerGauge.size;
				size.x = originPowerGaugeWidth * GetShotPowerLerp();
				powerGauge.size = size;
				if (localPosition.x == originBarPos.x || localPosition.x == moveLimit.max.localPosition.x)
				{
					moveDir *= -1;
				}
			}
			break;
		case Golf_GameManager.State.SHOT_IMPACT:
			localPosition.x -= Time.deltaTime * bar_MoveSpeed;
			localPosition.x = Mathf.Clamp(localPosition.x, moveLimit.min.localPosition.x, moveLimit.max.localPosition.x);
			bar.transform.localPosition = localPosition;
			if (!SingletonCustom<Golf_GameManager>.Instance.GetIsSkip() && localPosition.x == moveLimit.min.localPosition.x)
			{
				Golf_Player turnPlayer = SingletonCustom<Golf_PlayerManager>.Instance.GetTurnPlayer();
				turnPlayer.SetShotVec();
				turnPlayer.SetSwingAnimation();
			}
			break;
		}
	}
	public bool GetIsReturnOriginPos()
	{
		return isReturnOriginPos;
	}
	public float GetShotPowerLerp(bool _isInput = false)
	{
		float x = originBarPos.x;
		float x2 = moveLimit.max.transform.localPosition.x;
		Vector3 vector = moveLimitRoot.InverseTransformPoint(bar.transform.position);
		float x3 = vector.x;
		float result = 1f - (x2 - x3) / (x2 - x);
		if (_isInput)
		{
			UnityEngine.Debug.Log("打つパワ\u30fcの割合 : " + result.ToString());
			powerDecideEffect.transform.localPosition = vector;
			SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_good");
			powerDecideEffect.Play();
		}
		return result;
	}
	public float GetImpactDiff(bool _isInput = false)
	{
		float x = impactGoodCoverage.min.localPosition.x;
		float x2 = impactGoodCoverage.max.localPosition.x;
		Vector3 vector = impactRoot.InverseTransformPoint(bar.transform.position);
		float x3 = vector.x;
		UnityEngine.Debug.Log("barLocalPos " + x3.ToString());
		UnityEngine.Debug.Log("goodCoverageMinPos " + x.ToString());
		UnityEngine.Debug.Log("goodCoverageMaxPos " + x2.ToString());
		if (x3 >= x && x3 <= x2)
		{
			float num = (x + x2) / 2f;
			float num2 = num;
			if (x3 < num)
			{
				num2 = x;
			}
			else if (x3 > num)
			{
				num2 = x2;
			}
			float result = 1f - (num2 - x3) / (num2 - num);
			if (_isInput)
			{
				if (x3 >= impactPerfect.min.localPosition.x && x3 <= impactPerfect.max.localPosition.x)
				{
					impactPerfectEffect.transform.localPosition = vector;
					SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_perfect");
					impactPerfectEffect.Play();
				}
				else
				{
					impactGoodEffect.transform.localPosition = vector;
					SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_good");
					impactGoodEffect.Play();
				}
			}
			UnityEngine.Debug.Log("打つ場所とのズレの割合 : " + result.ToString());
			return result;
		}
		if (_isInput)
		{
			impactBadEffect.transform.localPosition = vector;
			SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_bad");
			impactBadEffect.Play();
		}
		return 1f;
	}
}
