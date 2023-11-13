using UnityEngine;
public class BlackSmith_EvaluationEffect : MonoBehaviour
{
	private ParticleSystem effect;
	[SerializeField]
	[Header("★のエフェクト")]
	private ParticleSystem starEffect;
	[SerializeField]
	[Header("テキストエフェクト")]
	private ParticleSystem textEffect;
	private void Awake()
	{
		effect = GetComponent<ParticleSystem>();
	}
	public void SetSize(int _playerNo)
	{
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			if (_playerNo > 0)
			{
				starEffect.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
			}
		}
		else
		{
			starEffect.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
		}
	}
	public void Play()
	{
		LeanTween.cancel(textEffect.gameObject);
		textEffect.transform.SetLocalPositionY(0f);
		effect.Play();
		LeanTween.moveLocalY(textEffect.gameObject, 110f, 0.25f);
	}
	public bool IsPlaying()
	{
		return effect.isPlaying;
	}
}
