using System;
using UnityEngine;
public class TournamentSheetData : MonoBehaviour {
    public enum LineType {
        Horizontal_Left,
        Horizontal_Right,
        Vertical
    }
    [Serializable]
    public struct LineSpriteData {
        [Header("左側のチ\u30fcムのライン詳細デ\u30fcタ")]
        public LineSpriteDetailData[] left_LineSprites;
        [Header("右側のチ\u30fcムのライン詳細デ\u30fcタ")]
        public LineSpriteDetailData[] right_LineSprites;
        [Header("共通のライン詳細デ\u30fcタ")]
        public LineSpriteDetailData[] common_LineSprites;
    }
    [Serializable]
    public struct LineSpriteDetailData {
        [Header("ライン画像")]
        public SpriteRenderer lineSprite;
        [Header("ライン種類")]
        public LineType lineType;
        public Vector3 defaultLineScale;
    }
}
