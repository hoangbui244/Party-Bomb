using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Extension;
using UnityEngine.Extension.L10;
[DefaultExecutionOrder(-100)]
public class LocalizationCompatible : DecoratedMonoBehaviour {
    [SerializeField]
    [DisplayName("デフォルト機能を利用")]
    private bool useDefaultLocalizeClass;
    [SerializeField]
    [Hide("useDefaultLocalizeClass", false)]
    [DisplayName("シ\u30fcンのル\u30fcトオブジェクト")]
    private GameObject sceneRoot;
    private void Awake() {
        Localization.RegisterCurrentLanguageHandler(Handler);
    }
    private void OnEnable() {
        if (useDefaultLocalizeClass) {
            ExecuteDefaultLocalizationMethod();
        }
    }
    private void ExecuteDefaultLocalizationMethod() {
        if (Localize_Define.Language != 0) {
            List<Localize_Target> list = new List<Localize_Target>(64);
            sceneRoot.GetComponentsInChildren(includeInactive: true, list);
            foreach (Localize_Target item in list) {
                item.Set();
            }
        }
    }
    private static Localization.SupportedLanguage Handler() {
        switch (Localize_Define.Language) {
            case Localize_Define.LanguageType.Japanese:
                return Localization.SupportedLanguage.Japanese;
            case Localize_Define.LanguageType.English:
                return Localization.SupportedLanguage.English;
            default:
                return Localization.SupportedLanguage.Japanese;
        }
    }
}
