using UnityEngine;
public class GS_CourseSelect : MonoBehaviour
{
	[SerializeField]
	[Header("3コ\u30fcス選択のル\u30fcト")]
	private GameObject threeCourseSelectRoot;
	[SerializeField]
	[Header("5コ\u30fcス選択のル\u30fcト")]
	private GameObject fiveCourseSelectRoot;
	[SerializeField]
	[Header("3コ\u30fcス選択カ\u30fcソル")]
	private CursorManager threeCursor;
	[SerializeField]
	[Header("5コ\u30fcス選択カ\u30fcソル")]
	private CursorManager fiveCursor;
	[SerializeField]
	[Header("最高記録管理クラス")]
	private GS_BestRecord bestRecord;
	[SerializeField]
	[Header("フレ\u30fcム画像")]
	private SpriteRenderer backFrame;
	[SerializeField]
	[Header("サムネイル")]
	private SpriteRenderer thumb;
	[SerializeField]
	[Header("雑巾がけコ\u30fcスサムネイル")]
	private Sprite[] arrayDustClothRaceThumb;
	[SerializeField]
	[Header("消しゴムレ\u30fcスコ\u30fcスサムネイル")]
	private Sprite[] arrayEraserRaceThumb;
	private GS_Define.GameType currentType;
	public void SetThreeCourse(GS_Define.GameType _type)
	{
		threeCourseSelectRoot.SetActive(value: true);
		fiveCourseSelectRoot.SetActive(value: false);
		currentType = _type;
		base.gameObject.SetActive(value: true);
		threeCursor.SetCursorPos(0, SingletonCustom<GameSettingManager>.Instance.SelectCourseIdx);
		SetBackFrame(_type);
	}
	public void SetFiveCourse(GS_Define.GameType _type)
	{
		threeCourseSelectRoot.SetActive(value: false);
		fiveCourseSelectRoot.SetActive(value: true);
		currentType = _type;
		base.gameObject.SetActive(value: true);
		fiveCursor.SetCursorPos(0, SingletonCustom<GameSettingManager>.Instance.SelectCourseIdx);
		SetBackFrame(_type);
	}
	public void Close()
	{
		base.gameObject.SetActive(value: false);
	}
	private void Update()
	{
		if (threeCourseSelectRoot.activeSelf)
		{
			if (threeCursor.IsPushMovedButtonMoment(CursorManager.MoveDir.RIGHT))
			{
				SingletonCustom<GameSettingManager>.Instance.SelectCourseIdx = (SingletonCustom<GameSettingManager>.Instance.SelectCourseIdx + 1) % threeCursor.GetButtonObjLength(0);
				thumb.sprite = arrayDustClothRaceThumb[SingletonCustom<GameSettingManager>.Instance.SelectCourseIdx];
				if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
				{
					bestRecord.Set(currentType);
				}
			}
			else if (threeCursor.IsPushMovedButtonMoment(CursorManager.MoveDir.LEFT))
			{
				SingletonCustom<GameSettingManager>.Instance.SelectCourseIdx = (SingletonCustom<GameSettingManager>.Instance.SelectCourseIdx + (threeCursor.GetButtonObjLength(0) - 1)) % threeCursor.GetButtonObjLength(0);
				thumb.sprite = arrayDustClothRaceThumb[SingletonCustom<GameSettingManager>.Instance.SelectCourseIdx];
				if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
				{
					bestRecord.Set(currentType);
				}
			}
		}
		if (!fiveCourseSelectRoot.activeSelf)
		{
			return;
		}
		if (fiveCursor.IsPushMovedButtonMoment(CursorManager.MoveDir.RIGHT))
		{
			SingletonCustom<GameSettingManager>.Instance.SelectCourseIdx = (SingletonCustom<GameSettingManager>.Instance.SelectCourseIdx + 1) % fiveCursor.GetButtonObjLength(0);
			thumb.sprite = arrayEraserRaceThumb[SingletonCustom<GameSettingManager>.Instance.SelectCourseIdx];
			if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
			{
				bestRecord.Set(currentType);
			}
		}
		else if (fiveCursor.IsPushMovedButtonMoment(CursorManager.MoveDir.LEFT))
		{
			SingletonCustom<GameSettingManager>.Instance.SelectCourseIdx = (SingletonCustom<GameSettingManager>.Instance.SelectCourseIdx + (fiveCursor.GetButtonObjLength(0) - 1)) % fiveCursor.GetButtonObjLength(0);
			thumb.sprite = arrayEraserRaceThumb[SingletonCustom<GameSettingManager>.Instance.SelectCourseIdx];
			if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
			{
				bestRecord.Set(currentType);
			}
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
