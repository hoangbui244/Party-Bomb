using System.Collections;
using TMPro;
using UnityEngine;
namespace BeachSoccer
{
	public class AdditionalTimeEffect : MonoBehaviour
	{
		private float EFFECT_TIME = 2f;
		private float effectTime;
		[SerializeField]
		[Header("文字")]
		private ParticleSystem[] text;
		[SerializeField]
		[Header("文字アンカ\u30fc")]
		private Transform textAnchor;
		[SerializeField]
		[Header("残り時間")]
		private TextMeshPro additionalTime;
		private bool isPlay;
		public void Play(float _time)
		{
			if (SingletonCustom<GameUiManager>.Instance.IsNotJapanese)
			{
				additionalTime.text = _time.ToString() + " minutes remain";
			}
			else
			{
				additionalTime.text = "残り時間  " + _time.ToString() + " 分";
			}
			StartCoroutine(_MoveText());
			isPlay = true;
		}
		private IEnumerator _MoveText()
		{
			float defLocal = textAnchor.localPosition.x;
			LeanTween.moveLocalX(textAnchor.gameObject, 0f, 0.75f).setEaseOutBack();
			yield return new WaitForSeconds(1f);
			LeanTween.moveLocalX(textAnchor.gameObject, 0f - defLocal, 0.5f).setEaseOutQuad();
		}
		private void Update()
		{
			if (isPlay)
			{
				effectTime += Time.deltaTime;
				if (effectTime >= EFFECT_TIME)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}
	}
}
