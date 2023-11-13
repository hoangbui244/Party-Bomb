using System.Collections;
using TMPro;
using UnityEngine;
namespace BeachSoccer
{
	public class GoalEffect : MonoBehaviour
	{
		private float EFFECT_TIME = 3f;
		private float effectTime;
		[SerializeField]
		[Header("文字")]
		private ParticleSystem[] text;
		[SerializeField]
		[Header("文字アンカ\u30fc")]
		private Transform textAnchor;
		[SerializeField]
		[Header("ポジション")]
		private TextMeshPro positionText;
		[SerializeField]
		[Header("背番号")]
		private TextMeshPro uniformNumber;
		[SerializeField]
		[Header("名前")]
		private new TextMeshPro name;
		[SerializeField]
		[Header("オウンゴ\u30fcル")]
		private TextMeshPro ownGoal;
		[SerializeField]
		[Header("シュ\u30fcトキャラ情報アンカ\u30fc")]
		private GameObject shootCharaAnchor;
		private bool isPlay;
		public void Play(CharacterScript _shootChara, bool _own = false)
		{
			if (_own)
			{
				ownGoal.gameObject.SetActive(value: true);
				shootCharaAnchor.SetActive(value: false);
			}
			else
			{
				positionText.text = _shootChara.GetPositionType().ToString();
				uniformNumber.text = _shootChara.GetUniformNumber().ToString();
				name.text = _shootChara.GetName();
			}
			for (int i = 0; i < text.Length; i++)
			{
				LeanTween.moveLocalX(text[i].gameObject, text[i].transform.localPosition.x - 900f, 0.15f).setDelay((float)i * 0.075f).setEaseLinear();
			}
			StartCoroutine(_MoveText());
			isPlay = true;
		}
		private IEnumerator _MoveText()
		{
			float defLocal = textAnchor.localPosition.x;
			LeanTween.moveLocalX(textAnchor.gameObject, 0f, 0.75f).setEaseOutBack();
			yield return new WaitForSeconds(1.75f);
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
