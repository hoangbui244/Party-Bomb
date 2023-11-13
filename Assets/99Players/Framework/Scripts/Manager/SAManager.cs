using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
/// <summary>
/// 
/// </summary>
public class SAManager : SingletonCustom<SAManager> {
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public struct SAData {
        /// <summary>
        /// 
        /// </summary>
        public SpriteAtlas sa;
        /// <summary>
        /// 
        /// </summary>
        public string saName;
        /// <summary>
        /// 
        /// </summary>
        public Material saMat;
    }
    /// <summary>
    /// 
    /// </summary>
    private const string LOCALIZE_ENGLISH_HEAD_TEXT = "en";
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    public SAData[] saData;
    /// <summary>
    /// 
    /// </summary>
    private Dictionary<string, Sprite> dicSpData = new Dictionary<string, Sprite>();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_atlasType"></param>
    /// <returns></returns>
    public SpriteAtlas GetSA(SAType _atlasType) {
        return saData[(int)_atlasType].sa;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_atlasType"></param>
    /// <param name="_name"></param>
    /// <returns></returns>
    public Sprite GetSprite(SAType _atlasType, string _name) {
        string key = _name + "_" + _atlasType.ToString();
        Sprite sprite;
        if (!dicSpData.ContainsKey(key)) {
            if (Localize_Define.Language == Localize_Define.LanguageType.Japanese) {
                sprite = saData[(int)_atlasType].sa.GetSprite(_name);
            } else {
                Sprite sprite2 = saData[(int)_atlasType].sa.GetSprite(LOCALIZE_ENGLISH_HEAD_TEXT + _name);
                sprite = ((sprite2 == null) ? saData[(int)_atlasType].sa.GetSprite(_name) : sprite2);
            }
            dicSpData.Add(key, sprite);
            if (sprite == null) {
                Debug.Log("Failed to locate " + _name + " : " + _atlasType);
            }
            return sprite;
        }
        dicSpData.TryGetValue(key, out sprite);
        if (sprite == null) {
            Debug.Log("Failed to locate " + _name + " : " + _atlasType);
        }
        return sprite;
    }
}
