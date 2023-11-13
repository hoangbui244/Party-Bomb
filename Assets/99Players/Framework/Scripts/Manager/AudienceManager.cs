using System;
using System.Collections.Generic;
using UnityEngine;
namespace BeachSoccer
{
	public class AudienceManager : SingletonCustom<AudienceManager>
	{
		public enum AnimType
		{
			NONE,
			NORMAL,
			EXCITE,
			VERY_EXCITE
		}
		[SerializeField]
		[Header("観客オブジェクト")]
		private GameObject[] AudienceObj;
		[SerializeField]
		[Header("決勝用：観客オブジェクト")]
		private GameObject[] finalAudienceObj;
		[SerializeField]
		[Header("水着かどうか")]
		private bool isSwimsuit;
		private List<GameObject> peopleObj = new List<GameObject>();
		private List<Vector3> initPos = new List<Vector3>();
		private AnimType animType;
		private void Start()
		{
			for (int i = 0; i < AudienceObj.Length; i++)
			{
				foreach (Transform item in AudienceObj[i].transform)
				{
					bool isSwimsuit2 = isSwimsuit;
					peopleObj.Add(item.gameObject);
					initPos.Add(item.localPosition);
				}
			}
			if (GameSaveData.GetSelectMainGameMode() == GameSaveData.MainGameMode.TOURNAMENT && GameSaveData.GetTournamentBattleNum() == 3 && finalAudienceObj != null)
			{
				for (int j = 0; j < finalAudienceObj.Length; j++)
				{
					finalAudienceObj[j].SetActive(value: true);
					foreach (Transform item2 in finalAudienceObj[j].transform)
					{
						bool isSwimsuit3 = isSwimsuit;
						peopleObj.Add(item2.gameObject);
						initPos.Add(item2.localPosition);
					}
				}
			}
			ChangePeopleAnim(AnimType.NORMAL);
		}
		public void ChangePeopleAnim(AnimType _type)
		{
			if (animType != _type)
			{
				animType = _type;
				for (int i = 0; i < peopleObj.Count; i++)
				{
				}
				Invoke(new Action(DelaySe).Method.Name, 0f);
			}
		}
		private void DelaySe()
		{
			if (animType == AnimType.EXCITE)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
			}
			else if (animType == AnimType.VERY_EXCITE)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
			}
		}
	}
}
