using UnityEngine;
public class WaterSlider_Cpu : MonoBehaviour
{
	private Transform currentCheckPointAnchor;
	private int checkPointIdx;
	private int checkPointDiff;
	private float inputHorizontal;
	public bool isLogOut;
	private float targetLength;
	private float checkLength = 15f;
	private float checkAngle = 10f;
	private bool isBranch;
	private int corvePower;
	public int CurrentCheckPointIdx => checkPointIdx;
	public bool IsReverse
	{
		get;
		set;
	}
	public void Init()
	{
		checkPointIdx = 0;
		currentCheckPointAnchor = SingletonCustom<WaterSlider_CourseManager>.Instance.GetCheckPointAnchor(base.transform.position, checkPointIdx, checkPointDiff);
		switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength)
		{
		case 0:
			corvePower = 5;
			break;
		case 1:
			corvePower = 10;
			break;
		case 2:
			corvePower = 16;
			break;
		}
		targetLength = CalcManager.Length(currentCheckPointAnchor.position, base.transform.position);
	}
	private void Update()
	{
		if (targetLength + 0.85f < CalcManager.Length(currentCheckPointAnchor.position, base.transform.position))
		{
			IsReverse = true;
			targetLength = CalcManager.Length(currentCheckPointAnchor.position, base.transform.position);
			if (isBranch)
			{
				IsReverse = false;
			}
		}
		else if (targetLength - 0.85f > CalcManager.Length(currentCheckPointAnchor.position, base.transform.position))
		{
			IsReverse = false;
			targetLength = CalcManager.Length(currentCheckPointAnchor.position, base.transform.position);
		}
	}
	public float GetInputHorizontal()
	{
		inputHorizontal = 0f;
		if (checkPointIdx < SingletonCustom<WaterSlider_CourseManager>.Instance.CheckPointLength)
		{
			bool isLogOut2 = isLogOut;
			if (CalcManager.Length(currentCheckPointAnchor.position, base.transform.position) <= checkLength || Vector3.Angle(base.transform.forward, currentCheckPointAnchor.position - base.transform.position) >= checkAngle)
			{
				CalcManager.mCalcVector3 = currentCheckPointAnchor.position;
				CalcManager.mCalcVector3.y = base.transform.position.y;
				Vector3 vector = CalcManager.mCalcVector3 - base.transform.position;
				Vector3 vector2 = Vector3.Cross(base.transform.forward, vector);
				float value = Vector3.Angle(base.transform.forward, vector) * (float)((!(vector2.y < 0f)) ? 1 : (-1));
				inputHorizontal = Mathf.Clamp(value, -1f, 1f);
			}
		}
		return inputHorizontal;
	}
	private void OnDrawGizmos()
	{
		if (isLogOut)
		{
			Gizmos.color = Color.red;
		}
	}
	public void OnGoal()
	{
	}
	public bool IsAcceleratorButton()
	{
		return true;
	}
	public bool IsBackButton()
	{
		return false;
	}
	public float GetCheckPointDistance()
	{
		return Vector3.Distance(base.transform.position, currentCheckPointAnchor.position);
	}
	public int GetCorvePower()
	{
		return corvePower;
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.name.Contains("point_"))
		{
			string[] array = other.name.Split('_');
			int num = int.Parse(array[1]);
			checkPointIdx = num + 1;
			if (array.Length > 2)
			{
				checkPointDiff = int.Parse(array[2]);
			}
			else
			{
				checkPointDiff = -1;
			}
			if (checkPointIdx == 11)
			{
				isBranch = true;
			}
			else
			{
				isBranch = false;
			}
			if (checkPointIdx < SingletonCustom<WaterSlider_CourseManager>.Instance.CheckPointLength)
			{
				currentCheckPointAnchor = SingletonCustom<WaterSlider_CourseManager>.Instance.GetCheckPointAnchor(base.transform.position, checkPointIdx, checkPointDiff);
			}
			if (checkPointIdx < num + 1)
			{
				IsReverse = false;
			}
			targetLength = CalcManager.Length(currentCheckPointAnchor.position, base.transform.position);
		}
	}
}
