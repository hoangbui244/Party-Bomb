using System;
using UnityEngine;
public class ResultManager : SingletonCustom<ResultManager> {
    [Serializable]
    public struct PhotoSpriteData {
        [Header("「手裏剣の術」の写真")]
        public Sprite photo_0;
        [Header("「水蜘蛛の術」の写真")]
        public Sprite photo_1;
        [Header("「隠れ身の術」の写真")]
        public Sprite photo_2;
        [Header("「鉤縄の術」の写真")]
        public Sprite photo_3;
        [Header("「忍刀の術」の写真")]
        public Sprite photo_4;
        [Header("「変身の術」の写真")]
        public Sprite photo_5;
        [Header("「印結びの術」の写真")]
        public Sprite photo_6;
        [Header("「ムササビの術」の写真")]
        public Sprite photo_7;
        [Header("「吹き矢の術」の写真")]
        public Sprite photo_8;
        [Header("「韋駄天の術」の写真")]
        public Sprite photo_9;
    }
    [SerializeField]
    [Header("写真画像デ\u30fcタ")]
    private PhotoSpriteData photoSpriteData;
    public Sprite GetPhotoSprite(SceneManager.SceneType _sceneType) {
        switch (_sceneType) {
            case SceneManager.SceneType.GET_BALL:
                return photoSpriteData.photo_0;
            case SceneManager.SceneType.ARCHER_BATTLE:
                return photoSpriteData.photo_1;
            case SceneManager.SceneType.BLOCK_WIPER:
                return photoSpriteData.photo_2;
            case SceneManager.SceneType.MOLE_HAMMER:
                return photoSpriteData.photo_3;
            case SceneManager.SceneType.BOMB_ROULETTE:
                return photoSpriteData.photo_4;
            case SceneManager.SceneType.RECEIVE_PON:
                return photoSpriteData.photo_5;
            case SceneManager.SceneType.BLACKSMITH:
                return photoSpriteData.photo_6;
            case SceneManager.SceneType.CANNON_SHOT:
                return photoSpriteData.photo_7;
            case SceneManager.SceneType.ATTACK_BALL:
                return photoSpriteData.photo_8;
            case SceneManager.SceneType.BLOW_AWAY_TANK:
                return photoSpriteData.photo_9;
            default:
                return null;
        }
    }
}
