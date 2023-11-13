using System.Collections;
using UnityEngine;
namespace BeachSoccer
{
	public class EndGameEffect : MonoBehaviour
	{
		private float EFFECT_TIME = 3f;
		private float effectTime;
		[SerializeField]
		[Header("文字")]
		private ParticleSystem[] text;
		[SerializeField]
		[Header("文字アンカ\u30fc")]
		private Transform textAnchor;
		private void Start()
		{
			StartCoroutine(_MoveText());
		}
		private IEnumerator _MoveText()
		{
			float defLocal = textAnchor.localPosition.x;
			LeanTween.moveLocalX(textAnchor.gameObject, 0f, 0.75f).setEaseOutBack();
			yield return new WaitForSeconds(2f);
			LeanTween.moveLocalX(textAnchor.gameObject, 0f - defLocal, 0.5f).setEaseOutQuad();
		}
		private void Update()
		{
			effectTime += Time.deltaTime;
			if (effectTime >= EFFECT_TIME)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
