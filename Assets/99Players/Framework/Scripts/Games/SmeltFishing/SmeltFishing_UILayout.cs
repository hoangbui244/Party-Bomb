using UnityEngine;
public class SmeltFishing_UILayout : MonoBehaviour
{
	[SerializeField]
	private GameObject[] uiRoots;
	[SerializeField]
	private CommonGameTimeUI_Font_Time timeUI;
	[SerializeField]
	private SmeltFishing_PlayerUI[] playersUI;
	public void Init()
	{
		Enable();
		SmeltFishing_PlayerUI[] array = playersUI;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Init();
		}
	}
	public void Enable()
	{
		GameObject[] array = uiRoots;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
	}
	public void Disable()
	{
		GameObject[] array = uiRoots;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
	}
	public void UpdateGameTime(float time)
	{
		timeUI.SetTime(time);
	}
	public void SetPlayerIcon(int no, Sprite _sprite)
	{
		playersUI[no].SetPlayerIcon(_sprite);
	}
	public void SetCharaIcon(int no, Sprite _sprite)
	{
		playersUI[no].SetCharaIcon(_sprite);
	}
	public void UpdateScore(int no, int score)
	{
		playersUI[no].UpdateScore(score);
	}
	public void SetControlModeMoving(int no)
	{
		playersUI[no].SetControlModeMoving();
	}
	public void SetControlModeCastLine(int no)
	{
		playersUI[no].SetControlModeCastLine();
	}
	public void SetControlModeRollUp(int no)
	{
		playersUI[no].SetControlModeRollUp();
	}
	public void ShowHit(int no)
	{
		playersUI[no].ShowHit();
	}
	public void ShowGet(int no, int count)
	{
		playersUI[no].ShowGet(count);
	}
	public void HideCurrentIfNeeded(int no)
	{
		playersUI[no].HideCurrentIfNeeded();
	}
	public void ShowAssistComment(int no, int assistIndex, bool isForceHide = false)
	{
		playersUI[no].ShowAssistComment(assistIndex, isForceHide);
	}
	public void HideAssistComment(int no)
	{
		playersUI[no].HideAssistComment();
	}
	public void ForceHideAssistComment(int no)
	{
		playersUI[no].ForceHideAssistComment();
	}
	public bool IsOnceAssistComment(int no, int assistIndex)
	{
		return playersUI[no].IsOnceAssistComment(assistIndex);
	}
	public void ShowAssistCommentAll(int assistIndex)
	{
		SmeltFishing_PlayerUI[] array = playersUI;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ShowAssistComment(assistIndex);
		}
	}
	public void ShowOperationInformation(int no, int infoIndex)
	{
		playersUI[no].ShowOperationInformation(infoIndex);
	}
	public bool IsOnceOperationInformation(int no, int infoIndex)
	{
		return playersUI[no].IsOnceOperationInformation(infoIndex);
	}
	public void ShowOperationInformationAll(int infoIndex)
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerNum; i++)
		{
			playersUI[i].ShowOperationInformation(infoIndex);
		}
	}
	public void HideOperationInformation(int no)
	{
		playersUI[no].HideOperationInformation();
	}
	public void HideOperationInformationAll()
	{
		for (int i = 0; i < SingletonCustom<GameSettingManager>.Instance.PlayerNum; i++)
		{
			playersUI[i].HideOperationInformation();
		}
	}
	public void ShowBaitDepth(int no, float _smeltValue)
	{
		playersUI[no].ShowBaitDepth(_smeltValue);
	}
	public void HideBaitDepth(int no)
	{
		playersUI[no].HideBaitDepth();
	}
	public void SetProperDepth(int no, float properDepth)
	{
		playersUI[no].SetProperDepth(properDepth);
	}
	public void SetDepth(int no, float depth)
	{
		playersUI[no].SetDepth(depth);
	}
	public void SetDepthImmediate(int no, float depth)
	{
		playersUI[no].SetDepthImmediate(depth);
	}
	public void SetClipPosition(int no, Vector3 position)
	{
		playersUI[no].SetClipPosition(position);
	}
	public void SetProperDepthRangeActive(int no, bool _isActive)
	{
		playersUI[no].SetProperDepthRangeActive(_isActive);
	}
	public void SetDepthArrowMoveDir(int no, bool _isRollUp)
	{
		playersUI[no].SetDepthArrowMoveDir(_isRollUp);
	}
	public bool IsRollUpComplete(int no)
	{
		return playersUI[no].IsRollUpComplete();
	}
	public bool IsArrowGaugeCenterDown(int no)
	{
		return playersUI[no].IsArrowGaugeCenterDown();
	}
	public bool IsArrowMaxDepthPosition(int no)
	{
		return playersUI[no].IsArrowMaxDepthPosition();
	}
	public bool IsDownDepthArrowToProperDepth(int no)
	{
		return playersUI[no].IsDownDepthArrowToProperDepth();
	}
	public bool CheckHitRange(int no, float _diff)
	{
		return playersUI[no].CheckHitRange(_diff);
	}
	public bool CheckHitRange(int no)
	{
		return playersUI[no].CheckHitRange();
	}
	public void PlayHitEffect(int no)
	{
		playersUI[no].PlayHitEffect();
	}
	public void SetProperSize(int no, float _smeltValue, float _time)
	{
		playersUI[no].SetProperSize(_smeltValue, _time);
	}
	public void ShowSitDownUI(int no)
	{
		playersUI[no].ShowSitDownUI(no);
	}
	public void HideSitDownUI(int no)
	{
		playersUI[no].HideSitDownUI();
	}
}
