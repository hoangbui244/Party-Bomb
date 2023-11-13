using System;
using UnityEngine;
public class Golf_Cursor : MonoBehaviour
{
	private Golf_Ball ball;
	[SerializeField]
	[Header("ル\u30fcト")]
	private Transform root;
	[SerializeField]
	[Header("プレイヤ\u30fcアイコン")]
	private SpriteRenderer playerIcon;
	private bool isViewPlayerIcon;
	[SerializeField]
	[Header("カ\u30fcソル")]
	private MeshRenderer cursor;
	public void Init()
	{
		base.gameObject.SetActive(value: false);
	}
	public void InitPlay()
	{
		isViewPlayerIcon = false;
		playerIcon.SetAlpha(0f);
		playerIcon.gameObject.SetActive(value: false);
	}
	public void SetBall(int _playerNo)
	{
		ball = SingletonCustom<Golf_BallManager>.Instance.GetBall(_playerNo);
	}
	public void SetPlayerIconSprite(int _userType)
	{
		if (_userType < 4)
		{
			if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
			{
				playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_c_you");
			}
			else
			{
				playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_c_" + (_userType + 1).ToString() + "P");
			}
		}
		else
		{
			playerIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_c_cp" + (_userType - 4 + 1).ToString());
		}
		BoxCollider boxCollider = playerIcon.gameObject.AddComponent<BoxCollider>();
		boxCollider.isTrigger = true;
		boxCollider.size = new Vector3(boxCollider.size.x, 100f, 100f);
	}
	public void SetCursorSprite(Material _mat)
	{
		cursor.material = _mat;
	}
	public void UpdateMethod()
	{
		if (base.gameObject.activeSelf)
		{
			SetScale();
			SetPlayerIconRot();
			SetPos();
			cursor.transform.AddLocalEulerAnglesY(SingletonCustom<Golf_CursorManager>.Instance.GetCursorRotSpeed() * Time.deltaTime);
		}
	}
	private void SetScale()
	{
		Vector3 position = SingletonCustom<Golf_CameraManager>.Instance.GetCamera().GetCameraObj().transform.position;
		position.y = 0f;
		Vector3 position2 = base.transform.position;
		position2.y = 0f;
		float changeScale = SingletonCustom<Golf_CursorManager>.Instance.GetChangeScale(CalcManager.Length(position, position2));
		root.localScale = new Vector3(changeScale, changeScale, changeScale);
	}
	private void SetPlayerIconRot()
	{
		Vector3 a = SingletonCustom<Golf_CameraManager>.Instance.GetCamera().GetCameraObj().transform.position - playerIcon.transform.position;
		a.y = 0f;
		Quaternion rotation = Quaternion.LookRotation(-a);
		playerIcon.transform.rotation = rotation;
	}
	private void SetPos()
	{
		base.transform.position = ball.transform.position;
		root.transform.localPosition = root.transform.localScale.x * SingletonCustom<Golf_CursorManager>.Instance.GetCursorDiffPos();
	}
	public void Show()
	{
		if (!ball.GetIsOB())
		{
			SetScale();
			SetPlayerIconRot();
			SetPos();
			base.gameObject.SetActive(value: true);
		}
	}
	public void SetPlayerIconActive(bool _isFade, bool _isActive)
	{
		if (_isFade)
		{
			if (_isActive)
			{
				isViewPlayerIcon = true;
				playerIcon.gameObject.SetActive(value: true);
			}
			LeanTween.value(playerIcon.gameObject, _isActive ? 0f : 1f, _isActive ? 1f : 0f, 0.5f).setOnUpdate(delegate(float _value)
			{
				playerIcon.SetAlpha(_value);
			}).setOnComplete((Action)delegate
			{
				if (!_isActive)
				{
					playerIcon.gameObject.SetActive(value: false);
				}
			});
		}
		else
		{
			playerIcon.gameObject.SetActive(_isActive);
		}
	}
	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("HitDefaultOnly"))
		{
			UnityEngine.Debug.Log("カメラと接触した");
			if (isViewPlayerIcon && playerIcon.color.a > 0f)
			{
				isViewPlayerIcon = false;
				float time = playerIcon.color.a / 1f * 0.5f;
				LeanTween.cancel(playerIcon.gameObject);
				LeanTween.value(playerIcon.gameObject, playerIcon.color.a, 0f, time).setOnUpdate(delegate(float _value)
				{
					playerIcon.SetAlpha(_value);
				});
			}
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("HitDefaultOnly"))
		{
			UnityEngine.Debug.Log("カメラと接触はずれた");
			if (!isViewPlayerIcon && playerIcon.color.a < 1f)
			{
				isViewPlayerIcon = true;
				float time = (1f - playerIcon.color.a / 1f) * 0.5f;
				LeanTween.cancel(playerIcon.gameObject);
				LeanTween.value(playerIcon.gameObject, playerIcon.color.a, 1f, time).setOnUpdate(delegate(float _value)
				{
					playerIcon.SetAlpha(_value);
				});
			}
		}
	}
}
