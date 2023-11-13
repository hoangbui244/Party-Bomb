using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 
/// </summary>
public class Scene_Blank : MonoBehaviour {
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("フェ\u30fcド画像")]
    private SpriteRenderer spFade;
    /// <summary>
    /// 
    /// </summary>
    public void Start() {
        Color color = spFade.color;
        color.a = 1f;
        spFade.color = color;
        StartCoroutine(_Splash());
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator _Splash() {
        yield return new WaitForEndOfFrame();
        for (float frame = 1f; frame > 0f; frame -= 0.025f) {
            Color color = spFade.color;
            color.a = frame;
            spFade.color = color;
            yield return new WaitForEndOfFrame();
        }
        while (!SingletonCustom<AudioManager>.Instance.IsLoadResource) {
            yield return new WaitForEndOfFrame();
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_Main");
    }
}
