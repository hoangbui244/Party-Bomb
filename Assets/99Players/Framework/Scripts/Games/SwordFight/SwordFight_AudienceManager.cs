using System.Collections.Generic;
using UnityEngine;
public class SwordFight_AudienceManager : SingletonCustom<SwordFight_AudienceManager>
{
	public enum AudienceType
	{
		Chair,
		Floor
	}
	public enum AudienceAnimType
	{
		NONE,
		NORMAL,
		EXCITE,
		VERY_EXCITE
	}
	[SerializeField]
	[Header("観客オブジェクト(椅子の上)")]
	private GameObject audienceObj_Chair;
	[SerializeField]
	[Header("観客オブジェクト(床の上)")]
	private GameObject audienceObj_Floor;
	[SerializeField]
	[Header("観客の服装マテリアル")]
	private Material[] audienceClothMaterial;
	private List<GameObject> peopleObj_Chair = new List<GameObject>();
	private List<Vector3> initPos_Chair = new List<Vector3>();
	private List<GameObject> peopleObj_Floor = new List<GameObject>();
	private List<Vector3> initPos_Floor = new List<Vector3>();
	private AudienceAnimType audienceAnimType_Chair;
	private AudienceAnimType audienceAnimType_Floor;
	private void Start()
	{
		int num = 0;
		foreach (Transform item in audienceObj_Chair.transform)
		{
			item.gameObject.GetComponent<MeshRenderer>().material = audienceClothMaterial[num];
			num++;
			if (num >= audienceClothMaterial.Length)
			{
				num = 0;
			}
			peopleObj_Chair.Add(item.gameObject);
			initPos_Chair.Add(item.localPosition);
		}
		ChangePeopleAnim(AudienceType.Chair, AudienceAnimType.NONE);
		num = 0;
		foreach (Transform item2 in audienceObj_Floor.transform)
		{
			item2.gameObject.GetComponent<MeshRenderer>().material = audienceClothMaterial[num];
			num++;
			if (num >= audienceClothMaterial.Length)
			{
				num = 0;
			}
			peopleObj_Floor.Add(item2.gameObject);
			initPos_Floor.Add(item2.localPosition);
		}
		ChangePeopleAnim(AudienceType.Floor, AudienceAnimType.NORMAL);
	}
	public void ChangePeopleAnim(AudienceType _audienceType, AudienceAnimType _audienceAnimType)
	{
		switch (_audienceType)
		{
		case AudienceType.Chair:
			if (audienceAnimType_Chair == _audienceAnimType)
			{
				UnityEngine.Debug.Log("同じ観客の演出なので処理しない");
				break;
			}
			audienceAnimType_Chair = _audienceAnimType;
			for (int j = 0; j < peopleObj_Chair.Count; j++)
			{
				LeanTween.cancel(peopleObj_Chair[j]);
				peopleObj_Chair[j].transform.localPosition = initPos_Chair[j];
				if (audienceAnimType_Chair == AudienceAnimType.NORMAL)
				{
					LeanTween.moveLocal(peopleObj_Chair[j], initPos_Chair[j] + peopleObj_Chair[j].transform.up * Random.Range(0f, 0.08f), Random.Range(0.2f, 0.4f)).setEaseLinear().setLoopPingPong();
				}
				else if (audienceAnimType_Chair == AudienceAnimType.EXCITE)
				{
					LeanTween.moveLocal(peopleObj_Chair[j], initPos_Chair[j] + peopleObj_Chair[j].transform.up * Random.Range(0.12f, 0.2f), Random.Range(0.1f, 0.3f)).setEaseOutQuad().setLoopPingPong();
				}
				else if (audienceAnimType_Chair == AudienceAnimType.VERY_EXCITE)
				{
					LeanTween.moveLocal(peopleObj_Chair[j], initPos_Chair[j] + peopleObj_Chair[j].transform.up * Random.Range(0.2f, 0.36f), Random.Range(0.1f, 0.3f)).setEaseOutQuad().setLoopPingPong();
				}
			}
			break;
		case AudienceType.Floor:
			if (audienceAnimType_Floor == _audienceAnimType)
			{
				UnityEngine.Debug.Log("同じ観客の演出なので処理しない");
				break;
			}
			audienceAnimType_Floor = _audienceAnimType;
			for (int i = 0; i < peopleObj_Floor.Count; i++)
			{
				LeanTween.cancel(peopleObj_Floor[i]);
				peopleObj_Floor[i].transform.localPosition = initPos_Floor[i];
				if (audienceAnimType_Floor == AudienceAnimType.NORMAL)
				{
					LeanTween.moveLocal(peopleObj_Floor[i], initPos_Floor[i] + peopleObj_Floor[i].transform.up * Random.Range(0f, 0.08f), Random.Range(0.2f, 0.4f)).setEaseLinear().setLoopPingPong();
				}
				else if (audienceAnimType_Floor == AudienceAnimType.EXCITE)
				{
					LeanTween.moveLocal(peopleObj_Floor[i], initPos_Floor[i] + peopleObj_Floor[i].transform.up * Random.Range(0.12f, 0.2f), Random.Range(0.1f, 0.3f)).setEaseOutQuad().setLoopPingPong();
				}
				else if (audienceAnimType_Floor == AudienceAnimType.VERY_EXCITE)
				{
					LeanTween.moveLocal(peopleObj_Floor[i], initPos_Floor[i] + peopleObj_Floor[i].transform.up * Random.Range(0.2f, 0.36f), Random.Range(0.1f, 0.3f)).setEaseOutQuad().setLoopPingPong();
				}
			}
			break;
		}
	}
	private void DelaySe()
	{
		if (audienceAnimType_Chair == AudienceAnimType.EXCITE)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
		}
		else if (audienceAnimType_Chair == AudienceAnimType.VERY_EXCITE)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
		}
	}
}
