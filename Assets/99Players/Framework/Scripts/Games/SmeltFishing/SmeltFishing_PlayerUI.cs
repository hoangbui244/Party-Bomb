using UnityEngine;
public class SmeltFishing_PlayerUI : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer playerIcon;
	[SerializeField]
	private SpriteRenderer charaIcon;
	[SerializeField]
	private SpriteNumbers scoreLabel;
	[SerializeField]
	private SmeltFishing_OperationUI operation;
	[SerializeField]
	private SmeltFishing_FishingUI fishingUI;
	[SerializeField]
	private SmeltFishing_BaitDepthUI baitDepthUI;
	[SerializeField]
	private SmeltFishing_AssistComment assistComment;
	[SerializeField]
	private SmeltFishing_OperationInformationSwticher operationInformationSwticher;
	[SerializeField]
	private SmeltFishing_SitDownUI sitDownUI;
	public void Init()
	{
		UpdateScore(0);
		if ((bool)operation)
		{
			operation.Init();
		}
		if ((bool)fishingUI)
		{
			fishingUI.Init();
		}
		if ((bool)assistComment)
		{
			assistComment.Init();
		}
		if ((bool)operationInformationSwticher)
		{
			operationInformationSwticher.Init();
		}
		if ((bool)baitDepthUI)
		{
			baitDepthUI.Init();
		}
		if ((bool)sitDownUI)
		{
			sitDownUI.Init();
		}
	}
	public void SetPlayerIcon(Sprite _sprite)
	{
		playerIcon.sprite = _sprite;
	}
	public void SetCharaIcon(Sprite _sprite)
	{
		charaIcon.sprite = _sprite;
	}
	public void UpdateScore(int score)
	{
		scoreLabel.Set(score);
	}
	public void SetControlModeMoving()
	{
		if (!(operation == null))
		{
			UnityEngine.Debug.Log("操作説明を移動用に切り替える");
			operation.ShowMoveControl();
			operation.SetActionControlAsSitDown();
			operation.HideCancelControl();
			operation.HidePullUpControl();
		}
	}
	public void SetControlModeCastLine()
	{
		if (!(operation == null))
		{
			UnityEngine.Debug.Log("操作説明を釣り開始用に切り替える");
			operation.HideMoveControl();
			operation.SetActionControlAsCastLine();
			operation.ShowCancelControl();
		}
	}
	public void SetControlModeRollUp()
	{
		if (!(operation == null))
		{
			UnityEngine.Debug.Log("操作説明を釣り上げ用に切り替える");
			operation.HideMoveControl();
			operation.SetActionControlAsRollUp();
			operation.HideCancelControl();
		}
	}
	public void ShowHit()
	{
		if ((bool)fishingUI)
		{
			fishingUI.ShowHit();
		}
	}
	public void ShowGet(int count)
	{
		if ((bool)fishingUI)
		{
			fishingUI.ShowGet();
		}
	}
	public void HideCurrentIfNeeded()
	{
		if ((bool)fishingUI)
		{
			fishingUI.HideCurrentIfNeeded();
		}
	}
	public void ShowAssistComment(int assistIndex, bool isForceHide = false)
	{
		if (!(assistComment == null))
		{
			assistComment.ShowAssistComment(assistIndex, isForceHide);
		}
	}
	public void HideAssistComment()
	{
		if (!(assistComment == null))
		{
			assistComment.HideAssistComment();
		}
	}
	public void ForceHideAssistComment()
	{
		if (!(assistComment == null))
		{
			assistComment.ForceHideAssistComment();
		}
	}
	public bool IsOnceAssistComment(int assistIndex)
	{
		if (assistComment == null)
		{
			return false;
		}
		return assistComment.IsOnceAssistComment(assistIndex);
	}
	public void ShowOperationInformation(int infoIndex)
	{
		if (!(operationInformationSwticher == null))
		{
			operationInformationSwticher.ShowOperationInformation(infoIndex);
		}
	}
	public bool IsOnceOperationInformation(int infoIndex)
	{
		if (operationInformationSwticher == null)
		{
			return false;
		}
		return operationInformationSwticher.IsOnceOperationInformation(infoIndex);
	}
	public void HideOperationInformation()
	{
		if (!(operationInformationSwticher == null))
		{
			operationInformationSwticher.HideOperationInformation();
		}
	}
	public void ShowBaitDepth(float _smeltValue)
	{
		if ((bool)baitDepthUI)
		{
			baitDepthUI.Show(_smeltValue);
		}
	}
	public void HideBaitDepth()
	{
		if ((bool)baitDepthUI)
		{
			baitDepthUI.Hide();
		}
	}
	public void SetProperDepth(float properDepth)
	{
		if ((bool)baitDepthUI)
		{
			baitDepthUI.SetProperDepth(properDepth);
		}
	}
	public void SetDepth(float depth)
	{
		if ((bool)baitDepthUI)
		{
			baitDepthUI.SetDepth(depth);
		}
	}
	public void SetDepthImmediate(float depth)
	{
		if ((bool)baitDepthUI)
		{
			baitDepthUI.SetDepthImmediate(depth);
		}
	}
	public void SetClipPosition(Vector3 position)
	{
		if ((bool)sitDownUI)
		{
			sitDownUI.SetClipPosition(position);
		}
	}
	public void SetProperDepthRangeActive(bool _isActive)
	{
		if ((bool)baitDepthUI)
		{
			baitDepthUI.SetProperDepthRangeActive(_isActive);
		}
	}
	public void SetDepthArrowMoveDir(bool _isRollUp)
	{
		if ((bool)baitDepthUI)
		{
			baitDepthUI.SetDepthArrowMoveDir(_isRollUp);
		}
	}
	public bool IsRollUpComplete()
	{
		if ((bool)baitDepthUI)
		{
			return baitDepthUI.IsRollUpComplete();
		}
		return false;
	}
	public bool IsArrowGaugeCenterDown()
	{
		if ((bool)baitDepthUI)
		{
			return baitDepthUI.IsArrowGaugeCenterDown();
		}
		return false;
	}
	public bool IsArrowMaxDepthPosition()
	{
		if ((bool)baitDepthUI)
		{
			return baitDepthUI.IsArrowMaxDepthPosition();
		}
		return false;
	}
	public bool IsDownDepthArrowToProperDepth()
	{
		if ((bool)baitDepthUI)
		{
			return baitDepthUI.IsDownDepthArrowToProperDepth();
		}
		return false;
	}
	public bool CheckHitRange(float _diff)
	{
		if ((bool)baitDepthUI)
		{
			return baitDepthUI.CheckHitRange(_diff);
		}
		return false;
	}
	public bool CheckHitRange()
	{
		if ((bool)baitDepthUI)
		{
			return baitDepthUI.CheckHitRange();
		}
		return false;
	}
	public void PlayHitEffect()
	{
		if ((bool)baitDepthUI)
		{
			baitDepthUI.PlayHitEffect();
		}
	}
	public void SetProperSize(float _smeltValue, float _time)
	{
		if ((bool)baitDepthUI)
		{
			baitDepthUI.SetProperSize(_smeltValue, _time);
		}
	}
	public void ShowSitDownUI(int no)
	{
		if ((bool)sitDownUI)
		{
			sitDownUI.Show(no);
		}
	}
	public void HideSitDownUI()
	{
		if ((bool)sitDownUI)
		{
			sitDownUI.Hide();
		}
	}
}
