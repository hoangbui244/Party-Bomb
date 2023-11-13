using UnityEngine;
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonCustom<T> : MonoBehaviourExtension where T : MonoBehaviourExtension {
    /// <summary>
    /// 
    /// </summary>
    private static T instance;
    /// <summary>
    /// 
    /// </summary>
    public static T Instance {
        get {
            if ((Object)instance == (Object)null) {
                instance = (T)UnityEngine.Object.FindObjectOfType(typeof(T));
                if ((Object)instance == (Object)null) {
                    UnityEngine.Debug.LogError(typeof(T)?.ToString() + "is nothing");
                }
            }
            return instance;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected bool CheckInstance() {
        if (this == Instance) {
            return true;
        }
        UnityEngine.Object.Destroy(base.gameObject);
        return false;
    }
    /// <summary>
    /// 
    /// </summary>
    public void OnEnable() {
        CheckInstance();
        Resume();
    }
    /// <summary>
    /// 
    /// </summary>
    public virtual void OnDestroy() {
        if (this == Instance) {
            instance = null;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public virtual void Resume() {
    }
}
