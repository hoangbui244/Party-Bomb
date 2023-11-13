using UnityEngine;
public class SmeltFishing_UI : SingletonCustom<SmeltFishing_UI>
{
	[SerializeField]
	private SmeltFishing_UILayout singleLayout;
	[SerializeField]
	private SmeltFishing_UILayout multiLayout;
	private bool IsSinglePlay => SingletonCustom<GameSettingManager>.Instance.IsSinglePlay;
	private SmeltFishing_UILayout ActiveUILayout
	{
		get;
		set;
	}
	public void Init()
	{
		if (IsSinglePlay)
		{
			singleLayout.Init();
			multiLayout.Disable();
			ActiveUILayout = singleLayout;
		}
		else
		{
			multiLayout.Init();
			singleLayout.Disable();
			ActiveUILayout = multiLayout;
		}
		UpdateGameTime(120f);
	}
	public void UpdateGameTime(float time)
	{
		ActiveUILayout.UpdateGameTime(time);
	}
	public void SetPlayerIcon(int no, int _controlUser)
	{
		Sprite sprite = null;
		switch (_controlUser)
		{
		case 0:
			sprite = ((SingletonCustom<GameSettingManager>.Instance.PlayerNum != 1) ? SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_" + (_controlUser + 1).ToString() + "p") : SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, (Localize_Define.Language == Localize_Define.LanguageType.Japanese) ? "_screen_you" : "en_screen_you"));
			break;
		case 1:
		case 2:
		case 3:
			sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_" + (_controlUser + 1).ToString() + "p");
			break;
		case 4:
		case 5:
		case 6:
		case 7:
			if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
			{
				return;
			}
			sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp" + (_controlUser - 4 + 1).ToString());
			break;
		}
		ActiveUILayout.SetPlayerIcon(no, sprite);
	}
	public void SetCharaIcon(int no, int _controlUser)
	{
		Sprite sprite = null;
		switch (SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[_controlUser])
		{
		case 0:
			sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_yuto_02");
			break;
		case 1:
			sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_hina_02");
			break;
		case 2:
			sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_ituki_02");
			break;
		case 3:
			sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_souta_02");
			break;
		case 4:
			sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_takumi_02");
			break;
		case 5:
			sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_rin_02");
			break;
		case 6:
			sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_akira_02");
			break;
		case 7:
			sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "character_rui_02");
			break;
		}
		ActiveUILayout.SetCharaIcon(no, sprite);
	}
	public void UpdateScore(int no, int score)
	{
		ActiveUILayout.UpdateScore(no, score);
	}
	public void SetControlModeMoving(int no)
	{
		UnityEngine.Debug.Log("SetControlModeMoving");
		ActiveUILayout.SetControlModeMoving(no);
	}
	public void SetControlModeCastLine(int no)
	{
		UnityEngine.Debug.Log("SetControlModeCastLine");
		ActiveUILayout.SetControlModeCastLine(no);
	}
	public void SetControlModeRollUp(int no)
	{
		UnityEngine.Debug.Log("SetControlModeRollUp");
		ActiveUILayout.SetControlModeRollUp(no);
	}
	public void ShowHit(int no)
	{
		ActiveUILayout.ShowHit(no);
	}
	public void ShowGet(int no, int count)
	{
		ActiveUILayout.ShowGet(no, count);
	}
	public void HideCurrentIfNeeded(int no)
	{
		ActiveUILayout.HideCurrentIfNeeded(no);
	}
	public void ShowAssistComment(int no, int assistIndex, bool isForceHide = false)
	{
		ActiveUILayout.ShowAssistComment(no, assistIndex, isForceHide);
	}
	public void HideAssistComment(int no)
	{
		ActiveUILayout.HideAssistComment(no);
	}
	public void ForceHideAssistComment(int no)
	{
		ActiveUILayout.ForceHideAssistComment(no);
	}
	public bool IsOnceAssistComment(int no, int assistIndex)
	{
		return ActiveUILayout.IsOnceAssistComment(no, assistIndex);
	}
	public void ShowAssistCommentAll(int assistIndex)
	{
		ActiveUILayout.ShowAssistCommentAll(assistIndex);
	}
	public void ShowOperationInformation(int no, int infoIndex)
	{
		ActiveUILayout.ShowOperationInformation(no, infoIndex);
	}
	public bool IsOnceOperationInformation(int no, int infoIndex)
	{
		return ActiveUILayout.IsOnceOperationInformation(no, infoIndex);
	}
	public void ShowOperationInformationAll(int infoIndex)
	{
		ActiveUILayout.ShowOperationInformationAll(infoIndex);
	}
	public void HideOperationInformation(int no)
	{
		ActiveUILayout.HideOperationInformation(no);
	}
	public void HideOperationInformationAll()
	{
		ActiveUILayout.HideOperationInformationAll();
	}
	public void ShowBaitDepth(int no, float _smeltValue)
	{
		ActiveUILayout.ShowBaitDepth(no, _smeltValue);
	}
	public void HideBaitDepth(int no)
	{
		ActiveUILayout.HideBaitDepth(no);
	}
	public void SetProperDepth(int no, float properDepth)
	{
		ActiveUILayout.SetProperDepth(no, properDepth);
	}
	public void SetDepth(int no, float depth)
	{
		ActiveUILayout.SetDepth(no, depth);
	}
	public void SetDepthImmediate(int no, float depth)
	{
		ActiveUILayout.SetDepthImmediate(no, depth);
	}
	public void SetClipPosition(int no, Vector3 position)
	{
		ActiveUILayout.SetClipPosition(no, position);
	}
	public void SetProperDepthRangeActive(int no, bool _isActive)
	{
		ActiveUILayout.SetProperDepthRangeActive(no, _isActive);
	}
	public void SetDepthArrowMoveDir(int no, bool _isRollUp)
	{
		ActiveUILayout.SetDepthArrowMoveDir(no, _isRollUp);
	}
	public bool IsRollUpComplete(int no)
	{
		return ActiveUILayout.IsRollUpComplete(no);
	}
	public bool IsArrowGaugeCenterDown(int no)
	{
		return ActiveUILayout.IsArrowGaugeCenterDown(no);
	}
	public bool IsArrowMaxDepthPosition(int no)
	{
		return ActiveUILayout.IsArrowMaxDepthPosition(no);
	}
	public bool IsDownDepthArrowToProperDepth(int no)
	{
		return ActiveUILayout.IsDownDepthArrowToProperDepth(no);
	}
	public bool CheckHitRange(int no, float _diff)
	{
		return ActiveUILayout.CheckHitRange(no, _diff);
	}
	public bool CheckHitRange(int no)
	{
		return ActiveUILayout.CheckHitRange(no);
	}
	public void PlayHitEffect(int no)
	{
		ActiveUILayout.PlayHitEffect(no);
	}
	public void SetProperSize(int no, float _smeltValue, float _time = 0.5f)
	{
		ActiveUILayout.SetProperSize(no, _smeltValue, _time);
	}
	public void ShowSitDownUI(int no)
	{
		ActiveUILayout.ShowSitDownUI(no);
	}
	public void HideSitDownUI(int no)
	{
		ActiveUILayout.HideSitDownUI(no);
	}
}
