using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// BKB: this class control all character instance to use within the game
/// </summary>
public class GameCharacterManager : SingletonCustom<GameCharacterManager> {
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("メッシュ")]
    public GameObject[] arrayCharacters;
    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start() {
    }
    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {
    }
}
