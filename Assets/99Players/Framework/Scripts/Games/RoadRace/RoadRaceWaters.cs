using UnityEngine;
public class RoadRaceWaters : MonoBehaviour
{
	private const float WATER_SET_DISTANCE = 0.6f;
	public RoadRaceBoxCreatCheck[] hitBoxs;
	public Transform characterObject;
	private int nextCreateBoxId;
	private int lastCreateBoxId;
	private bool boxZero;
	private Vector3 startBoxLocalAngle;
	public RoadRaceCharacterScript raceCharacterScript;
	public void Init()
	{
	}
	public void Start()
	{
		startBoxLocalAngle = hitBoxs[0].transform.localEulerAngles;
		nextCreateBoxId = 0;
		lastCreateBoxId = 0;
		for (int i = 0; i < hitBoxs.Length; i++)
		{
			InitBox(i);
			hitBoxs[i].SetID(i);
		}
		boxZero = true;
	}
	public void InitBox(int _id)
	{
		hitBoxs[_id].transform.SetParent(base.transform);
		hitBoxs[_id].transform.localPosition = Vector3.zero;
		hitBoxs[_id].transform.localEulerAngles = startBoxLocalAngle;
		hitBoxs[_id].gameObject.SetActive(value: false);
	}
	public void UpdateMethod()
	{
	}
	public void Update()
	{
		if (boxZero || Vector3.Distance(hitBoxs[lastCreateBoxId].transform.position, base.transform.position) > 0.6f)
		{
			CreateBox();
		}
		bool flag = true;
		for (int i = 0; i < hitBoxs.Length; i++)
		{
			if (hitBoxs[i].gameObject.activeSelf)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			boxZero = true;
		}
	}
	private void CreateBox()
	{
		hitBoxs[nextCreateBoxId].transform.SetParent(characterObject.parent);
		hitBoxs[nextCreateBoxId].gameObject.SetActive(value: true);
		hitBoxs[nextCreateBoxId].transform.position = base.transform.position;
		hitBoxs[nextCreateBoxId].transform.eulerAngles = new Vector3(0f, base.transform.eulerAngles.y, 0f);
		hitBoxs[nextCreateBoxId].ResetTime();
		lastCreateBoxId = nextCreateBoxId;
		nextCreateBoxId++;
		if (nextCreateBoxId >= hitBoxs.Length)
		{
			nextCreateBoxId = 0;
		}
		boxZero = false;
	}
}
