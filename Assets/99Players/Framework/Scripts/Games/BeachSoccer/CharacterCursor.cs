using UnityEngine;
namespace BeachSoccer
{
	public class CharacterCursor : MonoBehaviour
	{
		[SerializeField]
		[Header("シュ\u30fcトゲ\u30fcジ")]
		private Renderer shootGauge;
		[SerializeField]
		[Header("矢印")]
		private MeshRenderer arrow;
		[SerializeField]
		[Header("カ\u30fcソル")]
		private MeshRenderer cursorCircle;
		public static float SCALE_ANI_SPEED = 6f;
		private float scaleAniTime;
		[SerializeField]
		[Header("キャラ")]
		private CharacterScript chara;
		private float gaugeValue;
		public void UpdateMethod()
		{
			Vector3 eulerAngle = base.transform.rotation.eulerAngles;
			base.transform.SetEulerAngles(0f, base.transform.rotation.eulerAngles.y, 0f);
			scaleAniTime += Time.deltaTime * SCALE_ANI_SPEED;
			cursorCircle.transform.localScale = Vector3.one * (1f - Mathf.Sin(scaleAniTime) * 0.12f);
		}
		public void SetGauge(float _per)
		{
			gaugeValue = _per;
			shootGauge.material.SetFloat("_Cutoff", 1f - ((_per > 0.5f) ? Mathf.Max(_per, 0.55f) : _per));
		}
		public void ResetGauge()
		{
			gaugeValue = 0f;
			if (shootGauge != null)
			{
				shootGauge.material.SetFloat("_Cutoff", 1f);
			}
		}
		public void SetCharacter(CharacterScript _chara)
		{
			chara = _chara;
			if (_chara != null)
			{
				base.transform.SetParent(_chara.transform);
				base.transform.SetLocalPosition(0f, 0f, 0f);
				base.transform.SetLocalEulerAngles(0f, 0f, 0f);
				ResetGauge();
			}
		}
		public void Show(bool _show)
		{
			ResetGauge();
			base.gameObject.SetActive(_show);
		}
		public void SetArrowPos(bool _haveBall)
		{
			if (_haveBall)
			{
				arrow.transform.SetLocalPositionZ(1.3f);
			}
			else
			{
				arrow.transform.SetLocalPositionZ(1.1f);
			}
		}
		public void ShowCircle(bool _show)
		{
			cursorCircle.gameObject.SetActive(_show);
		}
		public void ShowCircleAlpha(bool _haveBall)
		{
			cursorCircle.material.color = new Color(cursorCircle.material.color.r, cursorCircle.material.color.g, cursorCircle.material.color.b, _haveBall ? 1f : 0.5f);
		}
		public CharacterScript GetChara()
		{
			return chara;
		}
		public float GetGaugeValue()
		{
			return gaugeValue;
		}
	}
}
