using UnityEngine;
using UnityEngine.AI;
public class Scene_BigMerchant : MonoBehaviour {
    private bool isInit;
    private bool isStart;
    [SerializeField]
    [Header("ナビメッシュデ\u30fcタ")]
    private NavMeshData navMeshData;
    private NavMeshDataInstance instance;
    private void OnEnable() {
        LightingSettings.ChangeSceneLighting();
        SingletonCustom<GameSettingManager>.Instance.SetCpuToPlayerGroupList();
        SingletonCustom<AudioManager>.Instance.PlayGameBgm();
        instance = NavMesh.AddNavMeshData(navMeshData);
    }
    private void OnDisable() {
        NavMesh.RemoveNavMeshData(instance);
    }
    private void Start() {
        SingletonCustom<FireworksGameManager>.Instance.Init();
        SingletonCustom<FireworksPlayerManager>.Instance.Init();
        SingletonCustom<BigMerchantCustomerManager>.Instance.Init();
        SingletonCustom<BigMerchantSupplyManager>.Instance.Init();
        SingletonCustom<FireworksUIManager>.Instance.Init();
    }
    private void Update() {
        if (!isInit && !SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<SceneManager>.Instance.IsLoading) {
            isInit = true;
            SingletonCustom<CommonNotificationManager>.Instance.OpenGameTitle(delegate {
                isStart = true;
                SingletonCustom<FireworksGameManager>.Instance.OnGameStart();
            });
        }
        if (!isStart) {
            SingletonCustom<FireworksUIManager>.Instance.UpdateMethod();
            return;
        }
        SingletonCustom<FireworksGameManager>.Instance.UpdateMethod();
        SingletonCustom<FireworksPlayerManager>.Instance.UpdateMethod();
        SingletonCustom<BigMerchantCustomerManager>.Instance.UpdateMethod();
        SingletonCustom<FireworksUIManager>.Instance.UpdateMethod();
    }
}
