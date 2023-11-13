using UnityEngine;
public class AlpineSkiing_AI : MonoBehaviour {
    [SerializeField]
    [Header("プレイヤ\u30fc処理")]
    private AlpineSkiing_Player player;
    [SerializeField]
    [Header("スキ\u30fc板処理")]
    private AlpineSkiing_SkiBoard skiBoard;
    private int playerNo;
    private AlpineSkiing_Define.AiStrength aiStrength;
    private Transform currentCheckPointAnchor;
    private int checkPointIdx;
    private float inputHorizontal;
    private float checkAngle;
    private bool isBranch;
    private bool targetDir;
    public int PlayerNo => playerNo;
    public int CurrentCheckPointIdx => checkPointIdx;
    public bool IsReverse {
        get;
        set;
    }
    public void Init() {
        aiStrength = (AlpineSkiing_Define.AiStrength)SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
        checkPointIdx = 0;
        playerNo = skiBoard.PlayerNo;
        currentCheckPointAnchor = SingletonCustom<AlpineSkiing_CourseManager>.Instance.GetCheckPointAnchor(playerNo, checkPointIdx);
        switch (aiStrength) {
            case AlpineSkiing_Define.AiStrength.WEAK:
                checkAngle = 15f;
                break;
            case AlpineSkiing_Define.AiStrength.COMMON:
                checkAngle = 10f;
                break;
            case AlpineSkiing_Define.AiStrength.STRONG:
                checkAngle = 10f;
                break;
        }
    }
    public void UpdateMethod() {
        if (Vector3.Cross(currentCheckPointAnchor.position - base.gameObject.transform.position, skiBoard.characterAnchor.transform.forward).y < 0f) {
            targetDir = true;
        } else {
            targetDir = false;
        }
        if (Vector3.Angle(skiBoard.characterAnchor.transform.forward, currentCheckPointAnchor.position - base.gameObject.transform.position) > checkAngle) {
            if (targetDir) {
                skiBoard.MoveCursor(AlpineSkiing_SkiBoard.MoveCursorDirection.RIGHT, 1f);
            } else {
                skiBoard.MoveCursor(AlpineSkiing_SkiBoard.MoveCursorDirection.LEFT, -1f);
            }
        }
        switch (aiStrength) {
            case AlpineSkiing_Define.AiStrength.WEAK:
                if (Vector3.Angle(skiBoard.characterAnchor.transform.forward, currentCheckPointAnchor.position - base.gameObject.transform.position) > 15f) {
                    if (skiBoard.processType != AlpineSkiing_SkiBoard.SkiBoardProcessType.CURVE) {
                        skiBoard.ProcessTypeChange(AlpineSkiing_SkiBoard.SkiBoardProcessType.CURVE);
                    }
                } else if (skiBoard.processType != AlpineSkiing_SkiBoard.SkiBoardProcessType.SLIDING) {
                    skiBoard.ProcessTypeChange(AlpineSkiing_SkiBoard.SkiBoardProcessType.SLIDING);
                }
                break;
            case AlpineSkiing_Define.AiStrength.COMMON:
                if (Vector3.Angle(skiBoard.characterAnchor.transform.forward, currentCheckPointAnchor.position - base.gameObject.transform.position) > 12f) {
                    if (skiBoard.processType != AlpineSkiing_SkiBoard.SkiBoardProcessType.CURVE) {
                        skiBoard.ProcessTypeChange(AlpineSkiing_SkiBoard.SkiBoardProcessType.CURVE);
                    }
                } else if (skiBoard.processType != AlpineSkiing_SkiBoard.SkiBoardProcessType.SLIDING) {
                    skiBoard.ProcessTypeChange(AlpineSkiing_SkiBoard.SkiBoardProcessType.SLIDING);
                }
                break;
            case AlpineSkiing_Define.AiStrength.STRONG:
                if (skiBoard.processType != AlpineSkiing_SkiBoard.SkiBoardProcessType.CURVE) {
                    skiBoard.ProcessTypeChange(AlpineSkiing_SkiBoard.SkiBoardProcessType.CURVE);
                }
                break;
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "CheckPoint_Player" + playerNo.ToString() && other.gameObject == SingletonCustom<AlpineSkiing_CourseManager>.Instance.arrayCheckPointAuto[playerNo][checkPointIdx] && checkPointIdx < SingletonCustom<AlpineSkiing_CourseManager>.Instance.currentCheckPointLength[playerNo]) {
            checkPointIdx++;
            currentCheckPointAnchor = SingletonCustom<AlpineSkiing_CourseManager>.Instance.GetCheckPointAnchor(playerNo, checkPointIdx);
        }
    }
}
