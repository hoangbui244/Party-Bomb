using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
public class AocAssetBundleManager : SingletonCustom<AocAssetBundleManager> {
    private StringBuilder stringBuilder = new StringBuilder();
    private List<DlcDataDefine.DlcBaseData> listDLC = new List<DlcDataDefine.DlcBaseData>();
    private int DLC_NUM = 3;
    private string[] DLC_NAME = new string[3]
    {
        "dlc_1",
        "dlc_2",
        "dlc_3"
    };
    public bool IsLoaded {
        get;
        set;
    }
    [RuntimeInitializeOnLoadMethod]
    private static void OnStart() {
        GameObject gameObject = new GameObject();
        gameObject.AddComponent<AocAssetBundleManager>();
        gameObject.name = "AocAssetBundleManager(IAS)";
    }
    public int GetDlcCount() {
        int num = 0;
        for (int i = 0; i < DLC_NUM; i++) {
            if (IsDlcEnableId(i)) {
                num++;
            }
        }
        return num;
    }
    public bool IsDlcEnableId(int _id) {
        return true;
        //if (_id >= SteamApps.GetDLCCount()) {
        //    return false;
        //}
        //if (SteamApps.BGetDLCDataByIndex(_id, out AppId_t pAppID, out bool _, out string _, 128)) {
        //    return SteamApps.BIsDlcInstalled(pAppID);
        //}
        //return false;
    }
    public bool IsDLCAvailable(int _id) {
        return true;
        //if (_id >= SteamApps.GetDLCCount()) {
        //    return false;
        //}
        //if (SteamApps.BGetDLCDataByIndex(_id, out AppId_t _, out bool pbAvailable, out string _, 128)) {
        //    return pbAvailable;
        //}
        //return false;
    }
    public void LoadAoc() {
        IsLoaded = false;
        IsLoaded = true;
    }
    private IEnumerator __LoadAssetBundle(string assetBundleName) {
        byte[] binary = File.ReadAllBytes(Application.dataPath + "/DLC/" + assetBundleName);
        AssetBundleCreateRequest resultAssetBundle = AssetBundle.LoadFromMemoryAsync(binary);
        yield return new WaitWhile(() => !resultAssetBundle.isDone);
        AssetBundle assetBundle = resultAssetBundle.assetBundle;
        string[] assetNames = assetBundle.GetAllAssetNames();
        for (int i = 0; i < assetNames.Length; i++) {
            AssetBundleRequest resultObject = assetBundle.LoadAssetAsync<GameObject>(assetNames[i]);
            yield return new WaitWhile(() => !resultObject.isDone);
            UnityEngine.Debug.Log("[AOC Load]▶Name:" + resultObject.asset.name);
            DlcDataDefine.DlcBaseData component = (Object.Instantiate(resultObject.asset, base.transform) as GameObject).GetComponent<DlcDataDefine.DlcBaseData>();
            if (component != null) {
                component.Init();
                listDLC.Add(component);
            }
        }
        yield return new WaitForEndOfFrame();
        assetBundle.Unload(unloadAllLoadedObjects: false);
        UnityEngine.Debug.Log("AOC読込完了");
    }
    private void Awake() {
        IsLoaded = true;
        Object.DontDestroyOnLoad(base.gameObject);
    }
}
