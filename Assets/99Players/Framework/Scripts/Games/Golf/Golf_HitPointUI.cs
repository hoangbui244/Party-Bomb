using UnityEngine;
public class Golf_HitPointUI : MonoBehaviour
{
	private Vector3 originPos;
	[SerializeField]
	[Header("画面外へ移動させる座標")]
	private Vector3 movePos;
	[SerializeField]
	[Header("ボ\u30fcルの打つ位置を表示するUI")]
	private SpriteRenderer icon;
	[SerializeField]
	[Header("ボ\u30fcルの打つ位置を表示するUIの移動限界値")]
	private float iconLimitValue;
	[SerializeField]
	[Header("ボ\u30fcルの打つ位置を表示を動かす速度")]
	private float iconMoveSpeed;
	private Vector3 iconOriginPos;
	public void Init()
	{
		originPos = base.transform.localPosition;
		iconOriginPos = icon.transform.localPosition;
	}
	public void InitPlay()
	{
		base.transform.localPosition = movePos;
		icon.transform.localPosition = iconOriginPos;
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
	public void MoveHitPoint(Vector3 _stickDir)
	{
		if (_stickDir == Vector3.zero)
		{
			icon.transform.localPosition = Vector3.Lerp(icon.transform.localPosition, iconOriginPos, iconMoveSpeed * Time.deltaTime);
			return;
		}
		float num = _stickDir.magnitude;
		if (num > 1f)
		{
			num = 1f;
		}
		Vector3 b = iconOriginPos + _stickDir.normalized * iconLimitValue * num;
		icon.transform.localPosition = Vector3.Lerp(icon.transform.localPosition, b, iconMoveSpeed * Time.deltaTime);
	}
}
