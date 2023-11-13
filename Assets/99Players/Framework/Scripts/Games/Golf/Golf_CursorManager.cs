using UnityEngine;
public class Golf_CursorManager : SingletonCustom<Golf_CursorManager>
{
	[SerializeField]
	[Header("ル\u30fcト")]
	private GameObject root;
	[SerializeField]
	[Header("カ\u30fcソル")]
	private Golf_Cursor[] arrayCursor;
	[SerializeField]
	[Header("カ\u30fcソルのマテリアル")]
	private Material[] arrayCursorMaterial;
	[SerializeField]
	[Header("ボ\u30fcルの位置との補正座標")]
	private Vector3 CURSOR_DIFF_POS;
	private float CURSOR_MAX_SCALE_DISTANCE;
	[SerializeField]
	[Header("カ\u30fcソルを最小サイズで表示する時の距離（最大距離に対する倍率）")]
	private float CURSOR_MIN_SCALE_MAG;
	private float CURSOR_MIN_SCALE_DISTANCE;
	[SerializeField]
	[Header("カ\u30fcソルの最大サイズ")]
	private float CURSOR_MAX_SCALE;
	[SerializeField]
	[Header("カ\u30fcソルの回転速度")]
	private float CURSOR_ROT_SPEED;
	public void Init()
	{
		for (int i = 0; i < arrayCursor.Length; i++)
		{
			int num = SingletonCustom<GameSettingManager>.Instance.PlayerGroupList[i][0];
			int num2 = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[num];
			arrayCursor[i].Init();
			arrayCursor[i].SetBall(i);
			arrayCursor[i].SetPlayerIconSprite(num);
			arrayCursor[i].SetCursorSprite(arrayCursorMaterial[num2]);
		}
		Vector3 position = SingletonCustom<Golf_CameraManager>.Instance.GetCamera().GetCameraObj().transform.position;
		position.y = 0f;
		Vector3 cupPos = SingletonCustom<Golf_FieldManager>.Instance.GetHole().GetCupPos();
		cupPos.y = 0f;
		CURSOR_MAX_SCALE_DISTANCE = CalcManager.Length(position, cupPos);
		CURSOR_MIN_SCALE_DISTANCE = CURSOR_MAX_SCALE_DISTANCE * CURSOR_MIN_SCALE_MAG;
		InitPlay(_isInit: true);
	}
	public void InitPlay(bool _isInit = false)
	{
		for (int i = 0; i < arrayCursor.Length; i++)
		{
			arrayCursor[i].InitPlay();
			if (!_isInit && i == SingletonCustom<Golf_PlayerManager>.Instance.GetTurnPlayer().GetPlayerNo())
			{
				arrayCursor[i].Hide();
			}
		}
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < arrayCursor.Length; i++)
		{
			arrayCursor[i].UpdateMethod();
		}
	}
	public void Show(int _playerNo)
	{
		arrayCursor[_playerNo].Show();
	}
	public void SetAllPlayerIconActive(bool _isFade, bool _isActive)
	{
		for (int i = 0; i < arrayCursor.Length; i++)
		{
			arrayCursor[i].SetPlayerIconActive(_isFade, _isActive);
		}
	}
	public void Hide(int _playerNo)
	{
		arrayCursor[_playerNo].Hide();
	}
	public void SetRootActive(bool _isActive)
	{
		root.SetActive(_isActive);
	}
	public Vector3 GetCursorDiffPos()
	{
		return CURSOR_DIFF_POS;
	}
	public float GetChangeScale(float _distance)
	{
		return ClampDistance(_distance, CURSOR_MIN_SCALE_DISTANCE, CURSOR_MAX_SCALE_DISTANCE) / CURSOR_MAX_SCALE_DISTANCE * CURSOR_MAX_SCALE;
	}
	public float ClampDistance(float _distance, float _min, float _max)
	{
		return Mathf.Clamp(_distance, _min, _max);
	}
	public float GetCursorRotSpeed()
	{
		return CURSOR_ROT_SPEED;
	}
}
