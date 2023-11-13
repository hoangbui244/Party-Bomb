using UnityEngine;
public class FireworksUIManager : SingletonCustom<FireworksUIManager>
{
	[SerializeField]
	[Header("プレイヤ\u30fcスコア表示")]
	private FireworksPlayerScore[] arrayPlayerScore;
	[SerializeField]
	[Header("プレイヤ\u30fc番号表示")]
	private FireworksOrderList[] arrayPlayerNumber;
	[SerializeField]
	[Header("一人用ボタンレイアウト")]
	private GameObject objSingleButtonLayout;
	[SerializeField]
	[Header("複数人用ボタンレイアウト")]
	private GameObject objMultiButtonLayout;
	[SerializeField]
	[Header("組数表示")]
	private SpriteRenderer spGroup;
	[SerializeField]
	[Header("ゲ\u30fcム時間")]
	private CommonGameTimeUI_Font_Time commonGameTime;
	[SerializeField]
	[Header("上画面カメラ")]
	private Camera topViewCamera;
	[SerializeField]
	[Header("下画面カメラ")]
	private Camera bottomViewCamera;
	[SerializeField]
	[Header("ポイント表示")]
	private FireworksPoint pointPrefab;
	private bool isHideScore;
	private int scoreCount;
	private bool isMoveOut;
	public bool IsHideScore => isHideScore;
	public void Init()
	{
		if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
		{
			objSingleButtonLayout.SetActive(value: true);
			objMultiButtonLayout.SetActive(value: false);
		}
		else
		{
			objSingleButtonLayout.SetActive(value: false);
			objMultiButtonLayout.SetActive(value: true);
		}
		SetGroupIcon();
		commonGameTime.SetTime(90f);
	}
	public void SetGameTime(float _time)
	{
		commonGameTime.SetTime(_time);
	}
	public void SetGroupIcon()
	{
		if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP && SingletonCustom<GameSettingManager>.Instance.PlayerNum > 2)
		{
			spGroup.gameObject.SetActive(value: true);
			if (SingletonCustom<FireworksPlayerManager>.Instance.IsGroup1Playing)
			{
				spGroup.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_play_1group");
			}
			else
			{
				spGroup.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_play_2group");
			}
		}
		else
		{
			spGroup.gameObject.SetActive(value: false);
		}
	}
	public void SetUserUIData(FireworksPlayerManager.UserData[] _userDatas)
	{
		FireworksDefine.UserType[] array = new FireworksDefine.UserType[FireworksDefine.MAX_JOIN_MEMBER_NUM];
		FireworksDefine.TeamType[] array2 = new FireworksDefine.TeamType[FireworksDefine.MAX_JOIN_MEMBER_NUM];
		for (int i = 0; i < _userDatas.Length; i++)
		{
			array[i] = _userDatas[i].userType;
			array2[i] = _userDatas[i].teamType;
		}
		for (int j = 0; j < arrayPlayerScore.Length; j++)
		{
			arrayPlayerScore[j].Init(_userDatas[j].player, _userDatas[j].teamType);
			arrayPlayerNumber[j].Init(_userDatas[j].player);
		}
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < arrayPlayerScore.Length; i++)
		{
			arrayPlayerScore[i].UpdateMethod();
		}
		for (int j = 0; j < arrayPlayerNumber.Length; j++)
		{
			CalcManager.mCalcVector3 = bottomViewCamera.WorldToScreenPoint(SingletonCustom<FireworksPlayerManager>.Instance.GetArrayPlayer()[j].transform.position);
			CalcManager.mCalcVector3 = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(CalcManager.mCalcVector3);
			CalcManager.mCalcVector3.y += 80f;
			arrayPlayerNumber[j].transform.position = CalcManager.mCalcVector3;
		}
		if (SingletonCustom<FireworksGameManager>.Instance.GameTime <= 10f && !isMoveOut)
		{
			for (int k = 0; k < arrayPlayerScore.Length; k++)
			{
				LeanTween.moveLocalY(arrayPlayerScore[k].gameObject, arrayPlayerScore[k].transform.localPosition.y + 500f, 1.25f).setEaseInQuint().setDelay((float)k * 0.15f);
			}
			isMoveOut = true;
		}
	}
	public FireworksOrderList GetOrderList(FireworksPlayer _player)
	{
		for (int i = 0; i < arrayPlayerNumber.Length; i++)
		{
			if (arrayPlayerNumber[i].Player == _player)
			{
				return arrayPlayerNumber[i];
			}
		}
		return null;
	}
	public void ShowPoint(FireworksPlayer _player, Vector3 _pos, int _score, bool _isTopViewCamera)
	{
		if (!_isTopViewCamera)
		{
			CalcManager.mCalcVector3 = bottomViewCamera.WorldToScreenPoint(_pos);
			CalcManager.mCalcVector3 = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(CalcManager.mCalcVector3);
			CalcManager.mCalcVector3.x += Random.Range(-20f, 20f);
			CalcManager.mCalcVector3.y += Random.Range(0f, 20f);
		}
		else
		{
			CalcManager.mCalcVector3 = topViewCamera.WorldToScreenPoint(_pos);
			CalcManager.mCalcVector3 = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(CalcManager.mCalcVector3);
			CalcManager.mCalcVector3.y += 100f;
		}
		_player.AddScore(_score);
		FireworksPoint fireworksPoint = Object.Instantiate(pointPrefab, base.transform);
		fireworksPoint.transform.SetLocalPosition(CalcManager.mCalcVector3.x, CalcManager.mCalcVector3.y, 500f - CalcManager.mCalcVector3.y / 1000f - (float)scoreCount * 0.01f);
		fireworksPoint.Set(_player.UserType, _score);
		fireworksPoint.gameObject.SetActive(value: true);
		scoreCount++;
	}
	private new void OnDestroy()
	{
		base.OnDestroy();
		for (int i = 0; i < arrayPlayerScore.Length; i++)
		{
			LeanTween.cancel(arrayPlayerScore[i].gameObject);
		}
	}
}
