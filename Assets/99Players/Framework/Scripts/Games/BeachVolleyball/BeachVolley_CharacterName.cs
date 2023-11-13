using TMPro;
using UnityEngine;
public class BeachVolley_CharacterName : MonoBehaviour
{
	[SerializeField]
	[Header("時間アンカ\u30fc")]
	private Transform timeAnchor;
	[SerializeField]
	[Header("時間")]
	private TextMeshPro timeText;
	[SerializeField]
	[Header("カウントアンカ\u30fc")]
	private Transform cntAnchor;
	private float cntAnchorLocalPosXDef;
	[SerializeField]
	[Header("カウント")]
	private TextMeshPro cntText;
	private float time;
	private bool isFinishTime = true;
	public void ChangeBackMultiColor()
	{
	}
	public void SetName(string _name)
	{
	}
	public float GetTime()
	{
		return time;
	}
	public void Init()
	{
		timeAnchor.SetLocalPositionX(0f);
		cntAnchorLocalPosXDef = cntAnchor.localPosition.x;
	}
	public void SetCnt(int _num)
	{
		cntText.text = _num.ToString();
		if (timeAnchor.gameObject.activeSelf)
		{
			cntAnchor.SetLocalPositionX(cntAnchorLocalPosXDef);
		}
		else
		{
			cntAnchor.SetLocalPositionX(0f);
		}
		cntAnchor.gameObject.SetActive(value: true);
	}
	public void HideCnt()
	{
		cntAnchor.gameObject.SetActive(value: false);
	}
	public void StartTimeLimit(float _time)
	{
		time = _time;
		timeText.text = time.ToString("0.0");
		cntAnchor.SetLocalPositionX(cntAnchorLocalPosXDef);
		timeAnchor.gameObject.SetActive(value: true);
		isFinishTime = false;
	}
	public void FinishTimeLimit()
	{
		time = 0f;
		timeText.text = time.ToString("0.0");
		cntAnchor.SetLocalPositionX(0f);
		timeAnchor.gameObject.SetActive(value: false);
		isFinishTime = false;
	}
	public bool IsFinishTimeLimit()
	{
		if (isFinishTime)
		{
			return time <= 0f;
		}
		return false;
	}
	private void Update()
	{
		if (timeAnchor.gameObject.activeSelf && time > 0f)
		{
			time -= Time.deltaTime;
			if (time <= 0f)
			{
				isFinishTime = true;
				time = 0f;
				timeAnchor.gameObject.SetActive(value: false);
				cntAnchor.SetLocalPositionX(0f);
			}
			timeText.text = time.ToString("0.0");
		}
	}
}
