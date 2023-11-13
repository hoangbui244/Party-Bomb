using UnityEngine;
using UnityEngine.Rendering;
public class WaterSlider_CourseManager : SingletonCustom<WaterSlider_CourseManager>
{
	[SerializeField]
	[Header("チェックポイント配列（コ\u30fcス1）")]
	private Transform[] arrayCheckPoint;
	[SerializeField]
	[Header("チェックポイント配列（コ\u30fcス2）")]
	private Transform[] arrayCheckPoint2;
	[SerializeField]
	[Header("ゴ\u30fcルポイント（コ\u30fcス1）")]
	private Transform goalPoint1;
	[SerializeField]
	[Header("ゴ\u30fcルポイント（コ\u30fcス2）")]
	private Transform goalPoint2;
	[SerializeField]
	[Header("水流１")]
	private GameObject waterPressure1;
	[SerializeField]
	[Header("水流２")]
	private GameObject waterPressure2;
	[SerializeField]
	[Header("屋根のメッシュレンダラ\u30fc")]
	private MeshRenderer roofMesh;
	private int throwCount;
	private int currentCheckPointLength;
	public int CheckPointLength => currentCheckPointLength;
	public void Init()
	{
		currentCheckPointLength = arrayCheckPoint.Length;
		throwCount = 0;
	}
	public void ChangeCourse()
	{
		switch (SingletonCustom<GameSettingManager>.Instance.SelectCourseIdx)
		{
		case 0:
			currentCheckPointLength = arrayCheckPoint.Length;
			break;
		case 1:
			currentCheckPointLength = arrayCheckPoint2.Length;
			break;
		}
	}
	public Transform GetGoalPoint()
	{
		switch (SingletonCustom<GameSettingManager>.Instance.SelectCourseIdx)
		{
		case 0:
			return goalPoint1;
		case 1:
			return goalPoint2;
		default:
			return goalPoint1;
		}
	}
	public Transform GetCheckPointAnchor(Vector3 _pos, int _idx, int _diff)
	{
		switch (SingletonCustom<GameSettingManager>.Instance.SelectCourseIdx)
		{
		case 0:
		{
			for (int m = 0; m < arrayCheckPoint.Length; m++)
			{
				if (arrayCheckPoint[m].name.Contains("point_" + _idx.ToString() + "_" + _diff.ToString()))
				{
					return arrayCheckPoint[m];
				}
			}
			Transform transform2 = null;
			if (_diff == -1)
			{
				for (int n = 0; n < arrayCheckPoint.Length; n++)
				{
					for (int num = 0; num < 2; num++)
					{
						if (arrayCheckPoint[n].name.Contains("point_" + _idx.ToString() + "_" + num.ToString()) && (transform2 == null || CalcManager.Length(_pos, transform2.position) > CalcManager.Length(_pos, arrayCheckPoint[n].position)))
						{
							transform2 = arrayCheckPoint[n];
						}
					}
				}
			}
			if (transform2 != null)
			{
				return transform2;
			}
			for (int num2 = 0; num2 < arrayCheckPoint.Length; num2++)
			{
				if (arrayCheckPoint[num2].name.Contains("point_" + _idx.ToString()))
				{
					return arrayCheckPoint[num2];
				}
			}
			break;
		}
		case 1:
		{
			for (int i = 0; i < arrayCheckPoint2.Length; i++)
			{
				if (arrayCheckPoint2[i].name.Contains("point_" + _idx.ToString() + "_" + _diff.ToString()))
				{
					return arrayCheckPoint2[i];
				}
			}
			Transform transform = null;
			if (_diff == -1)
			{
				for (int j = 0; j < arrayCheckPoint2.Length; j++)
				{
					for (int k = 0; k < 2; k++)
					{
						if (arrayCheckPoint2[j].name.Contains("point_" + _idx.ToString() + "_" + k.ToString()) && (transform == null || CalcManager.Length(_pos, transform.position) > CalcManager.Length(_pos, arrayCheckPoint2[j].position)))
						{
							transform = arrayCheckPoint2[j];
							UnityEngine.Debug.Log("★:" + arrayCheckPoint2[j].name);
						}
					}
				}
			}
			if (transform != null)
			{
				return transform;
			}
			for (int l = 0; l < arrayCheckPoint2.Length; l++)
			{
				if (arrayCheckPoint2[l].name.Contains("point_" + _idx.ToString()))
				{
					return arrayCheckPoint2[l];
				}
			}
			break;
		}
		}
		return arrayCheckPoint[_idx];
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.name.Contains("MoveObj"))
		{
			throwCount++;
			if (throwCount >= 4)
			{
				waterPressure1.gameObject.SetActive(value: false);
				waterPressure2.gameObject.SetActive(value: false);
				roofMesh.shadowCastingMode = ShadowCastingMode.On;
			}
		}
	}
}
