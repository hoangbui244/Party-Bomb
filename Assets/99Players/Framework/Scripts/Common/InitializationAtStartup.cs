using System;
using UnityEngine;
public class InitializationAtStartup : MonoBehaviour {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnStart() {
        UnityEngine.Random.InitState(Environment.TickCount);
        UnityEngine.Object.Instantiate(Resources.Load("TextDataBaseManager"));
        GameObject gameObject = new GameObject();
        gameObject.AddComponent<SaveDataManager>();
        gameObject.name = "SaveDataManager(IAS)";
    }
}
