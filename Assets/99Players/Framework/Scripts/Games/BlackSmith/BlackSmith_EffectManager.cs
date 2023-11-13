using System.Collections.Generic;
using UnityEngine;
public class BlackSmith_EffectManager : SingletonCustom<BlackSmith_EffectManager>
{
	private enum EffectType
	{
		HammerStrike,
		CreateWeaponComplete,
		EvaluationPerfect,
		EvaluationNice,
		EvaluationGood,
		EvaluationBad
	}
	[SerializeField]
	[Header("ハンマ\u30fcを叩いた時のエフェクト（3D）")]
	private ParticleSystem hammerStrikeEffect;
	private Dictionary<int, List<ParticleSystem>> hammerStrikeEffectList = new Dictionary<int, List<ParticleSystem>>();
	[SerializeField]
	[Header("武器が作成完了した時のエフェクト（3D）")]
	private ParticleSystem createWeaponCompleteEffect;
	private Dictionary<int, List<ParticleSystem>> createWeaponCompleteEffectList = new Dictionary<int, List<ParticleSystem>>();
	[SerializeField]
	[Header("ハンマ\u30fcを叩いた時の評価エフェクト（Bad）（2D）")]
	private BlackSmith_EvaluationEffect evaluationBadEffect;
	private Dictionary<int, List<BlackSmith_EvaluationEffect>> evaluationBadEffectList = new Dictionary<int, List<BlackSmith_EvaluationEffect>>();
	[SerializeField]
	[Header("ハンマ\u30fcを叩いた時の評価エフェクト（Good）（2D）")]
	private BlackSmith_EvaluationEffect evaluationGoodEffect;
	private Dictionary<int, List<BlackSmith_EvaluationEffect>> evaluationGoodEffectList = new Dictionary<int, List<BlackSmith_EvaluationEffect>>();
	[SerializeField]
	[Header("ハンマ\u30fcを叩いた時の評価エフェクト（Nice）（2D）")]
	private BlackSmith_EvaluationEffect evaluationNiceEffect;
	private Dictionary<int, List<BlackSmith_EvaluationEffect>> evaluationNiceEffectList = new Dictionary<int, List<BlackSmith_EvaluationEffect>>();
	[SerializeField]
	[Header("ハンマ\u30fcを叩いた時の評価エフェクト（Perfect）（2D）")]
	private BlackSmith_EvaluationEffect evaluationPerfectEffect;
	private Dictionary<int, List<BlackSmith_EvaluationEffect>> evaluationPerfectEffectList = new Dictionary<int, List<BlackSmith_EvaluationEffect>>();
	public void Init()
	{
		Vector3 cameraRot = SingletonCustom<BlackSmith_CameraManager>.Instance.GetCameraRot(0);
		hammerStrikeEffect.transform.localEulerAngles = cameraRot;
		createWeaponCompleteEffect.transform.localEulerAngles = cameraRot;
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length; i++)
		{
			hammerStrikeEffectList.Add(i, new List<ParticleSystem>());
			createWeaponCompleteEffectList.Add(i, new List<ParticleSystem>());
			evaluationPerfectEffectList.Add(i, new List<BlackSmith_EvaluationEffect>());
			evaluationNiceEffectList.Add(i, new List<BlackSmith_EvaluationEffect>());
			evaluationGoodEffectList.Add(i, new List<BlackSmith_EvaluationEffect>());
			evaluationBadEffectList.Add(i, new List<BlackSmith_EvaluationEffect>());
		}
	}
	private ParticleSystem GetEffect(int _playerNo, EffectType _effectType)
	{
		Dictionary<int, List<ParticleSystem>> dictionary = null;
		switch (_effectType)
		{
		case EffectType.HammerStrike:
			dictionary = hammerStrikeEffectList;
			break;
		case EffectType.CreateWeaponComplete:
			dictionary = createWeaponCompleteEffectList;
			break;
		}
		ParticleSystem particleSystem = null;
		for (int i = 0; i < dictionary[_playerNo].Count; i++)
		{
			if (!dictionary[_playerNo][i].isPlaying)
			{
				particleSystem = dictionary[_playerNo][i];
				break;
			}
		}
		if (particleSystem == null)
		{
			switch (_effectType)
			{
			case EffectType.HammerStrike:
				particleSystem = Object.Instantiate(hammerStrikeEffect);
				break;
			case EffectType.CreateWeaponComplete:
				particleSystem = Object.Instantiate(createWeaponCompleteEffect);
				break;
			}
			particleSystem.gameObject.SetActive(value: true);
			dictionary[_playerNo].Add(particleSystem);
		}
		return particleSystem;
	}
	private BlackSmith_EvaluationEffect GetEvaluationEffect(int _playerNo, EffectType _effectType)
	{
		Dictionary<int, List<BlackSmith_EvaluationEffect>> dictionary = null;
		switch (_effectType)
		{
		case EffectType.EvaluationPerfect:
			dictionary = evaluationPerfectEffectList;
			break;
		case EffectType.EvaluationNice:
			dictionary = evaluationNiceEffectList;
			break;
		case EffectType.EvaluationGood:
			dictionary = evaluationGoodEffectList;
			break;
		case EffectType.EvaluationBad:
			dictionary = evaluationBadEffectList;
			break;
		}
		BlackSmith_EvaluationEffect blackSmith_EvaluationEffect = null;
		for (int i = 0; i < dictionary[_playerNo].Count; i++)
		{
			if (!dictionary[_playerNo][i].IsPlaying())
			{
				blackSmith_EvaluationEffect = dictionary[_playerNo][i];
				break;
			}
		}
		if (blackSmith_EvaluationEffect == null)
		{
			switch (_effectType)
			{
			case EffectType.EvaluationPerfect:
				blackSmith_EvaluationEffect = Object.Instantiate(evaluationPerfectEffect);
				break;
			case EffectType.EvaluationNice:
				blackSmith_EvaluationEffect = Object.Instantiate(evaluationNiceEffect);
				break;
			case EffectType.EvaluationGood:
				blackSmith_EvaluationEffect = Object.Instantiate(evaluationGoodEffect);
				break;
			case EffectType.EvaluationBad:
				blackSmith_EvaluationEffect = Object.Instantiate(evaluationBadEffect);
				break;
			}
			blackSmith_EvaluationEffect.gameObject.SetActive(value: true);
			dictionary[_playerNo].Add(blackSmith_EvaluationEffect);
		}
		return blackSmith_EvaluationEffect;
	}
	public ParticleSystem GetHammerStrikeEffect(int _playerNo)
	{
		return GetEffect(_playerNo, EffectType.HammerStrike);
	}
	public ParticleSystem GetCreateWeaponCompleteEffect(int _playerNo)
	{
		return GetEffect(_playerNo, EffectType.CreateWeaponComplete);
	}
	public BlackSmith_EvaluationEffect GetEvaluationBadEffect(int _playerNo)
	{
		return GetEvaluationEffect(_playerNo, EffectType.EvaluationBad);
	}
	public BlackSmith_EvaluationEffect GetEvaluationGoodEffect(int _playerNo)
	{
		return GetEvaluationEffect(_playerNo, EffectType.EvaluationGood);
	}
	public BlackSmith_EvaluationEffect GetEvaluationNiceEffect(int _playerNo)
	{
		return GetEvaluationEffect(_playerNo, EffectType.EvaluationNice);
	}
	public BlackSmith_EvaluationEffect GetEvaluationPerfectEffect(int _playerNo)
	{
		return GetEvaluationEffect(_playerNo, EffectType.EvaluationPerfect);
	}
}
