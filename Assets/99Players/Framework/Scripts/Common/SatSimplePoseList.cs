using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class SatSimplePoseList : SingletonCustom<SatSimplePoseList> {
    [SerializeField]
    private Skijump_Character charaModel;
    [SerializeField]
    public string folderPath = "Assets/SatSimplePose/PoseData/";
    [SerializeField]
    [HideInInspector]
    public string poseName = "default";
    [SerializeField]
    public List<SatSimplePose> poses;
    [SerializeField]
    private int selectedIndex;
    public SatSimplePose GetPose(int index) {
        return poses[index];
    }
    private void Start() {
        if (charaModel != null) {
            charaModel.gameObject.SetActive(value: false);
        }
    }
}
