using UnityEngine;
public class BeachFlag_Player : MonoBehaviour
{
	[SerializeField]
	[Header("BeachFlag_Chara")]
	private BeachFlag_Chara chara;
	[SerializeField]
	[Header("各キャラクタ\u30fcの表示オブジェクト")]
	private GameObject[] charaObj;
	[SerializeField]
	[Header("AI処理")]
	private BeachFlag_AI ai;
	[SerializeField]
	[Header("CharacterStyle")]
	public CharacterStyle characterStyle;
	private bool visible;
	private BeachFlag_Define.UserType userType;
	public bool isPlay;
	public bool goal;
	public bool isover = true;
	public bool Visible
	{
		get
		{
			return visible;
		}
		set
		{
			visible = value;
		}
	}
	public BeachFlag_Chara Chara => chara;
	public BeachFlag_Define.UserType UserType => userType;
	public void Init(BeachFlag_Define.UserType _userType, bool _isPlay)
	{
		userType = _userType;
		visible = true;
		isover = false;
		HideCharacterModeSet();
		characterStyle.SetGameStyle(GS_Define.GameType.DELIVERY_ORDER, (int)userType);
		characterStyle.SetFacial(StyleTextureManager.FacialType.DEFAULT);
		isPlay = _isPlay;
		chara.PlayerInit(this);
		ai.Init();
	}
	private void HideCharacterModeSet()
	{
		for (int i = 0; i < charaObj.Length; i++)
		{
			charaObj[i].SetActive(value: false);
		}
	}
	private static GameObject[] GetChildren(Transform parent)
	{
		GameObject[] array = new GameObject[parent.childCount];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = parent.GetChild(i).gameObject;
		}
		return array;
	}
	public void StartMethod()
	{
		chara.StartMethod();
	}
	public void UpdateMethod()
	{
		bool flag = BeachFlag_Define.GM.GetGameState() >= BeachFlag_GameManager.GameState.GAME_START_WAIT && userType <= BeachFlag_Define.UserType.PLAYER_4;
		if (!((BeachFlag_Define.GM.GetGameState() >= BeachFlag_GameManager.GameState.DURING_GAME) | flag) || !isPlay || chara.processType == BeachFlag_Chara.ProcessType.GOAL)
		{
			return;
		}
		if (userType <= BeachFlag_Define.UserType.PLAYER_4)
		{
			if (!BeachFlag_Define.CM.IsStickTiltDirection(userType, BeachFlag_ControllerManager.StickType.L, BeachFlag_ControllerManager.StickDirType.UP) && !BeachFlag_Define.CM.IsStickTiltDirection(userType, BeachFlag_ControllerManager.StickType.L, BeachFlag_ControllerManager.StickDirType.RIGHT) && !BeachFlag_Define.CM.IsStickTiltDirection(userType, BeachFlag_ControllerManager.StickType.L, BeachFlag_ControllerManager.StickDirType.LEFT))
			{
				BeachFlag_Define.CM.IsStickTiltDirection(userType, BeachFlag_ControllerManager.StickType.L, BeachFlag_ControllerManager.StickDirType.DOWN);
			}
			if (!BeachFlag_Define.CM.IsStickTilt(userType, BeachFlag_ControllerManager.StickType.L) && !BeachFlag_Define.CM.IsPushCrossKey(userType, BeachFlag_ControllerManager.CrossKeyType.UP, BeachFlag_ControllerManager.ButtonPushType.DOWN) && !BeachFlag_Define.CM.IsPushCrossKey(userType, BeachFlag_ControllerManager.CrossKeyType.UP, BeachFlag_ControllerManager.ButtonPushType.HOLD) && !BeachFlag_Define.CM.IsPushCrossKey(userType, BeachFlag_ControllerManager.CrossKeyType.RIGHT, BeachFlag_ControllerManager.ButtonPushType.DOWN) && !BeachFlag_Define.CM.IsPushCrossKey(userType, BeachFlag_ControllerManager.CrossKeyType.RIGHT, BeachFlag_ControllerManager.ButtonPushType.HOLD) && !BeachFlag_Define.CM.IsPushCrossKey(userType, BeachFlag_ControllerManager.CrossKeyType.LEFT, BeachFlag_ControllerManager.ButtonPushType.DOWN) && !BeachFlag_Define.CM.IsPushCrossKey(userType, BeachFlag_ControllerManager.CrossKeyType.LEFT, BeachFlag_ControllerManager.ButtonPushType.HOLD) && !BeachFlag_Define.CM.IsPushCrossKey(userType, BeachFlag_ControllerManager.CrossKeyType.DOWN, BeachFlag_ControllerManager.ButtonPushType.DOWN))
			{
				BeachFlag_Define.CM.IsPushCrossKey(userType, BeachFlag_ControllerManager.CrossKeyType.DOWN, BeachFlag_ControllerManager.ButtonPushType.HOLD);
			}
			if (BeachFlag_Define.CM.IsPushButton_A(userType, BeachFlag_ControllerManager.ButtonPushType.DOWN))
			{
				UnityEngine.Debug.Log("Aボタンを押した");
				chara.AccelInput();
			}
		}
		else
		{
			ai.UpdateMethod();
		}
	}
}
