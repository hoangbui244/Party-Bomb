using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// 
/// </summary>
public class GlobalCameraManager : SingletonCustom<GlobalCameraManager> {
    #region Inspector
    /// <summary>
    /// 
    /// </summary>
    [Header("References")]
    [SerializeField] private Camera mMainCamera;
    #endregion
    /// <summary>
    /// 
    /// </summary>
    private Rect mGlobalRect;
    private void Update() {
        if (mMainCamera == null) {
            return;
        }
        // Update orthographic size
//#if UNITY_EDITOR
//        Vector2 gameViewSize = Handles.GetMainGameViewSize();
//#else
//        Vector2 gameViewSize = new Vector2(Screen.width, Screen.height);
//#endif
//        mMainCamera.orthographicSize = gameViewSize.y / 2f;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetMainCamera<T>() {
        return (T)Convert.ChangeType(mMainCamera, typeof(T));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float GetWidth() {
        return GetHeight() * mMainCamera.aspect;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float GetHeight() {
        return mMainCamera.orthographicSize * 2;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_camera"></param>
    public void SetCameraGlobalRect(Camera _camera) {
        _camera.rect = mGlobalRect;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_rect"></param>
    public void SetGlobalRect(Rect _rect) {
        mGlobalRect = _rect;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Rect GetGlobalRect() {
        return mGlobalRect;
    }
}