using I2.Loc;
using System;
using UnityEngine;
public class CommonStartGameRuleProductionUI : MonoBehaviour {
    #region Inspector
    [field: Header("References")]
    [field: SerializeField] public GameObject GameRuleRoot { get; private set; }
    [field: SerializeField] public Localize GameRuleTextLocalize { get; private set; }
    [field: Header("Data")]
    [field: SerializeField] public string[] TermArray { get; private set; }
    #endregion
    private Vector3 m_OriginalScale;
    public void Init() {
        m_OriginalScale = GameRuleRoot.transform.localScale;
        GameRuleRoot.transform.localScale = Vector3.zero;
    }
    public void ShowStartGameRule(Action _callBack) {
        int gameId = (int)SingletonCustom<SceneManager>.Instance.GetNowScene() - 2;
        GameRuleTextLocalize.SetTerm(TermArray[gameId]);
        // Tween
        LeanTween.scale(GameRuleRoot.gameObject, m_OriginalScale, 0.5f).setEaseOutBack();
        LeanTween.delayedCall(gameObject, 3.5f, () => {
            LeanTween.scale(GameRuleRoot.gameObject, Vector3.zero, 0.5f).setEaseInBack();
            LeanTween.delayedCall(gameObject, 0.5f, () => _callBack?.Invoke());
        });
    }
}