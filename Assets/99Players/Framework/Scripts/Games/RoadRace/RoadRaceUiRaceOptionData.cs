using System;
using UnityEngine;
public class RoadRaceUiRaceOptionData : MonoBehaviour
{
	[Serializable]
	public struct SpriteList
	{
		public SpriteRenderer[] sprite;
	}
	private int layoutIdx;
	[SerializeField]
	private GameObject[] uiAnchor;
	[SerializeField]
	private Sprite[] nextLapSpriteList;
	[SerializeField]
	private Sprite[] nextLapSpriteListEn;
	[SerializeField]
	private SpriteList[] nowLapList;
	[SerializeField]
	private SpriteList[] updateLapList;
	[SerializeField]
	private AnimationCurve updateLapScaleCurve = new AnimationCurve();
	[SerializeField]
	private AnimationCurve updateLapAlphaCurve = new AnimationCurve();
	[SerializeField]
	private AnimationCurve updateLapMoveXCurve = new AnimationCurve();
	[SerializeField]
	private CourseMapUI[] courseMapList;
	private CourseMapUI courseMap => courseMapList[layoutIdx];
	public void Init(int _playerNum)
	{
		switch (_playerNum)
		{
		case 2:
			layoutIdx = 1;
			break;
		case 3:
		case 4:
			layoutIdx = 2;
			break;
		}
		uiAnchor[layoutIdx].SetActive(value: true);
		courseMapList[layoutIdx].Init();
		RoadRaceStageScript stageData = Scene_RoadRace.FM.StageData;
		courseMap.SetWorldRangeAnchor(stageData.SizeAnchorRightTop, stageData.SizeAnchorLeftBottom);
		courseMap.SetWorldTargetAnchor(Scene_RoadRace.CM.GetCarTranses());
		courseMap.SetMapData(0);
		courseMap.UpdateMethod();
	}
	public void UpdateMethod()
	{
		courseMap.UpdateMethod();
	}
}
