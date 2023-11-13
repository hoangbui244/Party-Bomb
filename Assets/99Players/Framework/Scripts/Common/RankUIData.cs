using System;
using TMPro;
using UnityEngine;
public class RankUIData : MonoBehaviour {
    [Serializable]
    public struct RankingUIData {
        [Header("順位のアンカ\u30fc")]
        public Transform rankAnchor;
        [SerializeField]
        [Header("背景色")]
        public SpriteRenderer backColor;
        [Header("王冠のアイコン")]
        public SpriteRenderer crownIcon;
        [Header("王冠後ろオブジェクト")]
        public SpriteRenderer crownBack;
        [Header("王冠のキラキラエフェクト")]
        public GlitterSpriteEffect glitterSpriteEffect;
        [Header("順位のキャプション")]
        public SpriteRenderer rankCaption;
        [Header("キャラアイコン")]
        public SpriteRenderer characterIcon;
        [Header("順位の団名アイコン")]
        public SpriteRenderer teamIcon;
        [Header("順位のプレイヤ\u30fcアイコン")]
        public SpriteRenderer playerIcon;
        [Header("順位のCPUアイコン")]
        public SpriteRenderer cpuIcon;
        [Header("ポイント数")]
        public SpriteNumbers pointNumbers;
        [Header("ポイント文字")]
        public SpriteRenderer pointText;
        [Header("評価アイコン")]
        public SpriteRenderer evaluationIcon;
        [Header("フレ\u30fcム")]
        public SpriteRenderer frame;
        [Header("加算ポイント")]
        public TextMeshPro textAddScore;
    }
    [Serializable]
    public struct RecordUIDataList {
        [Header("[000]のスコア記録のUIデ\u30fcタ")]
        public RecordUIData scoreUIData;
        [Header("[00.00]のスコア記録のUIデ\u30fcタ")]
        public RecordUIData doubleDecimalScoreUIData;
        [Header("[000.0]のスコア記録のUIデ\u30fcタ")]
        public RecordUIData decimalScoreUIData;
        [Header("[0:00.0]の時間記録のUIデ\u30fcタ")]
        public RecordUIData timeUIData;
        [Header("[00秒00]の時間記録のUIデ\u30fcタ")]
        public RecordUIData secondTimeUIData;
        [Header("[0000]のスコア記録のUIデ\u30fcタ")]
        public RecordUIData scoreFourDigitUIData;
    }
    [SerializeField]
    [Header("順位UIデ\u30fcタ")]
    private RankingUIData rankingUIData;
    [SerializeField]
    [Header("順位記録UIデ\u30fcタ")]
    private RecordUIDataList rankingRecordUIData;
    private void Awake() {
    }
    public RankingUIData GetRankUIData() {
        return rankingUIData;
    }
    public RecordUIData GetRecordUIData(RecordUIData.RecordType _recordType) {
        switch (_recordType) {
            case RecordUIData.RecordType.Score:
                return rankingRecordUIData.scoreUIData;
            case RecordUIData.RecordType.DoubleDecimalScore:
                return rankingRecordUIData.doubleDecimalScoreUIData;
            case RecordUIData.RecordType.DecimalScore:
                return rankingRecordUIData.decimalScoreUIData;
            case RecordUIData.RecordType.Time:
                return rankingRecordUIData.timeUIData;
            case RecordUIData.RecordType.SecondTime:
                return rankingRecordUIData.secondTimeUIData;
            case RecordUIData.RecordType.ScoreFourDigit:
                return rankingRecordUIData.scoreFourDigitUIData;
            default:
                return null;
        }
    }
}
