using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class SatMotionList : SingletonCustom<SatMotionList> {
    [SerializeField]
    public Skijump_Character charaModel;
    [SerializeField]
    [HideInInspector]
    public List<SatMotion> motions;
    [SerializeField]
    public string folderPath = "Assets/Game/Common/Asset/SatSimplePose/MotionData/";
    [SerializeField]
    [HideInInspector]
    public string saveMotionName = "default";
    [SerializeField]
    [HideInInspector]
    public KeyPoseData[] keys;
    [SerializeField]
    private int selectedIndex;
    public SatMotion GetMotionData(int index) {
        return motions[index];
    }
    public void Start() {
        if ((bool)charaModel) {
            charaModel.gameObject.SetActive(value: false);
        }
    }
}
