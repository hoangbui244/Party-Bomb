using System.Collections;
using UnityEngine;
public class RoadRaceFieldManager : SingletonCustom<RoadRaceFieldManager>
{
	[SerializeField]
	private RoadRaceStageScript[] stagePrefList;
	private RoadRaceStageScript stage;
	[SerializeField]
	[Header("コ\u30fcス生成アンカ\u30fc")]
	private Transform createAnchor;
	private Collider[] raceCheckPoint;
	[SerializeField]
	[Header("キャラアンカ\u30fc")]
	private Transform characterAnchor;
	private RoadRaceAipoint[] aiPoints;
	private int stageNo;
	private RoadRaceDefine.PlayBicycleType playBicycleType;
	[SerializeField]
	[Header("チュ\u30fcトリステ\u30fcジ")]
	private RoadRaceStageScript tutorialStage;
	public RoadRaceStageScript StageData => stage;
	public Collider[] RaceCheckPoint => raceCheckPoint;
	public Transform CharacterAnchor => characterAnchor;
	public RoadRaceAipoint[] AiPoints => aiPoints;
	public int StageNo => stageNo;
	public RoadRaceDefine.PlayBicycleType PlayBicycleType => playBicycleType;
	public int GetCheckPointLength()
	{
		return raceCheckPoint.Length;
	}
	public int GetCheckPointNo(GameObject _obj, int _add = 0)
	{
		for (int i = 0; i < raceCheckPoint.Length; i++)
		{
			if (raceCheckPoint[i].gameObject == _obj)
			{
				return i;
			}
		}
		return -1;
	}
	public void Init()
	{
		stageNo = Random.Range(0, stagePrefList.Length);
		stageNo = 0;
		stage = Object.Instantiate(stagePrefList[stageNo], createAnchor.position, Quaternion.identity, createAnchor);
		StandStartStopper(_stand: true, _lerp: false);
		raceCheckPoint = stage.RaceCheckPoints;
		aiPoints = stage.AiPoints;
		for (int i = 0; i < aiPoints.Length; i++)
		{
			aiPoints[i].SetAiPointID(i);
		}
		playBicycleType = RoadRaceDefine.PlayBicycleType.ROAD;
	}
	public int GetAiPointNo(Collider _col)
	{
		for (int i = 0; i < AiPoints.Length; i++)
		{
			if (_col.gameObject == AiPoints[i].gameObject)
			{
				return i;
			}
		}
		return 0;
	}
	public void StandStartStopper(bool _stand, bool _lerp = true)
	{
		if (_lerp)
		{
			StartCoroutine(_StandStartStopper(_stand));
		}
		else if (stage.StartStopper != null)
		{
			if (_stand)
			{
				stage.StartStopper.SetLocalEulerAnglesX(0f);
			}
			else
			{
				stage.StartStopper.SetLocalEulerAnglesX(90f);
			}
		}
	}
	private IEnumerator _StandStartStopper(bool _stand)
	{
		if (!(stage.StartStopper != null))
		{
			yield break;
		}
		if (_stand)
		{
			stage.StartStopper.SetLocalEulerAnglesX(90f);
		}
		else
		{
			stage.StartStopper.SetLocalEulerAnglesX(0f);
		}
		float time = 0f;
		do
		{
			time += 5f * Time.deltaTime;
			if (time > 1f)
			{
				time = 1f;
			}
			if (_stand)
			{
				stage.StartStopper.SetLocalEulerAnglesX(90f * (1f - time));
			}
			else
			{
				stage.StartStopper.SetLocalEulerAnglesX(90f * time);
			}
			yield return null;
		}
		while (time < 1f);
	}
}
