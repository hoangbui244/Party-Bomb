using System;
using System.Collections.Generic;
using UnityEngine;
public class LayoutThreeLive : MonoBehaviour {
    [Serializable]
    private class RankData {
        [Header("対象オブジェクト")]
        public GameObject gameObject;
        [SerializeField]
        [Header("順位画像")]
        private SpriteRenderer rankSprite;
        public int Idx {
            get;
            set;
        }
        public int SortIdx {
            get;
            set;
        }
        public int PlayerNo {
            get;
            set;
        }
        public int Rank {
            get;
            set;
        }
        public void SetRankSprite() {
            SetRankSprite(Rank);
        }
        public void SetRankSprite(int _rank) {
            rankSprite.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_rank_s_" + _rank.ToString());
        }
    }
    [SerializeField]
    [Header("プレイヤ\u30fc情報表示位置")]
    private Vector3[] arrayDataPos;
    [SerializeField]
    private RankData[] arrayData = new RankData[3];
    private List<RankData> listData;
    private const float RANK_MOVE_SPEED = 0.5f;
    private void Awake() {
        listData = new List<RankData>();
        for (int i = 0; i < arrayData.Length; i++) {
            arrayData[i].Idx = i;
            arrayData[i].SortIdx = i;
            arrayData[i].PlayerNo = i;
            arrayData[i].Rank = -1;
            listData.Add(arrayData[i]);
        }
    }
    public void SetRankData(int[] _rankArray, bool _isInitRankSet = false) {
        for (int i = 0; i < arrayData.Length; i++) {
            if (arrayData[i].Rank != _rankArray[arrayData[i].PlayerNo]) {
                arrayData[i].Rank = _rankArray[arrayData[i].PlayerNo];
                arrayData[i].SetRankSprite();
            }
        }
        Sort(_isInitRankSet);
    }
    public void Sort(bool _isInitRankSet) {
        listData.Sort((RankData a, RankData b) => a.Rank - b.Rank);
        for (int i = 0; i < arrayData.Length; i++) {
            for (int j = 0; j < listData.Count; j++) {
                if (arrayData[listData[j].SortIdx].Idx != j) {
                    LeanTween.moveLocalY(arrayData[listData[j].SortIdx].gameObject, arrayDataPos[j].y, _isInitRankSet ? 0f : 0.5f);
                    arrayData[listData[j].SortIdx].Idx = j;
                }
            }
        }
    }
}
