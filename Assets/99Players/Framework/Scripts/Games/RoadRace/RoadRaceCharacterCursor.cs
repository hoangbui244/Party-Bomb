using UnityEngine;
public class RoadRaceCharacterCursor : MonoBehaviour
{
	public static float SCALE_ANI_SPEED = 6f;
	private float scaleAniTime;
	[SerializeField]
	[Header("キャラ")]
	private RoadRaceCharacterScript chara;
	private float gaugeValue;
	[SerializeField]
	[Header("ベ\u30fcスサイズ")]
	private Vector3 baseScale;
	public void UpdateMethod()
	{
		Vector3 eulerAngle = base.transform.rotation.eulerAngles;
		base.transform.SetEulerAngles(0f, base.transform.rotation.eulerAngles.y, 0f);
		scaleAniTime += Time.deltaTime * SCALE_ANI_SPEED;
	}
	public void SetCharacter(RoadRaceCharacterScript _chara)
	{
		chara = _chara;
		if (_chara != null)
		{
			base.transform.SetParent(_chara.transform);
			base.transform.SetLocalPosition(0f, -0.24f, 0f);
			base.transform.SetLocalEulerAngles(0f, 0f, 0f);
		}
	}
	public void Show(bool _show)
	{
		base.gameObject.SetActive(_show);
	}
	public void ShowCircle(bool _show)
	{
	}
	public void ShowCircleAlpha(bool _haveBall)
	{
	}
	public RoadRaceCharacterScript GetChara()
	{
		return chara;
	}
	public void SetPlayrNum(int num)
	{
	}
}
