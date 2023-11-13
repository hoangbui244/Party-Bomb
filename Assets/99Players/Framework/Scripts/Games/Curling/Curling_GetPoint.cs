using UnityEngine;
public class Curling_GetPoint : MonoBehaviour
{
	[SerializeField]
	[Header("ポイントのSpriterenderer")]
	private SpriteRenderer pointSpriteRenderer;
	[SerializeField]
	[Header("ポイント用の画像")]
	private Sprite[] arrayPointSprite;
	public void SetPoint(int _teamNo, Vector3 _pos, int _pointIdx, int _pointCnt)
	{
		base.gameObject.SetActive(value: false);
		CalcManager.mCalcVector3 = SingletonCustom<Curling_GameManager>.Instance.GetCamera().WorldToScreenPoint(_pos);
		CalcManager.mCalcVector3 = SingletonCustom<Curling_UIManager>.Instance.GetGlobalCamera().ScreenToWorldPoint(CalcManager.mCalcVector3);
		base.transform.SetPosition(CalcManager.mCalcVector3.x, CalcManager.mCalcVector3.y, base.transform.position.z);
		pointSpriteRenderer.sprite = arrayPointSprite[_pointIdx];
		pointSpriteRenderer.sortingOrder += _pointCnt;
	}
	public void Show()
	{
		base.transform.SetLocalScaleY(0f);
		base.gameObject.SetActive(value: true);
		LeanTween.scaleY(base.gameObject, 1f, 0.25f).setEaseOutBack();
		LeanTween.value(base.gameObject, 0f, 1f, 0.25f).setOnUpdate(delegate(float _value)
		{
			float a = Mathf.Clamp(_value, 0f, 1f);
			pointSpriteRenderer.SetAlpha(a);
		});
	}
}
