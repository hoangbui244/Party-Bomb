using UnityEngine;
/// <summary>
/// 
/// </summary>
public class PartyCharacter : MonoBehaviour {
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("メッシュ")]
    private GameObject[] arrayMesh;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("王冠")]
    private GameObject objCrown;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("通常目")]
    private GameObject normalEye;
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Header("バツ目")]
    private GameObject crossEye;
    /// <summary>
    /// 
    /// </summary>
    public GameObject ActiveObject = null;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_idx"></param>
    /// <param name="OverrideController"></param>
    public void Init(int _idx, RuntimeAnimatorController OverrideController = null) {
        GameObject Mesh = UnityEngine.Object.Instantiate(GameCharacterManager.Instance.arrayCharacters[_idx], this.transform);
        Mesh.gameObject.transform.position = Vector3.zero;
        Mesh.gameObject.transform.localPosition = Vector3.zero;
        if (OverrideController!=null) {
            Mesh.GetComponent<Animator>().runtimeAnimatorController = OverrideController;
        }
        ActiveObject = Mesh;
        if (!(objCrown != null) || SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType != GameSettingManager.GameProgressType.ALL_SPORTS) {
            return;
        }
        int[] crownPlayerIdxArray = SingletonCustom<GameSettingManager>.Instance.GetCrownPlayerIdxArray();
        for (int j = 0; j < crownPlayerIdxArray.Length; j++) {
            if (crownPlayerIdxArray[j] == _idx) {
                objCrown.SetActive(value: true);
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public void ChangeCrossEye() {
        if (normalEye != null) {
            normalEye.SetActive(value: false);
        }
        if (crossEye != null) {
            crossEye.SetActive(value: true);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public void ChangeNormalEye() {
        if (normalEye != null) {
            normalEye.SetActive(value: true);
        }
        if (crossEye != null) {
            crossEye.SetActive(value: false);
        }
    }
}
