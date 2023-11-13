using System;
using System;
using UnityEngine;
[Serializable]
public struct CursorLayer {
    [SerializeField]
    [Header("ボタンオブジェクト")]
    public GameObject[] button;
    [SerializeField]
    [Header("カ\u30fcソルオブジェクト")]
    public Transform[] cursorObj;
    public CursorButtonObject[] buttonObj;
}
