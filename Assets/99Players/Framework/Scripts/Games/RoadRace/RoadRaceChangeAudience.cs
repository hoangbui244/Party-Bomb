using System;
using System.Collections.Generic;
using UnityEngine;
public class RoadRaceChangeAudience : MonoBehaviour
{
	public enum AnimType
	{
		NONE,
		NORMAL,
		EXCITE,
		VERY_EXCITE
	}
	[SerializeField]
	[Header("通過時リアクション無し")]
	private bool notChange;
	[SerializeField]
	[Header("太鼓の音無し")]
	private bool notDrum;
	private List<GameObject> peopleObj = new List<GameObject>();
	private List<Vector3> initPos = new List<Vector3>();
	private AnimType animType;
	private AnimType[] animTypes;
	private Transform _transform;
	private int rap;
	private int inCount;
	private int countNow;
	private int countState;
	private float seTimeExcite;
	private float seTimeVeryExcite;
	private void Start()
	{
		_transform = base.transform;
		foreach (Transform item in base.transform)
		{
			peopleObj.Add(item.gameObject);
			initPos.Add(item.localPosition);
		}
		animType = AnimType.NONE;
		animTypes = new AnimType[peopleObj.Count];
		for (int i = 0; i < animTypes.Length; i++)
		{
			animTypes[i] = AnimType.NONE;
		}
		ChangePeopleAnim(AnimType.NORMAL);
	}
	private void Update()
	{
		seTimeExcite += Time.deltaTime;
		seTimeVeryExcite += Time.deltaTime;
		for (int i = 0; i < peopleObj.Count; i++)
		{
			if (animTypes[i] != this.animType && !(Vector3.Distance(peopleObj[i].transform.localPosition, initPos[i]) > 0.01f))
			{
				animTypes[i] = this.animType;
				LeanTween.cancel(peopleObj[i]);
				peopleObj[i].transform.localPosition = initPos[i];
				if (this.animType == AnimType.NORMAL)
				{
					LeanTween.moveLocal(peopleObj[i], initPos[i] + peopleObj[i].transform.up * UnityEngine.Random.Range(0f, 0.08f), UnityEngine.Random.Range(0.2f, 0.4f)).setEaseLinear().setLoopPingPong();
				}
				else if (this.animType == AnimType.EXCITE)
				{
					LeanTween.moveLocal(peopleObj[i], initPos[i] + peopleObj[i].transform.up * UnityEngine.Random.Range(0.07f, 0.17f), UnityEngine.Random.Range(0.2f, 0.36f)).setEaseOutQuad().setLoopPingPong();
				}
				else if (this.animType == AnimType.VERY_EXCITE)
				{
					LeanTween.moveLocal(peopleObj[i], initPos[i] + peopleObj[i].transform.up * UnityEngine.Random.Range(0.2f, 0.36f), UnityEngine.Random.Range(0.21f, 0.35f)).setEaseOutQuad().setLoopPingPong();
				}
			}
		}
		if (notChange)
		{
			int rapCount = SingletonCustom<RoadRaceCharacterManager>.Instance.GetRapCount();
			if (rap < rapCount)
			{
				rap = rapCount;
				switch (rap)
				{
				case 2:
				case 4:
					break;
				case 1:
					ChangePeopleAnim(AnimType.NORMAL, seDontPlay: true);
					break;
				case 3:
					ChangePeopleAnim(AnimType.EXCITE, seDontPlay: true);
					break;
				case 5:
					ChangePeopleAnim(AnimType.VERY_EXCITE, seDontPlay: true);
					break;
				}
			}
		}
		else
		{
			if (countNow == inCount)
			{
				return;
			}
			AnimType animType = AnimType.NORMAL;
			if (inCount >= 4)
			{
				if (countState <= 1)
				{
					animType = AnimType.VERY_EXCITE;
					countState = 2;
					ChangePeopleAnim(animType);
				}
			}
			else if (inCount >= 1)
			{
				if (countState == 0)
				{
					animType = AnimType.EXCITE;
					countState = 1;
					ChangePeopleAnim(animType);
				}
			}
			else
			{
				animType = AnimType.NORMAL;
				countState = 0;
				ChangePeopleAnim(animType);
			}
			countNow = inCount;
		}
	}
	public void ChangePeopleAnim(AnimType _type, bool seDontPlay = false)
	{
		if (animType != _type)
		{
			animType = _type;
			if (!seDontPlay)
			{
				Invoke(new Action(DelaySe).Method.Name, 0f);
			}
		}
	}
	private void DelaySe()
	{
		if (animType == AnimType.EXCITE)
		{
			if (seTimeExcite > 1.1f)
			{
				seTimeExcite = 0f;
				AudienceSePlay(animType);
			}
		}
		else if (animType == AnimType.VERY_EXCITE && seTimeVeryExcite > 2f)
		{
			seTimeVeryExcite = 0f;
			if (!notDrum)
			{
				AudienceSePlay(animType);
			}
		}
	}
	private void AudienceSePlay(AnimType _animType)
	{
		if (Scene_RoadRace.GM.IsGameEnd)
		{
			return;
		}
		RoadRaceCharacterScript[] charas = SingletonCustom<RoadRaceCharacterManager>.Instance.GetCharas();
		float num = 100000f;
		for (int i = 0; i < SingletonCustom<RoadRaceCharacterManager>.Instance.PlayerNum; i++)
		{
			float num2 = Vector3.Distance(_transform.position, charas[i].GetPos());
			if (num2 < num)
			{
				num = num2;
			}
		}
		if (num < 10f)
		{
			float num3 = num / 10f;
			if (_animType != AnimType.EXCITE)
			{
			}
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Character")
		{
			inCount++;
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.transform.tag == "Character")
		{
			inCount--;
		}
	}
}
