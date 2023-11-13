using UnityEngine;
public class BeachVolley_CharacterCursor : MonoBehaviour
{
	private Transform moveParentCharacter;
	[SerializeField]
	[Header("シュ\u30fcトゲ\u30fcジ")]
	private Renderer shootGauge;
	[SerializeField]
	[Header("矢印")]
	private GameObject arrow;
	[SerializeField]
	[Header("矢印アンカ\u30fc")]
	private Transform arrowAnchor;
	[SerializeField]
	[Header("カ\u30fcソル")]
	private SpriteRenderer cursorCircle;
	public static float SCALE_ANI_SPEED = 6f;
	private float scaleAniTime;
	[SerializeField]
	[Header("キャラ")]
	private BeachVolley_Character chara;
	private float gaugeValue;
	[SerializeField]
	[Header("ベ\u30fcスサイズ")]
	private Vector3 baseScale;
	private Transform _transform;
	[SerializeField]
	[Header("親")]
	private Transform parent;
	private void Awake()
	{
		_transform = base.transform;
	}
	public void UpdateMethod()
	{
		Vector3 eulerAngle = base.transform.rotation.eulerAngles;
		base.transform.SetEulerAngles(0f, base.transform.rotation.eulerAngles.y, 0f);
		scaleAniTime += Time.deltaTime * SCALE_ANI_SPEED;
		cursorCircle.transform.localScale = baseScale * (1.12f + Mathf.Sin(scaleAniTime) * 0.12f);
		if (moveParentCharacter != null && _transform != null && _transform.parent == parent)
		{
			Vector3 vector = new Vector3(0f, 0.25f, 0f);
			Vector3 vector2 = (moveParentCharacter.position + vector - _transform.position) * 0.25f;
			if (vector2.magnitude < 0.1f)
			{
				vector2 = vector2.normalized * 0.1f;
			}
			_transform.position += vector2;
			if (Vector3.Distance(_transform.position, moveParentCharacter.position + vector) < 0.06f)
			{
				base.transform.SetParent(moveParentCharacter);
				base.transform.SetLocalPosition(0f, vector.y, 0f);
			}
		}
	}
	public void SetGauge(float _per)
	{
		gaugeValue = _per;
	}
	public void ResetGauge()
	{
		gaugeValue = 0f;
		if (shootGauge != null)
		{
			shootGauge.material.SetFloat("_Cutoff", 1f);
		}
	}
	public void SetPosition(BeachVolley_Character _chara)
	{
		moveParentCharacter = _chara.transform;
		base.transform.position = _chara.transform.position;
	}
	public void SetCharacter(BeachVolley_Character _chara, bool _isParentFlag = false)
	{
		chara = _chara;
		if (_chara != null)
		{
			if (_isParentFlag)
			{
				base.transform.SetParent(_chara.transform);
				base.transform.SetLocalPosition(0f, 0.25f, 0f);
			}
			else
			{
				base.transform.SetParent(parent);
				moveParentCharacter = _chara.transform;
				base.transform.SetLocalEulerAngles(0f, 0f, 0f);
			}
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
			arrow.transform.SetLocalPositionZ(1.2f);
		}
		else
		{
			arrow.transform.SetLocalPositionZ(1f);
		}
	}
	public void SetCursorDir(Vector3 _dirVec)
	{
		_dirVec.y = 0f;
		arrowAnchor.LookAt(arrowAnchor.position + _dirVec);
	}
	public void ShowCircle(bool _show)
	{
		cursorCircle.gameObject.SetActive(_show);
	}
	public void ShowCircleAlpha(bool _haveBall)
	{
		cursorCircle.color = new Color(cursorCircle.color.r, cursorCircle.color.g, cursorCircle.color.b, _haveBall ? 1f : 0.5f);
	}
	public BeachVolley_Character GetChara()
	{
		return chara;
	}
	public float GetGaugeValue()
	{
		return gaugeValue;
	}
	public static bool CheckToss(float _value)
	{
		return _value <= 0.25f;
	}
}
