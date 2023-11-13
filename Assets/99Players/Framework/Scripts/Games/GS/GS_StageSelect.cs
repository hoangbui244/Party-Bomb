using UnityEngine;
public class GS_StageSelect : MonoBehaviour
{
	[SerializeField]
	[Header("ステ\u30fcジ選択カ\u30fcソル")]
	private CursorManager stageSelectCurosr;
	[SerializeField]
	[Header("フレ\u30fcム画像")]
	private SpriteRenderer backFrame;
	[SerializeField]
	[Header("サムネイル")]
	private SpriteRenderer thumb;
	[SerializeField]
	[Header("定規バトルサムネイル")]
	private Sprite[] arrayRulerBattleThumb;
	public void Set(GS_Define.GameType _type)
	{
		base.gameObject.SetActive(value: true);
		stageSelectCurosr.SetCursorPos(0, SingletonCustom<GameSettingManager>.Instance.SelectStageIdx);
		SetBackFrame(_type);
	}
	public void Close()
	{
		base.gameObject.SetActive(value: false);
	}
	private void Update()
	{
		if (stageSelectCurosr.IsPushMovedButtonMoment(CursorManager.MoveDir.RIGHT))
		{
			SingletonCustom<GameSettingManager>.Instance.SelectStageIdx = (SingletonCustom<GameSettingManager>.Instance.SelectStageIdx + 1) % stageSelectCurosr.GetButtonObjLength(0);
			thumb.sprite = arrayRulerBattleThumb[SingletonCustom<GameSettingManager>.Instance.SelectStageIdx];
		}
		else if (stageSelectCurosr.IsPushMovedButtonMoment(CursorManager.MoveDir.LEFT))
		{
			SingletonCustom<GameSettingManager>.Instance.SelectStageIdx = (SingletonCustom<GameSettingManager>.Instance.SelectStageIdx + (stageSelectCurosr.GetButtonObjLength(0) - 1)) % stageSelectCurosr.GetButtonObjLength(0);
			thumb.sprite = arrayRulerBattleThumb[SingletonCustom<GameSettingManager>.Instance.SelectStageIdx];
		}
	}
	private void SetBackFrame(GS_Define.GameType _type)
	{
		switch (_type)
		{
		case GS_Define.GameType.GET_BALL:
		case GS_Define.GameType.CANNON_SHOT:
		case GS_Define.GameType.BLOCK_WIPER:
		case GS_Define.GameType.MOLE_HAMMER:
		case GS_Define.GameType.BOMB_ROULETTE:
		case GS_Define.GameType.RECEIVE_PON:
			return;
		}
	}
}
