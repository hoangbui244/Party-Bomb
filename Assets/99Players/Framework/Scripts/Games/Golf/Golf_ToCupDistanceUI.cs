using TMPro;
using UnityEngine;
public class Golf_ToCupDistanceUI : MonoBehaviour
{
	private Vector3 originPos;
	[SerializeField]
	[Header("画面横へ移動させる座標")]
	private Vector3 movePos;
	[SerializeField]
	[Header("カップまでのヤ\u30fcド")]
	private TextMeshPro toCupDistanceText;
	public void Init()
	{
		originPos = base.transform.localPosition;
		toCupDistanceText.text = ((int)SingletonCustom<Golf_FieldManager>.Instance.GetHole().GetReadyBallPosToCupDistance()).ToString();
	}
	public void InitPlay()
	{
		base.transform.localPosition = originPos;
	}
	public void Move(float _time)
	{
		LeanTween.moveLocal(base.gameObject, movePos, _time);
	}
}
